using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Bussiness.Domain.Model
{
    public class GameState
    {
        public List<Koma> KomaList;
        public Player TurnPlayer;

        public GameState(List<Koma> komaList, Player turnPlayer)
        {
            KomaList = komaList;
            TurnPlayer = turnPlayer;
        }

        public Koma FindBoardKoma(BoardPosition fromPosition)
        {
            return KomaList.FirstOrDefault(x => x.Position.Equals(fromPosition));
        }
        public Koma FindHandKoma(Player player, KomaType komaType)
        {
            return KomaList.FirstOrDefault(x => x.Player == player && x.Position == HandPosition.Hand && x.KomaType == komaType);
        }

        public BoardPositions BoardPositions(Player player)
        {
            return new BoardPositions(KomaList.Where(x => x.Player == player && x.Position is BoardPosition).Select(x => (BoardPosition)x.Position).ToList());
        }

        internal void FowardTurnPlayer()
        {
            TurnPlayer = TurnPlayer.Opponent;
        }

        public bool IsTurnPlayer(Player player)
        {
            return TurnPlayer == player;
        }
    }
}
