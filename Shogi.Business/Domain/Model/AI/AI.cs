using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Players;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    public enum ProgressTypeOfAIThinking
    {
        Started,
        Thinking,
        Completed,
    }
    public class ProgressRate
    {
        public ProgressRate(int current, int total)
        {
            Current = current;
            Total = total;
        }

        public int Current { get; private set; }
        public int Total { get; private set; }
        public double DoubleRate { get => (double)Current / Total; }
    }

    public class MoveEvaluation
    {
        public MoveEvaluation(MoveCommand moveCommand, GameEvaluation gameEvaluation)
        {
            MoveCommand = moveCommand;
            GameEvaluation = gameEvaluation;
        }
        public MoveCommand MoveCommand { get; private set; }
        public GameEvaluation GameEvaluation { get; private set; }

        public override string ToString()
        {
            return $"{MoveCommand.ToString()}, eval={GameEvaluation.Value}\n{GameEvaluation.Game.ToString()}";
        }
    }

    public interface AI
    {
        MoveEvaluation SelectMove(Game game, CancellationToken cancellation, Action<ProgressRate> progress);
    }
}
