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

        public PlayingGame(Player firstPlayer, Player secondPlayer, PlayerType firstTurnPlayer, GameTemplate gameTemplate)
        {
            Players = new Dictionary<PlayerType, Player>();
            Players.Add(PlayerType.Player1, firstPlayer);
            Players.Add(PlayerType.Player2, secondPlayer);
            TemplateName = gameTemplate.Name;
            Game = gameTemplate.Game.Clone();
            Game.ChangeFirstTurnPlayer(firstTurnPlayer);
        }
        public void Reset()
        {
            Game.Reset();
        }

    }
}
