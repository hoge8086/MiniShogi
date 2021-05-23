using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model
{
    public class Rule
    {
        public int PositionBoundary;
        public Rule(int positionBoundary)
        {
            PositionBoundary = positionBoundary;
        }
        public bool IsEnemyPosition(Player player, BoardPosition position, Board board)
        {
            if(player == Player.FirstPlayer)
            {
                return position.Y < PositionBoundary;
            }
            else
            {
                return ((board.Height - 1) - position.Y) < PositionBoundary;
            }
        }

        public bool CheckProhibitedMove(MoveCommand moveCommand, Game game)
        {
            // [二歩]
            // [打ち歩詰め
            // [行き所のない駒]
            // [王手放置]
            return true;
        }
    }
}
