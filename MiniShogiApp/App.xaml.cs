using MiniShogiApp.Presentation.View;
using Shogi.Business.Application;
using Shogi.Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MiniShogiApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static public GameService GameService;
        static public CreateGameService CreateGameService;

        void App_Startup(object sender, StartupEventArgs e)
        {
            var gameTemplateJsonRepository = new GameTemplateJsonRepository("games.json");
            var komaJsonRepository = new KomaTypeJsonRepository("komas.json");
            GameService = new GameService(gameTemplateJsonRepository);
            CreateGameService = new CreateGameService(gameTemplateJsonRepository, komaJsonRepository);
        }
    }
}
