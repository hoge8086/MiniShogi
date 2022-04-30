using MiniShogiMobile.ViewModels;

namespace MiniShogiMobile.Conditions
{
    public class EditCellCondition
    {
        public CellViewModel Cell{get;}
        public int Height { get; }
        public EditCellCondition(CellViewModel cell, int height)
        {
            Cell = cell;
            Height = height;
        }
    }
}
