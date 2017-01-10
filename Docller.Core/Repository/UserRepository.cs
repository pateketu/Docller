using System;
using System.Collections.Generic;
using System.Linq;
using Docller.Core.DB;
using Docller.Core.Models;
using Docller.Core.Repository.Accessors;
using Docller.Core.Repository.Collections;
using Docller.Core.Repository.Mappers;
using Microsoft.Practices.EnterpriseLibrary.Data;
using StructureMap;

namespace Docller.Core.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="federation">The federation.</param>
        /// <param name="federationKey">The federation key.</param>
        public UserRepository(FederationType federation, long federationKey)
            : base(federation, federationKey)
        {

        }

        /// <summary>
        /// Logs the on.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <returns></returns>
        public User GetUserForLogOn(string userName)
        {
            //UserId, UserName, FirstName,LastName,Email,IsLocked,
            //ForcePasswordChange,[Password], 
            //PasswordSalt, CreatedDate, FailedLogInAttempt
            Database db = this.GetDb();

            IRowMapper<User> rowMapper =
                MapBuilder<User>.MapNoProperties()
                    .MapByName(x => x.UserId)
                    .MapByName(x => x.UserName)
                    .MapByName(x => x.FirstName)
                    .MapByName(x => x.LastName)
                    .MapByName(x => x.Email)
                    .MapByName(x => x.IsLocked)
                    .MapByName(x => x.ForcePasswordChange)
                    .MapByName(x => x.Password)
                    .MapByName(x => x.PasswordSalt)
                    .MapByName(x => x.CreatedDate)
                    .MapByName(x => x.FailedLogInAttempt).Build();
            
            return SqlDataRepositoryHelper.Get(db, rowMapper, userName);
        }

        public int Update(User user)
        {
            Database db = this.GetDb();
            ModelParameterMapper<User> parameterMapper = new ModelParameterMapper<User>(db, user);
            return SqlDataRepositoryHelper.ExecuteNonQuery(db, user.UpdateProc, user, parameterMapper);
        }

        public User GetUserLogOnInfo(string userName, long customerId)
        {
            Database db = this.GetDb();

            IEnumerable<User> users = db.ExecuteSprocAccessor(StoredProcs.GetUserLogonInfo,DefaultMappers.ForUserLogOnInfo,
                                                              userName, customerId);
            return users.FirstOrDefault();
        }

        public int UpdateUserFailedLoginAttempt(User user)
        {
            Database db = this.GetDb();
            ModelParameterMapper<User> parameterMapper = new ModelParameterMapper<User>(db, user);
            return SqlDataRepositoryHelper.ExecuteNonQuery(db, StoredProcs.UpdateUserFailedLoginAttempt, user, parameterMapper);
        }

        public IEnumerable<User> EnsureUsers(IEnumerable<User> usersToCheck)
        {
            Database db = this.GetDb();
            IRowMapper<User> userRowMapper =
                MapBuilder<User>.MapNoProperties()
                    .MapByName(x => x.UserName)
                    .MapByName(x=>x.Email)
                    .MapByName(x => x.UserId)
                    .MapByName(x=>x.FirstName)
                    .MapByName(x=>x.LastName)
                    .MapByName(x=>x.IsNew)
                    .Build();

            StoredProcAccessor<User> accessor = db.CreateStoredProcAccessor(StoredProcs.EnsureUsers,
                                                                            new GenericParameterMapper(db),
                                                                            userRowMapper);
            return accessor.Execute(new UserCollection(usersToCheck));
        }
    }
}