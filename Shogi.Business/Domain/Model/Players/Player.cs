using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Text;

namespace Shogi.Business.Domain.Model.Players
{
    public abstract class Player
    {
        public abstract string Name { get; }
        public abstract bool IsAI { get; }
        public bool IsHuman => !IsAI;
    }

    public class Human : Player
    {
        public override string Name => "あなた";
        public override bool IsAI => false;
    }
}
