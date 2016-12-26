using DDAS.Models.Entities.Domain;
using DDAS.Models.Repository.Domain.SiteData;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDAS.Data.Mongo.Repositories.SiteData
{
    internal class UserRoleRepository : Repository<UserRole>, IUserRoleRepository
    {
        private IMongoDatabase _db;
        public UserRoleRepository(IMongoDatabase db): base(db)
        {
            _db = db;
        }

        public Task AddAsync(UserRole userRole)
        {
            return _db.GetCollection<UserRole>(typeof(UserRole).Name).
                InsertOneAsync(userRole);
        }

        //??
        public IList<Guid> GetRoleId(User user)
        {
            var filter = Builders<UserRole>.Filter.Eq("UserId", user.UserId);
            var collection = _db.GetCollection<UserRole>(typeof(UserRole).Name);
            var entity = collection.Find(filter);

            return null;
        }

        //Patrick:
        public IList<string> GetRoleValues(User user)
        {
            return GetRoleValues(user.UserId);
        }

        public IList<string> GetRoleValues(Guid UserId)
        {
 
            IList<string> roles = new List<string>();

            IList<UserRole> userRoles = GetAll();
            foreach (UserRole ur  in userRoles.Where(x => x.UserId == UserId))
            {

                var filter = Builders<Role>.Filter.Eq("RoleId", ur.RoleId);
                var collection = _db.GetCollection<Role>(typeof(Role).Name);
                Role role = collection.Find(filter).FirstOrDefault();

                roles.Add(role.Name);
            }
            
            return roles;
        }

        public IList<UserRole> GetUserRoles(Guid UserId)
        {

            IList<string> roles = new List<string>();

            IList<UserRole> userRoles = GetAll().Where(x => x.UserId == UserId).ToList();

            return userRoles;
        }

        public UserRole GetUserRole(Guid UserId, Guid RoleId)
        {
            return  GetAll().Where(x => x.UserId == UserId && x.RoleId == RoleId).FirstOrDefault();

        }

        public bool DropUserRole(Guid UserRoleId)
        {
            var filter = Builders<UserRole>.Filter.Eq("_id", UserRoleId);
            var collection = _db.GetCollection<UserRole>(typeof(UserRole).Name);
            var entity = collection.DeleteOne(filter);
            return true;
        }
    }
}
