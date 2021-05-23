using System.Collections.Generic;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Moves
{
    public interface IKomaMove
    {
        BoardPositions GetMovableBoardPositions(
            Player player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions iopponentKomaPositions);
    }
}
