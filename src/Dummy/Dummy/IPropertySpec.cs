using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    public interface IPropertySpec
    {
        PropertyGenerator GenerateWith(IFactory factory);

        PropertyGenerator GenerateWith(object value);

        PropertyGenerator GenerateWith(Func<ObjectCreationContext, object> func);
    }

    public interface IPropertySpec<in T> : IPropertySpec
    {
        PropertyGenerator GenerateWith(IFactory<T> factory);

        PropertyGenerator GenerateWith(T value);

        PropertyGenerator GenerateWith(Func<ObjectCreationContext,T> func);
    }

    public class PropertySpec<T> : IPropertySpec<T>
    {
        private readonly MemberInfo memberInfo;

        public PropertySpec(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public PropertyGenerator GenerateWith(IFactory<T> factory)
        {
            return new PropertyByFactoryGenerator(memberInfo, factory);
        }

        public PropertyGenerator GenerateWith<TFactory>() where TFactory : IFactory<T>, new()
        {
            return new PropertyByFactoryGenerator(memberInfo, Activator.CreateInstance<TFactory>());
        }

        public PropertyGenerator GenerateWith(T value)
        {
            return new PropertyByValueGenerator(memberInfo, value);
        }

        public PropertyGenerator GenerateWith(IFactory factory)
        {
            return new PropertyByFactoryGenerator(memberInfo, factory);
        }

        public PropertyGenerator GenerateWith(object value)
        {
            return new PropertyByValueGenerator(memberInfo, value);
        }

        public PropertyGenerator GenerateWith(Func<ObjectCreationContext, T> func)
        {
            return new PropertyByFuncGenerator(memberInfo, ctx=> func(ctx));
        }

        public PropertyGenerator GenerateWith(Func<ObjectCreationContext,object> func)
        {
            return new PropertyByFuncGenerator(memberInfo, func);
        }
    }
}
