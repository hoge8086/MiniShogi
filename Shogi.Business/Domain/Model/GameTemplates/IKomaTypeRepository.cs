using Shogi.Business.Domain.Model.Komas;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public interface IKomaTypeRepository
    {
        KomaType FindById(string id);
        List<KomaType> FindAll();
        void Add(KomaType komaType);
        void RemoveById(string id);
    }
}

