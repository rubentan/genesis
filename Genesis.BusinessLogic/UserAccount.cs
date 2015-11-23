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
    public class UserAccount
    {
        IUserAccount repo;

        public UserAccount()
        {
            repo = new RepoUserAccount();
        }

        public dtoResult Register(dtoUserAccount user)
        {
            return repo.Insert(user);
        }

        public dtoResult Update(dtoUserAccount user)
        {
            return repo.Update(user);
        }

        public dtoResult ActivateUser(int user)
        {
            return repo.UpdateStatus(user, 1);
        }

        public dtoResult DeactivateUser(int user)
        {
            return repo.UpdateStatus(user, 0);
        }

        public dtoResult ValidateCredentials(dtoUserAccount user)
        {
            var result = new dtoResult();
            
            try
            {

                result.returnObj = repo.Get(user);

                if(result.returnObj != null)
                result.isSuccessful = true;
            }
            catch (Exception ex)
            {
                result.errorMsg = ex.ToString();
                result.isSuccessful = false;
            }

            return result;
        }

        public List<dtoUserAccount> GetAllUsers(string search)
        {
            return repo.GetAll(search);
        }

        public List<dtoUserAccount> GetAllUsers(string search, object filter = null)
        {
            return repo.GetAll(search, filter);
        }

        public List<dtoUserAccount> GetAllUsers2(int page, int recordPerPage, object filter, bool isExport)
        {
            return repo.GetAll2(page, recordPerPage, filter, isExport);
        }

        public List<string> CheckUserNameExists(string username)
        {
            return repo.CheckUserNameExists(username);
        }
    }
}
