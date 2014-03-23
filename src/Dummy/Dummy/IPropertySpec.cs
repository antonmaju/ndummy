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
        PropertyFactoryInfo GenerateWith(IFactory factory);

        PropertyFactoryInfo GenerateWith(object value);
    }

    public interface IPropertySpec<in T> : IPropertySpec
    {
        PropertyFactoryInfo GenerateWith(IFactory<T> factory);

        PropertyFactoryInfo GenerateWith(T value);
    }

    public class PropertySpec<T> : IPropertySpec<T>
    {
        private readonly MemberInfo memberInfo;

        public PropertySpec(MemberInfo memberInfo)
        {
            this.memberInfo = memberInfo;
        }

        public PropertyFactoryInfo GenerateWith(IFactory<T> factory)
        {
            return new PropertyFactoryInfo
            {
                Factory = factory,
                MemberInfo = memberInfo
            };
        }

        public PropertyFactoryInfo GenerateWith(T value)
        {
            return new PropertyFactoryInfo
            {
                MemberInfo = memberInfo,
                Value = value
            };
        }

        public PropertyFactoryInfo GenerateWith(IFactory factory)
        {
            return new PropertyFactoryInfo
            {
                MemberInfo = memberInfo,
                Factory = factory
            };
        }

        public PropertyFactoryInfo GenerateWith(object value)
        {
            return new PropertyFactoryInfo
            {
                MemberInfo = memberInfo,
                Value = value
            };
        }
    }
}
