using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public class DefaultGame
    {
        public static readonly KomaType KomaHiyoko = new KomaType(
            new KomaTypeId("ひ", "に", KomaTypeKind.AsHu),
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
            new KomaTypeId("ぞ", KomaTypeKind.None),
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
            new KomaTypeId("き", KomaTypeKind.None),
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
            new KomaTypeId("ラ", KomaTypeKind.AsKing),
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
            new KomaTypeId("歩", "と", KomaTypeKind.AsHu),
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
            new KomaTypeId("王", KomaTypeKind.AsKing),
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
            new KomaTypeId("金", KomaTypeKind.None),
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
            new KomaTypeId("銀", "全", KomaTypeKind.None),
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
            new KomaTypeId("飛", "龍", KomaTypeKind.None),
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
            new KomaTypeId("角", "馬", KomaTypeKind.None),
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
            new KomaTypeId("香", "杏", KomaTypeKind.None),
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
            new KomaTypeId("桂", "圭", KomaTypeKind.None),
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

        public static readonly List<KomaType> DefaltKomaType = new List<KomaType>()
        {
            KomaOu,
            KomaKin,
            KomaGin,
            KomaHu,
            KomaHisya,
            KomaKema,
            KomaKaku,
            KomaKyousya,
            KomaKirin,
            KomaRaion,
            KomaZou,
            KomaHiyoko,
        };

        public static readonly List<GameTemplate> DefaltGameTemplate = new List<GameTemplate>()
        {
            new GameTemplate(
                "どうぶつ将棋", 3, 4, 1, WinConditionType.TakeKingOrEnterOpponentTerritory,
                new List<Koma>()
                {
                    new Koma(PlayerType.Player2, KomaKirin.Id, new OnBoard(new BoardPosition(0,0))),
                    new Koma(PlayerType.Player2, KomaRaion.Id, new OnBoard(new BoardPosition(1,0))),
                    new Koma(PlayerType.Player2, KomaZou.Id, new OnBoard(new BoardPosition(2,0))),
                    new Koma(PlayerType.Player2, KomaHiyoko.Id, new OnBoard(new BoardPosition(1,1))),
                    new Koma(PlayerType.Player1, KomaKirin.Id, new OnBoard(new BoardPosition(2,3))),
                    new Koma(PlayerType.Player1, KomaRaion.Id, new OnBoard(new BoardPosition(1,3))),
                    new Koma(PlayerType.Player1, KomaZou.Id, new OnBoard(new BoardPosition(0,3))),
                    new Koma(PlayerType.Player1, KomaHiyoko.Id, new OnBoard(new BoardPosition(1,2))),
                },
                new ProhibitedMoves(false, false, false, false)),
                //PlayerType.Player1),

            new GameTemplate(
                "5五将棋", 5, 5, 1, WinConditionType.Checkmate,
                new List<Koma>()
                {
                    new Koma(PlayerType.Player2, KomaHisya.Id, new OnBoard(new BoardPosition(0,0))),
                    new Koma(PlayerType.Player2, KomaKaku.Id, new OnBoard(new BoardPosition(1,0))),
                    new Koma(PlayerType.Player2, KomaGin.Id, new OnBoard(new BoardPosition(2,0))),
                    new Koma(PlayerType.Player2, KomaKin.Id, new OnBoard(new BoardPosition(3,0))),
                    new Koma(PlayerType.Player2, KomaOu.Id, new OnBoard(new BoardPosition(4,0))),
                    new Koma(PlayerType.Player2, KomaHu.Id, new OnBoard(new BoardPosition(4,1))),
                    new Koma(PlayerType.Player1, KomaHisya.Id, new OnBoard(new BoardPosition(4,4))),
                    new Koma(PlayerType.Player1, KomaKaku.Id, new OnBoard(new BoardPosition(3,4))),
                    new Koma(PlayerType.Player1, KomaGin.Id, new OnBoard(new BoardPosition(2,4))),
                    new Koma(PlayerType.Player1, KomaKin.Id, new OnBoard(new BoardPosition(1,4))),
                    new Koma(PlayerType.Player1, KomaOu.Id, new OnBoard(new BoardPosition(0,4))),
                    new Koma(PlayerType.Player1, KomaHu.Id, new OnBoard(new BoardPosition(0,3))),
                },
                new ProhibitedMoves(true, true, true, true)),
                //PlayerType.Player1),
            new GameTemplate(
                "香歩将棋", 3, 4, 2, WinConditionType.Checkmate,
                new List<Koma>()
                {
                    new Koma(PlayerType.Player2, KomaKyousya.Id, new OnBoard(new BoardPosition(1,0))),
                    new Koma(PlayerType.Player2, KomaHu.Id, InHand.State),
                    new Koma(PlayerType.Player2, KomaOu.Id, new OnBoard(new BoardPosition(2,0))),
                    new Koma(PlayerType.Player1, KomaKyousya.Id, new OnBoard(new BoardPosition(1,3))),
                    new Koma(PlayerType.Player1, KomaHu.Id, InHand.State),
                    new Koma(PlayerType.Player1, KomaOu.Id, new OnBoard(new BoardPosition(0,3))),
                },
                new ProhibitedMoves(true, true, true, true)),
                //PlayerType.Player1),
        };
    }
}
