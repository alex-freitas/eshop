using System.Runtime.Serialization;
using MediatR;

namespace Ordering.Application.Commands
{
    public class SetPaidOrderStatusCommand : IRequest<bool>
    {
        public SetPaidOrderStatusCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }

        [DataMember]
        public int OrderNumber { get; private set; }
    }
}
