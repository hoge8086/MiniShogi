using Shogi.Business.Domain.Model.Players;
using System;
using System.Text;

namespace Shogi.Business.Domain.Model.Users
{
    public abstract class User
    {
        public abstract string Name { get; }
    }

    public class Human : User
    {
        public override string Name => "あなた";
    }
}
