using System;
using System.Collections.Generic;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.Players;

namespace Shogi.Business.Domain.Model.Komas
{

    public class Koma
    {
        public Player Player { get;  private set;}
        public KomaType KomaType { get;  private set;}

        public IKomaState State { get; private set; }

        public Koma(Player player, KomaType komaType, IKomaState state)
        {
            Player = player;
            KomaType = komaType;
            State = state;
        }

        public void Move(BoardPosition toPosition, bool doTransform)
        {
            if(doTransform && !KomaType.CanBeTransformed)
                throw new InvalidProgramException("この駒は成ることができません.");

            State = State.ToBoard(toPosition, doTransform);
        }
        public void Taken()
        {
            if(IsInHand)
                throw new InvalidProgramException("持ち駒を取ることはできません.");

            Player = Player.Opponent;
            State = InHand.State;
        }

        public BoardPositions GetMovableBoardPositions(
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            return State.GetMovableBoardPositions(
                                        KomaType,
                                        Player,
                                        board,
                                        playerKomaPositions,
                                        opponentPlayerKomaPositions);
        }
        public BoardPosition BoardPosition => (State as OnBoard)?.Position;
        public bool IsOnBoard => State is OnBoard;
        public bool IsInHand => State is InHand;

        public bool IsTransformed => (State is OnBoard) && ((OnBoard)State).IsTransformed;
        public Koma Clone()
        {
            return new Koma(Player, KomaType, State);
        }
        public override string ToString()
        {
            return string.Format("{0}:player={1},state={2},IsTransformed={3}", KomaType.ToString(), Player.ToString(), State.ToString());
        }

        /// <summary>
        /// 値オブジェクトとしての比較(同自種類の持ち駒は同じ）
        /// </summary>
        public class ValueComparer : IEqualityComparer<Koma>
        {
            // Products are equal if their names and product numbers are equal.
            public bool Equals(Koma x, Koma y)
            {

                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                //Check whether the products' properties are equal.
                return x.Player == y.Player && x.KomaType == y.KomaType && x.State == y.State;
            }

            // If Equals() returns true for a pair of objects
            // then GetHashCode() must return the same value for these objects.

            public int GetHashCode(Koma koma)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(koma, null)) return 0;

                return HashCode.Combine(koma.KomaType, koma.Player, koma.State);
            }
        }
    }
}
