using System.Collections.Generic;

namespace MiniShogiMobile.Conditions
{
    public class SelectKomaConditions
    {
        public string SelectedKoma;

        public string Title;

        public SelectKomaConditions(string selectedKoma, string title = null)
        {
            SelectedKoma = selectedKoma;
            Title = title;
        }
    }
}
