using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    public class Dummy
    {
        private readonly DummyConfig config;

        public Dummy(DummyConfig config)
        {
            this.config = config;
        }

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

        public IFactory<T> GetFactory<T>()
        {
            return GetFactory(typeof (T)) as IFactory<T>;
        }
    }

    public class DummyConfig
    {
        private IDictionary<Type, TypeConfig> configTable;

        public DummyConfig()
        {
            configTable = new Dictionary<Type, TypeConfig>();
            MaxDepth = 3;
            DefaultListCount = 3;
        }

        public DummyConfig Configure<T>(IFactorySpec<T> spec)
        {
            configTable[typeof (T)] = new TypeConfig() {Spec = spec};
            return this;
        }

        public DummyConfig Configure(Type type, IFactorySpec spec)
        {
            configTable[type] = new TypeConfig() { Spec = spec };
            return this;
        }

        public DummyConfig Configure<T, TSpec>() where TSpec : IFactorySpec<T>, new()
        {
            configTable[typeof(T)] = new TypeConfig() { Spec = new TSpec() };
            return this;
        }

        public DummyConfig ConfigureFactory<T>(IFactory<T> factory)
        {
            configTable[typeof(T)] = new TypeConfig() { Factory = factory};
            return this;
        }

        public DummyConfig ConfigureFactory<T, TFactory>() where TFactory : IFactory<T>, new()
        {
            configTable[typeof(T)] = new TypeConfig() { Factory = new TFactory() };
            return this;
        }

        public DummyConfig ConfigureFactory(Type type, IFactory factory)
        {
            configTable[type] = new TypeConfig() {Factory = factory};
            return this;
        }

        internal IDictionary<Type, TypeConfig> ConfigTable
        {
            get { return configTable; }
        }

        public int MaxDepth { get; set; }

        public int DefaultListCount { get; set; }
    }

    internal class TypeConfig
    {
        public IFactory Factory { get; set; }

        public IFactorySpec Spec { get; set; }
    }
}
