using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace Genesis.DataAccess.Repositories
{
    public class RepoBase
    {
        public GenesisEntities DBContext = new GenesisEntities();       
    }
}
