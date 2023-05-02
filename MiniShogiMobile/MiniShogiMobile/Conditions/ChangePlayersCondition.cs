using Shogi.Business.Domain.Model.Players;

namespace MiniShogiMobile.Conditions
{
    public class ChangePlayersCondition
    {
        public Player Player1 { get; }
        public Player Player2 { get; }
        public ChangePlayersCondition(Player player1, Player player2)
        {
            Player1 = player1;
            Player2 = player2;
        }
    }
}
