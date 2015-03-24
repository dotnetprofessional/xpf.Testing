using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using xpf.Testing.Test.MockObjects;

namespace xpf.Testing.Test
{
    [TestClass]
    public class AssertionsTest
    {
        #region Compare Tests

        [TestMethod]
        public void IsComparable_IgnorePropertyOnAllEntities()
        {
            var expected = new EntityA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = null;
            expected.EntityCs.Add(new EntityC { Age = 20 });

            // Define the values for instance 2
            actual.Age = 15;
            actual.Name = null;
            actual.EntityCs.Add(new EntityC { Age = 25 });

            // Now check that both instances are the same.
            actual.IsComparable(expected, "*.Age");
        }

        [TestMethod]
        public void IsComparable_IgnorePropertyOnSingleEntity()
        {
            var expected = new EntityA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = null;
            expected.EntityCs.Add(new EntityC { Age = 25 });

            // Define the values for instance 2
            actual.Age = 15;
            actual.Name = null;
            actual.EntityCs.Add(new EntityC { Age = 25 });

            // Now check that both instances are the same.
            actual.IsComparable(expected, "EntityA.Age");
        }

        [TestMethod]
        //[ExpectedException(typeof(Exception))]
        public void IsComparable_IgnorePropertyOnSingleEntityWhenPropertyMismatchesAnotherEntity()
        {
            var expected = new EntityA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = null;
            expected.EntityCs.Add(new EntityC { Age = 20 });

            // Define the values for instance 2
            actual.Age = 15;
            actual.Name = null;
            actual.EntityCs.Add(new EntityC { Age = 25 });

            try
            {
                // Now check that both instances are the same.
                actual.IsComparable(expected, "EntityA.Age");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: 20 but was 25. System.Int32.Age", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable()
        {
            EntityB b;
            var expected = new EntityA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = "John Wayne";

            b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            expected.EntityBs.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.67;
            expected.EntityBs.Add(b);

            // Define the values for instance 2
            actual.Age = 5;
            actual.Name = "John Wayne";

            b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            actual.EntityBs.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.69;
            actual.EntityBs.Add(b);

            // Now check that both instances are the same.
            try
            {
            actual.IsComparable(expected);

            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: 200.67 but was 200.69. System.Double.Property2", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_DerivedTypes()
        {
            EntityB b;
            var expected = new EntityAA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = "John Wayne";

            b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            expected.EntityBs.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.67;
            expected.EntityBs.Add(b);

            // Define the values for instance 2
            actual.Age = 5;
            actual.Name = "John Wayne";

            b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            actual.EntityBs.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.67;
            actual.EntityBs.Add(b);

            // Now check that both instances are the same.
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_Collection()
        {
            var b1list = new List<EntityB>();
            var b2list = new List<EntityB>();

            var b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            b1list.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.67;
            b1list.Add(b);

            b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            b2list.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.67;
            b2list.Add(b);

            // Now check that both instances are the same.
            b1list.IsComparable(b2list);
        }

        // The reason we need to reference the MSTest.Exception is because the main code is using the MS Test API for Asserts etc.
        [TestMethod]
        public void IsComparable_Collection_InValid()
        {
            var actual = new List<EntityB>();
            var expected = new List<EntityB>();

            var b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            actual.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.67;
            actual.Add(b);

            b = new EntityB();
            b.Property1 = "Test1";
            b.Property2 = 100.67;
            expected.Add(b);

            b = new EntityB();
            b.Property1 = "Test2";
            b.Property2 = 200.99; // This value is different
            expected.Add(b);

            // Now check that both instances are the same.
            try
            {

                actual.IsComparable(expected);
                Assert.Fail("Expected Exception");

            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: 200.99 but was 200.67. System.Double.Property2", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_NullCollection()
        {
            var expected = new EntityA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = "John Wayne";
            expected.EntityBs = null;

            // Define the values for instance 2
            actual.Age = 5;
            actual.Name = "John Wayne";
            actual.EntityBs = null;

            // Now check that both instances are the same.
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_NullValue()
        {
            var expected = new EntityA();
            var actual = new EntityA();

            // Define the values for instance 1
            expected.Age = 5;
            expected.Name = null;

            // Define the values for instance 2
            actual.Age = 5;
            actual.Name = null;

            // Now check that both instances are the same.
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_ObjectValues_Null_NotEqual()
        {
            var expected = new EntityDataTypeCheck {ObjectProperty = 123456};
            var actual = new EntityDataTypeCheck {ObjectProperty = null};
            try
            {
                actual.IsComparable(expected);
                Assert.Fail("Expected Exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("actual: . xpf.Testing.Test.MockObjects.EntityDataTypeCheck.ObjectProperty was null but expected not null", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_ObjectValues_Int_NotEqual()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = 123456 };
            var actual = new EntityDataTypeCheck { ObjectProperty = 12 };
            try
            {
                actual.IsComparable(expected);
                Assert.Fail("Expected Exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: 123456 but was 12. System.Int32.ObjectProperty", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_ObjectValues_Int_Equal()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = 123456 };
            var actual = new EntityDataTypeCheck { ObjectProperty = 123456 };
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_ObjectValues_Null_Equal()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = null };
            var actual = new EntityDataTypeCheck { ObjectProperty = null };
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_ObjectValues_DateTime_NotEqual()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = DateTime.Parse("2009-01-01 12:12:12") };
            var actual = new EntityDataTypeCheck { ObjectProperty = DateTime.Parse("2009-01-01 12:12:13") };
            try
            {
                actual.IsComparable(expected);
                Assert.Fail("Expected Exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: 2009-01-01 12:12:12 but was 2009-01-01 12:12:13. System.DateTime.ObjectProperty", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_ObjectValues_DateTime_Equal()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = DateTime.Parse("2009-01-01 12:12:12") };
            var actual = new EntityDataTypeCheck { ObjectProperty = DateTime.Parse("2009-01-01 12:12:12") };
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_ObjectValues_String_NotEqual()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = "Hello" };
            var actual = new EntityDataTypeCheck { ObjectProperty = "Goodbye" };
            try
            {
                actual.IsComparable(expected);
                Assert.Fail("Expected Exception");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: Hello but was Goodbye. System.String.ObjectProperty", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_ObjectValues_String_Equal()
        {
            var expected = new EntityDataTypeCheck { ObjectProperty = "Hello" };
            var actual = new EntityDataTypeCheck { ObjectProperty = "Hello" };
            actual.IsComparable(expected);
        }

        [TestMethod]
        public void IsComparable_IgnoreEmptyCollectionIfNull()
        {
            var a = new EntityA();
            a.EntityBs = null;

            var b = new EntityA();
            b.EntityBs = new List<EntityB>();

            // Now check that both instances are the same.
           a.IsComparable(b, Assertions.CompareOptions.NullAndEmptyCollectionEqual);
        }

        [TestMethod]
        public void IsComparable_Collection_with_Empty_collection()
        {
            var actual = new EntityA();
            var expected = new EntityA();

            var data = new List<EntityB>();
            data.Add(new EntityB());
            data.Add(new EntityB());
            data.Add(new EntityB());

            expected.Data = data;
            actual.Data = new List<EntityB>();

            // Now check that both instances are the same.
            try
            {
                actual.IsComparable(expected);
                Assert.Fail("Expected failure");
            }
            catch (Exception ex)
            {
                Assert.AreEqual("expected: 3 but was 0. System.Collections.Generic.List`1[xpf.Testing.Test.MockObjects.EntityB].Data item count not equal", ex.Message);
            }
        }

        [TestMethod]
        public void IsComparable_ItemInCollectionVsNoItemInCollection_NullAndEmptyCollectionEqual_Set()
        {
            var a = new EntityA();
            a.EntityBs = null;

            var b = new EntityA();
            b.EntityBs = new List<EntityB>();
            b.EntityBs.Add(new EntityB());

            try
            {
                // Now check that both instances are the same.
                a.IsComparable(b, Assertions.CompareOptions.NullAndEmptyCollectionEqual);
                Assert.Fail("Expected Exception");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void IsComparable_NoItemInCollectionVsItemInCollection_NullAndEmptyCollectionEqual_Set()
        {
            var a = new EntityA();
            a.EntityBs = null;

            var b = new EntityA();
            b.EntityBs = new List<EntityB>();
            b.EntityBs.Add(new EntityB());

            try
            {
                // Now check that both instances are the same.
                a.IsComparable(b, Assertions.CompareOptions.NullAndEmptyCollectionEqual);

                Assert.Fail("Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(true);
            }
        }

        #endregion
    }
}
