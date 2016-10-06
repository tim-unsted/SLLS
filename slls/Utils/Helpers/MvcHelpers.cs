using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Mvc;

namespace slls.Utils.Helpers
{
    public static class MvcHelpers
    {
        private static List<Type> GetSubClasses<T>()
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

        public static List<string> GetControllerShortNames()
        {
            List<string> controllerNames = new List<string>();
            controllerNames.Add("");
            GetSubClasses<Controller>().ForEach(
                type => controllerNames.Add(type.Name.Replace("Controller",string.Empty)));
            controllerNames.Sort();
            return controllerNames;
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