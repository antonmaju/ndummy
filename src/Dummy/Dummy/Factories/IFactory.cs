using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dummy.Factories
{
    /// <summary>
    /// Interface for factory class
    /// </summary>
    public interface IFactory
    {
        object Create();
    }

    /// <summary>
    /// Interface for generic factory class
    /// </summary>
    public interface IFactory<out T> : IFactory
    {
        new T Create();
    }

    /// <summary>
    /// Generic factory which is responsible for object creation
    /// </summary>
    /// <typeparam name="T">Type to generate</typeparam>
    public class Factory<T> : IFactory<T>
    {
        private readonly DummyConfig config;
        private readonly int level;
        private int counter = 1;

        class GeneratorInfo
        {
            public IDictionary<FieldInfo, PropertyGenerator> FieldGenerators { get; set; }
            public IDictionary<PropertyInfo, PropertyGenerator> PropertyGenerators { get; set; }
        }

        private Lazy<GeneratorInfo> memberGenerator;


        /// <summary>
        /// Initializes a new instance of the <see cref="Factory{T}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public Factory(DummyConfig config)
            : this(config, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Factory{T}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="level">Factory level.</param>
        public Factory(DummyConfig config, int level)
        {
            this.config = config;
            this.level = level;

            memberGenerator = new Lazy<GeneratorInfo>(Prepare);
        }

        /// <summary>
        /// Creates GeneratorInfo which is needed for object creation
        /// </summary>
        /// <returns>GeneratorInfo</returns>
        private GeneratorInfo Prepare()
        {
            Type currentType = typeof(T);
            TypeConfig typeConfig = null;

            if (config.ConfigTable.ContainsKey(currentType))
                typeConfig = config.ConfigTable[currentType];

            IFactorySpec spec = typeConfig.Spec;

            var fieldGenerators = new Dictionary<FieldInfo, PropertyGenerator>();
            var propertyGenerators = new Dictionary<PropertyInfo, PropertyGenerator>();

            var fields = currentType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fields)
            {
                if (spec.IgnoredMembers.Contains(fieldInfo))
                    continue;

                var generator = spec.MemberGenerators.FirstOrDefault(c => c.MemberInfo == fieldInfo);
                if (generator != null)
                    fieldGenerators.Add(fieldInfo, generator);
                else
                {
                    var fieldType = fieldInfo.FieldType;
                    if (config.ConfigTable.ContainsKey(fieldType) && (fieldType.IsSimpleType() || level < config.MaxDepth))
                    {
                        if (config.ConfigTable[fieldType].Factory != null)
                            fieldGenerators.Add(fieldInfo,
                                new PropertyByFactoryGenerator(fieldInfo, config.ConfigTable[fieldType].Factory));
                        else if (config.ConfigTable[fieldType].Spec != null)
                        {
                            var factoryType = typeof(Factory<>).MakeGenericType(fieldType);
                            var factory = Activator.CreateInstance(factoryType, config, level + 1) as IFactory;
                            fieldGenerators.Add(fieldInfo, new PropertyByFactoryGenerator(fieldInfo, factory));
                        }
                    }
                    else if (fieldType.IsGenericType && (fieldType.IsSimpleType() || level < config.MaxDepth))
                    {
                        var arguments = fieldType.GetGenericArguments();
                        if (arguments.Length == 1)
                        {
                            var enumerableType = typeof(IEnumerable<>).MakeGenericType(arguments);
                            if (enumerableType.IsAssignableFrom(fieldType))
                            {
                                var factoryType = typeof(ListFactory<>).MakeGenericType(arguments);
                                var factory = Activator.CreateInstance(factoryType, config, config.ListCount, level + 1) as IFactory;
                                fieldGenerators.Add(fieldInfo, new PropertyByFactoryGenerator(fieldInfo, factory));
                            }
                        }
                    }
                }
            }

            var properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in properties)
            {
                if (spec.IgnoredMembers.Contains(propertyInfo))
                    continue;

                var generator = spec.MemberGenerators.FirstOrDefault(c => c.MemberInfo == propertyInfo);
                if (generator != null)
                    propertyGenerators.Add(propertyInfo, generator);
                else
                {
                    var propertyType = propertyInfo.PropertyType;
                    if (config.ConfigTable.ContainsKey(propertyType) && (propertyType.IsSimpleType() || level < config.MaxDepth))
                    {
                        if (config.ConfigTable[propertyType].Factory != null)
                            propertyGenerators.Add(propertyInfo, new PropertyByFactoryGenerator(propertyInfo, config.ConfigTable[propertyType].Factory));
                        else if (config.ConfigTable[propertyType].Spec != null)
                        {
                            var factoryType = typeof(Factory<>).MakeGenericType(propertyType);
                            var factory = Activator.CreateInstance(factoryType, config, level + 1) as IFactory;
                            propertyGenerators.Add(propertyInfo, new PropertyByFactoryGenerator(propertyInfo, factory));
                        }
                    }
                    else if (propertyType.IsGenericType && (propertyType.IsSimpleType() || level < config.MaxDepth))
                    {
                        var arguments = propertyType.GetGenericArguments();
                        if (arguments.Length == 1)
                        {
                            var enumerableType = typeof(IEnumerable<>).MakeGenericType(arguments);
                            if (enumerableType.IsAssignableFrom(propertyType))
                            {
                                var factoryType = typeof(ListFactory<>).MakeGenericType(arguments);
                                var factory = Activator.CreateInstance(factoryType, config, config.ListCount, level + 1) as IFactory;
                                propertyGenerators.Add(propertyInfo, new PropertyByFactoryGenerator(propertyInfo, factory));
                            }
                        }
                    }
                }
            }

            return new GeneratorInfo
            {
                FieldGenerators = fieldGenerators,
                PropertyGenerators = propertyGenerators
            };
        }

        /// <summary>
        /// Creates instance of T.
        /// </summary>
        /// <returns>new instance</returns>
        /// <exception cref="System.Exception">Type doesn't have spec</exception>
        public T Create()
        {
            //create instance
            //set temp properties
            //initialize public field
            //initialize public property
            //initialize post action
            //return instance

            Type currentType = typeof(T);
            TypeConfig typeConfig = null;

            if (config.ConfigTable.ContainsKey(currentType))
                typeConfig = config.ConfigTable[currentType];
            else
                throw new Exception("Type doesn't have spec");

            var fieldGenerators = memberGenerator.Value.FieldGenerators;
            var propertyGenerators = memberGenerator.Value.PropertyGenerators;

            T instance;

            if (typeConfig != null && typeConfig.Spec.Constructor != null)
                instance = (T)typeConfig.Spec.Constructor.Invoke();
            else
                instance = Activator.CreateInstance<T>();

            var tempGenerators = typeConfig.Spec.TemporaryProperties;
            var tempData = new Dictionary<string, object>();
            var creationContext = new ObjectCreationContext { CurrentObject = instance, TempProperties = tempData, Index = counter };

            if (tempGenerators.Count > 0)
            {
                foreach (var tempGenerator in tempGenerators)
                {
                    tempData.Add(tempGenerator.Name, tempGenerator.Create(creationContext));
                }
            }

            foreach (var pair in fieldGenerators)
            {
                pair.Key.SetValue(instance, pair.Value.Create(creationContext));
            }

            foreach (var pair in propertyGenerators)
            {
                pair.Key.SetValue(instance, pair.Value.Create(creationContext), null);
            }

            //post action
            typeConfig.Spec.CustomAction(creationContext);
            counter++;

            return instance;
        }

        object IFactory.Create()
        {
            return Create();
        }
    }

    /// <summary>
    /// Factory method wrapper
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    public class FactoryMethod<T> : IFactory<T>
    {
        private readonly Func<T> func;

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryMethod{T}"/> class.
        /// </summary>
        /// <param name="func">Func to generate T.</param>
        public FactoryMethod(Func<T> func)
        {
            this.func = func;
        }

        /// <summary>
        /// Creates new instance of T.
        /// </summary>
        /// <returns>new instance of T</returns>
        public T Create()
        {
            return func();
        }

        object IFactory.Create()
        {
            return Create();
        }
    }

    /// <summary>
    /// This class generates list of T
    /// </summary>
    /// <typeparam name="T">Result type</typeparam>
    public class ListFactory<T> : IFactory<IList<T>>
    {
        private IFactory<T> factory;

        private int listCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFactory{T}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public ListFactory(DummyConfig config)
            : this(config, config.ListCount)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFactory{T}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="listCount">The list count.</param>
        public ListFactory(DummyConfig config, int listCount)
            : this(config, config.ListCount, 2)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListFactory{T}"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="listCount">The list count.</param>
        /// <param name="level">The factory level.</param>
        public ListFactory(DummyConfig config, int listCount, int level)
        {
            this.listCount = listCount;

            var typeConfig = config.ConfigTable[typeof(T)];

            if (typeConfig.Factory != null)
                factory = typeConfig.Factory as IFactory<T>;
            else
                factory = new Factory<T>(config, level);
        }

        /// <summary>
        /// Creates list of T.
        /// </summary>
        /// <returns>list of T</returns>
        public IList<T> Create()
        {
            var list = new List<T>();

            for (int i = 0; i < listCount; i++)
                list.Add(factory.Create());

            return list;
        }

        object IFactory.Create()
        {
            return Create();
        }
    }
}
