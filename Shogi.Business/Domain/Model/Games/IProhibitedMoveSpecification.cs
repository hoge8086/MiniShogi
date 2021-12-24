using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Shogi.Business.Domain.Model.Games
{
    public interface IProhibitedMoveSpecification
    {
        bool IsSatisfiedBy(MoveCommand moveCommand, Game game);
    }
    [DataContract]
    public class NullProhibitedMoveSpecification : IProhibitedMoveSpecification
    {
        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {
            return false;
        }

    }
    [DataContract]
    [KnownType(typeof(GameTemplates.CheckmateByHandHu))]
    [KnownType(typeof(GameTemplates.LeaveOte))]
    [KnownType(typeof(GameTemplates.NiHu))]
    [KnownType(typeof(GameTemplates.KomaCannotMove))]
    public class MultiProhibitedMoveSpecification : IProhibitedMoveSpecification
    {
        [DataMember]
        public List<IProhibitedMoveSpecification> ProhibitedMoveList { get; private set; }
        public MultiProhibitedMoveSpecification(List<IProhibitedMoveSpecification> prohibitedMoveList)
        {
            ProhibitedMoveList = prohibitedMoveList;
        }

        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {
            foreach (var prohibitedMove in ProhibitedMoveList)
                if (prohibitedMove.IsSatisfiedBy(moveCommand, game))
                    return true;

            return false;

        }
    }
}
