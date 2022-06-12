﻿using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using Shogi.Business.Domain.Model.Moves;
using System.Linq;

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
        public abstract GameEvaluation Evaluate(Game game, PlayerType player);

        protected abstract int CalcMaxEvaluationValue(Game game);
        protected int MaxEvaluationValue { get; private set; }
        //public abstract int Evaluate(Game game);
    }

    /// <summary>
    /// 駒の損得の評価
    /// </summary>
    public class LossAndGainOfKomaEvaluator : Evaluator
    {
        public LossAndGainOfKomaEvaluator(Game game) : base(game) { }
        public override GameEvaluation Evaluate(Game game, PlayerType player)
        {
            if(game.State.IsEnd)
            {
                if (game.State.GameResult.Winner == player)
                    return GameEvaluation.WiningEvaluation(MaxEvaluationValue, game, player);
                else
                    return GameEvaluation.LosingEvaluation(MaxEvaluationValue, game, player);
            }

            // [どうぶつ将棋だと勝敗判定にチェックメイトがないので、]
            // [最後の読みで玉の捨て身で相手の駒をとることが可能になってしまい]
            // [1手浅い読みになってしまうのでここで評価する]
            // [TODO:同様に入玉も考慮したほうが良い(次の手で入玉できる/されてしまうケース)]
            if (game.DoOte(player) && game.State.TurnPlayer == player)
            {
                return GameEvaluation.WiningEvaluation(MaxEvaluationValue, game, player);
            }
            if (game.DoOte(player.Opponent) && game.State.TurnPlayer == player.Opponent)
            {
                return GameEvaluation.LosingEvaluation(MaxEvaluationValue, game, player);
            }

            // [駒得基準の判定]
            int evaluationValue = 0;

            foreach(var koma in game.State.KomaList)
            {
                int movablePositionCount = Evaluation(koma.IsTransformed ? game.GetKomaType(koma).TransformedMoves : game.GetKomaType(koma).Moves);
                evaluationValue += (player == koma.Player) ? movablePositionCount : -movablePositionCount;
            }

            return new GameEvaluation(evaluationValue, MaxEvaluationValue, game, player);
        }

        protected override int CalcMaxEvaluationValue(Game game)
        {
            // TODO:王は取られたら負けなのでそれを差し引きたい(王の成りの考慮が必要?)
            // 王を含め全ての駒評価値の合算を最大の評価値とする(王がとられていると負けのためこの指標を超えることはない)
            return game.State.KomaList.Sum(koma =>
            {
                var komaType = game.GetKomaType(koma.TypeId);
                // 成りがある駒の場合で、成りの方が評価値が高ければ成りの評価値を採用
                return System.Math.Max(
                    Evaluation(komaType.Moves),
                    komaType.CanBeTransformed ? Evaluation(komaType.TransformedMoves) : 0);
            });
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