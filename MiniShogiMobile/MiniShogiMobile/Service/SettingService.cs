using MiniShogiMobile.Service;
using MiniShogiMobile.Settings;
using Shogi.Business.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

[assembly:Xamarin.Forms.Dependency(typeof(SettingService))]
namespace MiniShogiMobile.Service
{
    public class SettingService : ISettingService
    {
        public PrivateSetting PrivateSetting { get; private set; }
        public SettingService()
        {
            PrivateSetting = new JsonRepository().Load<PrivateSetting>("MiniShogiMobile.Resources.private_setting.json", true);
        }
    }
}
