using MediatR;

namespace PIGI_PT_Domain.Base
{
    public abstract class DomainEvent: INotification
    {
        public bool IsPublished { get; set; }
        public DateTime DateOccurred { get; protected set; } = DateTime.UtcNow;
    }
}
