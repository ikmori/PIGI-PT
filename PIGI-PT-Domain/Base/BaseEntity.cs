using System.ComponentModel.DataAnnotations.Schema;

namespace PIGI_PT_Domain.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public Guid? CreatedBy { get; protected set; }
        public DateTime? ModifiedAt { get; protected set; }
        public Guid? ModifiedBy { get; protected set; }
        public bool IsActive { get; protected set; }

        private readonly List<DomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected BaseEntity()
        {
        }

        protected BaseEntity(bool generateId)
        {
            if (generateId)
            {
                Id = Guid.NewGuid();
                CreatedAt = DateTime.UtcNow;
                IsActive = true;
            }
        }

        public void AddDomainEvent(DomainEvent domainEvent) => _domainEvents.Add(domainEvent);
        public void ClearDomainEvents() => _domainEvents.Clear();
    }
}