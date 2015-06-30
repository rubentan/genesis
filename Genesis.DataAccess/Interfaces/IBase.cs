using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Genesis.DTO;

namespace Genesis.DataAccess.Interfaces
{
    public interface IBase<T> where T : class
    {
        T Get(T t);
        List<T> GetAll(string search, object filter = null);
        dtoResult Insert(T t);
        dtoResult Update(T t);
        dtoResult SoftDelete(T t);
        dtoResult Delete(T t); 
    }
}
