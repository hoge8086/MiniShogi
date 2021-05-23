using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Bussiness.Domain.Model
{

    public class Game
    {
        public Board Board;
        public GameState State;
        public Rule Rule;

        public Game(Board board, GameState state, Rule rule)
        {
            Board = board;
            State = state;
            Rule = rule;
        }

        public void Play(MoveCommand moveCommand)
        {
            if(!State.IsTurnPlayer(moveCommand.Player))
                throw new InvalidOperationException("プレイヤーのターンではありません.");

            var fromKoma = moveCommand.FindFromKoma(State);
            if(!State.IsTurnPlayer(fromKoma.Player))
                throw new InvalidOperationException("プレイヤーの駒ではないため動かせません.");

            var toKoma = State.FindBoardKoma(moveCommand.ToPosition);
            if(toKoma != null && toKoma.Player == State.TurnPlayer)
                throw new InvalidOperationException("移動先にあなたの駒があるため動かせません.");


            fromKoma.Move(moveCommand.ToPosition, moveCommand.DoTransform);
            
            if(toKoma != null)
            {
                toKoma.Taken();
            }

            State.FowardTurnPlayer();
            State.CheckEnd();

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

        public List<MoveCommand> GetAvailableMoveCommand()
        {
            return GetAvailableMoveCommand(State.TurnPlayer);
        }
        public List<MoveCommand> GetAvailableMoveCommand(Player player)
        {
            var moveCommandList = new List<MoveCommand>();
            foreach(var koma in State.KomaList.Where(x => x.Player == player).ToList())
            {

                // [持ち駒の場合で、同じ駒が複数ある場合は、2回目以降はスキップする]
                if (koma.Position is HandPosition)
                    if (moveCommandList.Where(x => x is HandKomaMoveCommand).Any(x => ((HandKomaMoveCommand)x).KomaType == koma.KomaType))
                        continue;

                moveCommandList.AddRange(GetAvailableMoveCommand(koma));
            }

            return moveCommandList;
        }

        private List<MoveCommand> GetAvailableMoveCommand(Komas.Koma koma)
        {
            var moveCommandList = new List<MoveCommand>();
            var positions = koma.GetMovableBoardPositions(
                    Board,
                    State.BoardPositions(koma.Player),
                    State.BoardPositions(koma.Player.Opponent));

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
            return moveCommandList;
        }
    }
}
