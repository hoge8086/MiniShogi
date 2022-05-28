using Shogi.Business.Domain.Model.Games;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    public class RandomAI : AI
    {

        public override string ToString() => "ランダムAI";
        public override MoveCommand SelectMove(Game game, CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
                return null;
            System.Threading.Thread.Sleep(1000);
            var moveCommands = game.CreateAvailableMoveCommand();
            return moveCommands[new System.Random().Next(0, moveCommands.Count)];
        }

    }
}
