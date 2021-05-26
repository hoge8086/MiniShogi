using System;
using System.Collections.Generic;

namespace Shogi.Bussiness.Domain.Model.Boards
{
    /// <summary>
    /// 先手から見たボート位置
    /// </summary>
    public class BoardPosition
    {
        /// <summary>
        /// 左から0始まり(将棋の筋とは数え方が異なる)
        /// </summary>
        public int X;
        /// <summary>
        /// 上から0始まり(将棋の段とは数え方が異なる)
        /// </summary>
        public int Y;

        public BoardPosition(int x, int y)
        {
            X = x;
            Y = y;
        }

        public BoardPosition Add(RelativeBoardPosition relativePosition)
        {
            return new BoardPosition(X + relativePosition.dX, Y + relativePosition.dY);
        }

        public override bool Equals(object obj)
        {
            return obj is BoardPosition position &&
                   X == position.X &&
                   Y == position.Y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(BoardPosition left, BoardPosition right)
        {
            return EqualityComparer<BoardPosition>.Default.Equals(left, right);
        }

        public static bool operator !=(BoardPosition left, BoardPosition right)
        {
            return !(left == right);
        }
        public override string ToString()
        {
            return string.Format("(Y={0},X={1})", Y.ToString(), X.ToString());
        }
    }

    public class RelativeBoardPosition
    {
        public int dX;
        public int dY;
        public RelativeBoardPosition(int dx, int dy)
        {
            dX = dx;
            dY = dy;
        }

        public RelativeBoardPosition Reverse()
        {
            return new RelativeBoardPosition(-dX, -dY);
        }
    }
}
