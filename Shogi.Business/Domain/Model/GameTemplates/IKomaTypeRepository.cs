using Shogi.Business.Domain.Model.Komas;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public interface IKomaTypeRepository
    {
        KomaType FindById(KomaTypeId id);
        List<KomaType> FindAll();
        void Add(KomaType komaType);
        void RemoveById(KomaTypeId id);
    }
}

