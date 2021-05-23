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
        public GameResult GameResult { get; private set; }
        public bool IsEnd => GameResult != null;

        public GameState(List<Koma> komaList, Player turnPlayer)
        {
            KomaList = komaList;
            TurnPlayer = turnPlayer;
            GameResult = null;
        }

        public Koma FindKing(Player player)
        {
            // [MEMO:プレイヤーの王は盤上に1つのみあることを前提]
            return KomaList.FirstOrDefault(x => x.Player == player && x.KomaType.IsKing && x.Position is BoardPosition);
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

        public List<Koma> GetBoardKomaList(Player player)
        {
            return KomaList.Where(x => x.Player == player && x.Position is BoardPosition).ToList();
        }

        public void FowardTurnPlayer()
        {
            TurnPlayer = TurnPlayer.Opponent;
        }
        public bool ExistKing(Player player)
        {
            return FindKing(player) != null;
        }

        public bool IsTurnPlayer(Player player)
        {
            return TurnPlayer == player;
        }

        public void CheckEnd()
        {
            if (!ExistKing(Player.FirstPlayer))
                GameResult = new GameResult(Player.SecondPlayer);
            else if (!ExistKing(Player.SecondPlayer))
                GameResult = new GameResult(Player.FirstPlayer);
        }

        public GameState Clone()
        {
            return new GameState(KomaList.Select(x => x.Clone()).ToList(), TurnPlayer);
        }
    }
}
