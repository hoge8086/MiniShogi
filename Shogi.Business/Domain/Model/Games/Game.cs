using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Bussiness.Domain.Model.Games
{

    public class Game
    {
        public Board Board { get; private set; }
        public GameState State { get; private set; }
        public CustomRule Rule { get; private set; }
        public IWinningChecker WinningChecker { get; private set; }

        public GameResult GameResult { get; private set; }
        public bool IsEnd => GameResult != null;
        public Game(Board board, GameState state, CustomRule rule, IWinningChecker winningChecker)
        {
            Board = board;
            State = state;
            Rule = rule;
            WinningChecker = winningChecker;
            CheckGameEnd();
        }

        private Game(Board board, GameState state, CustomRule rule, IWinningChecker winningChecker, GameResult gameResult)
        {
            Board = board;
            State = state;
            Rule = rule;
            WinningChecker = winningChecker;
            GameResult = gameResult;
        }
        public void CheckGameEnd()
        {
            if (WinningChecker.IsWinning(this, State.TurnPlayer))
                GameResult = new GameResult(State.TurnPlayer);
            else if (WinningChecker.IsWinning(this, State.TurnPlayer.Opponent))
                GameResult =  new GameResult(State.TurnPlayer.Opponent);
        }

        public Game Clone()
        {
            return new Game(Board, State.Clone(), Rule, WinningChecker, GameResult);
        }

        public Game Play(MoveCommand moveCommand)
        {
            if(!State.IsTurnPlayer(moveCommand.Player))
                throw new InvalidOperationException("プレイヤーのターンではありません.");

            var fromKoma = moveCommand.FindFromKoma(State);
            if(!State.IsTurnPlayer(fromKoma.Player))
                throw new InvalidOperationException("プレイヤーの駒ではないため動かせません.");

            var availableMoveCommand = CreateAvailableMoveCommand(fromKoma);
            if(!availableMoveCommand.Contains(moveCommand))
                throw new InvalidOperationException("この動きは不正です.");

            var toKoma = State.FindBoardKoma(moveCommand.ToPosition);
            if(toKoma != null && toKoma.Player == State.TurnPlayer)
                throw new InvalidOperationException("移動先にあなたの駒があるため動かせません.");

            PlayWithoutCheck(moveCommand);

            CheckGameEnd();

            return this;
        }

        public Game PlayWithoutCheck(MoveCommand moveCommand)
        {
            var fromKoma = moveCommand.FindFromKoma(State);
            if(fromKoma.Player != moveCommand.Player)
                throw new InvalidOperationException("差し手の駒ではありません.");

            var toKoma = State.FindBoardKoma(moveCommand.ToPosition);
            if(toKoma != null && toKoma.Player == fromKoma.Player)
                throw new InvalidOperationException("移動先にあなたの駒があるため動かせません.");


            fromKoma.Move(moveCommand.ToPosition, moveCommand.DoTransform);
            
            if(toKoma != null)
            {
                toKoma.Taken();
            }

            State.FowardTurnPlayer();

            return this;
        }

        public List<MoveCommand> CreateAvailableMoveCommand()
        {
            return CreateAvailableMoveCommand(State.TurnPlayer);
        }

        public List<MoveCommand> CreateAvailableMoveCommand(Player player)
        {
            return CreateAvailableMoveCommand(State.KomaList.Where(x => x.Player == player).ToList());
        }
        public List<MoveCommand> CreateAvailableMoveCommand(List<Koma> komaList)
        {
            var moveCommandList = new List<MoveCommand>();
            ForeachKomaWithoutDuplicatedHand(
                komaList,
                (koma) =>
                {
                    moveCommandList.AddRange(CreateAvailableMoveCommand(koma));
                });
            return moveCommandList;
        }
        public List<MoveCommand> CreateAvailableMoveCommand(Koma koma)
        {

            var moveCommandList = new List<MoveCommand>();
            var positions = MovablePosition(koma);
            foreach (var toPos in positions.Positions)
            {
                if (koma.Position is BoardPosition)
                {
                    moveCommandList.Add(new BoardKomaMoveCommand(koma.Player, toPos, (BoardPosition)koma.Position, false));
                    if (!koma.IsTransformed &&
                        koma.KomaType.CanBeTransformed &&
                        (Rule.IsEnemyPosition(koma.Player, toPos, Board) ||
                         Rule.IsEnemyPosition(koma.Player, (BoardPosition)koma.Position, Board)))
                    {
                        moveCommandList.Add(new BoardKomaMoveCommand(koma.Player, toPos, (BoardPosition)koma.Position, true));
                    }

                }
                else if (koma.Position is HandPosition)
                {
                    moveCommandList.Add(new HandKomaMoveCommand(koma.Player, toPos, koma.KomaType));
                }
            }
            return RemoveProhibitedMove(moveCommandList);
        }

        private List<MoveCommand> RemoveProhibitedMove(List<MoveCommand> moveCommands)
        {
            var result = new List<MoveCommand>();
            foreach(var move in moveCommands)
            {
                if (!Rule.IsProhibitedMove(move, this))
                    result.Add(move);
            }
            return result;
        }

        public BoardPositions MovablePosition(Koma koma)
        {
            return koma.GetMovableBoardPositions(
                            Board,
                            State.BoardPositions(koma.Player),
                            State.BoardPositions(koma.Player.Opponent));
        }

        public BoardPositions MovablePosition(List<Koma> komaList)
        {
            var movablePositions = new BoardPositions();
            ForeachKomaWithoutDuplicatedHand(
                komaList,
                (koma) =>
                {
                    movablePositions = movablePositions.Add(MovablePosition(koma));
                });
            return movablePositions;
        }

        public void ForeachKomaWithoutDuplicatedHand(List<Koma> komaList, Action<Koma> action)
        {
            var finishedHandKomaList = new List<KomaType>();
            foreach(var koma in komaList)
            {
                // [持ち駒の場合で、同じ駒が複数ある場合は、2回目以降はスキップする]
                if (koma.Position is HandPosition)
                    if (finishedHandKomaList.Contains(koma.KomaType))
                        continue;

                finishedHandKomaList.Add(koma.KomaType);
                action(koma);
            }
        }

        public bool DoCheckmate(Player player)
        {
            // [★チェックメイトの判定になっていない相打ちとか取り返すとか]
            if (IsEnd)
                throw new InvalidProgramException("すでに決着済みです.");

            if (!DoOte(player))
                return false;

            var moveCommands = CreateAvailableMoveCommand(player.Opponent);
            if (moveCommands.Count == 0)
                return true;

            return moveCommands.All(x => Clone().PlayWithoutCheck(x).DoOte(player));
        }

        /// <summary>
        /// 二歩検証用の詰み判定(二歩の詰み回避は駒を打っても回避できないため、駒を打つ必要がない)
        /// ※駒を打つ検証をしてしまうと再帰ループに陥る(歩を打って、さらにそれが二歩にならないか検証しないといけないので)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool DoCheckmateWithoutHandMove(Player player)
        {
            // [★チェックメイトの判定になっていない相打ちとか取り返すとか]
            if (IsEnd)
                throw new InvalidProgramException("すでに決着済みです.");

            if (!DoOte(player))
                return false;

            var moveCommands = CreateAvailableMoveCommand(State.GetBoardKomaList(player.Opponent));
            if (moveCommands.Count == 0)
                return true;
            return moveCommands.All(x => Clone().PlayWithoutCheck(x).DoOte(player));
        }

        public bool DoOte(Player player)
        {
            if (IsEnd)
                throw new InvalidProgramException("すでに決着済みです.");

            var movablePositions = MovablePosition(State.GetBoardKomaList(player));
            var kingPosition = State.FindKing(player.Opponent).Position as BoardPosition;
            return movablePositions.Contains(kingPosition);
        }
        public override string ToString()
        {
            string game = "";
            for(int y = 0; y<Board.Height; y++)
            {
                for(int x =0; x<Board.Width; x++)
                {
                    var koma = State.FindBoardKoma(new BoardPosition(x, y));
                    if(koma == null)
                    {
                        game += "____";
                    }else
                    {
                        if (koma.Player == Player.FirstPlayer)
                            game += " ";
                        else
                            game += ">";
                        game += koma.KomaType.ToString();
                        if(koma.IsTransformed)
                            game += "@";
                        else
                            game += " ";
                    }
                    game += "|";
                }
                game += "\n";
            }
            game += HandToString(Player.FirstPlayer) + "\n";
            game += HandToString(Player.SecondPlayer);
            return game;

            string HandToString(Player player)
            {
                return player.ToString() + ":" + string.Join(',', State.KomaList.Where(x => x.Position is HandPosition && x.Player == player).Select(x => x.KomaType.Id));
            }
        }
    }
}
