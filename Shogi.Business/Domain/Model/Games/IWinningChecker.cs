using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Games
{
    public interface IWinningChecker
    {
        bool IsWinning(Game game, PlayerType player);
    }

    [DataContract]
    [KnownType(typeof(CheckmateWinningChecker))]
    [KnownType(typeof(TakeKingWinningChecker))]
    [KnownType(typeof(EnterOpponentTerritoryWinningChecker))]
    [KnownType(typeof(MultiWinningChecker))]
    public class MultiWinningChecker : IWinningChecker
    {
        [DataMember]
        public List<IWinningChecker> WinningCheckers { get; set; }
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
    [DataContract]
    public class TakeKingWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, PlayerType player)
        {
            return !game.State.ExistKingOnBoard(player.Opponent, game.KomaTypes);
        }
    }
    [DataContract]
    public class EnterOpponentTerritoryWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, PlayerType player)
        {
            return game.KingEnterOpponentTerritory(player) && !game.DoOte(player.Opponent);
        }
    }
    [DataContract]
    public class CheckmateWinningChecker : IWinningChecker
    {
        public bool IsWinning(Game game, PlayerType player)
        {
            if (game.State.TurnPlayer == player)
                if (game.DoOte(player))
                    return true;

            return game.DoCheckmate(player);
        }
    }

}
