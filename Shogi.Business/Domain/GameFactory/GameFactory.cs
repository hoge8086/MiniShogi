using Shogi.Bussiness.Domain.Model;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Moves;
using Shogi.Bussiness.Domain.Model.Players;
using Shogi.Bussiness.Domain.Model.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Business.Domain.GameFactory
{

    public enum GameType
    {
        AnimalShogi,
        FiveFiveShogi
    }


    public class GameFactory
    {
        public readonly KomaType KomaHiyoko = new KomaType(
            "ひ",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
            }),
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public readonly KomaType KomaZou = new KomaType(
            "ぞ",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            false);
        public readonly KomaType KomaKirin = new KomaType(
            "き",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
            }
            ),
            null,
            false);
        public readonly KomaType KomaRaion = new KomaType(
            "ラ",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            true);


        public readonly KomaType KomaHu = new KomaType(
            "歩",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
            }),
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public readonly KomaType KomaOu = new KomaType(
            "王",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            true);
        public readonly KomaType KomaKin = new KomaType(
            "金",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            false);
        public readonly KomaType KomaGin = new KomaType(
            "銀",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public readonly KomaType KomaHisya= new KomaType(
            "飛",
            new KomaMoves(new List<IKomaMove>()
            {
                new StraightKomaMove(new RelativeBoardPosition(0, -1)),
                new StraightKomaMove(new RelativeBoardPosition(0, 1)),
                new StraightKomaMove(new RelativeBoardPosition(1, 0)),
                new StraightKomaMove(new RelativeBoardPosition(-1, 0)),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new StraightKomaMove(new RelativeBoardPosition(0, -1)),
                new StraightKomaMove(new RelativeBoardPosition(0, 1)),
                new StraightKomaMove(new RelativeBoardPosition(1, 0)),
                new StraightKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public readonly KomaType KomaKaku = new KomaType(
            "角",
            new KomaMoves(new List<IKomaMove>()
            {
                new StraightKomaMove(new RelativeBoardPosition(1, -1)),
                new StraightKomaMove(new RelativeBoardPosition(-1, -1)),
                new StraightKomaMove(new RelativeBoardPosition(1, 1)),
                new StraightKomaMove(new RelativeBoardPosition(-1, 1)),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new StraightKomaMove(new RelativeBoardPosition(1, -1)),
                new StraightKomaMove(new RelativeBoardPosition(-1, -1)),
                new StraightKomaMove(new RelativeBoardPosition(1, 1)),
                new StraightKomaMove(new RelativeBoardPosition(-1, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
            }
            ),
            false);

        public readonly KomaType KomaKyousya = new KomaType(
            "香",
            new KomaMoves(new List<IKomaMove>()
            {
                new StraightKomaMove(new RelativeBoardPosition(0, -1)),
            }),
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(0, 1)),
                new PinpointKomaMove(new RelativeBoardPosition(0, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(1, -1)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, 0)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public Game Create(GameType gameType)
        {
            if(gameType == GameType.AnimalShogi)
            {
                return new Game(
                    new Board(4, 3),
                    new GameState( new List<Koma>()
                    {
                        new Koma(new BoardPosition(0,0), Player.SecondPlayer, KomaKirin),
                        new Koma(new BoardPosition(1,0), Player.SecondPlayer, KomaRaion),
                        new Koma(new BoardPosition(2,0), Player.SecondPlayer, KomaZou),
                        new Koma(new BoardPosition(1,1), Player.SecondPlayer, KomaHiyoko),
                        new Koma(new BoardPosition(2,3), Player.FirstPlayer, KomaKirin),
                        new Koma(new BoardPosition(1,3), Player.FirstPlayer, KomaRaion),
                        new Koma(new BoardPosition(0,3), Player.FirstPlayer, KomaZou),
                        new Koma(new BoardPosition(1,2), Player.FirstPlayer, KomaHiyoko),

                    },
                    Player.FirstPlayer
                    ),
                    new CustomRule(1, new List<CustomRule.ProhibitedMoveChecker>()));
            }else if(gameType == GameType.FiveFiveShogi)
            {
                return new Game(
                    new Board(5, 5),
                    new GameState( new List<Koma>()
                    {
                        new Koma(new BoardPosition(0,0), Player.SecondPlayer, KomaHisya),
                        new Koma(new BoardPosition(1,0), Player.SecondPlayer, KomaKaku),
                        new Koma(new BoardPosition(2,0), Player.SecondPlayer, KomaGin),
                        new Koma(new BoardPosition(3,0), Player.SecondPlayer, KomaKin),
                        new Koma(new BoardPosition(4,0), Player.SecondPlayer, KomaOu),
                        new Koma(new BoardPosition(4,1), Player.SecondPlayer, KomaHu),
                        new Koma(new BoardPosition(4,4), Player.FirstPlayer, KomaHisya),
                        new Koma(new BoardPosition(3,4), Player.FirstPlayer, KomaKaku),
                        new Koma(new BoardPosition(2,4), Player.FirstPlayer, KomaGin),
                        new Koma(new BoardPosition(1,4), Player.FirstPlayer, KomaKin),
                        new Koma(new BoardPosition(0,4), Player.FirstPlayer, KomaOu),
                        new Koma(new BoardPosition(0,3), Player.FirstPlayer, KomaHu),

                    },
                    Player.FirstPlayer
                    ),
                    new CustomRule(1,
                        new List<CustomRule.ProhibitedMoveChecker>()
                        {
                            // [二歩]
                            (moveCommand, game) =>
                            {
                                return (moveCommand is HandKomaMoveCommand) && 
                                       game.State.KomaList.Any(x =>
                                                        x.Player == moveCommand.Player &&
                                                        x.KomaType == KomaHu &&
                                                        !x.IsTransformed &&
                                                        (x.Position is BoardPosition) &&
                                                        ((BoardPosition)x.Position).X == moveCommand.ToPosition.X);
                            },
                            // [打ち歩詰め]
                            (moveCommand, game) =>
                            {
                                return (moveCommand is HandKomaMoveCommand) &&
                                       ((HandKomaMoveCommand)moveCommand).KomaType == KomaHu &&
                                       game.Clone().Play(moveCommand).DoCheckmate(moveCommand.Player);
                                       //game.IsOte(new Koma(moveCommand.ToPosition, moveCommand.Player, KomaHu));
                            },
                            // [行き所のない駒]
                            (moveCommand, game) =>
                            {
                                var fromKoma = moveCommand.FindFromKoma(game.State);
                                // [その駒以外に他の駒が一つもないボードで動けるところが何もない場合は、行き場のない駒]
                                return new Koma(moveCommand.ToPosition, moveCommand.Player, moveCommand.FindFromKoma(game.State).KomaType, (moveCommand.DoTransform || fromKoma.IsTransformed))
                                            .GetMovableBoardPositions(game.Board, new BoardPositions(), new BoardPositions())
                                            .Positions.Count == 0;
                            },
                            // [王手放置]
                            (moveCommand, game) =>
                            {
                                return game.Clone().Play(moveCommand).DoOte(moveCommand.Player.Opponent);
                            },
                        })
                    );
            }

            return null;

        }
    }
}
