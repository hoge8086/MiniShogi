using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Shogi.Business.Domain.Model.AI
{
    [DataContract]
    public class NegaAlphaAI : AI
    {
        public override string ToString() => "AI (" + Depth.ToString() + "手読み)";
        [DataMember]
        public int Depth { get; set;}
        //private bool debug = true;
        private Evaluator evaluator = null;

        public NegaAlphaAI(int depth)
        {
            this.Depth = depth;
        }
        public MoveEvaluation SelectMove(Game game, CancellationToken cancellation, Action<ProgressRate> progress)
        {
            // MEMO:できればコンストラクタでやりたい
            evaluator = new LossAndGainOfKomaEvaluator(game);

            //if(debug)
            //{
            //    System.Diagnostics.Debug.WriteLine("▽▽▽▽▽▽▽▽▽▽[探索開始]▽▽▽▽▽▽▽▽▽▽");
            //    System.Diagnostics.Debug.WriteLine("深さ：" + Depth.ToString()) ;
            //    System.Diagnostics.Debug.WriteLine("---------盤面------------") ;
            //    System.Diagnostics.Debug.WriteLine(game.ToString()) ;
            //    System.Diagnostics.Debug.WriteLine("手番：" + game.State.TurnPlayer.ToString()) ;
            //    System.Diagnostics.Debug.WriteLine("---------[探索結果]------------") ;
            //}

            // [MEMO:アルファベータ法は枝刈りを行うので複数の最善手を得られない?]
            //var bestMove = Search(game, Depth, cancellation, progress);
            var bestMove = SearchParallel(game, Depth, cancellation, progress);

            //if (debug)
            //{

            //    System.Diagnostics.Debug.WriteLine("---------[ベスト手]------------") ;
            //    var temp = bestMove.GameEvaluation.Game.Clone();
            //    System.Diagnostics.Debug.WriteLine($"{temp.ToString()}");
            //    while(temp.CanUndo(Game.UndoType.Undo))
            //    {
            //        temp.Undo(Game.UndoType.Undo);
            //        System.Diagnostics.Debug.WriteLine($"{temp.ToString()}");
            //    }
            //    System.Diagnostics.Debug.WriteLine("△△△△△△△△△△[探索終了]△△△△△△△△△△");
            //}

            return bestMove;
        }


        public List<MoveCommand> SortByBetterMove(List<MoveCommand> moveCommands, Game game)
        {
            // [MEMO:手の偏りを防ぐためランダムソートする]
            moveCommands = moveCommands.OrderBy(i => System.Guid.NewGuid()).ToList();

            var priority1 = new List<MoveCommand>();
            var priority2 = new List<MoveCommand>();
            var remaining = new List<MoveCommand>();
            foreach(var move in moveCommands)
            {
                // [価値の低い駒で価値の高い駒を取る場合は良い手]
                //var fromKoma = move.FindFromKoma(game.State);
                var fromKoma = game.FindFromKoma(move);
                var toKoma = game.State.FindBoardKoma(move.ToPosition);
                // MEMO:LossAndGainOfKomaEvaluatorの直接参照はよくないが、ここは駒の損得での判定のためある意味正しい
                if (toKoma != null && LossAndGainOfKomaEvaluator.EvaluateMoves(game.GetKomaType(fromKoma).Moves) < LossAndGainOfKomaEvaluator.EvaluateMoves(game.GetKomaType(toKoma).Moves))
                    priority1.Add(move);
                // [成る手(なった場合に動きが増える場合)は良い手(成り捨ても上位に上がってしまうのは微妙だが)]
                else if (move.DoTransform && LossAndGainOfKomaEvaluator.EvaluateMoves(game.GetKomaType(fromKoma).Moves) < LossAndGainOfKomaEvaluator.EvaluateMoves(game.GetKomaType(fromKoma).TransformedMoves))
                    priority2.Add(move);
                else
                    remaining.Add(move);
            }

            var marged = new List<MoveCommand>();
            marged.AddRange(priority1);
            marged.AddRange(priority2);
            marged.AddRange(remaining);

            return marged;
        }
        private MoveEvaluation Search(Game game, int depth, CancellationToken cancellation, Action<ProgressRate> progress)
        {
            MoveEvaluation bestMove = null;

            GameEvaluation alpha = null;
            GameEvaluation beta = null;
            int beginingMoveCount = game.Record.CurrentMovesCount;

            cancellation.ThrowIfCancellationRequested();

            if (game.State.IsEnd || depth <= 0)
                throw new System.InvalidOperationException("すでに決着がついているため手の探索は不正です.");

            var moveCommands = game.CreateAvailableMoveCommand();

            // [αβ法は良い手の順に探索を行うと最も効率が良い]
            moveCommands = SortByBetterMove(moveCommands, game);


            // [全ての子ノードを展開し，再帰的に評価]
            for(int i=0; i<moveCommands.Count; i++)
            {
                var gameTmp = game.Clone().PlayWithoutCheck(moveCommands[i]);
                var eval = SearchSub(gameTmp, beta?.Reverse(), alpha?.Reverse(), beginingMoveCount, cancellation).Reverse();

                // [最善手(MAX)を求める]
                if((alpha == null) || (alpha.Value < eval.Value))
                {
                    alpha = eval;   // [α値の更新]
                    bestMove = new MoveEvaluation(moveCommands[i], eval);
                }

                //if(debug)
                //{
                //    System.Diagnostics.Debug.WriteLine(string.Join("\n", new MoveEvaluation(moveCommands[i], eval))) ;
                //}

                progress?.Invoke(new ProgressRate(i+1, moveCommands.Count));
            }


            return bestMove;
        }
        private MoveEvaluation SearchParallel(Game game, int depth, CancellationToken cancellation, Action<ProgressRate> progress)
        {
            MoveEvaluation bestMove = null;

            GameEvaluation alpha = null;
            GameEvaluation beta = null;
            int beginingMoveCount = game.Record.CurrentMovesCount;

            cancellation.ThrowIfCancellationRequested();

            if (game.State.IsEnd || depth <= 0)
                throw new System.InvalidOperationException("すでに決着がついているため手の探索は不正です.");

            var moveCommands = game.CreateAvailableMoveCommand();

            // [αβ法は良い手の順に探索を行うと最も効率が良い]
            moveCommands = SortByBetterMove(moveCommands, game);


            var lockObj = new Object();
            int searchedMoveCount = 0;

            // [全ての子ノードを展開し，再帰的に評価]
            try
            {
                // [各手の探索を並立処理]
                Parallel.ForEach(
                    moveCommands,
                    new ParallelOptions() {  MaxDegreeOfParallelism = 4},
                    (move =>
                    {
                        var gameTmp = game.Clone().PlayWithoutCheck(move);
                        var eval = SearchSub(gameTmp, beta?.Reverse(), alpha?.Reverse(), beginingMoveCount, cancellation).Reverse();

                        lock (lockObj)
                        {
                            // [最善手(MAX)を求める]
                            if ((alpha == null) || (alpha.Value < eval.Value))
                            {
                                alpha = eval;   // [α値の更新]
                                bestMove = new MoveEvaluation(move, eval);
                            }
                            // [進捗の表示]
                            searchedMoveCount++;
                            progress?.Invoke(new ProgressRate(searchedMoveCount, moveCommands.Count));
                        }
                    }
                ));
            }catch(AggregateException ex) when(ex.InnerExceptions.Any(x => x is OperationCanceledException))
            {
                throw new OperationCanceledException("思考を中断しました", ex);
            }

            return bestMove;
        }

        private GameEvaluation SearchSub(Game game, GameEvaluation alpha, GameEvaluation beta, int beginingMoveCount, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            if (game.State.IsEnd) // [ゲームに決着がついた]
            {
                return evaluator.Evaluate(game, beginingMoveCount, Depth);
            }
            if ((game.Record.CurrentMovesCount - beginingMoveCount) >= Depth) // [深さが最大に達した]
            {
                return evaluator.Evaluate(game, beginingMoveCount, Depth);
                // [タダでとれる駒を評価するため、2手先まで取り合いを評価する]
                //return SearchSubForEachTaking(game, null, null, beginingMoveCount, 2, cancellation);
            }

            var moveCommands = game.CreateAvailableMoveCommand();

            if(moveCommands.Count == 0){
                // 着手可能手0の場合は、IsEndがtureのはずなので、ここには来ないはず
                throw new System.Exception("★着手可能手0");
                //System.Diagnostics.Debug.WriteLine("★着手可能手0") ;
                //return -InfiniteEvaluationValue;
            }

            // [αβ法は良い手の順に探索を行うと最も効率が良い]
            moveCommands = SortByBetterMove(moveCommands, game);


            // [全ての子ノードを展開し，再帰的に評価]
            for(int i=0; i<moveCommands.Count; i++)
            {
                var gameTmp = game.Clone().PlayWithoutCheck(moveCommands[i]);

                // [現在，既に自分は最低でもα値の手が存在するため，相手が相手にとって-α値より良い手を見つけても]
                // [無意味となるため(自分はその手を指さないだけ)，-α値が相手の探索の上限となる．]
                //var eval = -Search(gameTmp, player.Opponent, -beta, -alpha, depth - 1, null, cancellation);
                var eval = SearchSub(gameTmp, beta?.Reverse(), alpha?.Reverse(), beginingMoveCount, cancellation).Reverse();

                // [最善手(MAX)を求める]
                if((alpha == null) || (alpha.Value < eval.Value))
                    alpha = eval;		// [α値の更新]

                // [MEMO:枝刈りした場合、最善手と同じ評価値を返しているのでα値と同じ値が返っても、]
                // [最善手ではないので注意(相手側がより最善の手があるので)]
                if((alpha != null && beta != null) && (alpha.Value >= beta.Value)){
                    return alpha;
                }
            }
            return alpha;
        }

        /// <summary>
    }
}
