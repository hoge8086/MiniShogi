using System;
using System.Linq;
using Shogi.Business.Domain.Model.GameTemplates;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;

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
                    //GameTemplateRepository.Add(new GameFactory(KomaTypeRepository).Create(temp));
                    GameTemplateRepository.Save(ResolveKomaTypes(temp));
        }

        /// <summary>
        /// [FIX:アプリケーションサービスではやりたくないので何とかする.TemplateとCreateTemplateCommandと分ける]
        /// </summary>
        /// <param name="template"></param>
        private GameTemplate ResolveKomaTypes(GameTemplate template)
        {
            template.KomaTypes = template.KomaList.Select(x => {
                var type =KomaTypeRepository.FindById(x.TypeId);
                if (type == null)
                    throw new System.Exception($"駒[{type.Id}]が存在しません.");
                return type;
            }).ToList();
            return template;
        }

        public void SaveGameTemplate(GameTemplate gameTemplate)
        {
            if (GameTemplateRepository.FindAll().Any(x => x.Id != gameTemplate.Id && x.Name == gameTemplate.Name))
                throw new Exception("既に存在する名前は作成できません");

            ResolveKomaTypes(gameTemplate);

            // 既に勝敗がついていないかチェック(ついている場合は例外)
            // FIX：正常系で例外は使ってはいけない
            new GameFactory().Create(gameTemplate, PlayerType.Player1);

            if(GameTemplateRepository.FindByName(gameTemplate.Name) != null)
                GameTemplateRepository.RemoveByName(gameTemplate.Name);

            GameTemplateRepository.Save(gameTemplate);
        }

        public void SaveKomaType(KomaType komaType)
        {
            if (KomaTypeRepository.FindById(komaType.Id) != null)
                throw new Exception("既に存在する駒は作成できません");

            KomaTypeRepository.Add(komaType);
        }

        public void CopyKomaType(KomaTypeId id)
        {
            var komaType = KomaTypeRepository.FindById(id);
            var copied = new KomaType(komaType.Id.NewId(), komaType.Moves, komaType.TransformedMoves);
            KomaTypeRepository.Add(copied);
        }

        public void CopyGame(string newTemplateName, string srcTemplateName)
        {
            var srcTemplateGame = GameTemplateRepository.FindByName(srcTemplateName);
            GameTemplateRepository.Save(srcTemplateGame.Copy(newTemplateName));
        }
    }
}
