namespace MiniShogiMobile.Conditions
{
    public class CreateGameCondition
    {
        public string GameName { get; }
        public CreateGameCondition(string name)
        {
            GameName = name;
        }
    }
}
