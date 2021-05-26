using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Users;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.AI
{
    public abstract class AI : User
    {
        public abstract MoveCommand SelectMove(List<MoveCommand> moveCommands);
        public void Play(Game game)
        {
            var moveCommands = game.CreateAvailableMoveCommand();
            var move = SelectMove(moveCommands);
            game.Play(move);
        }
    }
}
