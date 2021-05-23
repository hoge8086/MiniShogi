using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;
using System.Collections.Generic;

namespace Shogi.Bussiness.Domain.Model.Games
{
    public class CustomRule
    {
        public delegate bool ProhibitedMoveChecker(MoveCommand moveCommand, Game game);

        private int PositionBoundary;
        private List<ProhibitedMoveChecker> ProhibitedMoveCheckers;
        public CustomRule(int positionBoundary, List<ProhibitedMoveChecker> prohibitedMoveCheckers)
        {
            PositionBoundary = positionBoundary;
            ProhibitedMoveCheckers = prohibitedMoveCheckers;
        }
        public bool IsEnemyPosition(Player player, BoardPosition position, Board board)
        {
            if (player == Player.FirstPlayer)
            {
                return position.Y < PositionBoundary;
            }
            else
            {
                return ((board.Height - 1) - position.Y) < PositionBoundary;
            }
        }

        public bool IsProhibitedMove(MoveCommand moveCommand, Game game)
        {
            foreach (var cheker in ProhibitedMoveCheckers)
                if (cheker(moveCommand, game))
                    return true;

            return false;
        }
    }
}
