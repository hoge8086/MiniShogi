using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.PlayerTypes
{
    [DataContract]
    public class PlayerType
    {
        [DataMember]
        public string Id { get; private set; }

        private PlayerType(string id) { Id = id; }
        public static PlayerType Player1 = new PlayerType(nameof(Player1));
        public static PlayerType Player2 = new PlayerType(nameof(Player2));

        public override string ToString()
        {
            return Id;
        }
        public PlayerType Opponent
        {
            get {
                return (this == Player1) ? Player2 : Player1;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerType player &&
                   Id == player.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(PlayerType left, PlayerType right)
        {
            return EqualityComparer<PlayerType>.Default.Equals(left, right);
        }

        public static bool operator !=(PlayerType left, PlayerType right)
        {
            return !(left == right);
        }
    }
}
