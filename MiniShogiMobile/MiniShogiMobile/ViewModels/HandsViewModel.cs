using Prism.Mvvm;
using Shogi.Business.Domain.Model.Komas;
using Shogi.Business.Domain.Model.PlayerTypes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public class HandsViewModel<T> : BindableBase where T : HandKomaViewModel, new()
    {
        public ObservableCollection<T> Hands { get; set; }

        public HandsViewModel()
        {
            Hands = new ObservableCollection<T>();
        }

        public void RemoveOne(string komaTypeId)
        {
            var removed = Hands.FirstOrDefault(x => x.Name == komaTypeId);
            if(removed != null)
            {
                removed.Num.Value -= 1;
                if (removed.Num.Value <= 0)
                    Hands.Remove(removed);
            }
        }

        public void AddOne(string komaTypeId, PlayerType player)
        {
            var added = Hands.FirstOrDefault(x => x.Name == komaTypeId);
            if(added != null)
                added.Num.Value += 1;
            else
                Hands.Add(new T() { Name = komaTypeId, Player = player});
        }

        public void Update(List<Koma> komaListOfPlayerHands)// ObservableCollection<HandKomaPlayingViewModel> hands, PlayerType player)
        {
            // [MEMO:Clear()するとちらつきが発生＆順序が変わるため、順番を変えないように持ち手を更新する]

            // [初期化＆駒の数を0にする]
            Hands.ForEach(x => x.Clear());

            // [駒の数をカウントし直す]
            //App.GameService.GetGame().State.KomaList.Where(x => !x.IsOnBoard && x.Player == player).ForEach(koma =>
            komaListOfPlayerHands.ForEach(koma =>
            {
                var hand = Hands.FirstOrDefault(x => x.Name == koma.TypeId);
                if (hand == null)
                    Hands.Add(new T() { Name = koma.TypeId, Player = koma.Player});
                else
                    hand.Num.Value++;
            });

            // [存在しない駒は削除する]
            var nothings = Hands.Where(x => x.Num.Value == 0).ToList();
            foreach (var item in nothings)
                Hands.Remove(item);
        }

    }

}
