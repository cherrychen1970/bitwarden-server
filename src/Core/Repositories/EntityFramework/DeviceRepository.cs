using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using TableModel = Bit.Core.Models.Table;
using EFModel = Bit.Core.Models.EntityFramework;
using Bit.Core.Models.Table;
using System.Data;
using Dapper;
using Core.Models.Data;
using Bit.Core.Utilities;
using Newtonsoft.Json;
using Bit.Core.Models.Data;

namespace Bit.Core.Repositories.EntityFramework
{
    public class DeviceRepository : Repository<TableModel.Device, TableModel.Device, Guid>, IDeviceRepository
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
