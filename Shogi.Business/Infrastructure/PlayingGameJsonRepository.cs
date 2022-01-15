using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.PlayingGames;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shogi.Business.Infrastructure
{
    public class PlayingGameJsonRepository : IPlayingGameRepository
    {
        private string jsonPath;
        private List<PlayingGame> cache;
        public PlayingGameJsonRepository(string path, bool isCacheMode = true)
        {
            jsonPath = path;
            cache = new List<PlayingGame>();

            if (isCacheMode)
                return;

            var repo = new JsonRepository();
            try
            {
                cache = repo.Load<List<PlayingGame>>(jsonPath);
            }catch(Exception ex)
            {
            }
        }

        public void Add(PlayingGame playingGame)
        {
            cache.Add(playingGame);
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }

        public List<string> FindAllName()
        {
            return cache.Select(x => x.Name).ToList();
        }

        public PlayingGame FindByName(string name)
        {
            return cache.FirstOrDefault(x => x.Name == name);
        }

        public void RemoveByName(string name)
        {
            cache.RemoveAll(x => x.Name == name);
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }

        public List<PlayingGame> FindAll()
        {
            return  new List<PlayingGame>(cache);
        }
    }

    public class CurrentPlayingGameJsonRepository : ICurrentPlayingGameRepository
    {
        private string jsonPath;
        private PlayingGame cache;
        public CurrentPlayingGameJsonRepository(string path, bool isCacheMode = true)
        {
            jsonPath = path;
            cache = null;

            if (isCacheMode)
                return;

            var repo = new JsonRepository();
            try
            {
                cache = repo.Load<PlayingGame>(jsonPath);
            }catch(Exception ex)
            {
            }
        }

        public void Save(PlayingGame playingGame)
        {
            cache = playingGame;
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }


        public PlayingGame Current()
        {
            return cache.Clone();
        }
    }
}
