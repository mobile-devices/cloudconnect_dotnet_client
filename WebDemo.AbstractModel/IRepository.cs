using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebDemo.AbstractModel
{
    public interface  IRepository<T>
    {
        void Create(T item);
        void Create(IList<T> items);
        void Delete(string id);
        void Update(T item);
        void Update(IList<T> items);
        IList<T> GetAll(int limit = 0);
        T Get(string id);
        T Build();
    }
}
