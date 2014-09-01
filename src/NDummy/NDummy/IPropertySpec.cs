using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NDummy.Factories;

namespace NDummy
{
    /// <summary>
    /// Represents generator spec for property and field
    /// </summary>
    public interface IPropertySpec
    {
        PropertyGenerator GenerateWith(IFactory factory);

        PropertyGenerator GenerateWith(object value);

        PropertyGenerator GenerateWith(Func<ObjectCreationContext, object> func);
    }

    /// <summary>
    /// Represents generic generator spec
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPropertySpec<in T> : IPropertySpec
    {
        PropertyGenerator GenerateWith(IFactory<T> factory);

        PropertyGenerator GenerateWith(T value);

        PropertyGenerator GenerateWith(Func<ObjectCreationContext, T> func);
    }

    /// <summary>
    /// Represents generic generator spec for property and field
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PropertySpec<T> : IPropertySpec<T>
    {
        private readonly MemberInfo memberInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySpec{T}"/> class.
        /// </summary>
        /// <param name="memberInfo">The member information.</param>
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
            return new PropertyByFuncGenerator(memberInfo, ctx => func(ctx));
        }

        public PropertyGenerator GenerateWith(Func<ObjectCreationContext, object> func)
        {
            return new PropertyByFuncGenerator(memberInfo, func);
        }
    }
}
