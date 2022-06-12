using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.AI.Event
{
    public class ComputerThinkingProgressed : IDomainEvent
    {
        public ComputerThinkingProgressed(PlayerType playerType, ProgressRate progressRate)
        {
            PlayerType = playerType;
            ProgressRate = progressRate;
        }

        public PlayerType PlayerType { get; private set; }
        public ProgressRate ProgressRate { get; private set; }
    }
}
