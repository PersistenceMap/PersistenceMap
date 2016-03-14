using NUnit.Framework;
using PersistenceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using PersistenceMap.Interception;

namespace PersistenceMap.SqlServer.Test.Mocked
{
    [TestFixture]
    public class Select
    {
        [Test]
        [NUnit.Framework.Ignore("")]
        public void PersistenceMap_SqlServer_Mocked_Select_WhereConstraintWithStringCondition()
        {
            var lst = new List<Warrior>().Select(w => new { Name = w.Name });

            var provider = new SqlContextProvider("connection");
            provider.Interceptor(() => new
            {
                Name = string.Empty
            }).Returns(lst);

            using (var context = provider.Open())
            {
                string result = string.Empty;
                provider.Interceptor(() => new
                {
                    Name = string.Empty
                }).BeforeExecute(cq => result = cq.QueryString);

                context.From<Warrior>()
                    .Where(w => "DATEPART(YYYY,Von) " == DateTime.Now.Month.ToString())
                    .For(() => new
                    {
                        Name = string.Empty
                    })
                    .Select();

                context.From<ObjectWithDate>()
                    .Where(w => SqlFunctions.DatePart("YYYY", w.Date) == DateTime.Now.Month)
                    .For(() => new
                    {
                        Name = string.Empty
                    })
                    .Select();

                Assert.That("SELECT Name FROM Warrior WHERE Name = test", Is.EqualTo(result));
            }
        }

        private class SqlFunctions
        {
            public static int? DatePart(string part, DateTime date)
            {
                throw new NotImplementedException();
            }
        }

        private class ObjectWithDate
        {
            public DateTime Date { get; set; }

            public string Name { get; set; }
        }
    }
}
