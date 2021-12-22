using Shogi.Business.Domain.Model.Komas;
using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public interface IKomaTypeRepository
    {
        GameTemplate FindByName(string name);
        List<KomaType> FindAll();
        void Add(KomaType komaType);
        void RemoveByName(string name);
    }
}

