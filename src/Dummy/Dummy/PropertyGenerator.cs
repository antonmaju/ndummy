using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    /// <summary>
    /// Represents property / field generator
    /// </summary>
    public abstract class PropertyGenerator
    {
        /// <summary>
        /// Gets or sets the MemberInfo value
        /// </summary>
        /// <value>
        /// The member information.
        /// </value>
       public MemberInfo MemberInfo { get; set; }

        /// <summary>
        /// This property is used for temporary properties only
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates value
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <returns></returns>
        public abstract object Create(ObjectCreationContext context);
    }

    /// <summary>
    /// Represents property / field generator by IFactory implementation
    /// </summary>
    public class PropertyByFactoryGenerator : PropertyGenerator
    {
        private readonly IFactory factory;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyByFactoryGenerator"/> class.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="factory">The factory.</param>
        public PropertyByFactoryGenerator(MemberInfo member, IFactory factory)
        {
            this.MemberInfo = member;
            this.factory = factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyByFactoryGenerator"/> class.
        /// </summary>
        /// <param name="name">The temp property name.</param>
        /// <param name="factory">The factory.</param>
        public PropertyByFactoryGenerator(string name, IFactory factory)
        {
            this.Name = name;
            this.factory = factory;
        }

        /// <summary>
        /// Creates property/field value
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <returns>property/field value</returns>
        public override object Create(ObjectCreationContext context)
        {
            return factory.Create();
        }
    }


    /// <summary>
    /// Represents property / field generator by specified value
    /// </summary>
    public class PropertyByValueGenerator : PropertyGenerator
    {
        private readonly object value;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyByValueGenerator"/> class.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="value">The value.</param>
        public PropertyByValueGenerator(MemberInfo member, object value)
        {
            MemberInfo = member;
            this.value = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyByValueGenerator"/> class.
        /// </summary>
        /// <param name="name">The temp property name.</param>
        /// <param name="value">The value.</param>
        public PropertyByValueGenerator(string name, object value)
        {
            Name = name;
            this.value = value;
        }

        /// <summary>
        /// Creates property / field value
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <returns>value</returns>
        public override object Create(ObjectCreationContext context)
        {
            return value;
        }
    }

    /// <summary>
    /// Represents property / field generator by specified factory method
    /// </summary>
    public class PropertyByFuncGenerator : PropertyGenerator
    {
        private readonly Func<ObjectCreationContext, object> func;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyByFuncGenerator"/> class.
        /// </summary>
        /// <param name="member">The member.</param>
        /// <param name="func">The function.</param>
        public PropertyByFuncGenerator(MemberInfo member, Func<ObjectCreationContext, object> func)
        {
            MemberInfo = member;
            this.func = func;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyByFuncGenerator"/> class.
        /// </summary>
        /// <param name="name">The temp property name.</param>
        /// <param name="func">The function.</param>
        public PropertyByFuncGenerator(string name, Func<ObjectCreationContext, object> func)
        {
            Name = name;
            this.func = func;
        }

        /// <summary>
        /// Creates value
        /// </summary>
        /// <param name="context">The creation context.</param>
        /// <returns>value</returns>
        public override object Create(ObjectCreationContext context)
        {
            return func(context);
        }
    }


    /// <summary>
    /// This class contains helper for creating temporary property
    /// </summary>
    public static class TemporaryProperty
    {
        /// <summary>
        /// Creates temp property with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public static PropertyGenerator New<T>(string name, IFactory<T> factory)
        {
            return new PropertyByFactoryGenerator(name, factory);
        }

        /// <summary>
        /// Creates temp property with the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static PropertyGenerator New<T>(string name, T value)
        {
            return new PropertyByValueGenerator(name, value);
        }

        /// <summary>
        /// Creates temp property with the specified func.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="func">The function.</param>
        /// <returns></returns>
        public static PropertyGenerator New<T>(string name, Func<ObjectCreationContext,T> func)
        {
            return  new PropertyByFuncGenerator(name, ctx=>func(ctx) );
        }
    }
}
