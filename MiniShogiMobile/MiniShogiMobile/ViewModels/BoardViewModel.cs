using System.Collections.ObjectModel;

namespace MiniShogiMobile.ViewModels
{
    public class BoardViewModel<T> where T : CellBaseViewModel
    {
        public ObservableCollection<ObservableCollection<T>> Cells { get; set; }

        public BoardViewModel()
        {
            Cells = new ObservableCollection<ObservableCollection<T>>();
        }
    }

}
