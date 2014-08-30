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

        /// <summary>
        /// This property is used for temporary properties only
        /// </summary>
        public string Name { get; set; }

        public abstract object Create(ObjectCreationContext context);
    }

    public class PropertyByFactoryGenerator : PropertyGenerator
    {
        private readonly IFactory factory;

        public PropertyByFactoryGenerator(MemberInfo member, IFactory factory)
        {
            this.MemberInfo = member;
            this.factory = factory;
        }

        public PropertyByFactoryGenerator(string name, IFactory factory)
        {
            this.Name = name;
            this.factory = factory;
        }

        public override object Create(ObjectCreationContext context)
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

        public PropertyByValueGenerator(string name, object value)
        {
            Name = name;
            this.value = value;
        }

        public override object Create(ObjectCreationContext context)
        {
            return value;
        }
    }

    public class PropertyByFuncGenerator : PropertyGenerator
    {
        private readonly Func<ObjectCreationContext, object> func;

        public PropertyByFuncGenerator(MemberInfo member, Func<ObjectCreationContext, object> func)
        {
            MemberInfo = member;
            this.func = func;
        }

        public PropertyByFuncGenerator(string name, Func<ObjectCreationContext, object> func)
        {
            Name = name;
            this.func = func;
        }

        public override object Create(ObjectCreationContext context)
        {
            return func(context);
        }
    }


    public static class TemporaryProperty
    {
        public static PropertyGenerator New<T>(string name, IFactory<T> factory)
        {
            return new PropertyByFactoryGenerator(name, factory);
        }

        public static PropertyGenerator New<T>(string name, T value)
        {
            return new PropertyByValueGenerator(name, value);
        }

        public static PropertyGenerator New<T>(string name, Func<ObjectCreationContext,T> func)
        {
            return  new PropertyByFuncGenerator(name, ctx=>func(ctx) );
        }
    }
}
