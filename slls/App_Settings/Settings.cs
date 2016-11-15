using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;

namespace slls.App_Settings
{
    public class Settings
    {
        public string ParameterID { get; set; }
        public string ParameterValue { get; set; }

        public static string GetParameterValue(string id, string value = "", string usage = "")
        {
            var parameters = CacheProvider.GetAll<Parameter>("parameters").ToList();
            var parameter = parameters.FirstOrDefault(p => p.ParameterID == id);
            if (parameter == null)
            {
                if (AddParameter(id, value, usage))
                {
                    return value;
                }
                return null;
            }
            return parameter.ParameterValue;
        }

        public static bool UpdateParameter(string id, string value, string usage = "")
        {
            var repository = new GenericRepository(typeof(Models.Parameter));
            //var db = new DbEntities();
            var parameters = CacheProvider.GetAll<Parameter>("parameters").ToList();
            var parameter = parameters.FirstOrDefault(p => p.ParameterID == id);

            if (parameter == null)
            {
                return AddParameter(id, value, usage);
            }

            try
            {
                parameter.ParameterID = id;
                parameter.ParameterValue = value;
                parameter.ParamUsage = usage.Length == 0 ? parameter.ParamUsage : usage;
                repository.Update(parameter);
                CacheProvider.RemoveCache("parameters");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool AddParameter(string id, string value, string usage = "")
        {
            if (id == null) return false;
            if (value == null) return false;

            var db = new DbEntities();
            var parameter = (from p in db.Parameters where p.ParameterID == id select p).FirstOrDefault();

            if (parameter == null)
            {
                try
                {
                    parameter = new Parameter
                    {
                        ParameterID = id,
                        ParameterValue = value,
                        ParamUsage = usage,
                        InputDate = DateTime.Now,
                        Roles = "Admin;Bailey Admin"
                    };

                    var repository = new GenericRepository(typeof(Models.Parameter));
                    repository.Insert(parameter);
                    CacheProvider.RemoveCache("parameters");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
                
            }
            return false;
        }

    }
}