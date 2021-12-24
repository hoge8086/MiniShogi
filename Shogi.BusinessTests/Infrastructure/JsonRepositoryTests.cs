using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shogi.Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

using Shogi.Business.Domain.Model.GameFactorys;
using Shogi.Business.Domain.Model.GameTemplates;

namespace Shogi.Business.Infrastructure.Tests
{
    [TestClass()]
    public class JsonRepositoryTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            var factory = new GameFactory();
            var template = new GameTemplate() { Name = "どうぶつ将棋", Game = factory.Create(GameType.AnimalShogi) };
            var template2 = new GameTemplate() { Name = "5五将棋", Game = factory.Create(GameType.FiveFiveShogi) };
            var repo = new GameTemplateJsonRepository("games.json");
            repo.Add(template);
            repo.Add(template2);
            var names = repo.FindAllName();
            var temp = repo.FindByName("動物将棋");
        }

        [TestMethod()]
        public void SaveTest()
        {
            Assert.Fail();
        }
    }
}