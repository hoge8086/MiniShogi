using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Business.Domain.Model.Boards
{
    public class Board
    {
        public int Height { get; }
        public int Width { get; }
        public Board(int height, int width)
        {
            Height = height;
            Width = width;
            var positions = new List<BoardPosition>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    positions.Add(new BoardPosition(x, y));
            Positions = new BoardPositions(positions);

        }
        public BoardPositions Positions { get; } = new BoardPositions();
    }
}
