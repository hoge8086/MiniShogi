using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Moves;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Bussiness.Domain.Model.Tests
{
    [TestClass()]
    public class GameTests
    {
        [TestMethod()]
        public void GameTest()
        {
            KomaType hiyoko = new KomaType(
                "ひ",
                new Moves.KomaMoves(new List<Moves.IKomaMove>()
                {
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(0, -1)),
                }),
                new Moves.KomaMoves(new List<Moves.IKomaMove>()
                {
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(0, 1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(0, -1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(1, 0)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(1, 1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(1, -1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(-1, 0)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(-1, 1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(-1, -1)),
                }
                ));
            KomaType kyou = new KomaType(
                "香",
                new Moves.KomaMoves(new List<Moves.IKomaMove>()
                {
                    new StraightKomaMove(new Boards.RelativeBoardPosition(0, -1)),
                }),
                new Moves.KomaMoves(new List<Moves.IKomaMove>()
                {
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(0, 1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(0, -1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(1, 0)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(-1, 0)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(-1, 1)),
                    new PinpointKomaMove(new Boards.RelativeBoardPosition(-1, -1)),
                }
                ));
            var game = new Game(
                new Boards.Board(4, 3),
                new GameState(
                    new List<Koma>()
                    {
                        new Koma(new BoardPosition(1, 1), Player.FirstPlayer, hiyoko),
                        new Koma(new BoardPosition(0, 3), Player.FirstPlayer, kyou),
                        new Koma(HandPosition.Hand, Player.FirstPlayer, hiyoko),
                        new Koma(new BoardPosition(0, 0), Player.SecondPlayer, hiyoko),
                    },
                    Player.FirstPlayer
                ),
                new Rule(1));

            var stage = game.ToString();
            var command = game.GetAvailableMoveCommand(Player.FirstPlayer);
            game.Play(command[1]);
            var stage2 = game.ToString();
            var command2 = game.GetAvailableMoveCommand(Player.FirstPlayer);

        }
    }
}