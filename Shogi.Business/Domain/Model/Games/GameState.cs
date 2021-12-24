using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Games
{
    [DataContract]
    public class GameState
    {
        [DataMember]
        public List<Koma> KomaList { get; private set; }
        [DataMember]
        public PlayerType TurnPlayer { get; private set; }

        [DataMember]
        public GameResult GameResult { get; set; }
        public bool IsEnd => GameResult != null;
        public GameState(List<Koma> komaList, PlayerType turnPlayer)
        {
            KomaList = komaList;
            TurnPlayer = turnPlayer;
            GameResult = null;
        }
        public GameState(List<Koma> komaList, PlayerType turnPlayer, GameResult gameResult)
        {
            KomaList = komaList;
            TurnPlayer = turnPlayer;
            GameResult = gameResult;
        }

        public Koma FindKingOnBoard(PlayerType player)
        {
            // [MEMO:プレイヤーの王は盤上に1つのみあることを前提]
            return KomaList.FirstOrDefault(x => x.Player == player && x.KomaType.IsKing && x.IsOnBoard);
        }
        public Koma FindBoardKoma(BoardPosition fromPosition)
        {
            return KomaList.FirstOrDefault(x => x.BoardPosition == fromPosition);
        }
        public Koma FindHandKoma(PlayerType player, KomaType komaType)
        {
            return KomaList.FirstOrDefault(x => x.Player == player && x.IsInHand && x.KomaType == komaType);
        }

        public BoardPositions BoardPositions(PlayerType player)
        {
            return new BoardPositions(KomaList.Where(x => x.Player == player && x.IsOnBoard).Select(x => x.BoardPosition).ToList());
        }

        public List<Koma> GetKomaList(PlayerType player)
        {
            return KomaList.Where(x => x.Player == player).ToList();
        }
        public List<Koma> GetKomaListDistinct(PlayerType player)
        {
            return GetKomaList(player).Distinct(new Koma.ValueComparer()).ToList();
        }

        public List<Koma> GetBoardKomaList(PlayerType player)
        {
            return KomaList.Where(x => x.Player == player && x.IsOnBoard).ToList();
        }

        public void FowardTurnPlayer()
        {
            TurnPlayer = TurnPlayer.Opponent;
        }
        public bool ExistKingOnBoard(PlayerType player)
        {
            return FindKingOnBoard(player) != null;
        }

        public bool IsTurnPlayer(PlayerType player)
        {
            return TurnPlayer == player;
        }

        public GameState Clone()
        {
            return new GameState(KomaList.Select(x => x.Clone()).ToList(), TurnPlayer, GameResult);
        }
    }
}
