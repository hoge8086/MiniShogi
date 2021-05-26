using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.Games
{
    public interface IProhibitedMoveSpecification
    {
        bool IsSatisfiedBy(MoveCommand moveCommand, Game game);
    }
    public class NullProhibitedMoveSpecification : IProhibitedMoveSpecification
    {
        public bool IsSatisfiedBy(MoveCommand moveCommand, Game game)
        {
            return false;
        }

    }
    public class MultiProhibitedMoveSpecification : IProhibitedMoveSpecification
    {
        public List<IProhibitedMoveSpecification> ProhibitedMoveList;
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
