using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;

namespace Shogi.Bussiness.Domain.Model.Komas
{
    public interface IKomaState
    {
        public BoardPositions GetMovableBoardPositions(
            KomaType koma,
            Player player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions);
        public IKomaState ToBoard(BoardPosition toPosition, bool doTransform);

    }

    public class InHand : IKomaState
    {
        public static readonly InHand State = new InHand();
        private InHand() { }
        public BoardPositions GetMovableBoardPositions(
            KomaType koma,
            Player player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            // [手駒なら空き位置のどこでも置ける]
            var positions = board.Positions;
            positions = positions.Substract(playerKomaPositions);
            positions = positions.Substract(opponentPlayerKomaPositions);
            return positions;
        }


        public IKomaState ToBoard(BoardPosition toPosition, bool doTransform)
        {

            if(doTransform)
                throw new InvalidProgramException("打ち駒は成ることができません.");

            return new OnBoard(toPosition);
        }

        public override string ToString()
        {
            return "手駒";
        }
    }
    public class OnBoard : IKomaState
    {
        public BoardPosition Position { get; private set; }
        public bool IsTransformed { get;  private set;}

        public OnBoard(BoardPosition position)
        {
            Position = position;
            IsTransformed = false;
        }
        public OnBoard(BoardPosition position, bool isTransformed)
        {
            Position = position;
            IsTransformed = isTransformed;
        }
        public IKomaState ToBoard(BoardPosition toPosition, bool doTransform)
        {
            if(doTransform && IsTransformed)
                throw new InvalidProgramException("すでに成っているので成れません.");

            return new OnBoard(toPosition, IsTransformed || doTransform);
        }
        public BoardPositions GetMovableBoardPositions(
            KomaType komaType,
            Player player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            return komaType.GetMovableBoardPositions(
                                        player,
                                        Position,
                                        IsTransformed,
                                        board,
                                        playerKomaPositions,
                                        opponentPlayerKomaPositions);
        }

        public override string ToString()
        {
            return Position.ToString() + (IsTransformed ? "@" : "-");
        }

        public override bool Equals(object obj)
        {
            return obj is OnBoard board &&
                   EqualityComparer<BoardPosition>.Default.Equals(Position, board.Position) &&
                   IsTransformed == board.IsTransformed;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Position, IsTransformed);
        }

        public static bool operator ==(OnBoard left, OnBoard right)
        {
            return EqualityComparer<OnBoard>.Default.Equals(left, right);
        }

        public static bool operator !=(OnBoard left, OnBoard right)
        {
            return !(left == right);
        }
    }

}
