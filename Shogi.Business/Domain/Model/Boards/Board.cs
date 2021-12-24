using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.Linq;

namespace Shogi.Business.Domain.Model.Boards
{
    [DataContract]
    public class Board
    {
        public int Height => Positions.Positions.Max(p => p.Y) + 1;
        public int Width => Positions.Positions.Max(p => p.X) + 1;
        [DataMember]
        public BoardPositions Positions { get; private set; }
        public Board(int height, int width)
        {
            var positions = new List<BoardPosition>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    positions.Add(new BoardPosition(x, y));
            Positions = new BoardPositions(positions);

        }
    }
}
