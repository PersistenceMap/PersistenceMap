using System;

namespace PersistanceMap
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}
