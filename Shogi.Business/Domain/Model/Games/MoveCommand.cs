using Shogi.Bussiness.Domain.Model.Boards;
using Shogi.Bussiness.Domain.Model.Komas;
using Shogi.Bussiness.Domain.Model.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shogi.Bussiness.Domain.Model.Games
{
    public abstract class MoveCommand
    {
        public Player Player;
        public BoardPosition ToPosition;

        public MoveCommand(Player player, BoardPosition toPosition)
        {
            Player = player;
            ToPosition = toPosition;
        }
        public abstract Koma FindFromKoma(GameState state);

        public abstract bool DoTransform { get; }

        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();
    }
    public class BoardKomaMoveCommand : MoveCommand
    {
        public BoardPosition FromPosition;
        public override bool DoTransform { get; }

        public BoardKomaMoveCommand(Player player, BoardPosition toPosition, BoardPosition fromPosition, bool doTransform) : base(player, toPosition)
        {
            FromPosition = fromPosition;
            DoTransform = doTransform;
        }
        public override Koma FindFromKoma(GameState state)
        {
            return state.FindBoardKoma(FromPosition);
        }

        public override string ToString()
        {
            return string.Format("{0}->{1},{2}", FromPosition.ToString(), ToPosition.ToString(), (DoTransform ? "@" : "-"));
        }
        public override bool Equals(object obj)
        {
            return obj is BoardKomaMoveCommand command &&
                   EqualityComparer<Player>.Default.Equals(Player, command.Player) &&
                   EqualityComparer<BoardPosition>.Default.Equals(ToPosition, command.ToPosition) &&
                   DoTransform == command.DoTransform &&
                   EqualityComparer<BoardPosition>.Default.Equals(FromPosition, command.FromPosition) &&
                   DoTransform == command.DoTransform;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, ToPosition, DoTransform, FromPosition, DoTransform);
        }

        public static bool operator ==(BoardKomaMoveCommand left, BoardKomaMoveCommand right)
        {
            return EqualityComparer<BoardKomaMoveCommand>.Default.Equals(left, right);
        }

        public static bool operator !=(BoardKomaMoveCommand left, BoardKomaMoveCommand right)
        {
            return !(left == right);
        }
    }

    public class HandKomaMoveCommand : MoveCommand
    {
        public KomaType KomaType;

        public HandKomaMoveCommand(Player player, BoardPosition toPosition, KomaType komaType) : base(player, toPosition)
        {
            KomaType = komaType;
        }

        public override bool DoTransform => false;

        public override string ToString()
        {
            return string.Format("打:{0}->{1}", KomaType.ToString(), ToPosition.ToString());
        }
        public override bool Equals(object obj)
        {
            return obj is HandKomaMoveCommand command &&
                   EqualityComparer<Player>.Default.Equals(Player, command.Player) &&
                   EqualityComparer<BoardPosition>.Default.Equals(ToPosition, command.ToPosition) &&
                   DoTransform == command.DoTransform &&
                   EqualityComparer<KomaType>.Default.Equals(KomaType, command.KomaType) &&
                   DoTransform == command.DoTransform;
        }

        public override Koma FindFromKoma(GameState state)
        {
            return state.FindHandKoma(Player, KomaType);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Player, ToPosition, DoTransform, KomaType, DoTransform);
        }

        public static bool operator ==(HandKomaMoveCommand left, HandKomaMoveCommand right)
        {
            return EqualityComparer<HandKomaMoveCommand>.Default.Equals(left, right);
        }

        public static bool operator !=(HandKomaMoveCommand left, HandKomaMoveCommand right)
        {
            return !(left == right);
        }
    }
}
