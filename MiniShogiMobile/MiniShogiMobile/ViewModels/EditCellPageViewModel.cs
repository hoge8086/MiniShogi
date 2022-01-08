using MiniShogiMobile.Conditions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Shogi.Business.Domain.Model.PlayerTypes;

namespace MiniShogiMobile.ViewModels
{
    public class EditCellPageViewModel : ViewModelBase
    {
        public CellViewModel Cell { get; private set; }
        public EditCellPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

        }
        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var param = parameters[nameof(EditCellCondition)] as EditCellCondition;
            if(param == null)
                throw new ArgumentException(nameof(EditCellCondition));

            Cell = param.Cell;
            Cell.Koma.Value = new KomaViewModel() { Name = "あ", PlayerType = PlayerType.Player1 };
        }
    }
}
