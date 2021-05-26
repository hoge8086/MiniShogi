using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Moves;
using Shogi.Business.Domain.Model.Players;

namespace Shogi.Business.Domain.Model.AI
{
    public class NegaAlphaAI : AI
    {
        private int depth;

        private static int InfiniteEvaluationValue = 99999999;

        private MoveCommand bestMove = null;
        public NegaAlphaAI(int depth)
        {
            this.depth = depth;
        }
        public override MoveCommand SelectMove(Game game)
        {
            bestMove = null;
            Search(game, game.State.TurnPlayer, -InfiniteEvaluationValue-1, InfiniteEvaluationValue+1, depth);

            if (bestMove == null)
            {
                System.Diagnostics.Debug.WriteLine("★★★★★★★おかしい★★★★★★★");
            }
            return bestMove;
        }

        private int Evaluation(Game game, Player player)
        {
            // [駒得基準の判定]
            int evaluationValue = 0;
            foreach(var koma in game.State.KomaList)
            {
                int movablePositionCount = 0;
                var moves = koma.IsTransformed ? koma.KomaType.TransformedMoves : koma.KomaType.Moves;
                foreach(var move in moves.Moves)
                {
                    if (move is PinpointKomaMove)
                        movablePositionCount += 1;

                    // [香と桂が同じ価値ということは直線は2マス分換算ということ]
                    if (move is StraightKomaMove)
                        movablePositionCount += 2;
                }
                evaluationValue += (player == koma.Player) ? movablePositionCount : -movablePositionCount;
            }
            return evaluationValue;
        }

        private int Search(Game game, Player player, int alpha, int beta, int depth)
        {
            if (game.IsWinning(player.Opponent))
                return -InfiniteEvaluationValue;
            if (game.IsWinning(player))
                return InfiniteEvaluationValue;

            // NegaAlpha法
            if(depth <= 0){		// 深さが最大に達した
                return Evaluation(game, player);
            }

            var moveCommands = game.CreateAvailableMoveCommand();
            if(depth == this.depth)
            {
                System.Diagnostics.Debug.WriteLine("---------盤面------------") ;
                System.Diagnostics.Debug.WriteLine(game.ToString()) ;
                System.Diagnostics.Debug.WriteLine("---------着手可能手------------") ;
                System.Diagnostics.Debug.WriteLine(string.Join('\n', moveCommands)) ;
            }

            if(moveCommands.Count == 0){
                System.Diagnostics.Debug.WriteLine("★着手可能手0") ;
                return -InfiniteEvaluationValue;
            }

            // αβ法は良い手順に探索を行うと最も効率が良い
            //SortingTe(state, te_buff, te_num);

            // 全ての子ノードを展開し，再帰的に評価
            for(int i=0; i<moveCommands.Count; i++)
            {
                var gameTmp = game.Clone().PlayWithoutCheck(moveCommands[i]);

                // 現在，既に自分は最低でもα値の手が存在するため，相手が相手にとって-α値より良い手を見つけても
                // 無意味となるため(自分はその手を指さないだけ)，-α値が相手の探索の上限となる．

                //int val = -NegaAlpha(state, GameState::getEnemyPlayer(player), -beta, -alpha, depth - 1, NULL, debug - 1);
                //state->undo(&te_buff[i]);
                int val = -Search(gameTmp, player.Opponent, -beta, -alpha, depth - 1);

                // 最善手(MAX)を求める
                if(alpha < val){		// 以下だと悪くなる手を選ぶ可能性がある
                    alpha = val;		// α値の更新
                    if(depth == this.depth)
                        bestMove = moveCommands[i];
                }

                if(depth == this.depth)
                    System.Diagnostics.Debug.WriteLine("評価値：" + moveCommands[i].ToString() + " -> " + val.ToString()) ;
                //*********** Debug ******************************
                //if(debug > 0){
                //    printf("%d. (%d,%d)->(%d,%d)  evaluation=%d, (α=%d, β=%d)\n", depth,
                //            te_buff[i].from.x, te_buff[i].from.y, te_buff[i].to.x, te_buff[i].to.y, val, alpha, beta);
                //    getch();
                //}
                //*************************************************

                if(alpha >= beta){	// より良い手がないと枝切りを行う
                    return alpha;
                }
            }
            return alpha;
        }

    }
}
