using System;

namespace Syncer.Database
{
    public class MapToTableFieldAttribute : Attribute
    {
        public string Name { get; }

        public MapToTableFieldAttribute(string name) => Name = name;
    }
}