using Shogi.Business.Domain.Model.Players;
using System;
using System.Text;

namespace Shogi.Business.Domain.Model.Users
{
    public abstract class User
    {
        string Name { get; set; }
    }

    public class Human : User
    {

    }
}
