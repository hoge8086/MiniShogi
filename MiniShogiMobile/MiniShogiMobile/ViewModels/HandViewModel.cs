using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace MiniShogiMobile.ViewModels
{
    public class HandViewModel<T> : BindableBase where T : HandKomaViewModel
    {
        public ObservableCollection<T> Hands { get; set; }

        public HandViewModel()
        {
            Hands = new ObservableCollection<T>();
        }
    }

}
