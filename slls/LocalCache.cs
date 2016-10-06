using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

namespace slls
{
    public class LocalCache : ICache
    {
        public object Get(string key)
        {
            return HttpRuntime.Cache.Get(key);
        }

        public void Store(string key, object value, int minutesValidFor)
        {
            HttpRuntime.Cache
                       .Insert(
                        key,
                        value,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(minutesValidFor),
                        CacheItemPriority.Normal,
                        ItemRemoved);
        }

        public void Remove(string key)
        {
            HttpRuntime.Cache.Remove(key);
        }

        public void Clear()
        {
            var enumerator = HttpRuntime.Cache.GetEnumerator();
            while (enumerator.MoveNext())
            {
                HttpRuntime.Cache.Remove(enumerator.Key.ToString());
            }
        }

        private void ItemRemoved(string key, object value, CacheItemRemovedReason reason)
        {
            //TODO: Log removal
        }
    }
}