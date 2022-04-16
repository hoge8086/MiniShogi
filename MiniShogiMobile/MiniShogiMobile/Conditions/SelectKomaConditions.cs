using System.Collections.Generic;

namespace MiniShogiMobile.Conditions
{
    class SelectKomaConditions
    {
        public List<string> KomaNameList;
        public string SelectedKoma;

        public SelectKomaConditions(List<string> komaNameList, string selectedKoma)
        {
            KomaNameList = komaNameList;
            SelectedKoma = selectedKoma;
        }
    }
}
