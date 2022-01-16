using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniShogiMobile.Conditions
{
    public enum PlayMode
    {
        NewGame,
        ContinueGame,
    }
    public class PlayGameCondition
    {
        public PlayMode PlayMode { get; } 
        public string Name { get; }
        public Player Player1 { get; }
        public Player Player2 { get; }
        public PlayerType FirstTurnPlayer { get; }
        public PlayGameCondition(PlayMode playMode, string name, Player player1, Player player2, PlayerType firstTurnPlayer)
        {
            PlayMode = playMode;
            Name = name;
            Player1 = player1;
            Player2 = player2;
            FirstTurnPlayer = firstTurnPlayer;
        }
        public PlayGameCondition(PlayMode playMode, string name)
        {
            PlayMode = playMode;
            Name = name;
        }
    }
}
