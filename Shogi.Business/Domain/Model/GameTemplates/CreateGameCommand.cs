using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public class ProhibitedMoves
    {
        public bool EnableNiHu { get; set; }= false;
        public bool EnableCheckmateByHandHu { get; set; } = false;
        public bool EnableKomaCannotMove { get; set; } = false;
        public bool EnableLeaveOte { get; set; } = false;

        public ProhibitedMoves() { }
        public ProhibitedMoves(bool enableNiHu, bool enableCheckmateByHandHu, bool enableKomaCannotMove, bool enableLeaveOte)
        {
            EnableNiHu = enableNiHu;
            EnableCheckmateByHandHu = enableCheckmateByHandHu;
            EnableKomaCannotMove = enableKomaCannotMove;
            EnableLeaveOte = enableLeaveOte;
        }

        public IProhibitedMoveSpecification CreateProhibitedMoves()
        {
            var prohibitedMoves = new List<IProhibitedMoveSpecification>();
            if (EnableNiHu)
                prohibitedMoves.Add(new NiHu());
            if(EnableCheckmateByHandHu)
                prohibitedMoves.Add(new CheckmateByHandHu());
            if(EnableKomaCannotMove)
                prohibitedMoves.Add(new KomaCannotMove());
            if(EnableLeaveOte)
                prohibitedMoves.Add(new LeaveOte());
            return new MultiProhibitedMoveSpecification(prohibitedMoves);
        }
    }
    public enum WinConditionType
    {
        Checkmate,
        TakeKing,
        TakeKingOrEnterOpponentTerritory,
    }
    public class CreateGameCommand
    {
        public string Name { get; set; } = "新しい将棋";
        public int Width { get; set; } = 3;
        public int Height { get; set; } = 4;
        public int TerritoryBoundary { get; set; } = 1;
        public ProhibitedMoves ProhibitedMoves { get; set;} = new ProhibitedMoves();
        public WinConditionType WinCondition { get; set; } = WinConditionType.TakeKing;
        public List<Koma> KomaList { get; set; } = null;
        public PlayerType TurnPlayer { get; set; } = PlayerType.Player1;

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
        public IWinningChecker CreateWinningChecker() => winningDictionary[WinCondition];

        public GameTemplate Create(List<KomaType> komaTypes)
        {
            return new GameTemplate()
            {
                Name = Name,
                Game = new Game(
                    new Boards.Board(Height, Width),
                    new GameState(KomaList, TurnPlayer),
                    new CustomRule(
                        TerritoryBoundary,
                        ProhibitedMoves.CreateProhibitedMoves(),
                        CreateWinningChecker()),
                    komaTypes)
            };
        }
        public CreateGameCommand() { }
        public CreateGameCommand(string name, int width, int height, int territoryBoundary, WinConditionType winCondition, List<Koma> komaList, ProhibitedMoves prohibitedMoves, PlayerType turnPlayer)
        {
            Name = name;
            Width = width;
            Height = height;
            TerritoryBoundary = territoryBoundary;
            ProhibitedMoves = prohibitedMoves;
            WinCondition = winCondition;
            KomaList = komaList;
            TurnPlayer = turnPlayer;
        }
    }

}

