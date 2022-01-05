using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MiniShogiMobile.ViewModels
{
    public class HandsViewModel<T> : BindableBase where T : HandKomaViewModel
    {
        public ObservableCollection<T> Hands { get; set; }

        public HandsViewModel()
        {
            Hands = new ObservableCollection<T>();
        }
    }

}
