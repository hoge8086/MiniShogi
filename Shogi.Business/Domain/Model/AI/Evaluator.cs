using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using Shogi.Business.Domain.Model.Moves;
using System.Linq;
using System.Collections.Generic;
using Shogi.Business.Domain.Model.Komas;
using IsTransformed = System.Boolean;


namespace Shogi.Business.Domain.Model.AI
{
    public abstract class Evaluator
    {
        public Evaluator(Game game)
        {
            MaxEvaluationValue = CalcMaxEvaluationValue(game);
            if (MaxEvaluationValue <= 0)
                throw new InvalidProgramException("最大評価値がマイナスのためプログラム不正です.");
        }
        public abstract GameEvaluation Evaluate(Game game, int beginingMoveCount, int maxDepth);

        protected abstract int CalcMaxEvaluationValue(Game game);
        protected int MaxEvaluationValue { get; private set; }
        //public abstract int Evaluate(Game game);
    }

    /// <summary>
    /// 駒の損得の評価
    /// </summary>
    public class LossAndGainOfKomaEvaluator : Evaluator
    {
        // 駒の評価値を事前評価(計算量削減のためのキャッシュ)
        Dictionary<(KomaTypeId, IsTransformed), int> KomaEvaluations;
        public LossAndGainOfKomaEvaluator(Game game) : base(game)
        {
            KomaEvaluations = new Dictionary<(KomaTypeId, IsTransformed), int>();
            foreach(var komaType in game.KomaTypes)
            {
                KomaEvaluations.Add((komaType.Id, false), EvaluateMoves(komaType.Moves));
                if(komaType.CanBeTransformed)
                    KomaEvaluations.Add((komaType.Id, true), EvaluateMoves(komaType.TransformedMoves));
            }
        }
        /// <summary>
        /// 駒の価値
        /// </summary>
        /// <param name="koma"></param>
        /// <param name="onBoard">ボード上の価値として評価するか？(falseの場合は手持ちとしての価値)</param>
        /// <returns></returns>
        int EvaluateKoma(Koma koma, bool onBoard)
        {
            if(onBoard)
                return KomaEvaluations[(koma.TypeId, koma.IsTransformed)];

            return KomaEvaluations[(koma.TypeId, false)];
        }


        /// <summary>
        /// 評価値の計算
        /// 負け／勝ちが確定した際も最善手(最長手/最短手)を指すように、手の深さを評価値に加えた評価を返す
        /// 従って、評価値はMaxEvaluationValueの値を超えることがある
        /// </summary>
        /// <param name="game"></param>
        /// <param name="player"></param>
        /// <param name="remainingDepth"></param>
        /// <returns></returns>
        public override GameEvaluation Evaluate(Game game, int beginingMoveCount, int maxDepth)
        {
            if(game.State.IsEnd)
            {
                if (game.State.GameResult.Winner == game.State.TurnPlayer)
                    return WiningEvaluation();
                else
                    return LosingEvaluation();
            }

            // [どうぶつ将棋だと勝敗判定にチェックメイトがないので、]
            // [最後の読みで玉の捨て身で相手の駒をとることが可能になってしまい]
            // [1手浅い読みになってしまうのでここで評価する]
            // [TODO:同様に入玉も考慮したほうが良い(次の手で入玉できる/されてしまうケース)]
            if (game.DoOte(game.State.TurnPlayer))
            {
                return WiningEvaluation();
            }

            // [駒得基準の判定]
            int evaluationValue = 0;

            var playerKomaMobablePositions = game.GetKomaMovablePosition(game.State.GetBoardKomaList(game.State.TurnPlayer));
            var opponetMobablePositions = game.MovablePosition(game.State.GetBoardKomaList(game.State.TurnPlayer.Opponent), true);

            // [手番のプレイヤーが取ることのできる駒の価値]
            int takenKomaEval = 0;

            foreach(var koma in game.State.KomaList)
            {
                // [駒の持ち主に応じて、駒の価値を加算]
                evaluationValue += (game.State.TurnPlayer == koma.Player) ? EvaluateKoma(koma, true) : -EvaluateKoma(koma, true);

                // [以下、手番のプレイヤーが取ることのできる駒の価値を考慮する]

                // [その駒を手番のプレイヤーがタダで取ることができる場合]
                if(koma.IsOnBoard && koma.Player == game.State.TurnPlayer.Opponent && playerKomaMobablePositions.Keys.Contains(koma.BoardPosition) && !opponetMobablePositions.Contains(koma.BoardPosition))
                {
                    // [取ったときの価値を計算: 相手は盤上の駒がなくなり、自身は手持ちに駒が増える]
                    var val = EvaluateKoma(koma, true) + EvaluateKoma(koma, false);

                    // [取る手は一手しか指せないので他の取る手より価値が高ければ更新]
                    if (val > takenKomaEval)
                        takenKomaEval = val;
                }

                // [その駒を手番のプレイヤーが取ることができるが、相手に取り返される場合]
                if(koma.IsOnBoard && koma.Player == game.State.TurnPlayer.Opponent && playerKomaMobablePositions.Keys.Contains(koma.BoardPosition) && opponetMobablePositions.Contains(koma.BoardPosition))
                {
                    // [手番のプレイヤーがその位置に移動できる最も価値の低い駒を探す]
                    var lowerEvalKoma = playerKomaMobablePositions[koma.BoardPosition].MinBy(x => EvaluateKoma(x, false));

                    // [取り返される駒より、取る駒の方が価値が高ければ取る]
                    if ((EvaluateKoma(lowerEvalKoma, false) < EvaluateKoma(koma, false)) ||
                        (EvaluateKoma(lowerEvalKoma, false) == EvaluateKoma(koma, false) && EvaluateKoma(lowerEvalKoma, true) < EvaluateKoma(koma, true)))
                    {
                        // [取ったときの価値を計算: 相手は盤上の駒がなくなり、自身は手持ちに駒が増える、自分も盤上の駒がなくなり、相手は手持ちに駒が増える]
                        var val = EvaluateKoma(koma, true) + EvaluateKoma(koma, false) -  EvaluateKoma(lowerEvalKoma, true) - EvaluateKoma(lowerEvalKoma, false);

                        // [取る手は一手しか指せないので他の取る手より価値が高ければ更新]
                        if (val > takenKomaEval)
                            takenKomaEval = val;
                    }
                }
            }

            // [取る手を評価に加える]
            evaluationValue += takenKomaEval;

            return new GameEvaluation(evaluationValue, MaxEvaluationValue, game, game.State.TurnPlayer, beginingMoveCount);

            GameEvaluation WiningEvaluation()
            {
                // 勝ち確の場合は、最短手（残っている探索の深さが多い）ほど、評価が高い
                return new GameEvaluation(MaxEvaluationValue + CalcRemainingDepth(), MaxEvaluationValue, game, game.State.TurnPlayer, beginingMoveCount);
            }
            GameEvaluation LosingEvaluation()
            {
                // 負け確の場合は、最短手（残っている探索の深さが多い）ほど、評価が低い
                return new GameEvaluation(-MaxEvaluationValue - CalcRemainingDepth(), MaxEvaluationValue, game, game.State.TurnPlayer, beginingMoveCount);
            }
            int CalcRemainingDepth()
            {
                var remainingDepth = maxDepth -  (game.Record.CurrentMovesCount - beginingMoveCount);
                return remainingDepth > 0 ? remainingDepth : 0;
            }
        }

        /// <summary>
        /// 探索の深さは考慮しない最大の評価値(駒の損得の最大値)
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        protected override int CalcMaxEvaluationValue(Game game)
        {
            // TODO:王は取られたら負けなのでそれを差し引きたい(王の成りの考慮が必要?)
            // 王を含め全ての駒評価値の合算を最大の評価値とする(王がとられていると負けのためこの指標を超えることはない)
            return game.State.KomaList.Sum(koma =>
            {
                var komaType = game.GetKomaType(koma.TypeId);
                // 成りがある駒の場合で、成りの方が評価値が高ければ成りの評価値を採用
                return System.Math.Max(
                    EvaluateMoves(komaType.Moves),
                    komaType.CanBeTransformed ? EvaluateMoves(komaType.TransformedMoves) : 0);
            });
        }

        public static int EvaluateMoves(KomaMoves moves)
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

    public static class Ext {
        public static T MinBy<T, U>(this IEnumerable<T> xs, Func<T, U> key) where U : IComparable<U> {
            return xs.Aggregate((a, b) => key(a).CompareTo(key(b)) < 0 ? a : b);
        }

        public static T MaxBy<T, U>(this IEnumerable<T> xs, Func<T, U> key) where U : IComparable<U> {
            return xs.Aggregate((a, b) => key(a).CompareTo(key(b)) > 0 ? a : b);
        }
    }

}
