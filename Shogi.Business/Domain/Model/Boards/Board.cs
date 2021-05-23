using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Bussiness.Domain.Model.Boards
{
    public class Board
    {
        public int Height { get; }
        public int Width { get; }
        public Board(int height, int width)
        {
            Height = height;
            Width = width;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    Positions.Add(new BoardPosition(x, y));

        }
        public BoardPositions Positions { get; } = new BoardPositions();
    }
}
