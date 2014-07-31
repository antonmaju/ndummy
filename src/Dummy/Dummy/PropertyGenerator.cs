using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    public abstract class PropertyGenerator
    {
        public MemberInfo MemberInfo { get; set; }

        public abstract object Create();
    }

    public class PropertyByFactoryGenerator : PropertyGenerator
    {
        private readonly IFactory factory;

        public PropertyByFactoryGenerator(MemberInfo member, IFactory factory)
        {
            this.MemberInfo = member;
            this.factory = factory;
        }

        public override object Create()
        {
            return factory.Create();
        }
    }

    public class PropertyByValueGenerator : PropertyGenerator
    {
        private readonly object value;

        public PropertyByValueGenerator(MemberInfo member, object value)
        {
            MemberInfo = member;
            this.value = value;
        }

        public override object Create()
        {
            return value;
        }
    }

    public class PropertyByFuncGenerator : PropertyGenerator
    {
        private readonly Func<object> func;

        public PropertyByFuncGenerator(MemberInfo member, Func<object> func)
        {
            MemberInfo = member;
            this.func = func;
        }

        public override object Create()
        {
            return func();
        }
    }
}
