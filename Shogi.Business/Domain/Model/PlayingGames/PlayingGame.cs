using System.Collections.Generic;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.GameTemplates;
using System.Runtime.Serialization;
using System;

namespace Shogi.Business.Domain.Model.PlayingGames
{
    [DataContract]
    [KnownType(typeof(AI.AI))]
    [KnownType(typeof(AI.NegaAlphaAI))]
    [KnownType(typeof(Players.Human))]
    [KnownType(typeof(Players.Player))]
    public class PlayingGame
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        private Dictionary<string, Player> Players;
        [DataMember]
        public Game Game { get; private set; }
        [DataMember]
        public GameTemplate GameTemplate { get; private set;}

        public Player TurnPlayer => Players[Game.State.TurnPlayer.Id];
        public Player GerPlayer(PlayerType pleyerType) => Players[pleyerType.Id];

        public PlayingGame(Player firstPlayer, Player secondPlayer, Game game, GameTemplate gameTemplate)
        {
            Name = null;
            Players = new Dictionary<string, Player>();
            Players.Add(PlayerType.Player1.Id, firstPlayer);
            Players.Add(PlayerType.Player2.Id, secondPlayer);
            GameTemplate = gameTemplate;
            Game = game.Clone();
        }
        public PlayingGame(PlayingGame other)
        {
            Name = other.Name;
            Players = other.Players;
            GameTemplate = other.GameTemplate;
            Game = other.Game.Clone();
        }

        public void Reset()
        {
            Game.Reset();
        }

        public PlayingGame Clone()
        {
            return new PlayingGame(this);
        }

        internal void ChangeName(string playingName)
        {
            Name = playingName;
        }
    }
}
