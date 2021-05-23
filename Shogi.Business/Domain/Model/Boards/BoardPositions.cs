using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Bussiness.Domain.Model.Boards
{
    public class BoardPositions
    {
        public List<BoardPosition> Positions { get; }

        public BoardPositions()
        {
            Positions = new List<BoardPosition>();
        }
        public BoardPositions(List<BoardPosition> positions)
        {
            Positions = positions;
        }
        public BoardPositions Substract(BoardPositions positions)
        {
            return new BoardPositions(Positions.Except(positions.Positions).ToList());
        }
        public BoardPositions Add(BoardPositions positions)
        {
            var sum = new List<BoardPosition>(Positions);
            sum.AddRange(positions.Positions);
            return new BoardPositions(sum.Distinct().ToList());
        }

        public BoardPositions Add(BoardPosition position)
        {
            var positions = new BoardPositions(Positions);
            positions.Positions.Add(position);
            return positions;
        }

        public bool Contains(BoardPosition position)
        {
            return Positions.Contains(position);
        }
    }
}
