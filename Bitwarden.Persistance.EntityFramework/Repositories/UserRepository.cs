using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

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
            return await dbSet.FirstOrDefaultAsync(e => e.Email == email);
        }

        public async Task<DataModel.UserKdfInformation> GetKdfInformationByEmailAsync(string email)
        {
            return await dbSet.Where(e => e.Email == email)
                .Select(e => new DataModel.UserKdfInformation
                {
                    Kdf = e.Kdf,
                    KdfIterations = e.KdfIterations
                }).SingleOrDefaultAsync();
        }

        public async Task<ICollection<DomainModel.User>> SearchAsync(string email, int skip, int take)
        {
            var users = await dbSet
                .Where(e => email == null || e.Email.StartsWith(email))
                .OrderBy(e => e.Email)
                .Skip(skip).Take(take)
                .ToListAsync();
            return Mapper.Map<List<DomainModel.User>>(users);

        }

        public async Task<ICollection<DomainModel.User>> GetManyByPremiumAsync(bool premium)
        {
            var users = await dbSet.Where(e => e.Premium == premium).ToListAsync();
            return Mapper.Map<List<DomainModel.User>>(users);
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

        public async Task UpdateStorageAsync(Guid id)
        {
            await Task.CompletedTask;
            return;
        }

        public async Task UpdateRenewalReminderDateAsync(Guid id, DateTime renewalReminderDate)
        {
            var user = new EFModel.User
            {
                Id = id,
                RenewalReminderDate = renewalReminderDate
            };
            var set = dbSet;
            set.Attach(user);
            dbContext.Entry(user).Property(e => e.RenewalReminderDate).IsModified = true;
            await SaveChangesAsync();
        }

        public Task<DomainModel.User> GetBySsoUserAsync(string externalId, Guid? organizationId)
        {
            throw new NotImplementedException();
        }
    }
}
