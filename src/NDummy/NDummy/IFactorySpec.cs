using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NDummy.Factories;

namespace NDummy
{
    /// <summary>
    /// An interface which defines spec
    /// </summary>
    public interface IFactorySpec
    {
        /// <summary>
        /// Gets the temporary properties generators.
        /// </summary>
        /// <value>
        /// The temporary properties.
        /// </value>
        IList<PropertyGenerator> TemporaryProperties { get; }

        /// <summary>
        /// Gets the constructor method.
        /// </summary>
        /// <value>
        /// The constructor.
        /// </value>
        Func<object> Constructor { get; }

        /// <summary>
        /// Gets the member generators.
        /// </summary>
        /// <value>
        /// The member generators.
        /// </value>
        IList<PropertyGenerator> MemberGenerators { get; }

        /// <summary>
        /// Gets the ignored members.
        /// </summary>
        /// <value>
        /// The ignored members.
        /// </value>
        IList<MemberInfo> IgnoredMembers { get; }

        /// <summary>
        /// Gets the custom action.
        /// </summary>
        /// <value>
        /// The custom action.
        /// </value>
        Action<ObjectCreationContext> CustomAction { get; }
    }

    /// <summary>
    /// A generic interface which defines spec
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFactorySpec<T> : IFactorySpec
    {
        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <value>
        /// The constructor.
        /// </value>
        new Func<T> Constructor { get; }

        /// <summary>
        /// Gets the custom action which is executed after an object is created.
        /// </summary>
        /// <value>
        /// The custom action.
        /// </value>
        new Action<ObjectCreationContext> CustomAction { get; }
    }

    /// <summary>
    /// Base class for spec
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FactorySpec<T> : IFactorySpec<T> where T : class
    {
        private readonly List<PropertyGenerator> generators;
        private readonly List<PropertyGenerator> tempProperties;
        private readonly IList<MemberInfo> ignoredMembers;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactorySpec{T}"/> class.
        /// </summary>
        protected FactorySpec()
        {
            generators = new List<PropertyGenerator>();
            tempProperties = new List<PropertyGenerator>();
            ignoredMembers = new List<MemberInfo>();
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <value>
        /// The constructor.
        /// </value>
        public Func<T> Constructor { get; private set; }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <value>
        /// The constructor.
        /// </value>
        Func<object> IFactorySpec.Constructor
        {
            get
            {
                if (Constructor != null)
                    return () => Constructor();

                return null;
            }
        }

        /// <summary>
        /// Gets the member generators.
        /// </summary>
        /// <value>
        /// The member generators.
        /// </value>
        public IList<PropertyGenerator> MemberGenerators { get { return generators; } }

        /// <summary>
        /// Gets the temporary properties generators.
        /// </summary>
        /// <value>
        /// The temporary properties.
        /// </value>
        public IList<PropertyGenerator> TemporaryProperties { get { return tempProperties; } }

        /// <summary>
        /// Gets the custom action.
        /// </summary>
        /// <value>
        /// The custom action.
        /// </value>
        public Action<ObjectCreationContext> CustomAction { get { return PostAction; } }

        /// <summary>
        /// Gets the ignored members.
        /// </summary>
        /// <value>
        /// The ignored members.
        /// </value>
        public IList<MemberInfo> IgnoredMembers { get { return ignoredMembers; } }

        /// <summary>
        /// Gets the custom action.
        /// </summary>
        /// <value>
        /// The custom action.
        /// </value>
        Action<ObjectCreationContext> IFactorySpec.CustomAction
        {
            get { return ctx => CustomAction(ctx); }
        }

        /// <summary>
        /// Specifies factory method needed to create an object.
        /// </summary>
        /// <param name="constructor">The constructor func.</param>
        protected void ConstructWith(Func<T> constructor)
        {
            Constructor = constructor;
        }

        /// <summary>
        /// Specifies public property or field
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="memberExp">The member access expression.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        protected IPropertySpec<T> For<TProperty>(Expression<Func<T, TProperty>> memberExp)
        {
            if (memberExp.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();

            var accessExp = memberExp.Body as MemberExpression;
            return new PropertySpec<T>(accessExp.Member);
        }

        /// <summary>
        /// Specifies public property or field
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="name">The name of public property or field</param>
        /// <returns></returns>
        protected IPropertySpec For(Type propertyType, string name)
        {
            var specType = typeof(PropertySpec<>).MakeGenericType(propertyType);

            var propInfo = typeof(T).GetMember(name);

            return Activator.CreateInstance(specType, propInfo) as IPropertySpec;
        }

        /// <summary>
        /// Ignores the specified public property or field
        /// </summary>
        /// <typeparam name="TProperty">The type of the property or field.</typeparam>
        /// <param name="memberExp">The member access expression</param>
        /// <exception cref="System.ArgumentException"></exception>
        protected void Ignore<TProperty>(Expression<Func<T, TProperty>> memberExp)
        {
            if (memberExp.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();

            var accessExp = memberExp.Body as MemberExpression;
            if (accessExp != null)
                ignoredMembers.Add(accessExp.Member);
        }

        /// <summary>
        /// Configures properties' generators.
        /// </summary>
        /// <param name="generators">The generators.</param>
        protected void ConfigureProperties(params PropertyGenerator[] generators)
        {
            this.generators.AddRange(generators);
        }

        /// <summary>
        /// Configures the temporary properties generators.
        /// </summary>
        /// <param name="generators">The generators.</param>
        protected void ConfigureTemporaryProperties(params PropertyGenerator[] generators)
        {
            this.tempProperties.AddRange(generators);
        }

        /// <summary>
        /// Custom action to be executed after object is created
        /// </summary>
        /// <param name="obj">The created object.</param>
        /// <param name="creationContext">The creation context.</param>
        protected virtual void PostAction(ObjectCreationContext creationContext)
        {

        }
    }
}
