using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;

namespace PersistenceMap.SqlServer.Test.Mocked
{
    [TestFixture]
    public class Select
    {
        [Test]
        [NUnit.Framework.Ignore("")]
        public void PersistenceMap_SqlServer_Mocked_Select_WhereConstraintWithStringCondition()
        {
            var provider = new SqlContextProvider("connection");
            provider.Interceptor(() => new
            {
                Name = ""
            }).AsExecute(cq => new List<Warrior>());
            using (var context = provider.Open())
            {
                string result = string.Empty;
                provider.Interceptor(() => new
                {
                    Name = ""
                }).BeforeExecute(cq => result = cq.QueryString);

                context.From<Warrior>()
                    .Where(w => "DATEPART(YYYY,Von) " == DateTime.Now.Month.ToString())
                    .For(()=> new
                    {
                        Name = ""
                    })
                    .Select();

                Assert.That("SELECT Name FROM Warrior WHERE Name = test", Is.EqualTo(result));
            }
        }
    }
}
