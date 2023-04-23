using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.NavigationEx;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniShogiMobile.ViewModels
{
	public class LicensePageViewModel : NavigationViewModel
	{
        public ObservableCollection<SoftwareLicense> Licenses { get; set; }
        public LicensePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            Title = "ライセンス表記";

            var licenses = Load(); 
            Licenses = new ObservableCollection<SoftwareLicense>(licenses);
        }
        private List<SoftwareLicense> Load()
        {
            var assembly = typeof(LicensePageViewModel).GetTypeInfo().Assembly;
            using (Stream stream = assembly.GetManifestResourceStream("MiniShogiMobile.Resources.Licenses.xml"))
            using (StreamReader reader = new StreamReader(stream))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<SoftwareLicense>));
                return  serializer.Deserialize(reader) as List<SoftwareLicense>;
            }
        }
	}
    public class SoftwareLicense
    {
        public string SoftwareName { get; set; }
        public string LicenseContext { get; set; }
    }
}
