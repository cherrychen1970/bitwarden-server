using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

using DomainModel = Bit.Core.Models;
using DataModel = Bit.Core.Models.Data;
using EFModel = Bit.Core.Entities;
using Bit.Core.Repositories;


namespace Bit.Infrastructure.EntityFramework
{
    public class UserRepository : Repository<DomainModel.User, EFModel.User, Guid>, IUserRepository
    {
        public UserRepository(DatabaseContext context, IMapper mapper)
            : base(context, mapper)
        { }

        public async Task<DomainModel.User> GetByEmailAsync(string email)
        {
            return await dbSet.Where(e => e.Email == email)
                    .ProjectTo<DomainModel.User>(MapperProvider)
                    .SingleOrDefaultAsync();
        }

        public async Task<DataModel.UserKdfInformation> GetKdfInformationByEmailAsync(string email)
        {
            return await dbSet.Where(e => e.Email == email)
                .Select(e => new DataModel.UserKdfInformation
                {
                    KdfIterations = e.KdfIterations
                }).SingleOrDefaultAsync();
        }

        public async Task<string> GetPublicKeyAsync(Guid id)
        {
            return await dbSet.Where(e => e.Id == id).Select(e => e.PublicKey).SingleOrDefaultAsync();
        }

        public async Task<DateTime> GetAccountRevisionDateAsync(Guid id)
        {
            return await dbSet.Where(e => e.Id == id).Select(e => e.AccountRevisionDate)
                .SingleOrDefaultAsync();
        }
    }
}
