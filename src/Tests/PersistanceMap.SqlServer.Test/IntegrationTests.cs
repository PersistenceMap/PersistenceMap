using NUnit.Framework;
using PersistanceMap.Test.TableTypes;
using System;
using System.Linq;

namespace PersistanceMap.Test.Expression
{
    [TestFixture]
    public class DeleteExpressionTests : TestBase
    {
        [Test]
        [Description("A failing delete satement that defines the deletestatement according to the values from a distinct Keyproperty of a given entity")]
        public void DeleteEntityWithExpressionKeyThatFails()
        {
            Assert.Throws<ArgumentException>(() => new SqlContextProvider(null));
        }

        
    }
}
