using System;

namespace Data.Characteristics
{
    public class DataInfo : Attribute
    {
        public Type Json { get; }
        public Type Scriptable { get; }

        public DataInfo(Type json, Type scriptable)
        {
            Json = json;
            Scriptable = scriptable;
        }

    }
}
