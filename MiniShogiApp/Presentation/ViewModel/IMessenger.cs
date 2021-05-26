namespace MiniShogiApp.Presentation.ViewModel
{
    public interface IMessenger
    {
        void Message(string msg);
        bool MessageYesNo(string msg);
    }
}
