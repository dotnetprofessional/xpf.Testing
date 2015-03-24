using System;
using System.IO;
using System.Reflection;

namespace xpf.Testing
{
    /// <summary>
    /// Provides methods to access resource files
    /// </summary>
    internal static class EmbeddedResources
    {
        public static Stream GetResourceStream(string resourceName, Type caller)
        {
            var assembly = caller.GetTypeInfo().Assembly;

            var resourceStream = GetResourceStream(resourceName, assembly);
            return resourceStream;
        }

        public static Stream GetResourceStream(string resourceName, Assembly assembly)
        {
            string strFullResourceName = "";
            foreach (string r in assembly.GetManifestResourceNames())
            {
                if (r.EndsWith(resourceName))
                {
                    strFullResourceName = r;
                    break;
                }
            }

            if (strFullResourceName != "")
                return assembly.GetManifestResourceStream(strFullResourceName);
            else
                throw new ArgumentException(string.Format("Unable to locate resource {0} in assembly {1}", resourceName, assembly.FullName));
        }

        /// <summary>
        /// Returns the contents of an embedded resource file as a string
        /// </summary>
        /// <param name="resourceName">The name of the resource file to return. The filename is matched using EndsWith allowing for partial filenames to be used.</param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static string GetResourceString(string resourceName, Assembly assembly)
        {
            StreamReader objStream;
            string strText = "";

            using (objStream = new StreamReader(GetResourceStream(resourceName, assembly)))
            {
                strText = objStream.ReadToEnd();
            }

            return strText;
        }

        /// <summary>
        /// Returns the contents of an embedded resource file as a string
        /// </summary>
        /// <param name="resourceName">The name of the resource file to return. The filename is matched using EndsWith allowing for partial filenames to be used.</param>
        /// <returns></returns>
        public static byte[] GetResource(Type caller, string resourceName)
        {
            BinaryReader objStream;
            byte[] data;

            using (objStream = new BinaryReader(GetResourceStream(resourceName, caller)))
            {
                data = objStream.ReadBytes((int)objStream.BaseStream.Length);
            }

            return data;
        }

    }

}