using DDAS.Models.Entities;
using System.Collections.Generic;

namespace DDAS.Models.Repository
{
    public interface IParamRepository : IRepository<Param>
    {
        List<Param> FindByParId(int ParId);
    }
}
