using System.Collections.Generic;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public interface IGameTemplateRepository
    {
        GameTemplate First();
        GameTemplate FindByName(string name);
        GameTemplate FindById(string id);
        List<string> FindAllName();
        void Add(GameTemplate gameTemplate);
        void RemoveById(string id);
        List<GameTemplate> FindAll();
    }
}

