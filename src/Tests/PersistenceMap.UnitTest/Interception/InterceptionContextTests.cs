using NUnit.Framework;
using PersistenceMap.Interception;
using System.Diagnostics;
using System.Linq;

namespace PersistenceMap.UnitTest.Interception
{
    [TestFixture]
    public class InterceptionContextTests
    {
        [Test]
        public void PersistenceMap_Interception_InterceptionContext_Interceptors_Test()
        {
            var collection = new InterceptorCollection();

            var context = new InterceptionContext<Warrior>(collection);

            Assert.AreSame(context.Interceptors, collection);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptionContext_BeforeCompile_Test()
        {
            var collection = new InterceptorCollection();

            var context = new InterceptionContext<Warrior>(collection);
            context.BeforeCompile(q => Debug.WriteLine(q.AggregatePart.OperationType));

            Assert.That(collection.GetInterceptor<Warrior>(), Is.Not.Null);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptionContext_BeforeExecute_Test()
        {
            var collection = new InterceptorCollection();

            var context = new InterceptionContext<Warrior>(collection);
            context.BeforeExecute(q => Debug.WriteLine(q.QueryString));

            Assert.That(collection.GetInterceptor<Warrior>(), Is.Not.Null);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptionContext_Fluent_Test()
        {
            var collection = new InterceptorCollection();

            var context = new InterceptionContext<Warrior>(collection)
                .BeforeExecute(q => Debug.WriteLine(q.QueryString))
                .BeforeCompile(q => Debug.WriteLine(q.AggregatePart.OperationType));

            Assert.That(collection.GetInterceptors<Warrior>().Count() == 2);
        }

        private class Warrior
        {
        }
    }
}
