using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Dummy.Tests
{
    
    public class FactorySpecTest : FactorySpec<Wanderer>
    {

        private Action<Wanderer> PostAction; 

        protected override void DoAfter(Wanderer obj)
        {
            PostAction(obj);
        }

        [Fact]
        public void CanSetConstructor()
        {
            Func<Wanderer> constructor = ()=> new Wanderer();
            ConstructWith(constructor);
            Assert.Equal(constructor, Constructor);
        }

        [Fact]
        public void GenericForShouldReturnCorrectMemberInfo()
        {
            var propGenerator = For<string>(s => s.Name).GenerateWith("Hello");
            Assert.Equal("Name", propGenerator.MemberInfo.Name);
        }

        [Fact]
        public void ForShouldReturnCorrectMemberInfo()
        {
            var propGenerator = For(typeof(int),"Age").GenerateWith(12);
            Assert.Equal("Age", propGenerator.MemberInfo.Name);
        }

        [Fact]
        public void CanAddPropertyGenerator()
        {
            var propGenerator= For<string>(s => s.Name).GenerateWith("Hello");
            ConfigureProperties(propGenerator);
            Assert.Contains(propGenerator, PropertyGenerators);
        }

        [Fact]
        public void CanAddCustomAction()
        {
            int i = 1;
            PostAction = w =>
            {
                i++;
            };

            CustomAction(new Wanderer());
            Assert.Equal(2,i);
        }

    }

    public class Wanderer
    {
        public string Name { get; set; }

        public int Age { get; set; }

        public bool Gender { get; set; }
    }
}
