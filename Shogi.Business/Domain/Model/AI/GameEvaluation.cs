using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace Shogi.Business.Domain.Model.AI
{
    public class GameEvaluation
    {
        public int Value { get; private set; }
        public Game Game { get; private set; }
        public PlayerType PlayerType { get; private set; }

        private static readonly int InfiniteEvaluationValue = 99999999;

        //public bool IsWin => Value == InfiniteEvaluationValue;
        //public bool IsLose => Value == -InfiniteEvaluationValue;
        public static readonly GameEvaluation DefaultPositiveEvaluation = new GameEvaluation(InfiniteEvaluationValue + 1, null, null);
        public static readonly GameEvaluation DefaultNegativeEvaluation = new GameEvaluation(-InfiniteEvaluationValue-1, null, null);

        private GameEvaluation(int value, Game game, PlayerType player)
        {
            Value = value;
            Game = game;
            PlayerType = player;
        }

        public GameEvaluation(Game game, PlayerType player)
        {
            this.Game = game;
            this.PlayerType = player;

            if(game.State.IsEnd)
            {
                if(game.State.GameResult.Winner == player)
                    this.Value = InfiniteEvaluationValue;
                else
                    this.Value = -InfiniteEvaluationValue;
                return;
            }

            // [どうぶつ将棋だと勝敗判定にチェックメイトがないので、]
            // [最後の読みで玉の捨て身で相手の駒をとることが可能になってしまい]
            // [1手浅い読みになってしまうのでここで評価する]
            // [TODO:同様に入玉も考慮したほうが良い(次の手で入玉できる/されてしまうケース)]
            if (game.DoOte(player) && game.State.TurnPlayer == player)
            {
                this.Value = InfiniteEvaluationValue;
                return;
            }
            if (game.DoOte(player.Opponent) && game.State.TurnPlayer == player.Opponent)
            {
                this.Value = -InfiniteEvaluationValue;
                return;
            }

            // [駒得基準の判定]
            int evaluationValue = 0;

            foreach(var koma in game.State.KomaList)
            {
                int movablePositionCount = Evaluation(koma.IsTransformed ? game.GetKomaType(koma).TransformedMoves : game.GetKomaType(koma).Moves);
                evaluationValue += (player == koma.Player) ? movablePositionCount : -movablePositionCount;
            }
            this.Value = evaluationValue;
        }
        public GameEvaluation Reverse()
        {
            return new GameEvaluation(-Value, Game, PlayerType?.Opponent);
        }

        public static int Evaluation(KomaMoves moves)
        {
            int movablePositionCount = 0;
            foreach(var move in moves.Moves)
            {
                var moveBase = move as KomaMoveBase;
                if (moveBase != null && !moveBase.IsRepeatable)
                    movablePositionCount += 1;

                else if (moveBase != null && moveBase.IsRepeatable)
                    movablePositionCount += 2;
            }
            return movablePositionCount;
        }

    }
}
