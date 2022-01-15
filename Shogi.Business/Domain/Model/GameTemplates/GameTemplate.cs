using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

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

        [Description("詰み")]
        Checkmate,
        [Description("王取り")]
        TakeKing,
        [Description("王取りor入玉")]
        TakeKingOrEnterOpponentTerritory,
    }
    [DataContract]
    public class GameTemplate
    {

        [DataMember]
        public string Id{ get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int TerritoryBoundary { get; set; }
        [DataMember]
        public ProhibitedMoves ProhibitedMoves { get; set;}
        [DataMember]
        public WinConditionType WinCondition { get; set; }
        [DataMember]
        public List<Koma> KomaList { get; set; }
        [DataMember]

        public List<KomaType> KomaTypes;

        public GameTemplate()
        {
            Id = Guid.NewGuid().ToString();
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
            Id = Guid.NewGuid().ToString();
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

