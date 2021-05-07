using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EFModel = Bit.Core.Entities;
using Bit.Core.Repositories;
using Bit.Core.Models;


namespace Bit.Infrastructure.EntityFramework
{
    public class DeviceRepository : Repository<Device, Device, Guid>, IDeviceRepository
    {
        public DeviceRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public async Task<Device> GetByIdAsync(Guid id, Guid userId)
        {
            var device = await GetByIdAsync(id);
            if (device == null || device.UserId != userId)
            {
                return null;
            }

            return device;
        }

        public async Task<Device> GetByIdentifierAsync(string identifier)
        {
            return await GetOne<Device>(x=>x.Identifier==identifier);
        }

        public async Task<Device> GetByIdentifierAsync(string identifier, Guid userId)
        {
            return await GetOne<Device>(x=>x.Identifier==identifier && x.UserId==userId);
        }

        public async Task<ICollection<Device>> GetManyByUserIdAsync(Guid userId)
        {
            return await GetMany<Device>(x=>x.UserId==userId);
         }

        public async Task ClearPushTokenAsync(Guid id)
        {
            await DeleteAsync(id);
        }
    }
}
