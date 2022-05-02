using Shogi.Business.Domain.Model.Komas;
using System.Collections.Generic;

namespace MiniShogiMobile.Conditions
{
    public class SelectKomaConditions
    {
        public KomaTypeId SelectedKoma;

        public string Title;

        public SelectKomaConditions(KomaTypeId selectedKoma, string title = null)
        {
            SelectedKoma = selectedKoma;
            Title = title;
        }
    }
}
