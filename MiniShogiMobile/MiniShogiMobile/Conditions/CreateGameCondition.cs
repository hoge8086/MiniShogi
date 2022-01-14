namespace MiniShogiMobile.Conditions
{
    class CreateGameCondition
    {
        public string GameName { get; }
        public CreateGameCondition(string name)
        {
            GameName = name;
        }
    }
}
