using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Runtime.Serialization;
using System.Text;

namespace Shogi.Business.Domain.Model.Players
{
    [DataContract]
    public abstract class Player
    {
        public abstract string Name { get; }
        public abstract bool IsAI { get; }
        public bool IsHuman => !IsAI;
    }

    [DataContract]
    public class Human : Player
    {
        public override string Name => "あなた";
        public override bool IsAI => false;
    }
}
