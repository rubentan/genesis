using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface IUserAccount : IBase<dtoUserAccount>
    {
        List<dtoUserAccount> GetAll2(object filter = null, int? skip = null, int? take = null);

        List<string> CheckUserNameExists(string username);

        dtoResult UpdateStatus(int id, int status);
    }
}
