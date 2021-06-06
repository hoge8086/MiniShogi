using System.Collections.Generic;
using System.Linq;

namespace Shogi.Business.Domain.Model.Games
{
    public class GameRecord
    {
        private List<GameState> SateRecords;

        public int CurrentMovesCount { get; private set; }
        public GameRecord Clone()
        {
            return new GameRecord(new List<GameState>(SateRecords), CurrentMovesCount);
        }

        public GameRecord(List<GameState> sateRecords, int currentMovesCount)
        {
            SateRecords = sateRecords;
            CurrentMovesCount = currentMovesCount;
        }
        public GameRecord(GameState state)
        {
            SateRecords = new List<GameState>();
            SateRecords.Add(state.Clone());
            CurrentMovesCount = 0;
        }
        /// <summary>
        /// 履歴を記録する(Undoしていた場合はそれ以降の履歴は消える)
        /// </summary>
        /// <param name="state"></param>
        public void Record(GameState state)
        {
            SateRecords = SateRecords.Take(CurrentMovesCount + 1).ToList();
            SateRecords.Add(state.Clone());
            CurrentMovesCount++;
        }
        /// <summary>
        /// やり直し
        /// </summary>
        /// <param name="n">Redoはマイナスの値で表現する</param>
        /// <returns></returns>
        public GameState Undo(int n)
        {
            CurrentMovesCount += n;
            return SateRecords[CurrentMovesCount];
        }

        public bool CanUndo(int n)
        {
            int dest = CurrentMovesCount + n;
            return (0 <= dest) && (dest < SateRecords.Count);
        }

        /// <summary>
        /// ゲーム状態の出現回数を数える(千日手計算用)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public int CountOccurrences(GameState state)
        {
            return 0;
        }
    }
}
