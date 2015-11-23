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
        List<dtoUserAccount> GetAll2(int page, int recordPerPage, object filter, bool isExport);

        List<string> CheckUserNameExists(string username);

        dtoResult UpdateStatus(int id, int status);
    }
}
