using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniShogiMobile.Conditions
{
    class PlayGameCondition
    {
        public string Name { get; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public PlayerType FirstTurnPlayer { get; }
        public PlayGameCondition(string name, Player player1, Player player2, PlayerType firstTurnPlayer)
        {
            Name = name;
            Player1 = player1;
            Player2 = player2;
            FirstTurnPlayer = firstTurnPlayer;

        }
    }
}
