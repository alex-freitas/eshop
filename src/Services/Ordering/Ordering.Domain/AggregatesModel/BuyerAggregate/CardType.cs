using Ordering.Domain.SharedKernel;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class CardType : Enumeration
    {
        public static readonly CardType Amex = new CardType(1, "Amex");
        public static readonly CardType Visa = new CardType(2, "Visa");
        public static readonly CardType MasterCard = new CardType(3, "MasterCard");

        public CardType(int id, string name)
            : base(id, name)
        {
        }
    }
}
