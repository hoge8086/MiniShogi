using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Text;

namespace Shogi.Business.Domain.Model.Players
{
    public abstract class Player
    {
        public abstract string Name { get; }
    }

    public class Human : Player
    {
        public override string Name => "あなた";
    }
}
