using Shogi.Business.Domain.Model.Games;

namespace Shogi.Business.Domain.Model.AI
{
    public class RandomAI : AI
    {
        public override MoveCommand SelectMove(Game game)
        {
            System.Threading.Thread.Sleep(1000);
            var moveCommands = game.CreateAvailableMoveCommand();
            return moveCommands[new System.Random().Next(0, moveCommands.Count)];
        }

    }
}
