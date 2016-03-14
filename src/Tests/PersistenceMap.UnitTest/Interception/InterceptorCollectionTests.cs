using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;

namespace PersistenceMap.UnitTest.Interception
{
    [TestFixture]
    public class InterceptorCollectionTests
    {
        [Test]
        public void PersistenceMap_Interception_InterceptorCollection_Add_Test()
        {
            var interceptor = new Mock<IInterceptor>();

            var collection = new InterceptorCollection();
            var result = collection.Add<Warrior>(interceptor.Object);

            Assert.AreSame(collection.GetInterceptor<Warrior>(), result);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptorCollection_Add_Generic_Test()
        {
            var interceptor = new Mock<IInterceptor<Warrior>>();

            var collection = new InterceptorCollection();
            var result = collection.Add(interceptor.Object);

            Assert.AreSame(collection.GetInterceptor<Warrior>(), result);
        }
        
        [Test]
        public void PersistenceMap_Interception_InterceptorCollection_GetInterceptor_Generic_Test()
        {
            var collection = new InterceptorCollection();
            var interceptor = collection.Add(new Mock<IInterceptor<Warrior>>().Object);

            var result = collection.GetInterceptor<Warrior>();

            Assert.AreSame(interceptor, result);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptorCollection_GetInterceptors_Test()
        {
            var collection = new InterceptorCollection();
            collection.Add(new Mock<IInterceptor<Warrior>>().Object);
            collection.Add(new Mock<IInterceptor<Armour>>().Object);
            collection.Add(new Mock<IInterceptor<Warrior>>().Object);
            collection.Add(new Mock<IInterceptor<Warrior>>().Object);

            var result = collection.GetInterceptors(typeof(Warrior));

            Assert.That(result.Count() == 3);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptorCollection_GetInterceptors_Generic_Test()
        {
            var collection = new InterceptorCollection();
            collection.Add(new Mock<IInterceptor<Warrior>>().Object);
            collection.Add(new Mock<IInterceptor<Armour>>().Object);
            collection.Add(new Mock<IInterceptor<Warrior>>().Object);
            collection.Add(new Mock<IInterceptor<Warrior>>().Object);

            var result = collection.GetInterceptors<Warrior>();

            Assert.That(result.Count() == 3);
        }

        public class Warrior
        {
        }

        public class Armour
        {
        }
    }
}
