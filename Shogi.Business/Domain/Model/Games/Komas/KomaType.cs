using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Komas
{
    [DataContract]
    public class KomaType
    {
        [DataMember]
        public string Id { get; private set; }
        [DataMember]
        public KomaMoves Moves { get; private set; }
        [DataMember]
        public KomaMoves TransformedMoves { get; private set; }
        [DataMember]
        public bool IsKing { get; private set; }
        [DataMember]
        public bool IsHu { get; private set; }
        public bool CanBeTransformed { get => TransformedMoves != null; }

        public KomaType()
        {
            Id = string.Empty;
            Moves = new KomaMoves();
            TransformedMoves = null;
            IsKing = false;
            IsHu = false;
        }
        public KomaType(
            string id,
            KomaMoves moves,
            KomaMoves transformedMoves,
            bool isKing,
            bool isHu = false)
        {
            Id = id;
            Moves = moves;
            TransformedMoves = transformedMoves;
            IsKing = isKing;
            IsHu = isHu;
        }
        public BoardPositions GetMovableBoardPositions(
            PlayerType player,
            BoardPosition position,
            bool isTransformed,
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions opponentKomaPositions)
        {
            if(isTransformed)
                return TransformedMoves.GetMovableBoardPositions(player, position, board, turnPlayerKomaPositions, opponentKomaPositions);
            else
                return Moves.GetMovableBoardPositions(player, position, board, turnPlayerKomaPositions, opponentKomaPositions);
        }

        public override string ToString()
        {
            return Id;
        }

        public override bool Equals(object obj)
        {
            return obj is KomaType type &&
                   Id == type.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(KomaType left, KomaType right)
        {
            return EqualityComparer<KomaType>.Default.Equals(left, right);
        }

        public static bool operator !=(KomaType left, KomaType right)
        {
            return !(left == right);
        }
    }
}
