using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;

namespace Shogi.Business.Domain.Model.AI
{
    public class MoveEvaluation
    {
        public MoveEvaluation(MoveCommand moveCommand, GameEvaluation gameEvaluation)
        {
            MoveCommand = moveCommand;
            GameEvaluation = gameEvaluation;
        }
        public MoveCommand MoveCommand { get; private set; }
        public GameEvaluation GameEvaluation { get; private set; }

        public override string ToString()
        {
            return $"{MoveCommand.ToString()}, eval={GameEvaluation.Value}\n{GameEvaluation.Game.ToString()}";
        }
    }

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
            if(debug)
            {
                System.Diagnostics.Debug.WriteLine("▽▽▽▽▽▽▽▽▽▽[探索開始]▽▽▽▽▽▽▽▽▽▽");
                System.Diagnostics.Debug.WriteLine("深さ：" + Depth.ToString()) ;
                System.Diagnostics.Debug.WriteLine("---------盤面------------") ;
                System.Diagnostics.Debug.WriteLine(game.ToString()) ;
                System.Diagnostics.Debug.WriteLine("手番：" + game.State.TurnPlayer.ToString()) ;
                System.Diagnostics.Debug.WriteLine("---------[探索結果]------------") ;
            }

            // [MEMO:アルファベータ法は枝刈りを行うので複数の最善手を得られない?]
            var bestMove = Search(game, game.State.TurnPlayer, Depth, cancellation);

            if (bestMove == null)
            {
                System.Diagnostics.Debug.WriteLine("キャンセル") ;
                return null;
            }

            if (debug)
            {

                System.Diagnostics.Debug.WriteLine("---------[ベスト手]------------") ;
                var temp = bestMove.GameEvaluation.Game.Clone();
                System.Diagnostics.Debug.WriteLine($"{temp.ToString()}");
                while(temp.CanUndo(Game.UndoType.Undo))
                {
                    temp.Undo(Game.UndoType.Undo);
                    System.Diagnostics.Debug.WriteLine($"{temp.ToString()}");
                }
                System.Diagnostics.Debug.WriteLine("△△△△△△△△△△[探索終了]△△△△△△△△△△");
            }
            return bestMove.MoveCommand;
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
        private MoveEvaluation Search(Game game, PlayerType player, int depth, CancellationToken cancellation)
        {
            MoveEvaluation bestMove = null;

            var alpha = GameEvaluation.DefaultNegativeEvaluation;
            var beta = GameEvaluation.DefaultPositiveEvaluation; 

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
                var eval = SearchSub(gameTmp, player.Opponent, beta.Reverse(), alpha.Reverse(), depth - 1,cancellation).Reverse();
                if (eval == null)
                    return null;

                // [最善手(MAX)を求める]
                if(alpha.Value < eval.Value)
                {
                    alpha = eval;   // [α値の更新]
                    bestMove = new MoveEvaluation(moveCommands[i], eval);
                }

                if(debug)
                {
                    System.Diagnostics.Debug.WriteLine(string.Join("\n", new MoveEvaluation(moveCommands[i], eval))) ;
                }
            }

            return bestMove;
        }

        private GameEvaluation SearchSub(Game game, PlayerType player, GameEvaluation alpha, GameEvaluation beta, int depth, CancellationToken cancellation)
        {
            cancellation.ThrowIfCancellationRequested();

            if (game.State.IsEnd || depth <= 0) // [深さが最大に達したかゲームに決着がついた]
            {
                return new GameEvaluation(game, player);
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

            // [★勝ち確のときに遊ぶのを何とかしたい]

            // [全ての子ノードを展開し，再帰的に評価]
            for(int i=0; i<moveCommands.Count; i++)
            {
                var gameTmp = game.Clone().PlayWithoutCheck(moveCommands[i]);

                // [現在，既に自分は最低でもα値の手が存在するため，相手が相手にとって-α値より良い手を見つけても]
                // [無意味となるため(自分はその手を指さないだけ)，-α値が相手の探索の上限となる．]
                //var eval = -Search(gameTmp, player.Opponent, -beta, -alpha, depth - 1, null, cancellation);
                var eval = SearchSub(gameTmp, player.Opponent, beta.Reverse(), alpha.Reverse(), depth - 1, cancellation).Reverse();

                // [MEMO:キャンセルした場合]
                if (eval == null)
                    return null;

                // [最善手(MAX)を求める]
                if(alpha.Value < eval.Value)
                    alpha = eval;		// [α値の更新]

                // [MEMO:枝刈りした場合、最善手と同じ評価値を返しているのでα値と同じ値が返っても、]
                // [最善手ではないので注意(相手側がより最善の手があるので)]
                if(alpha.Value >= beta.Value){
                    return alpha;
                }
            }
            return alpha;
        }
    }
}
