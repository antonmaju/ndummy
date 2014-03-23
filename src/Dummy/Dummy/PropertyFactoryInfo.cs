using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    public class PropertyFactoryInfo
    {
        public MemberInfo MemberInfo { get; set; }

        public IFactory Factory { get; set; }

        public object Value { get; set; }
    }
}
