using System.Collections.Generic;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.Moves
{
    public interface IKomaMove
    {
        BoardPositions GetMovableBoardPositions(
            PlayerType player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions iopponentKomaPositions);
    }
}
