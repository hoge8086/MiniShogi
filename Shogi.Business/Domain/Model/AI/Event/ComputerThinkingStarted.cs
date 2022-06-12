using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Business.Domain.Model.AI.Event
{
    public class ComputerThinkingStarted : IDomainEvent
    {
        public ComputerThinkingStarted(PlayerType playerType)
        {
            PlayerType = playerType;
        }

        public PlayerType PlayerType { get; private set; }
    }
}
