
using System;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.Players
{
    public class Player
    {
        private string Id;
        private Player(string id) { Id = id; }
        public static Player FirstPlayer = new Player("先手");
        public static Player SecondPlayer = new Player("後手");

        public override string ToString()
        {
            return Id;
        }
        public Player Opponent
        {
            get {
                return (this == FirstPlayer) ? SecondPlayer : FirstPlayer;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Player player &&
                   Id == player.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Player left, Player right)
        {
            return EqualityComparer<Player>.Default.Equals(left, right);
        }

        public static bool operator !=(Player left, Player right)
        {
            return !(left == right);
        }
    }
}
