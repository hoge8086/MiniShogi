using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Boards
{
    [DataContract]
    public class BoardPositions
    {
        [DataMember]
        public List<BoardPosition> Positions { get; private set; }

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
            var sum = new List<BoardPosition>(Positions);
            sum.Add(position);
            return new BoardPositions(sum.Distinct().ToList());
        }

        public bool Contains(BoardPosition position)
        {
            return Positions.Contains(position);
        }
    }
}
