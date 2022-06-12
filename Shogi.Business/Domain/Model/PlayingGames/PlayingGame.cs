using System.Collections.Generic;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.GameTemplates;
using System.Runtime.Serialization;
using System;
using System.Linq;

namespace Shogi.Business.Domain.Model.PlayingGames
{
    [DataContract]
    public class PlayingGame
    {
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        private List<Player> Players;
        [DataMember]
        public Game Game { get; private set; }
        [DataMember]
        public GameTemplate GameTemplate { get; private set;}

        public Player TurnPlayer => Players.First(player => player.PlayerType == Game.State.TurnPlayer);
        public Player GerPlayer(PlayerType playerType) => Players.First(player => player.PlayerType == playerType);

        public PlayingGame(List<Player> players, Game game, GameTemplate gameTemplate)
        {
            if(players.Count != 2 ||
               !players.Any(x => x.PlayerType == PlayerType.Player1) ||
               !players.Any(x => x.PlayerType == PlayerType.Player2))
            {
                throw new ArgumentException("プレイヤーがP1とP2が一人ずつではありません.");
            }
            Name = null;
            Players = players;
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
