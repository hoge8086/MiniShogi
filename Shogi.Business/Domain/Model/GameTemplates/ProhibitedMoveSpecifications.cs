using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    /// <summary>
    /// 禁じ手：二歩
    /// </summary>
    [DataContract]
    public class NiHu : IProhibitedMoveSpecification
    {
        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {
            return (moveCommand is HandKomaMoveCommand) && 
                    ((HandKomaMoveCommand)moveCommand).KomaType.IsHu &&
                   game.State.KomaList.Any(x =>
                                    x.Player == moveCommand.Player &&
                                    x.KomaType.IsHu &&
                                    !x.IsTransformed &&
                                    x.IsOnBoard &&
                                    x.BoardPosition.X == moveCommand.ToPosition.X);
        }
    }
    /// <summary>
    /// 禁じ手：打ち歩詰め
    /// </summary>
    [DataContract]
    public class CheckmateByHandHu : IProhibitedMoveSpecification
    {
        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {

            return (moveCommand is HandKomaMoveCommand) &&
                   ((HandKomaMoveCommand)moveCommand).KomaType.IsHu &&
                   game.Clone().PlayWithoutRecord(moveCommand).DoCheckmateWithoutHandMove(moveCommand.Player);
        }
    }
    /// <summary>
    /// 禁じ手：動かせない駒
    /// </summary>
    [DataContract]
    public class KomaCannotMove : IProhibitedMoveSpecification
    {
        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {

            var fromKoma = moveCommand.FindFromKoma(game.State);
            // [その駒以外に他の駒が一つもないボードで動けるところが何もない場合は、行き場のない駒]
            return new Koma( moveCommand.Player, moveCommand.FindFromKoma(game.State).KomaType, new OnBoard(moveCommand.ToPosition,(moveCommand.DoTransform || fromKoma.IsTransformed)))
                        .GetMovableBoardPositions(game.Board, new BoardPositions(), new BoardPositions())
                        .Positions.Count == 0;
        }
    }
    /// <summary>
    /// 禁じ手：王手放置
    /// </summary>
    [DataContract]
    public class LeaveOte: IProhibitedMoveSpecification
    {
        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {
            return game.Clone().PlayWithoutRecord(moveCommand).DoOte(moveCommand.Player.Opponent);
        }
    }
}
