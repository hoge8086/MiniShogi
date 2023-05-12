using Shogi.Business.Domain.Model.Games;
using System;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    public class RandomAI : AI
    {

        public override string ToString() => "ランダムAI";
        public MoveEvaluation SelectMove(Game game, CancellationToken cancellation, Action<ProgressRate> progress)
        {
            cancellation.ThrowIfCancellationRequested();
            System.Threading.Thread.Sleep(1000);
            var moveCommands = game.CreateAvailableMoveCommand();
            return new MoveEvaluation(moveCommands[new System.Random().Next(0, moveCommands.Count)], new GameEvaluation(0, 100, game, game.State.TurnPlayer, 0));
        }

    }
}
