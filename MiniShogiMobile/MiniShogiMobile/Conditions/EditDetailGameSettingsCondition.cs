using Shogi.Business.Domain.Model.GameTemplates;

namespace MiniShogiMobile.Conditions
{
    public class EditDetailGameSettingsCondition
    {
        public GameTemplate GameTemplate { get; set; }

        public EditDetailGameSettingsCondition(GameTemplate gameTemplate)
        {
            GameTemplate = gameTemplate;
        }

    }
}
