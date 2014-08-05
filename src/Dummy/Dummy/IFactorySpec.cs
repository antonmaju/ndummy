using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    public interface IFactorySpec
    {
        IList<PropertyGenerator> TemporaryProperties { get; }
            
        Func<object> Constructor { get; }

        IList<PropertyGenerator> PropertyGenerators { get; }

        Action<object, IDictionary<string, object>> CustomAction { get; }
    }

    public interface IFactorySpec<T> : IFactorySpec
    {
        new Func<T> Constructor { get; }

        new Action<T, IDictionary<string, object>> CustomAction { get; } 
    }

    public abstract class FactorySpec<T> : IFactorySpec<T> where T:class
    {
        private readonly List<PropertyGenerator> generators;
        private readonly List<PropertyGenerator> tempProperties;  

        protected FactorySpec()
        {
            generators = new List<PropertyGenerator>();
            tempProperties = new List<PropertyGenerator>();
        }

        public Func<T> Constructor { get; private set; }

        Func<object> IFactorySpec.Constructor
        {
            get { return ()=> Constructor; }
        }

        public IList<PropertyGenerator> PropertyGenerators { get { return generators; } }

        public IList<PropertyGenerator> TemporaryProperties { get { return tempProperties; } }

        public Action<T, IDictionary<string, object>> CustomAction { get { return DoAfter; } }

        Action<object, IDictionary<string, object>> IFactorySpec.CustomAction
        {
            get { return (o, dict)=>CustomAction((T) o, dict); }
        }

        protected void ConstructWith(Func<T> constructor)
        {
            Constructor = constructor;
        }

        protected IPropertySpec<T> For<TProperty>(Expression<Func<T, TProperty>> memberExp)
        {
            if (memberExp.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();

            var accessExp = memberExp.Body as MemberExpression;
            return new PropertySpec<T>(accessExp.Member);
        } 

        protected IPropertySpec For(Type propertyType, string name)
        {
            var specType = typeof (PropertySpec<>).MakeGenericType(propertyType);

            var propInfo = typeof (T).GetMember(name);

            return Activator.CreateInstance(specType, propInfo) as IPropertySpec;
        }

        protected void ConfigureProperties(params PropertyGenerator[] generators)
        {
            this.generators.AddRange(generators);
        }

        protected void ConfigureTemporaryProperties(params PropertyGenerator[] generators)
        {
            this.tempProperties.AddRange(generators);
        }

        protected virtual void DoAfter(T obj, IDictionary<string, object> tempProperties)
        {
            
        }
    }


}
