using System.Runtime.Serialization;

namespace Ordering.Application.Dtos
{
    [DataContract]
    public class OrderItemDto
    {
        [DataMember]
        public int ProductId { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public decimal UnitPrice { get; set; }

        [DataMember]
        public decimal Discount { get; set; }

        [DataMember]
        public int Units { get; set; }

        [DataMember]
        public string PictureUrl { get; set; }
    }
}
