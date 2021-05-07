using System.Threading.Tasks;
using Bit.Core.Models;

namespace Bit.Core.Services
{
    public interface ISsoConfigService
    {
        Task SaveAsync(SsoConfig config);
    }
}
