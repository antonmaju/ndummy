using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NDummy.Factories;
using NDummy.Tests.Models;
using Xunit;

namespace NDummy.Tests
{
    public class FactoryTest
    {
        #region object creation

        public bool isConstructorCalled = false;

        class SoldierSpec1 : FactorySpec<Soldier>
        {
            
        }

        class SoldierSpec2 : FactorySpec<Soldier>
        {
            public SoldierSpec2(bool[] signal)
            {
                ConstructWith(() =>
                {
                    signal[0] = true;
                    return new Soldier();
                });
            }
        }

        [Fact]
        public void Should_Create_Object_By_Activator_If_Constructor_Not_Supplied()
        {
            var config = new FactoriesConfig()
                .Configure<Soldier, SoldierSpec1>();

            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(typeof(Soldier), instance.GetType());
        }

        [Fact]
        public void Should_Create_Object_ByFactoryMethod_If_Supplied()
        {
            var signal = new[] {false};
            var config = new FactoriesConfig()
               .Configure<Soldier>(new SoldierSpec2(signal));
            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(typeof(Soldier), instance.GetType());
            Assert.Equal(true, signal[0]);
        }

        [Fact]
        public void Should_Not_Assign_Values_for_Ignored_Members()
        {
            
        }

        #endregion


        #region properties / fields assignment

        class SoldierSpec3 : FactorySpec<Soldier>
        {
            public SoldierSpec3(int[] values)
            {
                ConfigureTemporaryProperties(
                    TemporaryProperty.New<int>("Data1", 9),
                    TemporaryProperty.New<int>("Data2", ctx =>
                    {
                        values[0] = Convert.ToInt32(ctx.TempProperties["Data1"]);
                        return values[0];
                    })
                    );
            }
        }

        class SoldierSpec4 : FactorySpec<Soldier>
        {
            public SoldierSpec4(string name)
            {
                ConfigureProperties(
                    For<string>(s =>s.Name).GenerateWith(name)
                    );
            }
        }

        class SoldierSpec5 : FactorySpec<Soldier>
        {
            public SoldierSpec5(string address)
            {
                ConfigureProperties(
                    For<string>(s => s.address).GenerateWith(address)
                    );
            }
        }

        class SoldierSpec6 : FactorySpec<Soldier>
        {
            public SoldierSpec6()
            {
                Ignore(c => c.Name);
            }
        }

        class LieutenantSpec1 : FactorySpec<Lieutenant>
        {
            public LieutenantSpec1()
            {
                
            }
        }


        [Fact]
        public void Should_Be_Able_To_Assign_Temporary_Property()
        {
            int[] values = new[] {0};
            var config = new FactoriesConfig()
                .Configure(new SoldierSpec3(values));
            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(9, values[0]);
        }

        [Fact]
        public void Should_Be_Able_To_Assign_Public_Property()
        {
            string name = "csharp";
            var config = new FactoriesConfig()
                .Configure(new SoldierSpec4(name));
            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(name, instance.Name);
        }

        [Fact]
        public void Should_Be_Able_To_Assign_Public_Field()
        {
            string address = "somewhere";
            var config = new FactoriesConfig()
                .Configure(new SoldierSpec5(address));
            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(address, instance.address);
        }


        [Fact]
        public void Should_Not_Assign_Values_To_IgnoredMembers()
        {
            string value = "String1";
            var config = new FactoriesConfig()
                .ConfigureFactory(new FactoryMethod<string>(() => value))
                .Configure(new SoldierSpec6());
            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(value, instance.address);
            Assert.Null(instance.Name);
        }

        [Fact]
        public void Should_Use_Factory_If_MemberType_Exists_In_Config()
        {
            string value = "String1";
            var config = new FactoriesConfig()
                .ConfigureFactory(new FactoryMethod<string>(() => value))
                .Configure(new SoldierSpec1());

            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(value, instance.address);
            Assert.Equal(value,instance.Name);
        }

        [Fact]
        public void Should_Use_Factory_If_MemberTypeSpec_Exists_In_Config()
        {
            string value = "js";
            var config = new FactoriesConfig()
            .Configure<Lieutenant, LieutenantSpec1>()
            .Configure(new SoldierSpec4(value));

            var factory = new Factory<Lieutenant>(config);
            var instance = factory.Create();
            Assert.NotNull(instance.Soldier);
            Assert.Equal(value, instance.Soldier.Name);

        }

        #endregion

        #region list creation


        class CaptainSpec1 : FactorySpec<Captain>
        {
            public CaptainSpec1()
            {
            }
        }

        class SoldierSpec7 : FactorySpec<Soldier>
        {
            public SoldierSpec7()
            {
                ConfigureProperties(
                     For<int>(c =>c.Id).GenerateWith(ctx => ctx.Index),
                    For<string>(c =>c.Name).GenerateWith(ctx =>"Soldier-"+ ctx.Index),
                    For<string>(c =>c.address).GenerateWith(ctx=>"No "+ctx.Index)
                    );
            }
        }

        [Fact]
        public void Should_Be_Able_To_Create_Enumerable_If_Type_Exists_In_Config()
        {
            var config = new FactoriesConfig()
                .Configure<Soldier, SoldierSpec7>()
                .Configure<Captain, CaptainSpec1>();

            var factory = new Factory<Captain>(config);
            var instance = factory.Create();

            Assert.NotNull(instance.Soldiers);
            Assert.Equal(config.ListCount, instance.Soldiers.Count());

            foreach (var soldier in instance.Soldiers)
            {
                int id = soldier.Id;
                Assert.Equal("Soldier-"+id, soldier.Name);
                Assert.Equal("No " + id, soldier.address);
            }
        }

        #endregion

        #region multi level object  creation

        class FriendlySoldierSpec1 : FactorySpec<FriendlySoldier>
        {
            public FriendlySoldierSpec1()
            {
                
            }
        }

        [Fact]
        public void Should_Be_Able_To_Create_MultiLevel()
        {
            string value = "String1";
            var config = new FactoriesConfig()
                .ConfigureFactory(new FactoryMethod<string>(() => value))
                .Configure(new FriendlySoldierSpec1());
            var factory = new Factory<FriendlySoldier>(config);
            var instance = factory.Create();
            var level2Soldier = instance.Friend;
            Assert.NotNull(level2Soldier);
            Assert.Equal(value, level2Soldier.Name);
            var level3Soldier = level2Soldier.Friend;
            Assert.NotNull(level3Soldier);
            Assert.Equal(value, level3Soldier.Name);
        }

        [Fact]
        public void Should_Not_Create_Chained_Object_More_Than_Allowed_Level()
        {
            const string value = "String1";
            var config = new FactoriesConfig()
                .ConfigureFactory(new FactoryMethod<string>(() => value))
                .Configure(new FriendlySoldierSpec1());
            config.MaxDepth = 2;
            var factory = new Factory<FriendlySoldier>(config);
            var instance = factory.Create();
            var level2Soldier = instance.Friend;
            Assert.NotNull(level2Soldier);
            Assert.Equal(value, level2Soldier.Name);
            Assert.Null(level2Soldier.Friend);
        }

        #endregion

        #region post action

        public class SoldierSpec8 : FactorySpec<Soldier>
        {
            private Action<ObjectCreationContext> action;

            public SoldierSpec8(Action<ObjectCreationContext> action)
            {
                this.action = action;
            }

            protected override void PostAction(ObjectCreationContext creationContext)
            {
                action(creationContext);
            }
        }

        [Fact]
        public void Should_Invoke_Post_Action()
        {
            bool invoked = false;
            Action<ObjectCreationContext> action = ctx => { invoked = true; };
            var config = new FactoriesConfig()
                .Configure(new SoldierSpec8(action));
            var factory = new Factory<Soldier>(config);
            var instance = factory.Create();
            Assert.Equal(true, invoked);
        }

        #endregion
    }
}
