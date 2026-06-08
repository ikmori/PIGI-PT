namespace PIGI_PT_Domain.Base
{
    public abstract class InquilinoEntity: BaseEntity
    {
        public Guid InquilinoId { get; protected set; }

        protected InquilinoEntity() : base()
        {
        }

        protected InquilinoEntity(Guid inquilinoId) : base(generateId: true)
        {
            if (inquilinoId == Guid.Empty)
                throw new ArgumentException("El identificador del inquilino es obligatorio.");

            InquilinoId = inquilinoId;
        }
    }
}
