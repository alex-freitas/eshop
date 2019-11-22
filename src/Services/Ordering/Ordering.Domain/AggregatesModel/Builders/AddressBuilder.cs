using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.Domain.AggregatesModel.Builders
{
    public static class AddressBuilder
    {
        public static Address Build()
        {
            return new Address("street", "city", "state", "country", "zipcode");
        }
    }
}
