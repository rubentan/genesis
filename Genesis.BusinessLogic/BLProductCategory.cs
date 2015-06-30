using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DataAccess.Repositories;
using Genesis.DTO;

namespace Genesis.BusinessLogic
{
    public class BLProductCategory
    {
        IProductCategory repo;

        public BLProductCategory()
        {
            repo = new RepoProductCategory();
        }

        public List<dtoProductCategory> GetAll(string search)
        {
            return repo.GetAll(search);
        }
    }
}
