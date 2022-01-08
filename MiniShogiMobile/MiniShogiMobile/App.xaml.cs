using MiniShogiMobile.ViewModels;
using MiniShogiMobile.Views;
using Prism;
using Prism.Ioc;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

using Shogi.Business.Application;
using Shogi.Business.Infrastructure;
using System.IO;

namespace MiniShogiMobile
{
    public partial class App
    {
        static public GameService GameService;
        static public CreateGameService CreateGameService;

        public App(IPlatformInitializer initializer)
            : base(initializer)
        {
            Device.SetFlags(new string[] { "Shapes_Experimental" });
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var dataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var gameTemplateJsonRepository = new GameTemplateJsonRepository(Path.Combine(dataDir, "games.json"));
            var komaJsonRepository = new KomaTypeJsonRepository(Path.Combine(dataDir, "games.json"));
            GameService = new GameService(gameTemplateJsonRepository);
            CreateGameService = new CreateGameService(gameTemplateJsonRepository, komaJsonRepository);

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<PlayGamePage, PlayGamePageViewModel>();
            containerRegistry.RegisterForNavigation<StartGamePage, StartGamePageViewModel>();
            containerRegistry.RegisterForNavigation<CreateGamePage, CreateGamePageViewModel>();
        }
    }
}
