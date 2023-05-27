using System.Collections.Generic;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Moves
{
    [DataContract]
    [KnownType(typeof(KomaMoveBase))]
    public class KomaMoves
    {
        [DataMember]
        public List<IKomaMove> Moves { get; private set; }

        public KomaMoves()
        {
            Moves = new List<IKomaMove>();
        }

        public KomaMoves(List<IKomaMove> moves)
        {
            Moves = moves;
        }
        public BoardPositions GetMovableBoardPositions(
            PlayerType player,
            BoardPosition position,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions,
            bool kiki = false)
        {
            BoardPositions positions = new BoardPositions();
            foreach(var move in Moves)
            {
                positions = positions.Add(
                    move.GetMovableBoardPositions(
                                        player,
                                        position as BoardPosition,
                                        board,
                                        turnPlayerKomaPositions,
                                        opponentKomaPositions,
                                        kiki));
            }
            return positions;
        }
    }
}
