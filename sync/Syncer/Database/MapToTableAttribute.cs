using System;

namespace Syncer.Database
{
    public class MapToTableAttribute : Attribute
    {
        public string Name { get; }

        public MapToTableAttribute(string name) => Name = name;
    }
}
