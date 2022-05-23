using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public class ProhibitedMoves
    {
        public bool EnableNiHu { get; private set; }= false;
        public bool EnableCheckmateByHandHu { get; private set; } = false;
        public bool EnableKomaCannotMove { get; private set; } = false;
        public bool EnableLeaveOte { get; private set; } = false;

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
            Name = "新しい将棋";
            Width = 3;
            Height = 4;
            TerritoryBoundary = 1;
            ProhibitedMoves = new ProhibitedMoves();
            WinCondition = WinConditionType.Checkmate;
            KomaList = new List<Koma>();

            // [Fix:保存時に種別一覧をリソルブしなくてもよいように、保存用とDTOに分ける?]
            KomaTypes = null;
        }
        private GameTemplate(string name, int width, int height, int territoryBoundary, WinConditionType winCondition, List<Koma> komaList, ProhibitedMoves prohibitedMoves, List<KomaType> komaTypes)
        :this(name, width, height, territoryBoundary, winCondition, komaList, prohibitedMoves)
        {
            KomaTypes = komaTypes?.ToList();
        }
        public GameTemplate(string name, int width, int height, int territoryBoundary, WinConditionType winCondition, List<Koma> komaList, ProhibitedMoves prohibitedMoves)
        {
            Name = name;
            Width = width;
            Height = height;
            TerritoryBoundary = territoryBoundary;
            ProhibitedMoves = prohibitedMoves;
            WinCondition = winCondition;
            KomaList = komaList;
        }

        public GameTemplate Clone()
        {
            return new GameTemplate(
                        Name,
                        Width,
                        Height,
                        TerritoryBoundary,
                        WinCondition,
                        KomaList.Select(x => x.Clone()).ToList(),
                        ProhibitedMoves,
                        KomaTypes);
        }
        public GameTemplate Copy(string newTemplateName)
        {
            return new GameTemplate(
                        newTemplateName,
                        Width,
                        Height,
                        TerritoryBoundary,
                        WinCondition,
                        KomaList.Select(x => x.Clone()).ToList(),
                        ProhibitedMoves,
                        KomaTypes);
        }
    }

}

