using System;
using System.Collections.Generic;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Komas
{
    public class Koma
    {
        public IPosition Position { get; private set; }
        public Player Player { get;  private set;}
        public KomaType KomaType { get;  private set;}

        // [TODO:手持ちの駒は成りの概念がないので、ちょっと違和感あり]
        public bool IsTransformed { get;  private set;}

        public Koma(IPosition position, Player player, KomaType komaType)
        {
            Position = position;
            Player = player;
            KomaType = komaType;
            IsTransformed = false;
        }

        public override string ToString()
        {
            return string.Format("{0}:player={1},position={2},IsTransformed={3}", KomaType.ToString(), Player.ToString(), Position.ToString(), IsTransformed);
        }

        public void Move(BoardPosition toPosition, bool doTransform)
        {

            var oldPosition = Position;
            Position = toPosition;
            if(doTransform)
            {
                if(oldPosition == HandPosition.Hand)
                    throw new InvalidOperationException("打ち駒は成ることができません.");
                if(!KomaType.CanBeTransformed)
                    throw new InvalidOperationException("この駒は成ることができません.");
                if(IsTransformed)
                    throw new InvalidOperationException("すでに成っているので成れません.");

                IsTransformed = true;
            }
        }
        public void Taken()
        {
            Position = HandPosition.Hand;
            Player = Player.Opponent;
            IsTransformed = false;
        }

        public BoardPositions GetMovableBoardPositions(
            Board board,
            BoardPositions turnPlayerKomaPositions,
            BoardPositions iopponentKomaPositions)
        {
            return KomaType.GetMovableBoardPositions(
                                        Player,
                                        Position,
                                        IsTransformed,
                                        board,
                                        turnPlayerKomaPositions,
                                        iopponentKomaPositions);
        }
    }
}
