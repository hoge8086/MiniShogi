﻿using MiniShogiMobile.Service;
using MiniShogiMobile.UWP.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(BaseUrl))]
namespace MiniShogiMobile.UWP.Service
{
    public class BaseUrl : IBaseUrl
    {
        public string Get()
        {
            return "ms-appx-web:///Assets/";
        }
    }
}
