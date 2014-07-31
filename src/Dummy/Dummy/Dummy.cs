using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dummy.Factories;

namespace Dummy
{
    public class Dummy
    {
        public Dummy(DummyConfig config)
        {
            
        }

        public IFactory GetFactory(Type type)
        {
            
            return null;
        }

        public IFactory<T> GetFactory<T>()
        {
            return null;
        }
    }

    public class DummyConfig
    {
        private IDictionary<Type, TypeConfig> configTable;

        public DummyConfig()
        {
            configTable = new Dictionary<Type, TypeConfig>();   
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

        public DummyConfig Configure<T>(IFactory<T> factory)
        {
            configTable[typeof(T)] = new TypeConfig() { Factory = factory};
            return this;
        }

        public DummyConfig Configure(Type type, IFactory factory)
        {
            configTable[type] = new TypeConfig() {Factory = factory};
            return this;
        }

        internal IDictionary<Type, TypeConfig> ConfigTable
        {
            get { return configTable; }
        }
    }

    internal class TypeConfig
    {
        public IFactory Factory { get; set; }

        public IFactorySpec Spec { get; set; }
    }
}
