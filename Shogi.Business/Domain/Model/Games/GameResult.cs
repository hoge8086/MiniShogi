using Shogi.Business.Domain.Model.PlayerTypes;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Games
{
    [DataContract]
    public class GameResult
    {
        [DataMember]
        public PlayerType Winner { get; private set; }

        public GameResult(PlayerType winner)
        {
            Winner = winner;
        }

    }
}
