namespace Shogi.Bussiness.Domain.Model.Boards
{
    public class HandPosition : IPosition
    {
        private HandPosition() { }
        public static readonly HandPosition Hand = new HandPosition();
        public override string ToString()
        {
            return "手駒";
        }
    }
}
