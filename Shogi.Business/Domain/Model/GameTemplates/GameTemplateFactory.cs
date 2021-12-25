using System.Linq;

namespace Shogi.Business.Domain.Model.GameTemplates
{
    public class GameTemplateFactory
    {
        private IKomaTypeRepository KomaTypeRepository;

        public GameTemplateFactory(IKomaTypeRepository komaTypeRepository)
        {
            KomaTypeRepository = komaTypeRepository;
        }
        public GameTemplate Create(CreateGameCommand createGameCommand)
        {
            var komaTypes = createGameCommand.KomaList.Select(x => {
                var type =KomaTypeRepository.FindById(x.TypeId);
                if (type == null)
                    throw new System.Exception($"駒[{type.Id}]が存在しません.");
                return type;
            }).ToList();
            return createGameCommand.Create(komaTypes);
        }
    }

}

