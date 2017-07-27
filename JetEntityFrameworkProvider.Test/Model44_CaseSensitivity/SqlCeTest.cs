﻿using System;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JetEntityFrameworkProvider.Test.Model44_CaseSensitivity
{
    [TestClass]
    public class Model44_CaseSensitivityCeTest : Test
    {

        [TestMethod]
        public override void Run()
        {
            // The SQL CE Connection is configured as case sensitive so 
            // this test should work properly
            base.Run();
        }

        protected override DbConnection GetConnection()
        {
            return Helpers.GetSqlCeConnection();
        }

    }
}
