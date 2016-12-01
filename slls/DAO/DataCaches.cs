using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Caching;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.UI;
using slls.Models;
using slls.ViewModels;

namespace slls.DAO
{
    public class CacheProvider
    {
        static readonly ObjectCache Cache = MemoryCache.Default;
        private readonly Type _entityType;
        //private readonly DbEntities _db;

        public CacheProvider(Type entityType)
        {
            _entityType = entityType;
            //_db = new DbEntities();
        }

        public static List<TEntityType> GetAll<TEntityType>(string key) where TEntityType : class
        {
            var data = Cache[key] as List<TEntityType>;
            if (data != null) return data;
            var db = new DbEntities();
            data = db.Set<TEntityType>().ToList();
            Cache[key] = data;
            return data;
        }

        public static List<NewTitlesSimpleViewModel> NewTitles()
        {
            var newTitles = Cache["newtitles"] as List<NewTitlesSimpleViewModel>;
            if (newTitles != null) return newTitles;
            using (var db = new DbEntities())
            {
                newTitles = (from t in db.Titles
                             join c in db.Copies on t.TitleID equals c.TitleID
                             where t.Deleted == false && c.Deleted == false && c.AcquisitionsList && c.StatusType.Opac && c.Volumes.Any()
                             select new NewTitlesSimpleViewModel { TitleId = t.TitleID, Title = t.Title1, NonFilingChars = t.NonFilingChars, Commenced = c.Commenced }).Distinct().ToList();
                Cache["newtitles"] = newTitles;
                return newTitles;
            }
        }

        public static List<Title> OpacTitles()
        {
            var opacTitles = Cache["opactitles"] as List<Title>;
            if (opacTitles != null) return opacTitles;
            var db = new DbEntities();
            opacTitles = (from t in db.Titles
                            join c in db.Copies on t.TitleID equals c.TitleID
                            where !t.Deleted && !c.Deleted && c.StatusType.Opac && c.Volumes.Any()
                            select t).Distinct().ToList();
            Cache["opactitles"] = opacTitles;
            return opacTitles;
        }

        public static List<DashboardGadget> DashboardGadgets()
        {
            string customerPackage = App_Settings.GlobalVariables.Package;
            var dashboardGadgets = Cache["dashboardgadgets"] as List<DashboardGadget>;
            if (dashboardGadgets != null) return dashboardGadgets;
            var db = new DbEntities();
            dashboardGadgets = (from g in db.DashboardGadgets
                                where g.Packages != null && g.Packages.Contains(customerPackage)
                            select g).ToList();
            Cache["dashboardgadgets"] = dashboardGadgets;
            return dashboardGadgets;
        }

        public static List<Menu> MenuItems()
        {
            string customerPackage = App_Settings.GlobalVariables.Package;
            var menuItems = Cache["menuitems"] as List<Menu>;
            if (menuItems != null) return menuItems;
            var db = new DbEntities();
            menuItems = (from m in db.Menus
                                where m.Packages != null && m.Packages.Contains(customerPackage)
                                select m).ToList();
            Cache["menuitems"] = menuItems;
            return menuItems;
        }

        public static void RemoveCache(string key)
        {
            Cache.Remove(key);
        }

        public static bool ContainsKey(string key)
        {
            return Cache.Contains(key);
        }

        public static long Count
        {
            get { return Cache.GetCount(); }
        }

        public static void RemoveAll()
        {
            List<string> cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }

        }
    }

    public class ActionOutputCacheAttribute : ActionFilterAttribute
    {
        // This hack is optional; I'll explain it later in the blog post
        private static MethodInfo _switchWriterMethod = typeof(HttpResponse).GetMethod("SwitchWriter", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

        public ActionOutputCacheAttribute(int cacheDuration)
        {
            _cacheDuration = cacheDuration;
        }

        private int _cacheDuration;
        private TextWriter _originalWriter;
        private string _cacheKey;

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _cacheKey = ComputeCacheKey(filterContext);
            string cachedOutput = (string)filterContext.HttpContext.Cache[_cacheKey];
            if (cachedOutput != null)
                filterContext.Result = new ContentResult { Content = cachedOutput };
            else
                _originalWriter = (TextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { new HtmlTextWriter(new StringWriter()) });
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (_originalWriter != null) // Must complete the caching
            {
                HtmlTextWriter cacheWriter = (HtmlTextWriter)_switchWriterMethod.Invoke(HttpContext.Current.Response, new object[] { _originalWriter });
                string textWritten = ((StringWriter)cacheWriter.InnerWriter).ToString();
                filterContext.HttpContext.Response.Write(textWritten);

                filterContext.HttpContext.Cache.Add(_cacheKey, textWritten, null, DateTime.Now.AddSeconds(_cacheDuration), Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
            }
        }

        private string ComputeCacheKey(ActionExecutingContext filterContext)
        {
            var keyBuilder = new StringBuilder();
            foreach (var pair in filterContext.RouteData.Values)
                keyBuilder.AppendFormat("rd{0}_{1}_", pair.Key.GetHashCode(), pair.Value.GetHashCode());
            foreach (var pair in filterContext.ActionParameters)
                keyBuilder.AppendFormat("ap{0}_{1}_", pair.Key.GetHashCode(), pair.Value.GetHashCode());
            return keyBuilder.ToString();
        }
    }
}