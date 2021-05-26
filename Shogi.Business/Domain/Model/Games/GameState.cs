using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Bussiness.Domain.Model.Games
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

        public Koma FindKingOnBoard(Player player)
        {
            // [MEMO:プレイヤーの王は盤上に1つのみあることを前提]
            return KomaList.FirstOrDefault(x => x.Player == player && x.KomaType.IsKing && x.IsOnBoard);
        }
        public Koma FindBoardKoma(BoardPosition fromPosition)
        {
            return KomaList.FirstOrDefault(x => x.BoardPosition == fromPosition);
        }
        public Koma FindHandKoma(Player player, KomaType komaType)
        {
            return KomaList.FirstOrDefault(x => x.Player == player && x.IsInHand && x.KomaType == komaType);
        }

        public BoardPositions BoardPositions(Player player)
        {
            return new BoardPositions(KomaList.Where(x => x.Player == player && x.IsOnBoard).Select(x => x.BoardPosition).ToList());
        }

        public List<Koma> GetBoardKomaList(Player player)
        {
            return KomaList.Where(x => x.Player == player && x.IsOnBoard).ToList();
        }

        public void FowardTurnPlayer()
        {
            TurnPlayer = TurnPlayer.Opponent;
        }
        public bool ExistKing(Player player)
        {
            return FindKingOnBoard(player) != null;
        }

        public bool IsTurnPlayer(Player player)
        {
            return TurnPlayer == player;
        }

        public GameState Clone()
        {
            return new GameState(KomaList.Select(x => x.Clone()).ToList(), TurnPlayer);
        }
    }
}
