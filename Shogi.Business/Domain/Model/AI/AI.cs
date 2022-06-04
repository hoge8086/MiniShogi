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
    public class ProgressInfoOfAIThinking
    {
        public ProgressInfoOfAIThinking(ProgressTypeOfAIThinking progressType, double progressRate, PlayerType playerType, GameEvaluation game)
        {
            ProgressRate = progressRate;
            PlayerType = playerType;
            GameEvaluation = game;
            ProgressType = progressType;
        }

        public double ProgressRate { get; private set; }
        public PlayerType PlayerType { get; private set; }
        public GameEvaluation GameEvaluation { get; private set; }
        public ProgressTypeOfAIThinking ProgressType { get; private set; }
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

    [DataContract]
    public abstract class AI
    {
        public abstract MoveEvaluation SelectMove(Game game, CancellationToken cancellation, IProgress<ProgressInfoOfAIThinking> progress);

        public MoveCommand Play(Game game, CancellationToken cancellation, IProgress<ProgressInfoOfAIThinking> progress)
        {
            progress?.Report(new ProgressInfoOfAIThinking(ProgressTypeOfAIThinking.Started, 0.0, game.State.TurnPlayer, null));
            var computer = game.State.TurnPlayer;
            var moveEval = SelectMove(game, cancellation, progress);
            progress?.Report(new ProgressInfoOfAIThinking(ProgressTypeOfAIThinking.Completed, 1.0, game.State.TurnPlayer, moveEval.GameEvaluation));
            game.Play(moveEval.MoveCommand);
            return moveEval.MoveCommand;
        }
    }
}
