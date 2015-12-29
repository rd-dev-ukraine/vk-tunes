using System;

namespace VkTunes.Infrastructure.AutoPropertyChange
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoNotifyOnPropertyChangeAttribute : Attribute
    {
    }
}