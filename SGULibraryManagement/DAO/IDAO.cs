using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGULibraryManagement.DAO
{
    public interface IDAO<TId, TModel> where TModel : class
    {
        public string TableName { get; }
        public List<TModel> GetAll(bool isActive);
        public TModel FindById(TId id);
        public TModel Create(TModel request);
        public bool Update(TId id, TModel request);
        public bool Delete(TId id);
    }
}
