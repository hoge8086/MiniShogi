using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Users;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    public abstract class AI : User
    {
        public abstract MoveCommand SelectMove(Game game, CancellationToken cancellation);

        public void Play(Game game, CancellationToken cancellation)
        {
            var move = SelectMove(game, cancellation);
            if (move == null)
                return;

            game.Play(move);
        }
    }
}
