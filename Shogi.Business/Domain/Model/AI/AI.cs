using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    [DataContract]
    public abstract class AI : Player
    {
        public abstract MoveCommand SelectMove(Game game, CancellationToken cancellation);
        public override bool IsAI => true;

        public void Play(Game game, CancellationToken cancellation)
        {
            var move = SelectMove(game, cancellation);
            if (move == null)
                return;

            game.Play(move);
        }
    }
}
