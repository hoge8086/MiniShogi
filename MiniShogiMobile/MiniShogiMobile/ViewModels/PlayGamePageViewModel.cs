using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MiniShogiMobile.ViewModels
{
    public class PlayGamePageViewModel : ViewModelBase
    {
        public PlayGamePageViewModel(INavigationService navigationService) : base(navigationService)
        {

        }
        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(PlayGameCondition)] as PlayGameCondition;
            if(param == null)
                throw new ArgumentException(nameof(PlayGameCondition));

            Title = param.Name;
            var cancelTokenSource = new CancellationTokenSource();
            //await Task.Run(() =>
            //{
                App.GameService.Start(param.FirstPlayer, param.SecondPlayer, param.Name, cancelTokenSource.Token);
            //});
            var game = App.GameService.GetGame();
            cancelTokenSource = null;
            // [MEMO:タスクが完了されるまでここは実行されない(AIThinkingのまま)]
            //UpdateOperationModeOnTaskFinished();
        }
    }

}
