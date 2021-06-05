using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.Games
{
    public interface IWinningChecker
    {
        bool IsWinning(Game game, PlayerType player);
    }

    public class MultiWinningChecker : IWinningChecker
    {
        public List<IWinningChecker> WinningCheckers;
        public MultiWinningChecker(List<IWinningChecker> winningCheckers)
        {
            WinningCheckers = winningCheckers;
        }

        public bool IsWinning(Game game, PlayerType player)
        {
            foreach (var checker in WinningCheckers)
                if (checker.IsWinning(game, player))
                    return true;

            return false;

        }
    }
    public class TakeKingWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, PlayerType player)
        {
            return !game.State.ExistKingOnBoard(player.Opponent);
        }
    }
    public class EnterOpponentTerritoryWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, PlayerType player)
        {
            return game.KingEnterOpponentTerritory(player) && !game.DoOte(player.Opponent);
        }
    }
    public class CheckmateWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, PlayerType player)
        {
            return game.DoCheckmate(player);
        }
    }

}
