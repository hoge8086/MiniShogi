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
            GameJsonFactory.CreateGame("games.json");
        }

        [TestMethod()]
        public void SaveTest()
        {
            GameJsonFactory.CreateKoma("komas.json");
        }
    }
}