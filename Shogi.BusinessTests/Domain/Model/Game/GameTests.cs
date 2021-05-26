using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Business.Domain.Model.GameFactorys;

namespace Shogi.Business.Domain.Model.Tests
{
    [TestClass()]
    public class GameTests
    {
        [TestMethod()]
        public void GameTest()
        {
            var factory = new GameFactory();
            var game = factory.Create(GameType.AnimalShogi);
            var cmd = game.CreateAvailableMoveCommand(Player.FirstPlayer);

        }
    }
}