using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public class ProhibitedMoves
    {
        public bool EnableNiHu { get; set; }= false;
        public bool EnableCheckmateByHandHu { get; set; } = false;
        public bool EnableKomaCannotMove { get; set; } = false;
        public bool EnableLeaveOte { get; set; } = false;

        public ProhibitedMoves() { }
        public ProhibitedMoves(bool enableNiHu, bool enableCheckmateByHandHu, bool enableKomaCannotMove, bool enableLeaveOte)
        {
            EnableNiHu = enableNiHu;
            EnableCheckmateByHandHu = enableCheckmateByHandHu;
            EnableKomaCannotMove = enableKomaCannotMove;
            EnableLeaveOte = enableLeaveOte;
        }

    }
    public enum WinConditionType
    {
        Checkmate,
        TakeKing,
        TakeKingOrEnterOpponentTerritory,
    }
    public class GameTemplate
    {
        public string Name { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int TerritoryBoundary { get; set; }
        public ProhibitedMoves ProhibitedMoves { get; set;}
        public WinConditionType WinCondition { get; set; }
        public List<Koma> KomaList { get; set; }

        public List<KomaType> KomaTypes = null;

        public GameTemplate()
        {
            Name = "新しい将棋";
            Width = 3;
            Height = 4;
            TerritoryBoundary = 1;
            ProhibitedMoves = new ProhibitedMoves();
            WinCondition = WinConditionType.TakeKing;
            KomaList = new List<Koma>();

            // [Fix:保存時に種別一覧をリソルブしなくてもよいように、保存用とDTOに分ける?]
            KomaTypes = null;
        }
        public GameTemplate(string name, int width, int height, int territoryBoundary, WinConditionType winCondition, List<Koma> komaList, ProhibitedMoves prohibitedMoves)//, PlayerType turnPlayer)
        {
            Name = name;
            Width = width;
            Height = height;
            TerritoryBoundary = territoryBoundary;
            ProhibitedMoves = prohibitedMoves;
            WinCondition = winCondition;
            KomaList = komaList;
        }
    }

}

