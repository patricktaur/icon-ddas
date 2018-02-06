using DDAS.EF;
using DDAS.Models.Entities;
using DDAS.Models.Repository;
using System.Collections.Generic;
using System.Linq;

namespace DDAS.EF.Repositories
{
    internal class ParamRepository : Repository<Param>, IParamRepository
    {
        internal ParamRepository(ApplicationIdentityDBContext context)
            : base(context)
        {
        }

        public List<Param> FindByParId(int ParId)
        {
            return Set.Where(x => x.ParId == ParId).ToList();
        }
    }
}
