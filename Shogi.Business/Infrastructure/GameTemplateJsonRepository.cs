using Shogi.Business.Domain.Model.GameTemplates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Business.Infrastructure
{
    public class GameTemplateJsonRepository : IGameTemplateRepository
    {

        private string jsonPath;
        private List<GameTemplate> cache;
        public GameTemplateJsonRepository(string path, bool isCacheMode = true)
        {
            jsonPath = path;
            cache = new List<GameTemplate>();

            if (isCacheMode)
                return;

            var repo = new JsonRepository();
            try
            {
                cache = repo.Load<List<GameTemplate>>(jsonPath);
            }catch(Exception ex)
            {
            }
        }

        public GameTemplate First()
        {
            return cache.First();
        }
        public void Add(GameTemplate gameTemplate)
        {
            cache.Add(gameTemplate);
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }

        public List<string> FindAllName()
        {
            return cache.Select(x => x.Name).ToList();
        }

        public GameTemplate FindByName(string name)
        {
            return cache.FirstOrDefault(x => x.Name == name);
        }
        public GameTemplate FindById(string id)
        {
            return cache.FirstOrDefault(x => x.Id == id);
        }

        public void RemoveById(string id)
        {
            cache.RemoveAll(x => x.Id == id);
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }

        public List<GameTemplate> FindAll()
        {
            return  new List<GameTemplate>(cache);
        }
    }
}
