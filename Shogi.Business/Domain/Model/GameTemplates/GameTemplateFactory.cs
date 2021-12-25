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
            return createGameCommand.Create(createGameCommand.KomaList.Select(x => KomaTypeRepository.FindById(x.TypeId)).ToList());
        }
    }

}

