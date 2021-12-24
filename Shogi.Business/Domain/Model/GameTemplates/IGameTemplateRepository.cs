using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public interface IGameTemplateRepository
    {
        GameTemplate First();
        GameTemplate FindByName(string name);
        List<string> FindAllName();
        void Add(GameTemplate gameTemplate);
        void RemoveByName(string name);

    }
}

