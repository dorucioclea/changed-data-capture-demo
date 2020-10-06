using System;

namespace Syncer.Database
{
    public class MapToIndexAttribute : Attribute
    {
        public string Name { get; }

        public MapToIndexAttribute(string name) => Name = name;
    }
}