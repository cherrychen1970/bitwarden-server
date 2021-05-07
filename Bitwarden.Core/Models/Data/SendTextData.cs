
namespace Bit.Core.Models.Data
{
    public class SendTextData : SendData
    {
        public SendTextData() { }

        public string Text { get; set; }
        public bool Hidden { get; set; }
    }
}
