using System.Collections.Generic;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.Moves
{
    public interface IKomaMove
    {
        /// <summary>
        /// 移動可能な位置を取得する
        /// </summary>
        /// <param name="player"></param>
        /// <param name="position"></param>
        /// <param name="board"></param>
        /// <param name="turnPlayerKomaPositions"></param>
        /// <param name="iopponentKomaPositions"></param>
        /// <param name="kiki">利き(自身の駒がいる位置を移動可能な位置に含めるかどうか)</param>
        /// <returns></returns>
        BoardPositions GetMovableBoardPositions(
            PlayerType player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions iopponentKomaPositions,
            bool kiki = false);
    }
}
