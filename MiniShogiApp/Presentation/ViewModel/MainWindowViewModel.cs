using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace MiniShogiApp.Presentation.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ShogiBoardViewModel _board;
        public ShogiBoardViewModel Board
        {
            get { return _board; }
            set
            {
                _board = value;
                RaisePropertyChanged(nameof(Board));
            }
        }

        public MainWindowViewModel()
        {
            Board = new ShogiBoardViewModel();
        }
    }
}
