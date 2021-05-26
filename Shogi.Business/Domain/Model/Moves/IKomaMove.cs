using System.Collections.Generic;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Players;

namespace Shogi.Business.Domain.Model.Moves
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
