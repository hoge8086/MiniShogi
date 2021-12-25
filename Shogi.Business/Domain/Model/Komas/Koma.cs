using System;
using System.Collections.Generic;
using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Komas
{

    [DataContract]
    [KnownType(typeof(OnBoard))]
    [KnownType(typeof(InHand))]
    public class Koma
    {
        [DataMember]
        public PlayerType Player { get; private set;}
        [DataMember]
        public string TypeId { get; private set;}
        [DataMember]
        public IKomaState State { get; private set; }

        public Koma(PlayerType player, string komaTypeId, IKomaState state)
        {
            Player = player;
            TypeId = komaTypeId;
            State = state;
        }

        public void Move(BoardPosition toPosition, bool doTransform)
        {
            //if(doTransform && !KomaType.CanBeTransformed)
            //    throw new InvalidProgramException("この駒は成ることができません.");

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
            KomaType komaType,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            return State.GetMovableBoardPositions(
                                        komaType,
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
            return new Koma(Player, TypeId, State);
        }
        public override string ToString()
        {
            return string.Format("{0}:player={1},state={2}", TypeId, Player.ToString(), State.ToString());
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
                return x.Player == y.Player && x.TypeId == y.TypeId && x.State == y.State;
            }

            // If Equals() returns true for a pair of objects
            // then GetHashCode() must return the same value for these objects.

            public int GetHashCode(Koma koma)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(koma, null)) return 0;

                return HashCode.Combine(koma.TypeId, koma.Player, koma.State);
            }
        }
    }
}
