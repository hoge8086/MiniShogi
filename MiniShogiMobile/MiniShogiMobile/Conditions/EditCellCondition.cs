using MiniShogiMobile.ViewModels;

namespace MiniShogiMobile.Conditions
{
    class EditCellCondition
    {
        public CellViewModel Cell{get;}
        public EditCellCondition(CellViewModel cell)
        {
            Cell = cell;
        }
    }
}
