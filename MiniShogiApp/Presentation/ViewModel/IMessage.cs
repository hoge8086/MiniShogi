namespace MiniShogiApp.Presentation.ViewModel
{
    public interface IMessage
    {
        void Message(string msg);
        bool MessageYesNo(string msg);
    }
}
