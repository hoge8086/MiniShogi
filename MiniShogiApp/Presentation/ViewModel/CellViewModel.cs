using Shogi.Bussiness.Domain.Model.Boards;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MiniShogiApp.Presentation.ViewModel
{
    public enum Player
    {
        FirstPlayer,
        SecondPlayer,
    }
    public static partial class PlayerExtend
    {
        public static Shogi.Bussiness.Domain.Model.Players.Player ToDomain(this Player player)
        {
            switch (player) {
                case Player.FirstPlayer:
                    return Shogi.Bussiness.Domain.Model.Players.Player.FirstPlayer;
                case Player.SecondPlayer:
                    return Shogi.Bussiness.Domain.Model.Players.Player.SecondPlayer;
            }
            return null;
        }
    }

    public class KomaViewModel
    {
        public bool IsTransformed { get; set; }
        public Player Player { get; set; }

        public string Name { get; set; }
    }
    public class CellViewModel
    {
        public BoardPosition Position { get; set; }
        public KomaViewModel Koma { get; set; }
        public string KomaName => Koma == null ? "" : Koma.Name;
        public bool CanMove { get; set; } = false;

        public bool IsSelected { get; set; } = false;
    }
}
