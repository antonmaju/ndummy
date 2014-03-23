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
        IPropertySpec For(Type propertyType, string name);
    }

    public interface IFactorySpec<T> : IFactorySpec
    {
        IPropertySpec<T> For<TProperty>(Expression<Func<T, TProperty>> memberExp);
    }

    public abstract class FactorySpec<T> : IFactorySpec<T>
    {
        private readonly List<PropertyFactoryInfo> propertyFactories; 

        protected FactorySpec()
        {
            propertyFactories = new List<PropertyFactoryInfo>();
        }
            
        public IPropertySpec<T> For<TProperty>(Expression<Func<T, TProperty>> memberExp)
        {
            if (memberExp.Body.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();

            var accessExp = memberExp.Body as MemberExpression;
            return new PropertySpec<T>(accessExp.Member);
        } 

        public IPropertySpec For(Type propertyType, string name)
        {
            var specType = typeof (PropertySpec<>).MakeGenericType(propertyType);

            var propInfo = typeof (T).GetMember(name);

            return Activator.CreateInstance(specType, propInfo) as IPropertySpec;
        }    

        public void Configure(IList<PropertyFactoryInfo> list)
        {
            propertyFactories.AddRange(list);
        }
    }
}
