using System;
using System.Collections.Generic;
using System.Text;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Moves;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Komas
{
    public class KomaType
    {
        public string Id { get; private set; }
        private KomaMoves Moves;
        private KomaMoves TransformedMoves;
        public bool CanBeTransformed { get => TransformedMoves != null; }

        public KomaType(
            string id,
            KomaMoves moves,
            KomaMoves transformedMoves)
        {
            Id = id;
            Moves = moves;
            TransformedMoves = transformedMoves;
        }
        public BoardPositions GetMovableBoardPositions(
            Player player,
            IPosition position,
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
