using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Dummy.Tests.Models;
using Xunit;

namespace Dummy.Tests
{
    
    public class FactorySpecTest : FactorySpec<Soldier>
    {
        private Action<Soldier, ObjectCreationContext> PostAction;

        protected override void DoAfter(Soldier obj, ObjectCreationContext tempProperties)
        {
            PostAction(obj, tempProperties);
        }

        [Fact]
        public void CanSetConstructor()
        {
            Func<Soldier> constructor = ()=> new Soldier();
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
            Assert.Contains(propGenerator, MemberGenerators);
        }

        [Fact]
        public void CanAddTempProperties()
        {
            var tempProperty = TemporaryProperty.New<string>("element", "fire");
            ConfigureTemporaryProperties(tempProperty);
            Assert.Contains(tempProperty, TemporaryProperties);
        }

        [Fact]
        public void CanAddCustomAction()
        {
            int i = 1;
            PostAction = (w, dict) =>
            {
                i++;
            };

            CustomAction(new Soldier(), null);
            Assert.Equal(2,i);
        }

    }

   
}
