#region License
// (c) Garry McGlennon 2009
// 
// UnitTestEx is made available under the Microsoft Public License (Ms-PL)
// Code is provided as is and with no warrenty – Use at your own risk
// View the project and the latest code at http://codeplex.com/UnitTestEx/
// 
// All other rights reserved.
// 
// blog: www.dotNetProfessional.com/blog/
#endregion

using System;

namespace xpf.Testing.Test.MockObjects
{
    public class EntityDataTypeCheck
    {
        public int IntProperty { get; set; }
        public double DoubleProperty { get; set; }
        public string StringProperty { get; set; }
        public DateTime DateTimeProperty { get; set; }
        public object ObjectProperty { get; set; }
    }
}
