using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDummy.Factories;

namespace NDummy
{
    /// <summary>
    /// Provides a way to get factory by its spec 
    /// I can name it to more reasonable name but why not use a weird name here
    /// </summary>
    public class FactoryManager
    {
        private readonly FactoriesConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryManager"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public FactoryManager(FactoriesConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Type passed does not spec</exception>
        public IFactory GetFactory(Type type)
        {
            if(! config.ConfigTable.ContainsKey(type))
                throw new ArgumentException("Type passed does not spec");

            if (config.ConfigTable[type].Factory != null)
            {
                return config.ConfigTable[type].Factory;
            }

            var factoryType = typeof (Factory<>).MakeGenericType(type);
            return Activator.CreateInstance(factoryType, config) as IFactory;
        }

        /// <summary>
        /// Gets the factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IFactory<T> GetFactory<T>()
        {
            return GetFactory(typeof (T)) as IFactory<T>;
        }
    }

    /// <summary>
    /// This class holds types configuration
    /// </summary>
    public class FactoriesConfig
    {
        private IDictionary<Type, TypeConfig> configTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoriesConfig"/> class.
        /// </summary>
        public FactoriesConfig()
        {
            configTable = new Dictionary<Type, TypeConfig>();
            MaxDepth = 3;
            ListCount = 3;
        }

        /// <summary>
        /// Configures the specified spec with spec.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public FactoriesConfig Configure<T>(IFactorySpec<T> spec)
        {
            configTable[typeof (T)] = new TypeConfig() {Spec = spec};
            return this;
        }

        /// <summary>
        /// Configures the specified type with spec.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="spec">The spec.</param>
        /// <returns></returns>
        public FactoriesConfig Configure(Type type, IFactorySpec spec)
        {
            configTable[type] = new TypeConfig() { Spec = spec };
            return this;
        }

        /// <summary>
        /// Configures the specified type with spec.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TSpec">The type of the spec.</typeparam>
        /// <returns></returns>
        public FactoriesConfig Configure<T, TSpec>() where TSpec : IFactorySpec<T>, new()
        {
            configTable[typeof(T)] = new TypeConfig() { Spec = new TSpec() };
            return this;
        }

        /// <summary>
        /// Configures the specified type with factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public FactoriesConfig ConfigureFactory<T>(IFactory<T> factory)
        {
            configTable[typeof(T)] = new TypeConfig() { Factory = factory};
            return this;
        }

        /// <summary>
        /// Configures the specified type with factory.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TFactory">The type of the factory.</typeparam>
        /// <returns></returns>
        public FactoriesConfig ConfigureFactory<T, TFactory>() where TFactory : IFactory<T>, new()
        {
            configTable[typeof(T)] = new TypeConfig() { Factory = new TFactory() };
            return this;
        }

        /// <summary>
        /// Configures the specified type with factory.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        public FactoriesConfig ConfigureFactory(Type type, IFactory factory)
        {
            configTable[type] = new TypeConfig() {Factory = factory};
            return this;
        }

        /// <summary>
        /// Gets the configuration table.
        /// </summary>
        /// <value>
        /// The configuration table.
        /// </value>
        internal IDictionary<Type, TypeConfig> ConfigTable
        {
            get { return configTable; }
        }

        /// <summary>
        /// Gets or sets the default maximum level of object creation.
        /// </summary>
        /// <value>
        /// The maximum depth.
        /// </value>
        public int MaxDepth { get; set; }

        /// <summary>
        /// Gets or sets the default list count.
        /// </summary>
        /// <value>
        /// The default list count.
        /// </value>
        public int ListCount { get; set; }
    }

    /// <summary>
    /// This class holds type specific configuration
    /// </summary>
    internal class TypeConfig
    {
        /// <summary>
        /// Gets or sets the factory.
        /// </summary>
        /// <value>
        /// The factory.
        /// </value>
        public IFactory Factory { get; set; }

        /// <summary>
        /// Gets or sets the spec.
        /// </summary>
        /// <value>
        /// The spec.
        /// </value>
        public IFactorySpec Spec { get; set; }
    }
}
