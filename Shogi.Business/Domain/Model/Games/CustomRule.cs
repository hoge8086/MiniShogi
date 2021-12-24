using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Games
{

    [DataContract]
    [KnownType(typeof(TakeKingWinningChecker))]
    [KnownType(typeof(CheckmateWinningChecker))]
    [KnownType(typeof(EnterOpponentTerritoryWinningChecker))]
    [KnownType(typeof(MultiWinningChecker))]
    [KnownType(typeof(NullProhibitedMoveSpecification))]
    [KnownType(typeof(MultiProhibitedMoveSpecification))]
    public class CustomRule
    {
        // [Gameクラスとの循環参照が発生しているがここではGameクラスの内部実装には依存しないため、許容する]
        // [MEMO:循環参照が発生する理由としては、例えば、打ち歩詰(禁じ手)は歩を打って詰むなら禁止というルールであり]
        // [     禁じ手というルールが、基本のルールより上位の(メタな)ルールに属しているからである(歩を打てるけど、実は打っちゃダメのように).]
        // [     上記概念を、GameクラスをGameBaseクラス(打ち歩詰めを許容する)とGameWithRuleクラス(打ち歩詰めを許容しない)]の2層に]
        // [     分けることで循環参照なしに表現できるかもしれないが、過剰設計と思われるのでやらない]
        [DataMember]
        public int TerritoryBoundary { get; private set; }
        [DataMember]
        public IProhibitedMoveSpecification ProhibitedMoveSpecification { get; private set; }
        [DataMember]
        public IWinningChecker WinningChecker { get; private set; }
        public CustomRule(int positionBoundary, IProhibitedMoveSpecification prohibitedMoveSpecification, IWinningChecker winningChecker)
        {
            TerritoryBoundary = positionBoundary;
            ProhibitedMoveSpecification = prohibitedMoveSpecification;
            WinningChecker = winningChecker;
        }
        public bool IsPlayerTerritory(PlayerType player, BoardPosition position, Board board)
        {
            if (player == PlayerType.FirstPlayer)
            {
                return ((board.Height - 1) - position.Y) < TerritoryBoundary;
            }
            else
            {
                return position.Y < TerritoryBoundary;
            }
        }
    }
}
