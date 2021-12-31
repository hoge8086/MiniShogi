using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.Komas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Shogi.Business.Infrastructure
{
    public class KomaTypeJsonRepository : IKomaTypeRepository
    {
        private string jsonPath;
        private List<KomaType> cache;

        public KomaTypeJsonRepository(string path, bool isCacheMode = true)
        {
            jsonPath = path;
            cache = new List<KomaType>();
            if (isCacheMode)
                return;

            var repo = new JsonRepository();
            try
            {
                cache = repo.Load<List<KomaType>>(jsonPath);
            }catch(Exception ex){ }
        }

        public KomaType FindById(string id)
        {
            return cache.FirstOrDefault(x => x.Id == id);
        }

        public List<KomaType> FindAll()
        {
            return new List<KomaType>(cache);
        }

        public void Add(KomaType komaType)
        {
            cache.Add(komaType);
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }

        public void RemoveById(string id)
        {
            cache.RemoveAll(x => x.Id == id);
            var repo = new JsonRepository();
            repo.Save(jsonPath, cache);
        }
    }
}
