using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.PlayingGames
{
    public interface ICurrentPlayingGameRepository
    {
        PlayingGame Current();
        void Save(PlayingGame playingGame);
    }
    public interface IPlayingGameRepository
    {
        PlayingGame FindByName(string name);
        List<PlayingGame> FindAll();
        void Add(PlayingGame playingGame);
        void RemoveByName(string name);
    }
}
