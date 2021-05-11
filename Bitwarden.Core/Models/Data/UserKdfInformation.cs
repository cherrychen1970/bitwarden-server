using System;
using Bit.Core.Enums;

namespace Bit.Core.Models.Data
{
    public class UserKdfInformation
    {
        public KdfType Kdf { get; set; }  =KdfType.PBKDF2_SHA256;
        public int KdfIterations { get; set; }
    }
}
