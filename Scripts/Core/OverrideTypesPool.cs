using System;

namespace Core
{
    public class OverrideTypesPool : Attribute
    {
        public string Id { get; }

        public OverrideTypesPool(string id)
        {
            Id = id;
        }
    }
}
