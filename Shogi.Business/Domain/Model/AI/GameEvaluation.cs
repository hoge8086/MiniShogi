using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.AI
{

    public class GameEvaluation
    {
        public int Value { get; private set; }
        public int MaxValue { get; private set; }
        public Game Game { get; private set; }
        public PlayerType PlayerType { get; private set; }

        public bool IsWining { get => Value == MaxValue; }
        public bool IsLosing { get => Value == -MaxValue; }

        public GameEvaluation(int value, int maxValue, Game game, PlayerType player)
        {
            MaxValue = maxValue;
            Value = value;
            Game = game;
            PlayerType = player;
        }

        public GameEvaluation Reverse()
        {
            return new GameEvaluation(-Value, MaxValue, Game, PlayerType?.Opponent);
        }
        public static GameEvaluation WiningEvaluation(int maxValue, Game game, PlayerType player)
        {
            return new GameEvaluation(maxValue, maxValue, game, player);
        }
        public static GameEvaluation LosingEvaluation(int maxValue, Game game, PlayerType player)
        {
            return WiningEvaluation(maxValue, game, player).Reverse();
        }
    }
}
