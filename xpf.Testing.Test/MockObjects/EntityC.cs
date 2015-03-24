using System.Collections.Generic;

namespace xpf.Testing.Test.MockObjects
{
    public class EntityC
    {
        private string _Name;
        private int _Age;
        private List<EntityB> _EntityBs = new List<EntityB>();

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
    }
}
