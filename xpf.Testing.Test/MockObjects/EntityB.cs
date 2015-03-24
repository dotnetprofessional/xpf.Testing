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
namespace xpf.Testing.Test.MockObjects
{
    public class EntityB
    {
        private string _Property1;
        private double _Property2;

        public string Property1
        {
            get { return this._Property1; }
            set { this._Property1 = value; }
        }

        public double Property2
        {
            get { return this._Property2; }
            set { this._Property2 = value; }
        }
    }
}