using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Users;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.AI
{
    public abstract class AI : User
    {
        public abstract MoveCommand SelectMove(Game game);
        public void Play(Game game)
        {
            var move = SelectMove(game);
            game.Play(move);
        }
    }
}
