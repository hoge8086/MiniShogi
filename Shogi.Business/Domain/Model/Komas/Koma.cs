using System;
using System.Collections.Generic;
using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Players;

namespace Shogi.Bussiness.Domain.Model.Komas
{

    public class Koma
    {
        public Player Player { get;  private set;}
        public KomaType KomaType { get;  private set;}

        public IKomaState State { get; private set; }


        public Koma(Player player, KomaType komaType, IKomaState state)
        {
            Player = player;
            KomaType = komaType;
            State = state;
        }

        public void Move(BoardPosition toPosition, bool doTransform)
        {
            if(doTransform)
            {
                if(State is InHand)
                    throw new InvalidProgramException("打ち駒は成ることができません.");
                if(!KomaType.CanBeTransformed)
                    throw new InvalidProgramException("この駒は成ることができません.");
                if((State is OnBoard) && ((OnBoard)State).IsTransformed)
                    throw new InvalidProgramException("すでに成っているので成れません.");
            }

            State = new OnBoard(toPosition, doTransform);

        }
        public void Taken()
        {
            if(State is InHand)
                throw new InvalidProgramException("持ち駒を取ることはできません.");

            Player = Player.Opponent;
            State = InHand.State;
        }

        public BoardPositions GetMovableBoardPositions(
            Board board,
            BoardPositions playerKomaPositions,
            BoardPositions opponentPlayerKomaPositions)
        {
            return State.GetMovableBoardPositions(
                                        KomaType,
                                        Player,
                                        board,
                                        playerKomaPositions,
                                        opponentPlayerKomaPositions);
        }
        public BoardPosition BoardPosition => (State as OnBoard)?.Position;
        public bool IsOnBoard => State is OnBoard;
        public bool IsInHand => State is InHand;

        public bool IsTransformed => (State is OnBoard) && ((OnBoard)State).IsTransformed;
        public Koma Clone()
        {
            return new Koma(Player, KomaType, State);
        }
        public override string ToString()
        {
            return string.Format("{0}:player={1},state={2},IsTransformed={3}", KomaType.ToString(), Player.ToString(), State.ToString());
        }

    }
}
