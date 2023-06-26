using System;
using Settings;

namespace Core
{
    public class PrefabInfo : Attribute
    {
        public string Location { get; }

        public PrefabInfo(string location)
        {
            Location = location;
        }
        public PrefabInfo(Enumerators.NamePrefabAddressable location)
        {
            Location = location.ToString();
        }
    }
}