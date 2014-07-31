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

        PropertyGenerator GenerateWith(Func<object> func);
    }

    public interface IPropertySpec<in T> : IPropertySpec
    {
        PropertyGenerator GenerateWith(IFactory<T> factory);

        PropertyGenerator GenerateWith(T value);

        PropertyGenerator GenerateWith(Func<T> func);
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

        public PropertyGenerator GenerateWith(Func<T> func)
        {
            return new PropertyByFuncGenerator(memberInfo, ()=> func());
        }

        public PropertyGenerator GenerateWith(Func<object> func)
        {
            return new PropertyByFuncGenerator(memberInfo, func);
        }
    }
}
