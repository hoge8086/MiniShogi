using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    [DataContract]
    public class NegaAlphaAI : AI
    {
        public override string ToString() => "AI (" + Depth.ToString() + "手読み)";
        [DataMember]
        private int Depth { get; set;}
        private bool debug = true;

        public NegaAlphaAI(int depth)
        {
            this.Depth = depth;
        }
        public override MoveCommand SelectMove(Game game, CancellationToken cancellation)
        {
            // [MEMO:アルファベータ法は枝刈りを行うので複数の最善手を得られない?]
            var bestMoveCommands = new List<MoveCommand>();
            //Search(game, game.State.TurnPlayer, -InfiniteEvaluationValue-1, InfiniteEvaluationValue+1, Depth, bestMoveCommands, cancellation);
            var eval = Search(game, game.State.TurnPlayer, GameEvaluation.DefaultNegativeEvaluation, GameEvaluation.DefaultPositiveEvaluation, Depth, bestMoveCommands, cancellation);

            if (eval == null)
            {
                System.Diagnostics.Debug.WriteLine("キャンセル") ;
                return null;
            }

            if(debug)
            {
                System.Diagnostics.Debug.WriteLine("---------ベスト手------------") ;
                System.Diagnostics.Debug.WriteLine(string.Join("\n", bestMoveCommands)) ;
            }
            return bestMoveCommands[new System.Random().Next(0, bestMoveCommands.Count)];
        }


        public List<MoveCommand> SortByBetterMove(List<MoveCommand> moveCommands, Game game)
        {
            // [MEMO:手の偏りを防ぐためランダムソートする]
            moveCommands = moveCommands.OrderBy(i => System.Guid.NewGuid()).ToList();

            var sorted = new List<MoveCommand>();
            foreach(var move in moveCommands)
            {
                // [価値の低い駒で価値の高い駒を取る場合は良い手]
                //var fromKoma = move.FindFromKoma(game.State);
                var fromKoma = game.FindFromKoma(move);
                var toKoma = game.State.FindBoardKoma(move.ToPosition);
                if (toKoma != null && GameEvaluation.Evaluation(game.GetKomaType(fromKoma).Moves) < GameEvaluation.Evaluation(game.GetKomaType(toKoma).Moves))
                    sorted.Insert(0, move);
                else
                    sorted.Add(move);
            }
            return sorted;
        }

        private GameEvaluation Search(Game game, PlayerType player, GameEvaluation alpha, GameEvaluation beta, int depth, List<MoveCommand> bestMoveCommands, CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
                return null;

            if (game.State.IsEnd || depth <= 0) // [深さが最大に達したかゲームに決着がついた]
            {
                return new GameEvaluation(game, player);
            }

            var moveCommands = game.CreateAvailableMoveCommand();
            if(debug)
            {
                if(depth == this.Depth)
                {
                  System.Diagnostics.Debug.WriteLine("------------------------------------") ;
                  System.Diagnostics.Debug.WriteLine("深さ：" + depth.ToString()) ;
                  System.Diagnostics.Debug.WriteLine("---------盤面------------") ;
                  System.Diagnostics.Debug.WriteLine(game.ToString()) ;
                  System.Diagnostics.Debug.WriteLine("手番：" + game.State.TurnPlayer.ToString()) ;
                  System.Diagnostics.Debug.WriteLine("---------着手可能手------------") ;
                  System.Diagnostics.Debug.WriteLine(string.Join("\n", moveCommands)) ;
                }
            }

            if(moveCommands.Count == 0){
                // 着手可能手0の場合は、IsEndがtureのはずなので、ここには来ないはず
                throw new System.Exception("★着手可能手0");
                //System.Diagnostics.Debug.WriteLine("★着手可能手0") ;
                //return -InfiniteEvaluationValue;
            }

            // [αβ法は良い手の順に探索を行うと最も効率が良い]
            moveCommands = SortByBetterMove(moveCommands, game);

            // [★勝ち確のときに遊ぶのを何とかしたい]

            // [全ての子ノードを展開し，再帰的に評価]
            for(int i=0; i<moveCommands.Count; i++)
            {
                var gameTmp = game.Clone().PlayWithoutCheck(moveCommands[i]);

                // [現在，既に自分は最低でもα値の手が存在するため，相手が相手にとって-α値より良い手を見つけても]
                // [無意味となるため(自分はその手を指さないだけ)，-α値が相手の探索の上限となる．]

                // [MEMO:枝刈りした場合、最善手と同じ評価値を返しているのでα値と同じ値が返っても、]
                // [最善手ではないので注意(相手側がより最善の手があるので)]
                //var eval = -Search(gameTmp, player.Opponent, -beta, -alpha, depth - 1, null, cancellation);
                var eval = Search(gameTmp, player.Opponent, beta.Reverse(), alpha.Reverse(), depth - 1, null, cancellation).Reverse();

                // [MEMO:キャンセルした場合は、候補手一覧は空にするため、ここでリターンする]
                if (eval == null)
                    return null;

                // [最善手(MAX)を求める]
                if(alpha.Value < eval.Value)
                {
                    alpha = eval;		// [α値の更新]

                    if (bestMoveCommands != null)
                    {
                        bestMoveCommands.Clear();
                        bestMoveCommands.Add(moveCommands[i]);
                    }
                }

                if(debug)
                    if(depth == this.Depth)
                        System.Diagnostics.Debug.WriteLine("評価値：" + moveCommands[i].ToString() + " -> " + eval.Value.ToString()) ;
                        //System.Diagnostics.Debug.WriteLine((new string(' ', Depth- depth)) + "評価値：" + moveCommands[i].ToString() + " -> " + val.ToString()) ;


                if(alpha.Value >= beta.Value){	// [より良い手がないと枝切りを行う]
                    return alpha;
                }
            }
            return alpha;
        }
    }
}
