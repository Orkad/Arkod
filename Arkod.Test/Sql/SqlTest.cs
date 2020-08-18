using Arkod.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NFluent;
using System;
using System.Collections.Generic;
using System.Text;

namespace Arkod.Test.Sql
{
    [TestClass]
    public class SqlTest
    {
        [TestMethod]
        public void StuffTest()
        {
            var script = @"
-- this is an inline comment that should be ignored / ;
/*
this is a multiline comment that should be ignored / ;
*/
create table FOO (myid INT NOT NULL); -- this is an inline comment that should be ignored / ;
";
            Check.That(new SqlStuff().RemoveSqlComments("-- test")).Equals("");
            Check.That(new SqlStuff().RemoveSqlComments(script)).Equals("create table FOO (myid INT NOT NULL)");
        }
    }
}
