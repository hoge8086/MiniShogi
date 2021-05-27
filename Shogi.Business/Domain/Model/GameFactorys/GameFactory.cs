using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Business.Domain.Model.GameFactorys
{

    public enum GameType
    {
        [System.ComponentModel.Description("どうぶつ将棋")]
        AnimalShogi,
        [System.ComponentModel.Description("5五将棋")]
        FiveFiveShogi,
        [System.ComponentModel.Description("9マス将棋(初級1)")]
        KyuMasuShogiLevel01, // [歩と銀]
        [System.ComponentModel.Description("9マス将棋(初級2)")]
        KyuMasuShogiLevel02, // [角と飛車]
        [System.ComponentModel.Description("9マス将棋(中級1)")]
        KyuMasuShogiLevel10, // [歩と持金]
        [System.ComponentModel.Description("9マス将棋(中級2)")]
        KyuMasuShogiLevel11, // [歩のみ]
        [System.ComponentModel.Description("9マス将棋(中級3)")]
        KyuMasuShogiLevel12, // [香と金]
        [System.ComponentModel.Description("9マス将棋(中級4)")]
        KyuMasuShogiLevel13, // [銀と持歩]
        [System.ComponentModel.Description("9マス将棋(上級1)")]
        KyuMasuShogiLevel21, // [角と飛車]
    }


    public class GameFactory
    {
        public static readonly KomaType KomaHiyoko = new KomaType(
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
        public static readonly KomaType KomaZou = new KomaType(
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
        public static readonly KomaType KomaKirin = new KomaType(
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
        public static readonly KomaType KomaRaion = new KomaType(
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


        public static readonly KomaType KomaHu = new KomaType(
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
        public static readonly KomaType KomaOu = new KomaType(
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
        public static readonly KomaType KomaKin = new KomaType(
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
        public static readonly KomaType KomaGin = new KomaType(
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
        public static readonly KomaType KomaHisya= new KomaType(
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
        public static readonly KomaType KomaKaku = new KomaType(
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

        public static readonly KomaType KomaKyousya = new KomaType(
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

        public static readonly KomaType KomaKema = new KomaType(
            "桂",
            new KomaMoves(new List<IKomaMove>()
            {
                new PinpointKomaMove(new RelativeBoardPosition(1, -2)),
                new PinpointKomaMove(new RelativeBoardPosition(-1, -2)),
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
        public readonly CustomRule KyumasuShogiRule = new CustomRule(
            1,
            new MultiProhibitedMoveSpecification(new List<IProhibitedMoveSpecification>()
            {
                    new NiHu(),
                    new CheckmateByHandHu(),
                    new KomaCannotMove(),
                    new LeaveOte()
            }),
            new CheckmateWinningChecker()
            );

        public Game Create(GameType gameType)
        {
            if (gameType == GameType.AnimalShogi)
            {
                return new Game(
                    new Board(4, 3),
                    new GameState(new List<Koma>()
                        {
                            new Koma(Player.SecondPlayer, KomaKirin, new OnBoard(new BoardPosition(0,0))),
                            new Koma(Player.SecondPlayer, KomaRaion, new OnBoard(new BoardPosition(1,0))),
                            new Koma(Player.SecondPlayer, KomaZou, new OnBoard(new BoardPosition(2,0))),
                            new Koma(Player.SecondPlayer, KomaHiyoko, new OnBoard(new BoardPosition(1,1))),
                            new Koma(Player.FirstPlayer, KomaKirin, new OnBoard(new BoardPosition(2,3))),
                            new Koma(Player.FirstPlayer, KomaRaion, new OnBoard(new BoardPosition(1,3))),
                            new Koma(Player.FirstPlayer, KomaZou, new OnBoard(new BoardPosition(0,3))),
                            new Koma(Player.FirstPlayer, KomaHiyoko, new OnBoard(new BoardPosition(1,2))),

                        },
                        Player.FirstPlayer
                    ),
                    new CustomRule(
                        1,
                        new NullProhibitedMoveSpecification(),
                        new MultiWinningChecker(new List<IWinningChecker>()
                        {
                            new TakeKingWinningChecker(),
                            new EnterOpponentTerritoryWinningChecker(),
                        })
                    ));

            } else if (gameType == GameType.FiveFiveShogi)
            {
                return new Game(
                    new Board(5, 5),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaHisya, new OnBoard(new BoardPosition(0,0))),
                        new Koma(Player.SecondPlayer, KomaKaku, new OnBoard(new BoardPosition(1,0))),
                        new Koma(Player.SecondPlayer, KomaGin, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.SecondPlayer, KomaKin, new OnBoard(new BoardPosition(3,0))),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(4,0))),
                        new Koma(Player.SecondPlayer, KomaHu, new OnBoard(new BoardPosition(4,1))),
                        new Koma(Player.FirstPlayer, KomaHisya, new OnBoard(new BoardPosition(4,4))),
                        new Koma(Player.FirstPlayer, KomaKaku, new OnBoard(new BoardPosition(3,4))),
                        new Koma(Player.FirstPlayer, KomaGin, new OnBoard(new BoardPosition(2,4))),
                        new Koma(Player.FirstPlayer, KomaKin, new OnBoard(new BoardPosition(1,4))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,4))),
                        new Koma(Player.FirstPlayer, KomaHu, new OnBoard(new BoardPosition(0,3))),

                    },
                    Player.FirstPlayer
                    ),
                    new CustomRule(
                        1,
                        new MultiProhibitedMoveSpecification(new List<IProhibitedMoveSpecification>()
                        {
                            new NiHu(),
                            new CheckmateByHandHu(),
                            new KomaCannotMove(),
                            new LeaveOte()

                        }),
                        new CheckmateWinningChecker()
                    ));
            }
            else if (gameType == GameType.KyuMasuShogiLevel01)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaHu, new OnBoard(new BoardPosition(0,0))),
                        new Koma(Player.SecondPlayer, KomaGin, new OnBoard(new BoardPosition(1,0))),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaGin, new OnBoard(new BoardPosition(1,2))),
                        new Koma(Player.FirstPlayer, KomaHu, new OnBoard(new BoardPosition(2,2))),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }
            else if (gameType == GameType.KyuMasuShogiLevel02)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaHu, new OnBoard(new BoardPosition(0,0))),
                        new Koma(Player.SecondPlayer, KomaKaku, new OnBoard(new BoardPosition(1,0))),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaHisya, new OnBoard(new BoardPosition(1,2))),
                        new Koma(Player.FirstPlayer, KomaHu, new OnBoard(new BoardPosition(2,2))),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }
            else if (gameType == GameType.KyuMasuShogiLevel10)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaHu, new OnBoard(new BoardPosition(1,1))),
                        new Koma(Player.SecondPlayer, KomaKin, InHand.State),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaKin, InHand.State),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }
            else if (gameType == GameType.KyuMasuShogiLevel11)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaHu, InHand.State),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaHu, InHand.State),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }
            else if (gameType == GameType.KyuMasuShogiLevel12)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaKin, new OnBoard(new BoardPosition(0,0))),
                        new Koma(Player.SecondPlayer, KomaKyousya, new OnBoard(new BoardPosition(1,0))),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaKyousya, new OnBoard(new BoardPosition(1,2))),
                        new Koma(Player.FirstPlayer, KomaKin, new OnBoard(new BoardPosition(2,2))),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }
            else if (gameType == GameType.KyuMasuShogiLevel13)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaGin, new OnBoard(new BoardPosition(0,0))),
                        new Koma(Player.SecondPlayer, KomaHu, InHand.State),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaHu, InHand.State),
                        new Koma(Player.FirstPlayer, KomaGin, new OnBoard(new BoardPosition(2,2))),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }
            else if (gameType == GameType.KyuMasuShogiLevel21)
            {
                return new Game(
                    new Board(3, 3),
                    new GameState(new List<Koma>()
                    {
                        new Koma(Player.SecondPlayer, KomaKema, new OnBoard(new BoardPosition(0,0))),
                        new Koma(Player.SecondPlayer, KomaHu, new OnBoard(new BoardPosition(1,0))),
                        new Koma(Player.SecondPlayer, KomaOu, new OnBoard(new BoardPosition(2,0))),
                        new Koma(Player.FirstPlayer, KomaOu, new OnBoard(new BoardPosition(0,2))),
                        new Koma(Player.FirstPlayer, KomaHu, new OnBoard(new BoardPosition(1,2))),
                        new Koma(Player.FirstPlayer, KomaKema, new OnBoard(new BoardPosition(2,2))),
                    },
                    Player.FirstPlayer
                    ),
                    KyumasuShogiRule);
            }

            return null;

        }

        /// <summary>
        /// 禁じ手：二歩
        /// </summary>
        public class NiHu : IProhibitedMoveSpecification
        {
            public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
            {
                return (moveCommand is HandKomaMoveCommand) && 
                        ((HandKomaMoveCommand)moveCommand).KomaType == GameFactory.KomaHu &&
                       game.State.KomaList.Any(x =>
                                        x.Player == moveCommand.Player &&
                                        x.KomaType == KomaHu &&
                                        !x.IsTransformed &&
                                        x.IsOnBoard &&
                                        x.BoardPosition.X == moveCommand.ToPosition.X);
            }
        }
        /// <summary>
        /// 禁じ手：打ち歩詰め
        /// </summary>
        public class CheckmateByHandHu : IProhibitedMoveSpecification
        {
            public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
            {

                return (moveCommand is HandKomaMoveCommand) &&
                       ((HandKomaMoveCommand)moveCommand).KomaType == KomaHu &&
                       game.Clone().PlayWithoutCheck(moveCommand).DoCheckmateWithoutHandMove(moveCommand.Player);
            }
        }
        /// <summary>
        /// 禁じ手：動かせない駒
        /// </summary>
        public class KomaCannotMove : IProhibitedMoveSpecification
        {
            public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
            {

                var fromKoma = moveCommand.FindFromKoma(game.State);
                // [その駒以外に他の駒が一つもないボードで動けるところが何もない場合は、行き場のない駒]
                return new Koma( moveCommand.Player, moveCommand.FindFromKoma(game.State).KomaType, new OnBoard(moveCommand.ToPosition,(moveCommand.DoTransform || fromKoma.IsTransformed)))
                            .GetMovableBoardPositions(game.Board, new BoardPositions(), new BoardPositions())
                            .Positions.Count == 0;
            }
        }
        /// <summary>
        /// 禁じ手：王手放置
        /// </summary>
        public class LeaveOte: IProhibitedMoveSpecification
        {
            public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
            {
                return game.Clone().PlayWithoutCheck(moveCommand).DoOte(moveCommand.Player.Opponent);
            }
        }
    }
}
