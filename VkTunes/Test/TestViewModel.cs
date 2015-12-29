using System.Diagnostics;

using Caliburn.Micro;

using VkTunes.Infrastructure.AutoPropertyChange;

namespace VkTunes.Test
{
    [AutoNotifyOnPropertyChange]
    public class TestViewModel : PropertyChangedBase
    {
        public virtual int A { get; set; }

        public virtual int B { get; set; }

        public virtual int Sum => A + B;
    }
}