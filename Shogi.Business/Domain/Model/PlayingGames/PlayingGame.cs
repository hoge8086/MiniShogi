using System.Collections.Generic;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.GameTemplates;

namespace Shogi.Business.Domain.Model.PlayingGames
{
    public class PlayingGame
    {
        private Dictionary<PlayerType, Player> Players;
        public Game Game { get; }
        public string TemplateName { get; }

        public Player TurnPlayer => Players[Game.State.TurnPlayer];
        public Player GerPlayer(PlayerType pleyerType) => Players[pleyerType];

        public PlayingGame(Player firstPlayer, Player secondPlayer, PlayerType firstTurnPlayer, Game game)
        {
            Players = new Dictionary<PlayerType, Player>();
            Players.Add(PlayerType.Player1, firstPlayer);
            Players.Add(PlayerType.Player2, secondPlayer);
            TemplateName = "<TODO>";//gameTemplate.Name;
            Game = game.Clone();
            //Game.ChangeFirstTurnPlayer(firstTurnPlayer);
        }
        public void Reset()
        {
            Game.Reset();
        }

    }
}
