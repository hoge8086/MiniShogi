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
        public string Name { get => IsComputer ? "AI" : "あなた"; }

        [DataMember]
        public AI.AI Computer { get; private set; }

        [DataMember]
        public PlayerType PlayerType { get; private set; }

        public Player(PlayerType playerType, AI.AI ai = null)
        {
            PlayerType = playerType;
            Computer = ai;
        }
        public bool IsComputer => Computer != null;
        public bool IsHuman => !IsComputer;
    }
}
