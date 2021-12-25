using System;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.Komas;

namespace Shogi.Business.Application
{
    public class CreateGameService
    {
        //private Object thisLock = new Object();
        public IGameTemplateRepository GameTemplateRepository;
        public IKomaTypeRepository KomaTypeRepository;

        public CreateGameService(IGameTemplateRepository gameTemplateRepository, IKomaTypeRepository komaTypeRepository)
        {
            GameTemplateRepository = gameTemplateRepository;
            KomaTypeRepository = komaTypeRepository;

            if (KomaTypeRepository.FindAll().Count == 0)
                foreach (var type in DefaultGame.DefaltKomaType)
                    KomaTypeRepository.Add(type);

            if (GameTemplateRepository.FindAllName().Count == 0)
                foreach (var temp in DefaultGame.DefaltGameTemplate)
                    GameTemplateRepository.Add(new GameTemplateFactory(KomaTypeRepository).Create(temp));
        }

        public void CreateGame(CreateGameCommand createGameCommand)
        {
            if (GameTemplateRepository.FindByName(createGameCommand.Name) != null)
                throw new Exception("既に存在する名前は作成できません");

            var template = new GameTemplateFactory(KomaTypeRepository).Create(createGameCommand);
            GameTemplateRepository.Add(template);
        }

        public void CreateKomaType(KomaType komaType)
        {
            if (KomaTypeRepository.FindById(komaType.Id) != null)
                throw new Exception("既に存在する駒は作成できません");

            KomaTypeRepository.Add(komaType);
        }
    }
}
