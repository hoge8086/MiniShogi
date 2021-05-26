using Shogi.Bussiness.Domain.Model.Players;
using System.Collections.Generic;

namespace Shogi.Bussiness.Domain.Model.Games
{
    public interface IWinningChecker
    {
        bool IsWinning(Game game, Player player);
    }

    public class MultiWinningChecker : IWinningChecker
    {
        public List<IWinningChecker> WinningCheckers;
        public MultiWinningChecker(List<IWinningChecker> winningCheckers)
        {
            WinningCheckers = winningCheckers;
        }

        public bool IsWinning(Game game, Player player)
        {
            foreach (var checker in WinningCheckers)
                if (checker.IsWinning(game, player))
                    return true;

            return false;

        }
    }
    public class TakeKingWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, Player player)
        {
            return !game.State.ExistKing(player.Opponent);
        }
    }
    public class EnterOpponentTerritoryWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, Player player)
        {
            return game.KingEnterOpponentTerritory(player);
        }
    }
    public class CheckmateWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, Player player)
        {
            return game.DoCheckmate(player);
        }
    }

}
