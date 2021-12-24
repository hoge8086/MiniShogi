using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Komas
{
    public interface IKomaState
    {
        BoardPositions GetMovableBoardPositions(
            KomaType koma,
            PlayerType player,
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions);
        IKomaState ToBoard(BoardPosition toPosition, bool doTransform);

    }

    [DataContract]
    public class InHand : IKomaState
    {
        /// <remark>
        /// シリアライズ化するのであれば、シングルトンは使えないので、このインスタンスで同一性を判断してはいけない
        /// </remark>
        public static readonly InHand State = new InHand();
        private InHand() { }
        public BoardPositions GetMovableBoardPositions(
            KomaType koma,
            PlayerType player,
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

    [DataContract]
    public class OnBoard : IKomaState
    {
        [DataMember]
        public BoardPosition Position { get; private set; }
        [DataMember]
        public bool IsTransformed { get; private set;}

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
            PlayerType player,
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
