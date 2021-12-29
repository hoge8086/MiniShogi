using Shogi.Business.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniShogiMobile.Conditions
{
    class PlayGameCondition
    {
        public string Name { get; }
        public Player FirstPlayer { get; }
        public Player SecondPlayer { get; }
        public PlayGameCondition(string name, Player firstPlayer, Player secondPlayer)
        {
            Name = name;
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;

        }
    }
}
