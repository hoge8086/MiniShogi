﻿using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Business.Domain.Model.Games
{

    public class Game
    {
        public Board Board { get; private set; }
        public GameState State { get; private set; }

        public CustomRule Rule { get; private set; }

        public GameRecord Record { get; private set; }

        private Object thisLock = new Object();

        public Game(Board board, GameState state, CustomRule rule)
        {
            Board = board;
            State = state;
            Rule = rule;
            Record = new GameRecord(State);
            State.GameResult = CreateGameResult();
        }

        private Game(Board board, GameState state, CustomRule rule, GameRecord record)
        {
            Board = board;
            State = state;
            Rule = rule;
            Record = record;
        }

        public bool IsWinning(PlayerType player)
        {
            lock(thisLock)
            {
                return Rule.WinningChecker.IsWinning(this, player);
            }
        }

        private GameResult CreateGameResult()
        {
            if (IsWinning(State.TurnPlayer))
                return new GameResult(State.TurnPlayer);
            else if (IsWinning(State.TurnPlayer.Opponent))
                return new GameResult(State.TurnPlayer.Opponent);

            return null;
           
        }

        public Game Clone()
        {
            lock(thisLock)
            {
                return new Game(Board, State.Clone(), Rule, Record.Clone());
            }
        }

        public Game Play(MoveCommand moveCommand)
        {
            lock (thisLock)
            {
                if (!State.IsTurnPlayer(moveCommand.Player))
                    throw new InvalidOperationException("プレイヤーのターンではありません.");

                var fromKoma = moveCommand.FindFromKoma(State);
                if (!State.IsTurnPlayer(fromKoma.Player))
                    throw new InvalidOperationException("プレイヤーの駒ではないため動かせません.");

                var availableMoveCommand = CreateAvailableMoveCommand(fromKoma);
                if (!availableMoveCommand.Contains(moveCommand))
                    throw new InvalidOperationException("この動きは不正です.");

                var toKoma = State.FindBoardKoma(moveCommand.ToPosition);
                if (toKoma != null && toKoma.Player == State.TurnPlayer)
                    throw new InvalidOperationException("移動先にあなたの駒があるため動かせません.");

                PlayWithoutCheck(moveCommand);

                return this;
            }
        }

        /// <summary>
        /// ゲーム終了判定のためにこのメソッドを使用しないこと(PlayWithoutRecord()を使用すること)
        /// このメソッド内でここでさらにゲーム終了判定をしているため無限ループに陥る
        /// </summary>
        /// <param name="moveCommand"></param>
        /// <returns></returns>
        public Game PlayWithoutCheck(MoveCommand moveCommand)
        {
            lock (thisLock)
            {
                PlayWithoutRecord(moveCommand);
                State.GameResult = CreateGameResult();
                Record.Record(State);
                return this;
            }
        }
        // [★WithoutRecordというよりは終了判定のチェックなし版というのが正しい]
        public Game PlayWithoutRecord(MoveCommand moveCommand)
        {
            lock (thisLock)
            {
                var fromKoma = moveCommand.FindFromKoma(State);
                if (fromKoma.Player != moveCommand.Player)
                    throw new InvalidOperationException("差し手の駒ではありません.");

                var toKoma = State.FindBoardKoma(moveCommand.ToPosition);
                if (toKoma != null && toKoma.Player == fromKoma.Player)
                    throw new InvalidOperationException("移動先にあなたの駒があるため動かせません.");


                fromKoma.Move(moveCommand.ToPosition, moveCommand.DoTransform);

                if (toKoma != null)
                {
                    toKoma.Taken();
                }

                State.FowardTurnPlayer();
                return this;
            }
        }

        public enum UndoType
        {
            Undo = -1,
            Redo = 1,
        }

        public bool CanUndo(UndoType undoType)
        {
            lock (thisLock)
            {
                return Record.CanUndo((int)undoType);
            }
        }

        public void Undo(UndoType undoType)
        {
            lock (thisLock)
            {
                if (!CanUndo(undoType))
                    throw new InvalidOperationException("Undoできません.");

                State = Record.Undo((int)undoType).Clone();
            }
        }

        public List<MoveCommand> CreateAvailableMoveCommand()
        {
            lock (thisLock)
            {
                return CreateAvailableMoveCommand(State.TurnPlayer);
            }
        }

        public List<MoveCommand> CreateAvailableMoveCommand(PlayerType player)
        {
            lock (thisLock)
            {
                return CreateAvailableMoveCommand(State.GetKomaListDistinct(player));
            }
        }
        public BoardPositions MovablePosition(List<Koma> komaList)
        {
            lock (thisLock)
            {
                var movablePositions = new BoardPositions();
                foreach (var koma in komaList)
                    movablePositions = movablePositions.Add(MovablePosition(koma));
                return movablePositions;
            }
        }
        public List<MoveCommand> CreateAvailableMoveCommand(List<Koma> komaList)
        {
            lock (thisLock)
            {
                var moveCommandList = new List<MoveCommand>();
                foreach (var koma in komaList)
                    moveCommandList.AddRange(CreateAvailableMoveCommand(koma));
                return moveCommandList;
            }
        }

        public BoardPositions MovablePosition(Koma koma)
        {
            lock (thisLock)
            {
                return koma.GetMovableBoardPositions(
                                Board,
                                State.BoardPositions(koma.Player),
                                State.BoardPositions(koma.Player.Opponent));
            }
        }

        public List<MoveCommand> CreateAvailableMoveCommand(Koma koma)
        {
            lock (thisLock)
            {
                var moveCommandList = new List<MoveCommand>();
                var positions = MovablePosition(koma);
                foreach (var toPos in positions.Positions)
                {
                    if (koma.IsOnBoard)
                    {
                        moveCommandList.Add(new BoardKomaMoveCommand(koma.Player, toPos, koma.BoardPosition, false));
                        if (!koma.IsTransformed &&
                            koma.KomaType.CanBeTransformed &&
                            (Rule.IsPlayerTerritory(koma.Player.Opponent, toPos, Board) ||
                             Rule.IsPlayerTerritory(koma.Player.Opponent, koma.BoardPosition, Board)))
                        {
                            moveCommandList.Add(new BoardKomaMoveCommand(koma.Player, toPos, koma.BoardPosition, true));
                        }

                    }
                    else if (koma.IsInHand)
                    {
                        moveCommandList.Add(new HandKomaMoveCommand(koma.Player, toPos, koma.KomaType));
                    }
                }
                return RemoveProhibitedMove(moveCommandList);
            }
        }

        private List<MoveCommand> RemoveProhibitedMove(List<MoveCommand> moveCommands)
        {
            var result = new List<MoveCommand>();
            foreach (var move in moveCommands)
            {
                if (!Rule.ProhibitedMoveSpecification.IsSatisfiedBy(move, this))
                    result.Add(move);
            }
            return result;
        }

        public bool DoCheckmate(PlayerType player)
        {
            lock (thisLock)
            {
                if (State.IsEnd)
                    throw new InvalidProgramException("すでに決着済みです.");


                // [MEMO:今王手じゃなくても、着手可能な手が一つもない場合があるので、こちらを先にチェックする]
                var moveCommands = CreateAvailableMoveCommand(player.Opponent);
                if (moveCommands.Count == 0)
                    return true;

                if (!DoOte(player))
                    return false;

                return moveCommands.All(x => Clone().PlayWithoutRecord(x).DoOte(player));
            }
        }

        /// <summary>
        /// 二歩検証用の詰み判定(二歩の詰み回避は駒を打っても回避できないため、駒を打つ必要がない)
        /// ※駒を打つ検証をしてしまうと再帰ループに陥る(歩を打って、さらにそれが二歩にならないか検証しないといけないので)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool DoCheckmateWithoutHandMove(PlayerType player)
        {
            lock (thisLock)
            {
                if (State.IsEnd)
                    throw new InvalidProgramException("すでに決着済みです.");

                if (!DoOte(player))
                    return false;

                // [MEMO:盤上の駒に重複はないのでDistinct()する必要はない]
                var moveCommands = CreateAvailableMoveCommand(State.GetBoardKomaList(player.Opponent));
                if (moveCommands.Count == 0)
                    return true;
                return moveCommands.All(x => Clone().PlayWithoutRecord(x).DoOte(player));
            }
        }

        public bool DoOte(PlayerType player)
        {
            lock (thisLock)
            {
                if (State.IsEnd)
                    throw new InvalidProgramException("すでに決着済みです.");

                // [MEMO:盤上の駒に重複はないのでDistinct()する必要はない]
                var movablePositions = MovablePosition(State.GetBoardKomaList(player));
                var kingPosition = State.FindKingOnBoard(player.Opponent).BoardPosition;
                return movablePositions.Contains(kingPosition);
            }
        }
        public bool KingEnterOpponentTerritory(PlayerType player)
        {
            lock (thisLock)
            {
                var king = State.FindKingOnBoard(player);
                if (king == null)
                    return false;
                return Rule.IsPlayerTerritory(player.Opponent, king.BoardPosition, Board);
            }
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
                        if (koma.Player == PlayerType.FirstPlayer)
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
            game += HandToString(PlayerType.FirstPlayer) + "\n";
            game += HandToString(PlayerType.SecondPlayer) + "\n";
            game += "手番：" + State.TurnPlayer.ToString();
            return game;

            string HandToString(PlayerType player)
            {
                return player.ToString() + ":" + string.Join(",", State.KomaList.Where(x => x.IsInHand && x.Player == player).Select(x => x.KomaType.Id));
            }
        }
    }
}
