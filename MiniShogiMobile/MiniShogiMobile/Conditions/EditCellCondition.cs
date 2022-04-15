using MiniShogiMobile.ViewModels;

namespace MiniShogiMobile.Conditions
{
    class EditCellCondition
    {
        public CellViewModel Cell{get;}
        public int Height { get; }
        public bool ShowDeleteKomaCommand { get; }
        public EditCellCondition(CellViewModel cell, int height, bool showDeleteKomaCommand)
        {
            Cell = cell;
            Height = height;
            ShowDeleteKomaCommand = showDeleteKomaCommand;
        }
    }
}
