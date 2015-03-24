using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace xpf.Testing
{
    public static class Assertions
    {
        #region Static Properties
        /// <summary>
        /// This is used to by the IsComparable method to eliminate circular references (ie stack overflow)
        /// </summary>
        [ThreadStatic]
        private static Dictionary<int, string> _identityMap;

        [ThreadStatic]
        private static Dictionary<string, Action<object, object, PropertyInfo>> _compareRules;

        [ThreadStatic]
        private static Dictionary<string, string> _ignoreProperties;

        [ThreadStatic]
        private static CompareOptions _options;

        /// <summary>
        /// This is used to by the CompareObjectIntance method to eliminate circular references (ie stack overflow)
        /// </summary>
        private static Dictionary<int, string> IdentityMap
        {
            get
            {
                if (_identityMap == null)
                    _identityMap = new Dictionary<int, string>();
                return _identityMap;
            }
        }

        private static Dictionary<string, string> IgnoreProperties
        {
            get
            {
                if (_ignoreProperties == null)
                    _ignoreProperties = new Dictionary<string, string>();
                return _ignoreProperties;
            }
        }

        private static CompareOptions Options
        {
            get { return _options; }
        }

        private static Dictionary<string, Action<object, object, PropertyInfo>> CompareRules
        {
            get
            {
                if (_compareRules == null)
                    _compareRules = new Dictionary<string, Action<object, object, PropertyInfo>>();
                return _compareRules;
            }
        }
        #endregion
        [Flags]
        public enum CompareOptions
        {
            Deafult,
            MustBeDerivable = 1,
            NullAndEmptyCollectionEqual = 2
        }

        /// <summary>
        /// Given two object instances (of the same type) will assert that each
        /// property value on the exected instance matches that of the expected instance.
        /// This routine is only to be used with the Microsoft Unit Testing Framework.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="ignoreProperties">a comma seperated list of properties in object.property notation to exclude from comparison.
        /// The syntax is object.propertyname|*.propertyname ie MyObject.MyProperty or *.MyProperty. When an asterix is used in replace of an object name any object with that property will be ignored.</param>
        public static void IsComparable(this object actual, object expected, params string[] ignoreProperties)
        {
            IsComparable(actual, expected, CompareOptions.Deafult, ignoreProperties);
        }

        /// <summary>
        /// Given two object instances (of the same type) will assert that each
        /// property value on the exected instance matches that of the expected instance.
        /// This routine is only to be used with the Microsoft Unit Testing Framework.
        /// </summary>
        /// <param name="actual">The actual.</param>
        /// <param name="expected">The expected.</param>
        /// <param name="options">The IsComparable Options.</param>
        /// <param name="ignoreProperties">a comma seperated list of properties in object.property notation to exclude from comparison.
        /// The syntax is object.propertyname|*.propertyname ie MyObject.MyProperty or *.MyProperty. When an asterix is used in replace of an object name any object with that property will be ignored.</param>
        public static void IsComparable(this object actual, object expected, CompareOptions options, params string[] ignoreProperties)
        {
            Action<object, object, PropertyInfo> rule = null;

            InitializeCompare(options, ignoreProperties);

            // Check special cases
            if (actual.GetType().GetTypeInfo().GetInterface("IList") != null)
                rule = CompareIListTypeRule;
            else
            {
                // Handle special object types that are passes in as the root object
                if (CompareRules.ContainsKey(actual.GetType().FullName))
                    rule = CompareRules[actual.GetType().FullName];

            }
            if (rule == null)
                CompareInternal(actual, expected);
            else
                rule(actual, expected, null);

            IdentityMap.Clear();
        }

        static Type GetInterface(this TypeInfo typeInfo, string interfaceName)
        {
            foreach (var t in typeInfo.ImplementedInterfaces)
                if (t.Name == interfaceName)
                    return t;

            return null;
        }


        private static void InitializeCompare(CompareOptions options, string[] ignoreProperties)
        {
            _options = options;

            // Setup processing rules
            CompareRules.Clear();
            CompareRules.Add(typeof(string).FullName, CompareSimpleValueTypeRule);
            CompareRules.Add(typeof(char[]).FullName, CompareSimpleValueTypeRule);
            CompareRules.Add(typeof(DateTime).FullName, CompareDateTimeTypeRule);
#if DESKTOP
            CompareRules.Add(typeof(System.Xml.XmlDocument).FullName, CompareXmlDocumentTypeRule);
            CompareRules.Add(typeof(System.Data.DataTable).FullName, CompareDataTableRule);
#endif
            IgnoreProperties.Clear();
            if (ignoreProperties != null)
            {
                foreach (string name in ignoreProperties)
                    IgnoreProperties.Add(name, name);
            }

            // This method is needed to ensure that we can clean up the hashtable when the comapre routine is done,
            // the real work is done in the internal method.
            IdentityMap.Clear();
        }

        private static void CompareInternal(object actual, object expected)
        {
            // Check this instance hasn't been processed, and if not record it
            if (IdentityMap.ContainsKey(expected.GetHashCode()))
                return;

            IdentityMap.Add(expected.GetHashCode(), "");


            if (expected.GetType().GetTypeInfo().IsPrimitive || expected is string)
                // These types have no properties
                return;

            // Check if actual and expected are derivable
            if ((_options & CompareOptions.MustBeDerivable) == CompareOptions.MustBeDerivable && !actual.GetType().GetTypeInfo().IsAssignableFrom(expected.GetType().GetTypeInfo()))
                throw new ArgumentException("The expected type must be able to be cast from the actual type.");

            foreach (PropertyInfo expectedPropertyInfo in expected.GetType().GetTypeInfo().DeclaredProperties)
            {
                // Check for parameters to ignore
                if (IgnoreProperties.ContainsKey(expected.GetType().Name + "." + expectedPropertyInfo.Name) || IgnoreProperties.ContainsKey("*." + expectedPropertyInfo.Name))
                    // Ignore this property
                    continue;

                PropertyInfo actualPropertyInfo = actual.GetType().GetTypeInfo().GetDeclaredProperty(expectedPropertyInfo.Name);

                object expectedValue = expectedPropertyInfo.GetValue(expected, null);
                object actualValue = actualPropertyInfo.GetValue(actual, null);

                if (expectedValue == null && actualValue == null)
                    continue;

                if (!(typeof(IEnumerable).GetTypeInfo().IsAssignableFrom(expectedPropertyInfo.PropertyType.GetTypeInfo()) && (_options & CompareOptions.NullAndEmptyCollectionEqual) == CompareOptions.NullAndEmptyCollectionEqual))
                {
                    Assert.IsNotNull(expectedValue, expected.GetType() + "." + expectedPropertyInfo.Name + " was null but actual not null");
                    Assert.IsNotNull(actualValue, actual.GetType() + "." + actualPropertyInfo.Name + " was null but expected not null");
                }

                Type expectedType = expectedValue == null ? expectedPropertyInfo.PropertyType : expectedValue.GetType();

                // Lookup a processing rule for this type
                Action<object, object, PropertyInfo> rule = null;
                if (CompareRules.ContainsKey(expectedType.FullName))
                    rule = CompareRules[expectedType.FullName];

                if (rule == null)
                {
                    // No processing rule was found so use the default rules
                    if (expectedType.GetTypeInfo().IsClass)
                    {
                        var compareTypeInfo = expectedPropertyInfo.PropertyType.GetTypeInfo();

                        if (expectedPropertyInfo.PropertyType.Name == "Object")
                        {
                            // For properties tthat of type object, the type info can't be used as that doesn't tell what the instance is
                            // So if the values are not null (which were checked previously) we have to get the type info from the instance instead
                            compareTypeInfo = expectedValue.GetType().GetTypeInfo();
                        }

                        if (typeof (IEnumerable).GetTypeInfo().IsAssignableFrom(compareTypeInfo))
                            rule = CompareIListTypeRule;
                        else
                            rule = CompareClassTypeRule;
                    }
                    else
                        rule = CompareSimpleValueTypeRule;
                }

                rule(actualValue, expectedValue, actualPropertyInfo);
            }
        }

        /// <summary>
        /// Processes simple value types
        /// </summary>
        private static void CompareSimpleValueTypeRule(object actual, object expected, PropertyInfo p)
        {
            // Enums are special simple values
            if (expected.GetType().GetTypeInfo().IsEnum)
                Assert.AreEqual(actual.ToString(), expected.ToString(), expected.GetType() + "." + p.Name);
            else
                Assert.AreEqual(actual, expected, expected.GetType() + "." + p.Name);
        }

        /// <summary>
        /// Processes IList collections
        /// </summary>
        private static void CompareIListTypeRule(object actual, object expected, PropertyInfo p)
        {
            IList expectedValue = expected as IList;
            IList actualValue = actual as IList;

            // Check for the scenario that one collection is null but the other isn't
            if (expectedValue == null && actualValue != null && actualValue.Count != 0)
                Assert.IsNotNull(expectedValue, actual.GetType() + "." + (p == null ? "" : p.Name) + " is null but expected is not null.");

            if (actualValue == null && expectedValue != null && expectedValue.Count != 0)
                Assert.IsNotNull(actualValue, expected.GetType() + "." + (p == null ? "" : p.Name) + " is null but actual is not null.");

            if (!((expectedValue == null || actualValue == null) && (_options & CompareOptions.NullAndEmptyCollectionEqual) == CompareOptions.NullAndEmptyCollectionEqual))
            {
                Assert.AreEqual(actualValue.Count, expectedValue.Count, expected.GetType() + "." + (p == null ? "" : p.Name) + " item count not equal");

                for (int i = 0; i < expectedValue.Count; i++)
                {
                    CompareInternal(actualValue[i], expectedValue[i]);
                }
            }
        }

        /// <summary>
        /// Processes an object instance
        /// </summary>
        private static void CompareClassTypeRule(object actual, object expected, PropertyInfo p)
        {
            CompareInternal(actual, expected);
        }
#if DESKTOP
        private static void CompareDataTableRule(object actual, object expected, PropertyInfo p)
        {
            var actualDataTable = expected as System.Data.DataTable;
            var expectedDataTable = actual as System.Data.DataTable;

            Dictionary<string, string> excludedProperties = new Dictionary<string, string>();
            foreach (string name in IgnoreProperties.Keys)
                excludedProperties.Add(name, name);

            for (int row = 0; row < expectedDataTable.Rows.Count; row++)
            {
                var expectedRow = expectedDataTable.Rows[row];
                var actualRow = actualDataTable.Rows[row];

                for (int i = 0; i < expectedDataTable.Columns.Count; i++)
                    if (!excludedProperties.ContainsKey(expectedDataTable.Columns[i].Caption))
                        Assert.AreEqual(expectedRow[i], actualRow[i], string.Format("Column: {0}", expectedDataTable.Columns[i].Caption));
            }

        }
        /// <summary>
        /// Compares two XmlDocument instances by converting them into a normalized xml string
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        /// <param name="p"></param>

        private static void CompareXmlDocumentTypeRule(object actual, object expected, PropertyInfo p)
        {
            // Due to the way xml can be stored we attempt to strip out any non-important formatting
            string expectedXML = ((System.Xml.XmlDocument)actual).OuterXml.Replace("\r\n", "").Replace("\t", "");
            string actualXML = ((System.Xml.XmlDocument)expected).OuterXml.Replace("\r\n", "").Replace("\t", "");
            Assert.AreEqual(expectedXML, actualXML, actual.GetType().ToString() + "." + p.Name);
        }
#endif
        /// <summary>
        //  Due to the lack of precision of the SQL Database for DateTimes we have to handle them differently
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="p"></param>
        private static void CompareDateTimeTypeRule(object actual, object expected, PropertyInfo p)
        {
            Assert.AreEqual(((DateTime)actual).ToString("yyyy-MM-dd hh:mm:ss"), ((DateTime)expected).ToString("yyyy-MM-dd hh:mm:ss"), expected.GetType().ToString() + "." + p.Name);
        }

    }

    #if !DESKTOP
    internal class Assert
    {
        static public void AreEqual(object actual, object expected, string reason = "")
        {
            if (Equals(expected, actual))
                return;

            throw new Exception(string.Format("expected: {0} but was {1}. {2}", expected, actual, reason));
        }

        static public void IsNotNull(object actual, string reason = "")
        {
            if (actual != null)
                return;

            throw new Exception(string.Format("actual: {0}. {1}", actual, reason));

        }
    }
    #endif
}
