using System;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Moves
{
    [DataContract]
    public class PinpointKomaMove : IKomaMove
    {
        // [先手の向きで移動可能な位置]
        [DataMember]
        public RelativeBoardPosition RelativeBoardPosition { get; private set; }
        public PinpointKomaMove(RelativeBoardPosition relativeBoardPosition)
        {
            RelativeBoardPosition = relativeBoardPosition;
        }

        public BoardPositions GetMovableBoardPositions(
            PlayerType player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions)
        {
            var positions = new BoardPositions();

            // [後手なら移動可能位置は反転される]
            var toRelativePos = RelativeBoardPosition;
            if (player == PlayerType.SecondPlayer)
                toRelativePos = toRelativePos.Reverse();

            var toPos = position.Add(toRelativePos);
            if(board.Positions.Contains(toPos))
                if(!turnPlayerKomaPositions.Contains(toPos))
                    positions = positions.Add(toPos);

            return positions;
        }
    }
}
