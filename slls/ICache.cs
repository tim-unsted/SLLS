using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slls
{
    public interface ICache
    {
        void Clear();
        object Get(string key);
        void Remove(string key);
        void Store(string key, object value, int minutesValidFor);
    }
}
