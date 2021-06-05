
using System;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.PlayerTypes
{
    public class PlayerType
    {
        private string Id;
        private PlayerType(string id) { Id = id; }
        public static PlayerType FirstPlayer = new PlayerType("先手");
        public static PlayerType SecondPlayer = new PlayerType("後手");

        public override string ToString()
        {
            return Id;
        }
        public PlayerType Opponent
        {
            get {
                return (this == FirstPlayer) ? SecondPlayer : FirstPlayer;
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
