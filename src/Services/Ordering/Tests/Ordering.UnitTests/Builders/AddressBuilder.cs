using Ordering.Domain.AggregatesModel.OrderAggregate;

namespace Ordering.UnitTests.Builders
{
    public static class AddressBuilder
    {
        public static Address Build()
        {
            return new Address("street", "city", "state", "country", "zipcode");
        }
    }
}
