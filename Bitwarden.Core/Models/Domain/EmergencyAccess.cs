using System;
using Bit.Core.Enums;
using Bit.Core.Utilities;

namespace Bit.Core.Models
{
    public class EmergencyAccess : IKey<Guid>
    {
        public Guid Id { get; set; }
        public Guid GrantorId { get; set; }
        public Guid? GranteeId { get; set; }
        public string Email { get; set; }
        public string KeyEncrypted { get; set; }
        public EmergencyAccessType Type { get; set; }
        public EmergencyAccessStatusType Status { get; set; }
        public int WaitTimeDays { get; set; }
        public DateTime? RecoveryInitiatedDate { get; internal set; }
        public DateTime? LastNotificationDate { get; internal set; }
        public DateTime CreationDate { get; internal set; } = DateTime.UtcNow;
        public DateTime RevisionDate { get; internal set; } = DateTime.UtcNow;

        public void MarkRecovertyInitiated()
        {
            RevisionDate = DateTime.UtcNow;
            RecoveryInitiatedDate = DateTime.UtcNow;
            LastNotificationDate = DateTime.UtcNow;
        }

        public void MarkLastNotified()
        {
            LastNotificationDate = DateTime.UtcNow;
        }

        public void SetNewId()
        {
            Id = CoreHelpers.GenerateComb();
        }
        public void SetId(Guid id)=>Id=id;
        
        public EmergencyAccess ToEmergencyAccess()
        {
            return new EmergencyAccess
            {
                Id = Id,
                GrantorId = GrantorId,
                GranteeId = GranteeId,
                Email = Email,
                KeyEncrypted = KeyEncrypted,
                Type = Type,
                Status = Status,
                WaitTimeDays = WaitTimeDays,
                RecoveryInitiatedDate = RecoveryInitiatedDate,
                LastNotificationDate = LastNotificationDate,
                CreationDate = CreationDate,
                RevisionDate = RevisionDate,
            };
        }
    }
}
