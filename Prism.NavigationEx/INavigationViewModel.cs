using System;
using System.Threading.Tasks;
using Prism.Navigation;

namespace Prism.NavigationEx
{
    public interface INavigationViewModel : INavigationAware, IDestructible
    {
        void OnNavigatingFrom(INavigationParameters parameters);
    }

    public interface INavigationViewModel<TParameter> : INavigationViewModel
    {
        Task PrepareAsync(TParameter parameter);
    }

    public interface INavigationViewModelResult<TResult> : INavigationViewModel
    {
    }

    public interface INavigationViewModel<TParameter, TResult> : INavigationViewModel<TParameter>, INavigationViewModelResult<TResult>
    {
    }
}
