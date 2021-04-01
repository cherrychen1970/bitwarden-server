using Bit.Core.Models.Table;

namespace Bit.Core.Models.Data
{
    public class CollectionDetails : Collection
    {
        public bool ReadOnly { get; set; }
        public bool HidePasswords { get; set; }

        public void SetDetails(CollectionUser detail){
            ReadOnly=detail.ReadOnly;
            HidePasswords=detail.HidePasswords;
        }
    }
}
