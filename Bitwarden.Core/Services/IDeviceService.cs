using System.Threading.Tasks;
using Bit.Core.Models;

namespace Bit.Core.Services
{
    public interface IDeviceService
    {
        Task SaveAsync(Device device);
        Task ClearTokenAsync(Device device);
        Task DeleteAsync(Device device);
    }
}
