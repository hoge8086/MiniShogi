﻿using Shogi.Business.Domain.Model.Boards;
using Shogi.Business.Domain.Model.Games;
using Shogi.Business.Domain.Model.Komas;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.Internals;

namespace MiniShogiMobile.ViewModels
{
    public class BoardViewModel<T> where T : CellViewModel, new()
    {
        public ObservableCollection<ObservableCollection<T>> Cells { get; set; }

        public BoardViewModel()
        {
            Cells = new ObservableCollection<ObservableCollection<T>>();
        }

        public void Update(int height, int width, List<Koma> komaList)
        {
            UpdateSize(height, width);

            foreach(var row in Cells)
                foreach (var cell in row)
                    cell.ToEmpty();

            foreach (var koma in komaList)
            {
                if (koma.BoardPosition != null)
                {
                    var cell = Cells[koma.BoardPosition.Y][koma.BoardPosition.X];
                    cell.Koma.Value = new KomaViewModel(koma.TypeId, koma.Player, koma.IsTransformed);
                }
            }
        }
        public void UpdateSize(int height, int width)
        {
            UpdateBoardSize(Cells, height);
            Cells.ForEach(x => UpdateBoardSize(x, width));
            for (int y = 0; y < Cells.Count; y++)
                for (int x = 0; x < Cells[y].Count; x++)
                    Cells[y][x].Position = new BoardPosition(x, y);

        }

        private static void UpdateBoardSize<Item>(ObservableCollection<Item> list, int size) where Item : new()
        {
            if(list.Count > size)
            {
                var delNum = list.Count - size;
                for(int i=0; i<delNum; i++)
                    list.RemoveAt(list.Count - 1);
            }

            if(list.Count < size)
            {
                var addNum = size - list.Count;
                for(int i=0; i<addNum; i++)
                    list.Add(new Item());
            }
        }
    }

}
