using MiniShogiApp.Presentation.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MiniShogiApp.Presentation.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel(
                new MyMessageBox(),
                () =>
                {
                    var dialog = new StartGameWindow();
                    dialog.ShowDialog();
                    return dialog.ViewModel;
                }
                );
        }
    }

    public class MyMessageBox : IMessenger
    {
        public void Message(string msg)
        {
            MessageBox.Show(msg, "ミニ将棋");
        }

        public bool MessageYesNo(string msg)
        {
            return MessageBox.Show(msg, "ミニ将棋", MessageBoxButton.YesNo) == MessageBoxResult.Yes;
        }
    }
}
