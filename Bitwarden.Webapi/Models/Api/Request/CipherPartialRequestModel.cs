using System.ComponentModel.DataAnnotations;

namespace Bit.Core.Models.Api
{
    public class CipherPartialRequestModel
    {
        [StringLength(36)]
        public System.Guid FolderId { get; set; }
        public bool Favorite { get; set; }
    }
}
