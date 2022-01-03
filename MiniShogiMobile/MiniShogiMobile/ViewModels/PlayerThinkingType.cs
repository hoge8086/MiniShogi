using System.ComponentModel;

namespace MiniShogiMobile.ViewModels
{
    public enum PlayerThinkingType
    {
        [Description("あなた")]
        Human,
        [Description("AI")]
        AI,
    };
}
