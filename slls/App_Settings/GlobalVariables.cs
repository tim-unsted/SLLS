﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace slls.App_Settings
{
    public static class GlobalVariables
    {
        // Global read-write variables:
        private static string _siteName = "SiteName";
        private static string _opacName = "OpacName";
        private static string _dateFormat = "DateFormat";
        private static string _popupTimeout = "PopupTimeout";

        public static string SiteName
        {
            get
            {
                if (HttpContext.Current.Application[_siteName] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_siteName] = Settings.GetParameterValue("OPAC.SiteName", "Simple Little Library System", "The name you wish to call the entire application. This will appear on browser tabs, etc.");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_siteName].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_siteName] = value;
                HttpContext.Current.Application.UnLock();
            }
        }
        
        public static string OpacName
        {
            get
            {
                if (HttpContext.Current.Application[_opacName] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_opacName] = Settings.GetParameterValue("OPAC.OpacName", "Our Library", "The name you wish to call the public-facing 'OPAC'.");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_opacName].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_opacName] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string DateFormat
        {
            get
            {
                if (HttpContext.Current.Application[_dateFormat] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_dateFormat] = Settings.GetParameterValue("General.DatepickerDateFormat", "dd/mm/yy", "The date format for the date picker. Note: This must use a 2-digit year format.");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_dateFormat].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_dateFormat] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string PopupTimeout
        {
            get
            {
                if (HttpContext.Current.Application[_popupTimeout] == null)
                {
                    HttpContext.Current.Application.Lock();
                    var seconds = Int16.Parse(Settings.GetParameterValue("General.PopupTimeout", "3", "The number of seconds a confirmation or acknowledgement pop-up stays on screen. The default is 3 seconds."));
                    if (seconds == 0 || seconds == null)
                    {
                        seconds = 3;
                    }
                    HttpContext.Current.Application[_popupTimeout] = seconds*1000;
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_popupTimeout].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_popupTimeout] = value;
                HttpContext.Current.Application.UnLock();
            }
        }
    }
}