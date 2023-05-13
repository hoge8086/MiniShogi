using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.AI
{

    public class GameEvaluation
    {
        /// <summary>
        /// 現在の評価値
        /// 詰みの状況でも最善手(最長手/最短手)を評価するため、MaxValueの値を超えることがある
        /// </summary>
        public int Value { get; private set; }
        /// <summary>
        /// 勝ち／負けが確定したときの評価値
        /// </summary>
        public int MaxValue { get; private set; }
        public Game Game { get; private set; }
        public PlayerType PlayerType { get; private set; }

        public int BeginingMoveCount{ get; private set; }
        public bool IsWining { get => Value >= MaxValue; }
        public bool IsLosing { get => Value <= -MaxValue; }

        public GameEvaluation(int value, int maxValue, Game game, PlayerType player, int beginingMoveCount)
        {
            MaxValue = maxValue;
            Value = value;
            Game = game;
            PlayerType = player;
            BeginingMoveCount = beginingMoveCount;
        }

        public GameEvaluation Reverse()
        {
            return new GameEvaluation(-Value, MaxValue, Game, PlayerType?.Opponent, BeginingMoveCount);
        }
    }
}
