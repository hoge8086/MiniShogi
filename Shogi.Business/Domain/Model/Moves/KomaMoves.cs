using System.Collections.Generic;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Moves
{
    public class KomaMoves
    {
        private List<IKomaMove> Moves;

        public KomaMoves(List<IKomaMove> moves)
        {
            Moves = moves;
        }

        public BoardPositions GetMovableBoardPositions(
            Player player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions)
        {
            BoardPositions positions = new BoardPositions();
            foreach(var move in Moves)
            {
                positions = positions.Add(
                    move.GetMovableBoardPositions(
                                        player,
                                        position as BoardPosition,
                                        board,
                                        turnPlayerKomaPositions,
                                        opponentKomaPositions));
            }
            return positions;
        }
    }
}
