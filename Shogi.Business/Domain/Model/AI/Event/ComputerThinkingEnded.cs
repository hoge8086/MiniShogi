using Shogi.Business.Domain.Event;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.AI.Event
{
    public class ComputerThinkingEnded : IDomainEvent
    {
        public ComputerThinkingEnded(PlayerType playerType, GameEvaluation gameEvaluation)
        {
            PlayerType = playerType;
            GameEvaluation = gameEvaluation;
        }

        public PlayerType PlayerType { get; private set; }

        // キャンセルの場合はnull
        public GameEvaluation GameEvaluation { get; private set; }
    }
}
