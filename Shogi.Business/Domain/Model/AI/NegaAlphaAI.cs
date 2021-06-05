using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.Players;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    public class NegaAlphaAI : AI
    {
        public override string Name => "AI (" + Depth.ToString() + "手読み)";
        private int Depth;
        private bool debug = true;

        private static int InfiniteEvaluationValue = 99999999;

        public NegaAlphaAI(int depth)
        {
            this.Depth = depth;
        }
        public override MoveCommand SelectMove(Game game, CancellationToken cancellation)
        {
            // [MEMO:アルファベータ法は枝刈りを行うので複数の最善手を得られない?]
            var bestMoveCommands = new List<MoveCommand>();
            Search(game, game.State.TurnPlayer, -InfiniteEvaluationValue-1, InfiniteEvaluationValue+1, Depth, bestMoveCommands, cancellation);

            if (bestMoveCommands.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("キャンセル") ;
                return null;
            }

            if(debug)
            {
                System.Diagnostics.Debug.WriteLine("---------ベスト手------------") ;
                System.Diagnostics.Debug.WriteLine(string.Join('\n', bestMoveCommands)) ;
            }
            return bestMoveCommands[new System.Random().Next(0, bestMoveCommands.Count)];
        }

        private int Evaluation(KomaMoves moves)
        {
            int movablePositionCount = 0;
            foreach(var move in moves.Moves)
            {
                if (move is PinpointKomaMove)
                    movablePositionCount += 1;

                // [香と桂が同じ価値ということは直線は2マス分換算ということ]
                if (move is StraightKomaMove)
                    movablePositionCount += 2;
            }
            return movablePositionCount;
        }

        private int Evaluation(Game game, Player player)
        {
            // [どうぶつ将棋だと勝敗判定にチェックメイトがないので、]
            // [最後の読みで玉の捨て身で相手の駒をとることが可能になってしまい]
            // [1手浅い読みになってしまうのでここで評価する]
            // [TODO:同様に入玉も考慮したほうが良い(次の手で入玉できる/されてしまうケース)]
            if (game.DoOte(player) && game.State.TurnPlayer == player)
                return InfiniteEvaluationValue;
            if (game.DoOte(player.Opponent) && game.State.TurnPlayer == player.Opponent)
                return -InfiniteEvaluationValue;

            // [駒得基準の判定]
            int evaluationValue = 0;

            foreach(var koma in game.State.KomaList)
            {
                int movablePositionCount = Evaluation(koma.IsTransformed ? koma.KomaType.TransformedMoves : koma.KomaType.Moves);
                evaluationValue += (player == koma.Player) ? movablePositionCount : -movablePositionCount;
            }
            return evaluationValue;
        }

        public List<MoveCommand> SortByBetterMove(List<MoveCommand> moveCommands, Game game)
        {
            // [MEMO:手の偏りを防ぐためランダムソートする]
            moveCommands = moveCommands.OrderBy(i => System.Guid.NewGuid()).ToList();

            var sorted = new List<MoveCommand>();
            foreach(var move in moveCommands)
            {
                // [価値の低い駒で価値の高い駒を取る場合は良い手]
                var fromKoma = move.FindFromKoma(game.State);
                var toKoma = game.State.FindBoardKoma(move.ToPosition);
                if (toKoma != null && Evaluation(fromKoma.KomaType.Moves) < Evaluation(toKoma.KomaType.Moves))
                    sorted.Insert(0, move);
                else
                    sorted.Add(move);
            }
            return sorted;
        }

        private int Search(Game game, Player player, int alpha, int beta, int depth, List<MoveCommand> bestMoveCommands, CancellationToken cancellation)
        {
            if (cancellation.IsCancellationRequested)
                return 0;

            if (game.IsWinning(player.Opponent))
                return -InfiniteEvaluationValue;
            if (game.IsWinning(player))
                return InfiniteEvaluationValue;

            if(depth <= 0){		// [深さが最大に達した]
                return Evaluation(game, player);
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
                  System.Diagnostics.Debug.WriteLine(string.Join('\n', moveCommands)) ;
                }
            }

            if(moveCommands.Count == 0){
                System.Diagnostics.Debug.WriteLine("★着手可能手0") ;
                return -InfiniteEvaluationValue;
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
                int val = -Search(gameTmp, player.Opponent, -beta, -alpha, depth - 1, null, cancellation);

                // [MEMO:キャンセルした場合は、候補手一覧は空にするため、ここでリターンする]
                if (cancellation.IsCancellationRequested)
                    return 0;

                // [最善手(MAX)を求める]
                if(alpha < val)
                {
                    alpha = val;		// [α値の更新]

                    if (bestMoveCommands != null)
                    {
                        bestMoveCommands.Clear();
                        bestMoveCommands.Add(moveCommands[i]);
                    }
                }

                if(debug)
                    if(depth == this.Depth)
                        System.Diagnostics.Debug.WriteLine("評価値：" + moveCommands[i].ToString() + " -> " + val.ToString()) ;
                        //System.Diagnostics.Debug.WriteLine((new string(' ', Depth- depth)) + "評価値：" + moveCommands[i].ToString() + " -> " + val.ToString()) ;


                if(alpha >= beta){	// [より良い手がないと枝切りを行う]
                    return alpha;
                }
            }
            return alpha;
        }
    }
}
