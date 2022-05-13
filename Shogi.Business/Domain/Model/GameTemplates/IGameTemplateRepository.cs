using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public interface IGameTemplateRepository
    {
        GameTemplate First();
        GameTemplate FindByName(string name);
        List<string> FindAllName();
        void Add(GameTemplate gameTemplate);
        List<GameTemplate> FindAll();
        void RemoveByName(string name);
    }
}

