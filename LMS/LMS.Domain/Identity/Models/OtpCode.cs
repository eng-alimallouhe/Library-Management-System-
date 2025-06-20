﻿using LMS.Domain.Identity.Enums;

namespace LMS.Domain.Identity.Models
{
    public class OtpCode
    {
        //Primary Key:
        public Guid OtpCodeId { get; set; }

        //Foreign Key: UserId ==> one(user) to one(OtpCode) relationship
        public Guid UserId { get; set; }

        public required string HashedValue { get; set; }
        public bool IsUsed { get; set; }
        public bool IsVerified { get; set; }
        public int FailedAttempts { get; set; }
        public CodeType CodeType { get; set; }


        //Timestamp:
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiredAt { get; set; }



        public OtpCode()
        {
            OtpCodeId = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            ExpiredAt = DateTime.UtcNow.AddMinutes(10);
            IsUsed = false;
            IsVerified = false;
            FailedAttempts = 0;
        }
    }

}
