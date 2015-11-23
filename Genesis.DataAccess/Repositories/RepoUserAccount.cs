using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Genesis.DataAccess.Interfaces;
using Genesis.DTO;



namespace Genesis.DataAccess.Repositories
{
    public class RepoUserAccount :RepoBase, IUserAccount
    {

        public dtoUserAccount Get(dtoUserAccount t)
        {
            
            string hash = "";
            using (MD5 md5Hash = MD5.Create())
            {
                hash = GetMd5Hash(md5Hash, t.passWord);
            }
            var sqlString = String.Format("select top 1 * from tbl_users where username = '{0}' and password = '{1}'", t.userName, hash);
             t  = DBContext.Database.SqlQuery<dtoUserAccount>(sqlString).FirstOrDefault();

            if (t !=null)
            {
                sqlString = string.Format("update tbl_users set dateLastLogin = '{0}' where userId = {1}",DateTime.Now,t.userId );
                var q = DBContext.Database.ExecuteSqlCommand(sqlString);
            }

            return t;
        }

        public List<string> CheckUserNameExists(string username)
        {
            string sQuery = string.Format("select top 1 userName from tbl_users where userName = '{0}'", username);

            return DBContext.Database.SqlQuery<string>(sQuery).ToList();
        }

        public List<dtoUserAccount> GetAll(string search, object filter = null)
        {
            string sQuery = string.Format("select * from tbl_users where ('{0}' = '' or firstName like '%{0}%' or lastName like '%{0}%' or emailAddress like '%{0}%') ", search);

            if (filter != null)
            {
                var f = (dtoUserAccount)filter;
                sQuery += string.Format("and ('{0}' = 0 or cast( userId as varchar(20)) like '%{0}%' )", f.userId);
                sQuery += string.Format("and ('{0}' = '' or firstName like '%{0}%'  )", f.firstName);
                sQuery += string.Format("and ('{0}' = '' or lastName like '%{0}%'  )", f.lastName);
                sQuery += string.Format("and ('{0}' = '' or emailAddress like '%{0}%'  )", f.emailAddress);
                sQuery += string.Format("and ('{0}' = '' or userName like '%{0}%'  )", f.userName);
                sQuery += string.Format("and ({0} = 0 or branchId =  {0} )", f.branchId);
                
            }

            return DBContext.Database.SqlQuery<dtoUserAccount>(sQuery).ToList();            
        }

        public List<dtoUserAccount> GetAll2(int page, int recordPerPage, object filter, bool isExport)
        {
//            string sQuery = string.Format(@"SELECT TOP {0} a.*,b.branchName
//                                            FROM tbl_users  a
//                                            Left Join tbl_branch b
//                                            on a.branchId = b.branchId
//                                            WHERE (1 = 1)",take);

//            if (filter != null)
//            {
//                var f = (dtoUserAccount)filter;               
//                sQuery += string.Format("and ('{0}' = '' or a.firstName like '%{0}%'  )", f.firstName);
//                sQuery += string.Format("and ('{0}' = '' or a.lastName like '%{0}%'  )", f.lastName);
//                sQuery += string.Format("and ('{0}' = '' or a.emailAddress like '%{0}%'  )", f.emailAddress);
//                sQuery += string.Format("and ('{0}' = '' or a.userName like '%{0}%'  )", f.userName);
//                sQuery += string.Format("and ({0} = 0 or a.branchId =  {0} )", f.branchId);

//            }

//            return DBContext.Database.SqlQuery<dtoUserAccount>(sQuery).ToList();

            var f = (dtoUserAccount)filter;
            return DBContext.Database.SqlQuery<dtoUserAccount>("EXEC sp_GetAllUsers @Page,@RecsPerPage,@FirstName,@LastName,@UserName,@EmailAddress,@BranchId,@IsExport"
                , new SqlParameter("Page", page)
                , new SqlParameter("RecsPerPage", recordPerPage)
                , new SqlParameter("FirstName", f.firstName)
                , new SqlParameter("LastName", f.lastName)
                , new SqlParameter("UserName", f.userName)
                , new SqlParameter("EmailAddress", f.emailAddress)
                , new SqlParameter("BranchId", f.branchId)
                , new SqlParameter("IsExport", isExport)
                ).ToList();
        }

        public dtoResult Insert(dtoUserAccount t)
        {
        string hash = "";

            using (MD5 md5Hash = MD5.Create())
            {
                 hash= GetMd5Hash(md5Hash, t.passWord);
            }

            var result = new dtoResult();
            try
            {
                var obj = new tbl_users
                {
                    userName = t.userName,
                    passWord = hash,
                    firstName = t.firstName,
                    lastName = t.lastName,
                    middleName = t.middleName,
                    branchId = t.branchId,
                    emailAddress =  t.emailAddress,
                    status = 1,
                    
                    dateCreated = DateTime.Now

                };

                DBContext.tbl_users.Add(obj);

                DBContext.SaveChanges();

                t.userName = obj.userName;
                result.isSuccessful = true;
                result.returnObj = t;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }

            return result;
        }

        public dtoResult Update(dtoUserAccount t)
        {
            string hash = "";
            var result = new dtoResult();
            try
            {

                using (MD5 md5Hash = MD5.Create())
                {
                    hash = GetMd5Hash(md5Hash, t.passWord);
                }

                var user = DBContext.tbl_users.FirstOrDefault(d => d.userId == t.userId);

                if (user != null)
                {
                    user.userName = t.userName;
                    user.firstName = t.firstName;
                    user.middleName = t.userName;
                    user.lastName = t.lastName;
                    user.passWord = hash;
                    user.branchId = t.branchId;
                }

                DBContext.SaveChanges();

                //t.userName = obj.userName;
                result.isSuccessful = true;
                result.returnObj = t;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }

            return result;

        }

        public dtoResult UpdateStatus(int id,int status)
        {
            var result = new dtoResult();
            try
            {

                var user = DBContext.tbl_users.FirstOrDefault(d => d.userId == id);

                if (user != null)
                {
                    user.status = status;
                }

                DBContext.SaveChanges();

                //t.userName = obj.userName;
                result.isSuccessful = true;
                result.returnObj = id;
            }
            catch (Exception ex)
            {
                result.isSuccessful = false;
                result.errorMsg = ex.ToString();
            }

            return result;

        }

        public dtoResult SoftDelete(dtoUserAccount t)
        {
            throw new NotImplementedException();
        }

        public dtoResult Delete(dtoUserAccount t)
        {
            throw new NotImplementedException();
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}
