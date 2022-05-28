using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using Shogi.Business.Domain.Model.AI;

namespace Shogi.Business.Domain.Model.Players
{
    [DataContract]
    [KnownType(typeof(AI.AI))]
    [KnownType(typeof(AI.NegaAlphaAI))]
    public class Player
    {
        public string Name { get => IsComputer ? Computer.ToString() : "あなた"; }
        public AI.AI Computer { get; }

        public Player(AI.AI ai = null)
        {
            Computer = ai;
        }
        public void Play(Game game, CancellationToken cancellation)
        {
            if (IsComputer)
                Computer.Play(game, cancellation);
        }

        public bool IsComputer => Computer != null;
        public bool IsHuman => !IsComputer;
    }
}
