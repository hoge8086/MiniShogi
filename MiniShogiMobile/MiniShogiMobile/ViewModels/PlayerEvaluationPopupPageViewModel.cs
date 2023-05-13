using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Shogi.Business.Domain.Model.AI;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.PlayerTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace MiniShogiMobile.ViewModels
{
    public class PlayerEvaluationPopupPageViewModel : NavigationViewModel<GameEvaluation>
    {
        public Game game { get; set; }
        public AsyncReactiveCommand OkCommand { get; }
        public AsyncReactiveCommand UndoCommand { get; private set; }
        public AsyncReactiveCommand RedoCommand { get; private set; }
        private ReactiveProperty<int> CurrentMoveCount;
        public ReactiveProperty<PlayerType> CurrentTurn { get; private set; }

        private int beginingMoveCount;
        public ReactiveProperty<string> Evaluation { get; private set; }
        public GameViewModel<CellViewModel<KomaViewModel>, HandsViewModel<HandKomaViewModel>, HandKomaViewModel> Game { get; set; }
        public PlayerEvaluationPopupPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            CurrentMoveCount = new ReactiveProperty<int>();
            CurrentTurn = new ReactiveProperty<PlayerType>();
            Evaluation = new ReactiveProperty<string>();
            Game = new GameViewModel<CellViewModel<KomaViewModel>, HandsViewModel<HandKomaViewModel>, HandKomaViewModel>();
            OkCommand = new AsyncReactiveCommand();
            OkCommand.Subscribe(async () =>
            {
                await GoBackAsync();
            }).AddTo(this.Disposable);
            UndoCommand = CurrentMoveCount.Select(cnt => (game != null) && game.CanUndo(Shogi.Business.Domain.Model.Games.Game.UndoType.Undo) && cnt > beginingMoveCount).ToAsyncReactiveCommand().AddTo(this.Disposable);
            UndoCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    game.Undo(Shogi.Business.Domain.Model.Games.Game.UndoType.Undo);
                    UpdateView(game);
                });
            }).AddTo(Disposable);
            RedoCommand = CurrentMoveCount.Select(cnt => (game != null) && game.CanUndo(Shogi.Business.Domain.Model.Games.Game.UndoType.Redo)).ToAsyncReactiveCommand().AddTo(this.Disposable);
            RedoCommand.Subscribe(async x =>
            {
                await this.CatchErrorWithMessageAsync(async () =>
                {
                    game.Undo(Shogi.Business.Domain.Model.Games.Game.UndoType.Redo);
                    UpdateView(game);
                });
            }).AddTo(Disposable);


        }
        public override void Prepare(GameEvaluation evaluation)
        {
            game = evaluation.Game.Clone();
            beginingMoveCount = evaluation.BeginingMoveCount;
            UpdateView(game);
            Evaluation.Value = $"{(int)(evaluation.Value / (double)evaluation.MaxValue * 100)} ({evaluation.Value}/{evaluation.MaxValue})";
        }
        public void UpdateView(Game game)
        {
            Game.Update(game.Board.Height, game.Board.Width, game.State.KomaList);
            CurrentTurn.Value = game.State.TurnPlayer;
            CurrentMoveCount.Value = game.Record.CurrentMovesCount;
        }
    }
}
