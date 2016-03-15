using Moq;
using NUnit.Framework;
using PersistenceMap.Interception;
using PersistenceMap.QueryBuilder;
using PersistenceMap.QueryParts;

namespace PersistenceMap.UnitTest.Interception
{
    [TestFixture]
    public class InterceptionHandlerTests
    {
        [Test]
        public void PersistenceMap_Interception_InterceptionHandler_HandleBeforeCompile_Test()
        {
            var collection = new InterceptorCollection();
            var context = new Mock<IDatabaseContext>();
            var container = new QueryPartsContainer();

            var interceptor = new Mock<IInterceptor<Warrior>>();
            collection.Add(interceptor.Object);

            var handler = new InterceptionHandler(collection, typeof(Warrior), context.Object);
            handler.HandleBeforeCompile(container);

            interceptor.Verify(exp => exp.VisitBeforeCompile(It.Is<IQueryPartsContainer>(c => c == container)), Times.Once);
            interceptor.Verify(exp => exp.VisitBeforeExecute(It.IsAny<CompiledQuery>(), It.Is<IDatabaseContext>(c => c == context.Object)), Times.Never);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptionHandler_HandleBeforeExecute_Test()
        {
            var collection = new InterceptorCollection();
            var context = new Mock<IDatabaseContext>();

            var query = new CompiledQuery();

            var interceptor = new Mock<IInterceptor>();
            collection.Add<Warrior>(interceptor.Object);

            var handler = new InterceptionHandler(collection, typeof(Warrior), context.Object);
            handler.HandleBeforeExecute(query);

            interceptor.Verify(exp => exp.VisitBeforeCompile(It.IsAny<IQueryPartsContainer>()), Times.Never);
            interceptor.Verify(exp => exp.VisitBeforeExecute(It.Is<CompiledQuery>(c => c == query), It.Is<IDatabaseContext>(c => c == context.Object)), Times.Once);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptionHandler_Generic_HandleBeforeCompile_Test()
        {
            var collection = new InterceptorCollection();
            var context = new Mock<IDatabaseContext>();
            var container = new QueryPartsContainer();

            var interceptor = new Mock<IInterceptor<Warrior>>();
            collection.Add(interceptor.Object);

            var handler = new InterceptionHandler<Warrior>(collection, context.Object);
            handler.HandleBeforeCompile(container);

            interceptor.Verify(exp => exp.VisitBeforeCompile(It.Is<IQueryPartsContainer>(c => c == container)), Times.Once);
            interceptor.Verify(exp => exp.VisitBeforeExecute(It.IsAny<CompiledQuery>(), It.Is<IDatabaseContext>(c => c == context.Object)), Times.Never);
        }

        [Test]
        public void PersistenceMap_Interception_InterceptionHandler_Generic_HandleBeforeExecute_Test()
        {
            var collection = new InterceptorCollection();
            var context = new Mock<IDatabaseContext>();

            var query = new CompiledQuery();

            var interceptor = new Mock<IInterceptor>();
            collection.Add<Warrior>(interceptor.Object);

            var handler = new InterceptionHandler<Warrior>(collection, context.Object);
            handler.HandleBeforeExecute(query);

            interceptor.Verify(exp => exp.VisitBeforeCompile(It.IsAny<IQueryPartsContainer>()), Times.Never);
            interceptor.Verify(exp => exp.VisitBeforeExecute(It.Is<CompiledQuery>(c => c == query), It.Is<IDatabaseContext>(c => c == context.Object)), Times.Once);
        }

        public class Warrior
        {
        }
    }
}
