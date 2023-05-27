﻿using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Moves
{
    [DataContract]
    public class KomaMoveBase : IKomaMove
    {
        // [先手の向きで移動可能な位置]
        [DataMember]
        public RelativeBoardPosition RelativeBoardPosition { get; private set; }
        [DataMember]
        public bool IsRepeatable { get; private set; }
        public KomaMoveBase(RelativeBoardPosition relativeBoardPosition, bool isRepeatable = false)
        {
            RelativeBoardPosition = relativeBoardPosition;
            IsRepeatable = isRepeatable;
        }

        public BoardPositions GetMovableBoardPositions(
            PlayerType player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions,
            bool kiki = false)
        {
            var positions = new BoardPositions();

            // [後手なら移動可能位置は反転される]
            var toRelativePos = RelativeBoardPosition;
            if (player == PlayerType.Player2)
                toRelativePos = toRelativePos.Reverse();

            for(var toPos = position.Add(toRelativePos); ; toPos = toPos.Add(toRelativePos))
            {

                // [盤の範囲外に来たら移動不可]
                if (!board.Positions.Contains(toPos))
                    break;

                // [自分の駒に当たったら移動不可]
                if (turnPlayerKomaPositions.Contains(toPos) && !kiki)
                    break;

                positions = positions.Add(toPos);

                // [利きの場合は自身の駒を含めて、それ以上の移動は不可]
                if (kiki)
                    break;


                // [繰り返し不可な駒は1マスしか移動不可]
                if (!IsRepeatable)
                    break;

                // [相手の駒があったらその先は移動できない]
                if (opponentKomaPositions.Contains(toPos))
                    break;
            }

            return positions;
        }
    }
}
