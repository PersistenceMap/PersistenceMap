using Microsoft.VisualStudio.TestTools.UnitTesting;
//using NUnit.Framework;
using PersistanceMap.Test.LocalDb;
using PersistanceMap.Test.TableTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PersistanceMap.Test.Expression
{
    //[TestFixture]
    [TestClass]
    public class IntegrationTests : TestBase
    {
        //[Test]
        [TestMethod]
        public void LocalDbTest()
        {
            using (var manager = new LocalDbManager("Northwind"))
            {
                var file = new FileInfo(@"AppData\Nothwind.SqlServer.sql");

                //var file = new FileInfo(@" C:\Source\Repos\Educase 2.0 Prototype\Prototypes\Basenet.Education.People\northwind.sql");
                string script = file.OpenText().ReadToEnd();
                manager.ExecuteString(script);
            }
        }
    }
}
