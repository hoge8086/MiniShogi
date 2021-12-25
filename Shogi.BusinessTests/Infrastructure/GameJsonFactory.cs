using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shogi.Business.Domain.Model.GameTemplates;

using Shogi.Business.Infrastructure;

namespace Shogi.Business.Infrastructure.Tests
{
    public class GameJsonFactory
    {
        public static readonly KomaType KomaHiyoko = new KomaType(
            "ひ",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
            }),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public static readonly KomaType KomaZou = new KomaType(
            "ぞ",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            false);
        public static readonly KomaType KomaKirin = new KomaType(
            "き",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
            }
            ),
            null,
            false);
        public static readonly KomaType KomaRaion = new KomaType(
            "ラ",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            true);


        public static readonly KomaType KomaHu = new KomaType(
            "歩",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
            }),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false, true);
        public static readonly KomaType KomaOu = new KomaType(
            "王",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            true);
        public static readonly KomaType KomaKin = new KomaType(
            "金",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            null,
            false);
        public static readonly KomaType KomaGin = new KomaType(
            "銀",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public static readonly KomaType KomaHisya= new KomaType(
            "飛",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, -1), true),
                new KomaMoveBase(new RelativeBoardPosition(0, 1), true),
                new KomaMoveBase(new RelativeBoardPosition(1, 0), true),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0), true),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, -1), true),
                new KomaMoveBase(new RelativeBoardPosition(0, 1), true),
                new KomaMoveBase(new RelativeBoardPosition(1, 0), true),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0), true),
                new KomaMoveBase(new RelativeBoardPosition(1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);
        public static readonly KomaType KomaKaku = new KomaType(
            "角",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(1, -1), true),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1), true),
                new KomaMoveBase(new RelativeBoardPosition(1, 1), true),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1), true),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(1, -1), true),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1), true),
                new KomaMoveBase(new RelativeBoardPosition(1, 1), true),
                new KomaMoveBase(new RelativeBoardPosition(-1, 1), true),
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
            }
            ),
            false);

        public static readonly KomaType KomaKyousya = new KomaType(
            "香",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, -1), true),
            }),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
            }
            ),
            false);

        public static readonly KomaType KomaKema = new KomaType(
            "桂",
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(1, -2)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -2)),
            }
            ),
            new KomaMoves(new List<IKomaMove>()
            {
                new KomaMoveBase(new RelativeBoardPosition(0, 1)),
                new KomaMoveBase(new RelativeBoardPosition(0, -1)),
                new KomaMoveBase(new RelativeBoardPosition(1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(1, -1)),
                new KomaMoveBase(new RelativeBoardPosition(-1, 0)),
                new KomaMoveBase(new RelativeBoardPosition(-1, -1)),
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

        public static readonly Game DobutuShogi = new Game(
                    new Board(4, 3),
                    new GameState(new List<Koma>()
                        {
                            new Koma(PlayerType.SecondPlayer, KomaKirin.Id, new OnBoard(new BoardPosition(0,0))),
                            new Koma(PlayerType.SecondPlayer, KomaRaion.Id, new OnBoard(new BoardPosition(1,0))),
                            new Koma(PlayerType.SecondPlayer, KomaZou.Id, new OnBoard(new BoardPosition(2,0))),
                            new Koma(PlayerType.SecondPlayer, KomaHiyoko.Id, new OnBoard(new BoardPosition(1,1))),
                            new Koma(PlayerType.FirstPlayer, KomaKirin.Id, new OnBoard(new BoardPosition(2,3))),
                            new Koma(PlayerType.FirstPlayer, KomaRaion.Id, new OnBoard(new BoardPosition(1,3))),
                            new Koma(PlayerType.FirstPlayer, KomaZou.Id, new OnBoard(new BoardPosition(0,3))),
                            new Koma(PlayerType.FirstPlayer, KomaHiyoko.Id, new OnBoard(new BoardPosition(1,2))),

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
                    ),
                    new List<KomaType>() {KomaKirin, KomaRaion, KomaZou, KomaHiyoko });

        public static readonly Game GoGoShogi = new Game(
                    new Board(5, 5),
                    new GameState(new List<Koma>()
                    {
                        new Koma(PlayerType.SecondPlayer, KomaHisya.Id, new OnBoard(new BoardPosition(0,0))),
                        new Koma(PlayerType.SecondPlayer, KomaKaku.Id, new OnBoard(new BoardPosition(1,0))),
                        new Koma(PlayerType.SecondPlayer, KomaGin.Id, new OnBoard(new BoardPosition(2,0))),
                        new Koma(PlayerType.SecondPlayer, KomaKin.Id, new OnBoard(new BoardPosition(3,0))),
                        new Koma(PlayerType.SecondPlayer, KomaOu.Id, new OnBoard(new BoardPosition(4,0))),
                        new Koma(PlayerType.SecondPlayer, KomaHu.Id, new OnBoard(new BoardPosition(4,1))),
                        new Koma(PlayerType.FirstPlayer, KomaHisya.Id, new OnBoard(new BoardPosition(4,4))),
                        new Koma(PlayerType.FirstPlayer, KomaKaku.Id, new OnBoard(new BoardPosition(3,4))),
                        new Koma(PlayerType.FirstPlayer, KomaGin.Id, new OnBoard(new BoardPosition(2,4))),
                        new Koma(PlayerType.FirstPlayer, KomaKin.Id, new OnBoard(new BoardPosition(1,4))),
                        new Koma(PlayerType.FirstPlayer, KomaOu.Id, new OnBoard(new BoardPosition(0,4))),
                        new Koma(PlayerType.FirstPlayer, KomaHu.Id, new OnBoard(new BoardPosition(0,3))),

                    },
                    PlayerType.FirstPlayer
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
                    ),
                    new List<KomaType>() {KomaHisya, KomaKaku, KomaGin, KomaKin, KomaOu, KomaHu});
        public static void CreateGame(string fileName)
        {
            var repo = new GameTemplateJsonRepository(fileName);
            repo.Add(new GameTemplate() { Name = "どうぶつ将棋", Game = DobutuShogi });
            repo.Add(new GameTemplate() { Name = "5五将棋", Game = GoGoShogi });
        }
        public static void CreateKoma(string fileName)
        {
            var repo = new KomaTypeJsonRepository(fileName);
            repo.Add(KomaHiyoko);
            repo.Add(KomaZou);
            repo.Add(KomaKirin);
            repo.Add(KomaRaion);
            repo.Add(KomaHu);
            repo.Add(KomaOu);
            repo.Add(KomaKin);
            repo.Add(KomaGin);
            repo.Add(KomaHisya);
            repo.Add(KomaKaku);
            repo.Add(KomaKyousya);
            repo.Add(KomaKema);
        }
    }
}

