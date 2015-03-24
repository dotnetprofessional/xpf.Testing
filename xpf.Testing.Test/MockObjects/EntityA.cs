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

using System.Collections.Generic;

namespace xpf.Testing.Test.MockObjects
{
    public class EntityA
    {
        private string _Name;
        private int _Age;
        private List<EntityB> _EntityBs = new List<EntityB>();
        private List<EntityC> _EntityCs = new List<EntityC>();

        public string Name
        {
            get { return this._Name; }
            set { this._Name = value; }
        }

        public int Age
        {
            get { return this._Age; }
            set { this._Age = value; }
        }


        public List<EntityB> EntityBs
        {
            get { return this._EntityBs; }
            set { this._EntityBs = value; }
        }

        public List<EntityC> EntityCs
        {
            get { return this._EntityCs; }
            set { this._EntityCs = value; }
        }

        public object Data { get; set; }
    }
}