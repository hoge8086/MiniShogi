using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Linq;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public class GameFactory
    {
        //private IKomaTypeRepository KomaTypeRepository;

        private static Dictionary<WinConditionType, IWinningChecker> winningDictionary = new Dictionary<WinConditionType, IWinningChecker>()
        {
            { WinConditionType.Checkmate,  new CheckmateWinningChecker()},
            { WinConditionType.TakeKing,  new TakeKingWinningChecker()},
            { WinConditionType.TakeKingOrEnterOpponentTerritory,  new MultiWinningChecker(
                new List<IWinningChecker>(){
                    new TakeKingWinningChecker(),
                    new EnterOpponentTerritoryWinningChecker()
                })},
        };
        public static IWinningChecker CreateWinningChecker(WinConditionType winCondition) => winningDictionary[winCondition];

        public static IProhibitedMoveSpecification CreateProhibitedMoves(ProhibitedMoves prohibited)
        {
            var prohibitedMoves = new List<IProhibitedMoveSpecification>();
            if (prohibited.EnableNiHu)
                prohibitedMoves.Add(new NiHu());
            if(prohibited.EnableCheckmateByHandHu)
                prohibitedMoves.Add(new CheckmateByHandHu());
            if(prohibited.EnableKomaCannotMove)
                prohibitedMoves.Add(new KomaCannotMove());
            if(prohibited.EnableLeaveOte)
                prohibitedMoves.Add(new LeaveOte());
            return new MultiProhibitedMoveSpecification(prohibitedMoves);
        }

        public GameFactory()//IKomaTypeRepository komaTypeRepository)
        {
            //KomaTypeRepository = komaTypeRepository;
        }
        public Game Create(GameTemplate gameTemplate, PlayerType firstTurnPlayer)
        {
            //var komaTypes = gameTemplate.KomaList.Select(x => {
            //    var type =KomaTypeRepository.FindById(x.TypeId);
            //    if (type == null)
            //        throw new System.Exception($"駒[{type.Id}]が存在しません.");
            //    return type;
            //}).ToList();
            //return createGameCommand.Create(komaTypes);
            return new Game(
                    new Boards.Board(gameTemplate.Height, gameTemplate.Width),
                    new GameState(gameTemplate.KomaList, firstTurnPlayer),
                    new CustomRule(
                        gameTemplate.TerritoryBoundary,
                        CreateProhibitedMoves(gameTemplate.ProhibitedMoves),
                        CreateWinningChecker(gameTemplate.WinCondition)),
                    gameTemplate.KomaTypes);

        }
    }

}

