using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace slls.ViewModels
{
    public class CacheIndexViewModel
    {
        public List<CacheViewModel> CacheList { get; set; }
    }

    public class CacheViewModel
    {
        public string CacheName { get; set; }
        public string ItemCount { get; set; }   
    }
}