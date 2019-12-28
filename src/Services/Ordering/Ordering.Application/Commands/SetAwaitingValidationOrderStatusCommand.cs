using System.Runtime.Serialization;
using MediatR;

namespace Ordering.Application.Commands
{

    public class SetAwaitingValidationOrderStatusCommand : IRequest<bool>
    {
        public SetAwaitingValidationOrderStatusCommand(int orderNumber)
        {
            OrderNumber = orderNumber;
        }

        [DataMember]
        public int OrderNumber { get; private set; }
    }
}
