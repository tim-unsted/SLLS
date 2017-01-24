using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace slls.Utils.Helpers
{
    public static class MvcHelpers
    {
        private static List<Type> GetSubClasses<T>(string nameSpace = "")
        {
            return Assembly.GetCallingAssembly().GetTypes().Where(
                type => type.IsSubclassOf(typeof(T))).ToList();
        }

        public static List<string> GetControllerNames()
        {
            List<string> controllerNames = new List<string>();
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name));
            return controllerNames;
        }

        public static List<string> GetControllerShortNames(string nameSpace = "")
        {
            List<string> controllerNames = new List<string>();
            controllerNames.Add("");
            var controllers = GetSubClasses<Controller>();
            var c = controllers.Where(x => x.Namespace == nameSpace);

            c.ForEach(
                type => controllerNames.Add(type.Name.Replace("Controller", string.Empty)));
            controllerNames.Sort();
            return controllerNames;

            //var q = (from t in Assembly.GetExecutingAssembly().GetTypes()
            //         where t.IsClass //&& t.Namespace == nameSpace
            //        && typeof(Controller).IsSubclassOf(typeof(Controller))
            //        select t.Name).ToList();
            //controllerNames.AddRange(q);
            //controllerNames.Sort();
            //return controllerNames;
        }

        //public static Dictionary<string, string> GetControllerNames()
        //{
        //    Dictionary<string, string> controllerNames = new Dictionary<string, string>();
        //    GetSubClasses<Controller>().ForEach(
        //        type => controllerNames.Add(type.Name, type.Name));
        //    return controllerNames;
        //}

    }
}