using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shogi.Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;


namespace Shogi.Business.Infrastructure.Tests
{
    [TestClass()]
    public class JsonRepositoryTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            //try
            //{
                GameJsonFactory.Create("games.json");
            //}catch(Exception ex)
            //{ }
        }

        [TestMethod()]
        public void SaveTest()
        {
            Assert.Fail();
        }
    }
}