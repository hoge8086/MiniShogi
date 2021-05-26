using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;
using System.Collections.Generic;

namespace Shogi.Bussiness.Domain.Model.Games
{

    public class CustomRule
    {
        // [Gameクラスとの循環参照が発生しているがここではGameクラスの内部実装には依存しないため、許容する]
        // [MEMO:循環参照が発生する理由としては、例えば、打ち歩詰(禁じ手)は歩を打って詰むなら禁止というルールであり]
        // [     禁じ手というルールが、基本のルールより上位の(メタな)ルールに属しているからである(歩を打てるけど、実は打っちゃダメのように).]
        // [     上記概念を、GameクラスをGameBaseクラス(打ち歩詰めを許容する)とGameWithRuleクラス(打ち歩詰めを許容しない)]の2層に]
        // [     分けることで循環参照なしに表現できるかもしれないが、過剰設計と思われるのでやらない]

        private int TerritoryBoundary;
        public IProhibitedMoveSpecification ProhibitedMoveSpecification { get; private set; }
        public IWinningChecker WinningChecker { get; private set; }
        public CustomRule(int positionBoundary, IProhibitedMoveSpecification prohibitedMoveSpecification, IWinningChecker winningChecker)
        {
            TerritoryBoundary = positionBoundary;
            ProhibitedMoveSpecification = prohibitedMoveSpecification;
            WinningChecker = winningChecker;
        }
        public bool IsPlayerTerritory(Player player, BoardPosition position, Board board)
        {
            if (player == Player.FirstPlayer)
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
