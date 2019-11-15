namespace Ordering.Domain.SharedKernel
{
    using MediatR;
    using System.Collections.Generic;

    /* https://stackoverflow.com/a/151560 */

    public abstract class Entity
    {
        int _id;
        int? _requestedHashCode;

        public virtual int Id
        {
            get { return _id; }
            protected set { _id = value; }
        }

        private List<INotification> _domainEvents;

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(INotification @event)
        {
            _domainEvents = _domainEvents ?? new List<INotification>();
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(INotification @event)
        {
            _domainEvents?.Remove(@event);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }

        public bool IsTransient()
        {
            return Id == default;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            var item = (Entity)obj;

            if (item.IsTransient() || IsTransient())
                return false;
            else
                return item.Id == Id;
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                {
                    /* XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx) */
                    _requestedHashCode = Id.GetHashCode() ^ 31;
                }

                return _requestedHashCode.Value;
            }
            else
            {
                return base.GetHashCode();
            }
        }
        public static bool operator ==(Entity left, Entity right)
        {
            if (Equals(left, null))
                return (Equals(right, null));
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }
    }
}
