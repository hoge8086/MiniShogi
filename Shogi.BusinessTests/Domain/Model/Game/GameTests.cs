using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Moves;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Business.Domain.GameFactory;

namespace Shogi.Bussiness.Domain.Model.Tests
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