namespace Ordering.Domain.SharedKernel
{
    using System.Collections.Generic;
    using MediatR;

    /* https://stackoverflow.com/a/151560 */

    public abstract class Entity
    {
        private int _id;
        private int? _requestedHashCode;

        private List<INotification> _domainEvents;

        public virtual int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public static bool operator ==(Entity left, Entity right) => Equals(left, null) ? Equals(right, null) : left.Equals(right);

        public static bool operator !=(Entity left, Entity right) => !(left == right);

        public void AddDomainEvent(INotification @event)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(INotification @event) => _domainEvents?.Remove(@event);

        public void ClearDomainEvents() => _domainEvents?.Clear();

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            var item = (Entity)obj;

            return !(item.IsTransient() || IsTransient()) && item.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    /* XOR for random distribution http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx */
                    _requestedHashCode = Id.GetHashCode() ^ 31;
                }

                return _requestedHashCode.Value;
            }
            else
            {
                return base.GetHashCode();
            }
        }
    }
}
