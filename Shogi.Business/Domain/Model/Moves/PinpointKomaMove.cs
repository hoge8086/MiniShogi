using System;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Moves
{
    public class PinpointKomaMove : IKomaMove
    {
        // [先手の向きで移動可能な位置]
        private RelativeBoardPosition RelativeBoardPosition;
        public PinpointKomaMove(RelativeBoardPosition relativeBoardPosition)
        {
            RelativeBoardPosition = relativeBoardPosition;
        }

        public BoardPositions GetMovableBoardPositions(
            Player player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions)
        {
            var positions = new BoardPositions();

            // [後手なら移動可能位置は反転される]
            var toRelativePos = RelativeBoardPosition;
            if (player == Player.SecondPlayer)
                toRelativePos = toRelativePos.Reverse();

            var toPos = position.Add(toRelativePos);
            if(board.Positions.Contains(toPos))
                if(!turnPlayerKomaPositions.Contains(toPos))
                    positions = positions.Add(toPos);

            return positions;
        }
    }
}
