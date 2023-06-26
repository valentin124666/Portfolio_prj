using System;

namespace Data.Characteristics
{
    public class DataName: Attribute
    {
        private string name;

        public string Name => "/" + name;

        public DataName(string name)
        {
            this.name = name;
        }

    }
}
