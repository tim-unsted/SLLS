using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace slls
{
    public class NullCache : ICache
    {
        public void Clear()
        {
        }

        public object Get(string key)
        {
            return null;
        }

        public void Remove(string key)
        {
        }

        public void Store(string key, object value, int minutesValidFor)
        {
        }
    }
}