using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Moves;
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
        public string Id { get; private set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Width { get; set; }
        [DataMember]
        public int Height { get; set; }
        [DataMember]
        public int TerritoryBoundary { get; set; }
        [DataMember]
        public ProhibitedMoves ProhibitedMoves { get; set; }
        [DataMember]
        public WinConditionType WinCondition { get; set; }
        [DataMember]
        public List<Koma> KomaList { get; set; }
        [DataMember]

        // [Fix:保存時に種別一覧をリソルブしなくてもよいように、保存用とDTOに分ける?]
        public List<KomaType> KomaTypes;

        public GameTemplate() : this("新しい将棋", 3, 4, 1, WinConditionType.Checkmate, new List<Koma>(), new ProhibitedMoves(), null) { }

        public GameTemplate(GameTemplate gameTemplate)
        {
            Id = gameTemplate.Id;
            Name = gameTemplate.Name;
            Width = gameTemplate.Width;
            Height = gameTemplate.Height;
            TerritoryBoundary = gameTemplate.TerritoryBoundary;
            ProhibitedMoves = gameTemplate.ProhibitedMoves;
            WinCondition = gameTemplate.WinCondition;
            KomaList = gameTemplate.KomaList;
            KomaTypes = gameTemplate.KomaTypes;
        }

        public GameTemplate(string name, int width, int height, int territoryBoundary, WinConditionType winCondition, List<Koma> komaList, ProhibitedMoves prohibitedMoves, List<KomaType> komaTypes)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
            Width = width;
            Height = height;
            TerritoryBoundary = territoryBoundary;
            ProhibitedMoves = prohibitedMoves;
            WinCondition = winCondition;
            KomaList = komaList;
            KomaTypes = komaTypes ?? new List<KomaType>();
        }

        public GameTemplate Clone()
        {
            return new GameTemplate(this);
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

        /// <summary>
        /// どうぶつ将棋でComplexityを求めた値
        /// </summary>
        private readonly static int ComplexityOfAnimalShogi = 130;

        /// <summary>
        /// 探索の計算量の上限（どうぶつ将棋の6手読みを最大とする)
        /// </summary>
        private readonly static double MaxThinkingComplexityOfAnimalShogi = Math.Pow(ComplexityOfAnimalShogi, 7);
        /// <summary>
        /// とある局面での仮想的な着手可能手数
        /// 全ての駒を自身の駒とし、半分が持ち駒、半分が盤上とする(盤上の駒は無限の広さの盤で動くことを想定)
        /// </summary>
        public int Complexity => ((Height * Width * KomaList.Count) + KomaList.Sum(x => KomaMobilityEvaluation.Evaluate(KomaTypes.First(y => y.Id == x.TypeId).Moves)));
        public int MaxThinkingDepth => (int)Math.Log(MaxThinkingComplexityOfAnimalShogi, Complexity);
    }


    public class KomaMobilityEvaluation
    {
        public static int Evaluate(KomaMoves moves)
        {
            int movablePositionCount = 0;
            foreach(var move in moves.Moves)
            {
                var moveBase = move as KomaMoveBase;
                if (moveBase != null && !moveBase.IsRepeatable)
                    movablePositionCount += 1;

                else if (moveBase != null && moveBase.IsRepeatable)
                    movablePositionCount += 2;
            }
            return movablePositionCount;
        }
    }
}

