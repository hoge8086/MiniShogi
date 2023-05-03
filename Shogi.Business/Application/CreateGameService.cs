using System;
using System.Diagnostics;
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
                    GameTemplateRepository.Save(temp);
        }


        public void SaveGameTemplate(GameTemplate gameTemplate)
        {
            if (GameTemplateRepository.FindAll().Any(x => x.Id != gameTemplate.Id && x.Name == gameTemplate.Name))
                throw new Exception("既に存在する名前は作成できません");

            //ResolveKomaTypes(gameTemplate);
            // 不具合チェック(発生してはいけない)
            if(!gameTemplate.KomaList.All(x => gameTemplate.KomaTypes.Any(y => y.Id == x.TypeId)))
                throw new InvalidProgramException("存在しない駒種別の駒が存在します");
            if(!gameTemplate.KomaTypes.All(x => gameTemplate.KomaList.Any(y => x.Id == y.TypeId)))
                throw new InvalidProgramException("余分な駒種別が登録されています");


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
