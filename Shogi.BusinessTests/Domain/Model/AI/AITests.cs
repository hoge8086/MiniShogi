using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.GameFactorys;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Business.Domain.Model.AI.Tests
{
    [TestClass()]
    public class AITests
    {
        [TestMethod()]
        public void PlayTest()
        {
               var game= new Game(
                    new Board(4, 3),
                    new GameState(new List<Koma>()
                        {
                            new Koma(PlayerType.SecondPlayer, GameFactory.KomaRaion, new OnBoard(new BoardPosition(1,0))),
                            new Koma(PlayerType.SecondPlayer, GameFactory.KomaKirin, new OnBoard(new BoardPosition(0,1))),
                            new Koma(PlayerType.SecondPlayer, GameFactory.KomaZou, new OnBoard(new BoardPosition(2,0))),
                            new Koma(PlayerType.SecondPlayer, GameFactory.KomaHiyoko, InHand.State),
                            new Koma(PlayerType.FirstPlayer, GameFactory.KomaHiyoko, InHand.State),
                            new Koma(PlayerType.FirstPlayer, GameFactory.KomaRaion, new OnBoard(new BoardPosition(1,2))),
                            new Koma(PlayerType.FirstPlayer, GameFactory.KomaKirin, new OnBoard(new BoardPosition(2,3))),
                            new Koma(PlayerType.FirstPlayer, GameFactory.KomaZou, new OnBoard(new BoardPosition(0,3))),
                        },
                        PlayerType.FirstPlayer
                    ),
                    new CustomRule(
                        1,
                        new NullProhibitedMoveSpecification(),
                        new MultiWinningChecker(new List<IWinningChecker>()
                        {
                            new TakeKingWinningChecker(),
                            new EnterOpponentTerritoryWinningChecker(),
                        })
                    )) ;
            var ai = new NegaAlphaAI(7);
            //ai.Play(game, null);
        }
    }
}