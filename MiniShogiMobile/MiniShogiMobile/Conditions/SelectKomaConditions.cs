using Shogi.Business.Domain.Model.Komas;

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
