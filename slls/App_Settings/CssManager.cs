using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using slls.Controllers;
using slls.Models;

namespace slls.App_Settings
{
    public static class CssManager
    {
        private static string _bodyBackgroundColour = "BodyBackgroundColour";
        private static string _bodyTextColour = "BodyTextColour";
        private static string _bodyFontSize = "BodyFontSize";
        private static string _logoSectionBackgroundColour = "LogoSectionBackgroundColour";
        private static string _logoSectionHeight = "LogoSectionHeight";
        private static string _logoFileName = "LogoFileName";
        private static string _logoImageId = "LogoImageId";
        private static string _logoHeight = "LogoHeight";
        private static string _logoPadding = "LogoPadding";
        private static string _logoPosition = "LogoPosition";
        private static string _mainMenuBackgroundColour = "MainMenuBackgroundColour";
        private static string _mainMenuBorderColour = "MainMenuBorderColour";
        private static string _mainMenuFontColour = "MainMenuFontColour";
        private static string _mainMenuHoverFontColour = "MainMenuHoverFontColour";
        private static string _adminMenuHoverFontColour = "AdminMenuHoverFontColour";
        private static string _adminMenuDropdownHeaderUnderlineColour = "AdminMenuDropdownHeaderUnderlineColour";
        private static string _jumbotronBackgroundColour = "JumbotronBackgroundColour";
        private static string _jumbotronTextColour = "JumbotronTextColour";
        private static string _hyperlinkTextColour = "HyperlinkTextColour";
        private static string _hyperlinkHoverColour = "HyperlinkHoverColour";

        public static string BodyBackgroundColour
        {
            get
            {
                if (HttpContext.Current.Application[_bodyBackgroundColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_bodyBackgroundColour] = Settings.GetParameterValue("Styling.BodyBackgroundColor", "#FFFFFF", "The background colour of the main site body. The default is #FFFFFF (white).", dataType:"text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_bodyBackgroundColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_bodyBackgroundColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string BodyTextColour
        {
            get
            {
                if (HttpContext.Current.Application[_bodyTextColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_bodyTextColour] = Settings.GetParameterValue("Styling.BodyTextColor", "#444444", "The main font colour used throughout the site. The default is #444444 (very dark grey).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_bodyTextColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_bodyTextColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string BodyFontSize
        {
            get
            {
                if (HttpContext.Current.Application[_bodyFontSize] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_bodyFontSize] = Settings.GetParameterValue("Styling.BodyFontSize", "14px", "The default standard font size used throughout the site. The default is 14px.", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_bodyFontSize].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_bodyFontSize] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string LogoSectionBackgroundColour
        {
            get
            {
                if (HttpContext.Current.Application[_logoSectionBackgroundColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_logoSectionBackgroundColour] = Settings.GetParameterValue("Styling.LogoSectionBackgroundColor", "#555555", "The background colour of the top panel containing the site's logo. The default is #555555 (dark grey).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_logoSectionBackgroundColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoSectionBackgroundColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string LogoSectionHeight
        {
            get
            {
                if (HttpContext.Current.Application[_logoSectionHeight] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_logoSectionHeight] = Settings.GetParameterValue("Styling.LogoSectionHeight", "150px", "The height of the top panel containing the site's logo. Adjust this to suit your logo. The default is 150px.", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_logoSectionHeight].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoSectionHeight] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string LogoFileName
        {
            get
            {
                if (HttpContext.Current.Application[_logoFileName] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_logoFileName] = Settings.GetParameterValue("Styling.LogoFileName", "slls-logo.png", "The name of the image file to use as the site logo (stored in the ../customer/images folder)", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_logoFileName].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoFileName] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static int LogoImageId
        {
            get
            {
                if (HttpContext.Current.Application[_logoImageId] == null)
                {
                    var imageId = int.Parse(Settings.GetParameterValue("Styling.LogoImageID", "0", "The stored ID of image file to use as the site logo. Supercedes the Styling.FileName parameter.", dataType: "int"));
                    if (imageId == 0)
                    {
                        //Upload the images/slls_logo.png ...
                        var physicalPath = HostingEnvironment.MapPath("~/images/slls-logo.png");
                        using (var sr = new StreamReader(physicalPath))
                        {
                            imageId = FilesController.UploadImage(sr.BaseStream, "slls-logo.png", "image/png", ".png", "~/images/slls-logo.png");
                            Settings.UpdateParameter("Styling.LogoImageID", imageId.ToString());
                        }
                        HttpContext.Current.Application.Lock();
                        HttpContext.Current.Application[_logoImageId] = imageId; 
                        HttpContext.Current.Application.UnLock();
                    }
                    else
                    {
                        HttpContext.Current.Application.Lock();
                        HttpContext.Current.Application[_logoImageId] = imageId; 
                        HttpContext.Current.Application.UnLock();
                    }
                }
                return (int) HttpContext.Current.Application[_logoImageId];
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoImageId] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string LogoHeight
        {
            get
            {
                if (HttpContext.Current.Application[_logoHeight] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_logoHeight] = Settings.GetParameterValue("Styling.LogoHeight", "150px", "The height of the site's logo. Adjust this to suit your logo. The default is 150px.", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_logoHeight].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoHeight] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string LogoPadding
        {
            get
            {
                if (HttpContext.Current.Application[_logoPadding] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_logoPadding] = Settings.GetParameterValue("Styling.LogoPadding", "15px", "The amount of padding (space) around the site's logo. Adjust this to suit your logo. The default is 15px.", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_logoPadding].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoPadding] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string LogoPosition
        {
            get
            {
                if (HttpContext.Current.Application[_logoPosition] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_logoPosition] = Settings.GetParameterValue("Styling.LogoPosition", "Right", "The position (i.e. left or right) of the site's logo. The default is Right.", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_logoPosition].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_logoPosition] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string MainMenuBackgroundColour
        {
            get
            {
                if (HttpContext.Current.Application[_mainMenuBackgroundColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_mainMenuBackgroundColour] = Settings.GetParameterValue("Styling.MainMenuBackgroundColor", "#222222", "The background colour of menu bar containing the main menu and login links. The default is #222222 (very dark grey).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_mainMenuBackgroundColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_mainMenuBackgroundColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string MainMenuBorderColour
        {
            get
            {
                if (HttpContext.Current.Application[_mainMenuBorderColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_mainMenuBorderColour] = Settings.GetParameterValue("Styling.MainMenuBorderColour", "#222222", "The colour of the border above and below the main menu and login links. The default is #222222 (very dark grey).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_mainMenuBorderColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_mainMenuBorderColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string MainMenuFontColour
        {
            get
            {
                if (HttpContext.Current.Application[_mainMenuFontColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_mainMenuFontColour] = Settings.GetParameterValue("Styling.MainMenuFontColor", "#FF1493", "The Font colour of the main menu items and login links. The default is #FF1493 (deep pink).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_mainMenuFontColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_mainMenuFontColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string MainMenuHoverFontColour
        {
            get
            {
                if (HttpContext.Current.Application[_mainMenuHoverFontColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_mainMenuHoverFontColour] = Settings.GetParameterValue("Styling.MainMenuHoverFontColor", "#FFFFFF", "The hover-over Font colour of the main menu items and login links. The default is #FFFFFF (white).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_mainMenuHoverFontColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_mainMenuHoverFontColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string AdminMenuHoverFontColour
        {
            get
            {
                if (HttpContext.Current.Application[_adminMenuHoverFontColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_adminMenuHoverFontColour] = Settings.GetParameterValue("Styling.AdminMenuHoverFontColor", "#FF1493", "The hover-over font colour for the Library Admin and Config menus. The default is #FF1493 (deep pink).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_adminMenuHoverFontColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_adminMenuHoverFontColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string AdminMenuDropdownHeaderUnderlineColour
        {
            get
            {
                if (HttpContext.Current.Application[_adminMenuDropdownHeaderUnderlineColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_adminMenuDropdownHeaderUnderlineColour] = Settings.GetParameterValue("Styling.AdminMenuDropdownHeaderUnderlineColour", "#FF1493", "The heading underline colour in the Library Admin and Config menu drop-downs. The default is #FF1493 (deep pink).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_adminMenuDropdownHeaderUnderlineColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_adminMenuDropdownHeaderUnderlineColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string JumbotronBackgroundColour
        {
            get
            {
                if (HttpContext.Current.Application[_jumbotronBackgroundColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_jumbotronBackgroundColour] = Settings.GetParameterValue("Styling.JumbotronBackgroundColor", "#FFFFFF", "The background colour of the 'Jumbotron' (the large section on the home page holding the welcome message). The default is #FFFFFF (white).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_jumbotronBackgroundColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_jumbotronBackgroundColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string JumbotronTextColour
        {
            get
            {
                if (HttpContext.Current.Application[_jumbotronTextColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_jumbotronTextColour] = Settings.GetParameterValue("Styling.JumbotronTextColor", "#444444", "The font colour of the 'Jumbotron' (the large section on the home page holding the welcome message). The default is #444444 (very dark grey).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_jumbotronTextColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_jumbotronTextColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string HyperlinkTextColour
        {
            get
            {
                if (HttpContext.Current.Application[_hyperlinkTextColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_hyperlinkTextColour] = Settings.GetParameterValue("Styling.HyperlinkTextColor", "#337ab7", "The default colour for all hyperlinks used throughout the site. The default is #337ab7 (standard mid blue).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_hyperlinkTextColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_hyperlinkTextColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static string HyperlinkHoverColour
        {
            get
            {
                if (HttpContext.Current.Application[_hyperlinkHoverColour] == null)
                {
                    HttpContext.Current.Application.Lock();
                    HttpContext.Current.Application[_hyperlinkHoverColour] = Settings.GetParameterValue("Styling.HyperlinkHoverColor", "#23527c", "The main font colour used throughout the site. The default is #23527c (standard darker blue).", dataType: "text");
                    HttpContext.Current.Application.UnLock();
                }
                return HttpContext.Current.Application[_hyperlinkHoverColour].ToString();
            }
            set
            {
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application[_hyperlinkHoverColour] = value;
                HttpContext.Current.Application.UnLock();
            }
        }

        public static void LoadCss()
        {
            var db = new DbEntities();
            var parameters = db.Parameters.Where(x => x.ParameterID.StartsWith("Styling."));

            foreach (var parm in parameters)
            {
                switch (parm.ParameterID)
                {
                    case "Styling.BodyBackgroundColor":
                    {
                        BodyBackgroundColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.BodyTextColour":
                    {
                        BodyTextColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.BodyFontSize":
                    {
                        BodyFontSize = parm.ParameterValue;
                        break;
                    }
                    case "Styling.LogoSectionBackgroundColor":
                    {
                        LogoSectionBackgroundColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.LogoSectionHeight":
                    {
                        LogoSectionHeight = parm.ParameterValue;
                        break;
                    }
                    case "Styling.LogoFileName":
                    {
                        LogoFileName = parm.ParameterValue;
                        break;
                    }
                    case "Styling.LogoFileID":
                    {
                        LogoImageId = int.Parse(parm.ParameterValue);
                        break;
                    }
                    case "Styling.LogoHeight":
                    {
                        LogoHeight = parm.ParameterValue;
                        break;
                    }
                    case "Styling.LogoPadding":
                    {
                        LogoPadding = parm.ParameterValue;
                        break;
                    }
                    case "Styling.LogoPosition":
                    {
                        LogoPosition = parm.ParameterValue;
                        break;
                    }
                    case "Styling.MainMenuBackgroundColor":
                    {
                        MainMenuBackgroundColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.MainMenuBorderColour":
                    {
                        MainMenuBorderColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.MainMenuFontColor":
                    {
                        MainMenuFontColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.MainMenuHoverFontColor":
                    {
                        MainMenuHoverFontColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.AdminMenuHoverFontColor":
                    {
                        AdminMenuHoverFontColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.AdminMenuDropdownHeaderUnderlineColour":
                    {
                        AdminMenuDropdownHeaderUnderlineColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.JumbotronBackgroundColor":
                    {
                        JumbotronBackgroundColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.JumbotronTextColor":
                    {
                        _jumbotronTextColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.HyperlinkTextColor":
                    {
                        HyperlinkTextColour = parm.ParameterValue;
                        break;
                    }
                    case "Styling.HyperlinkHoverColor":
                    {
                        HyperlinkHoverColour = parm.ParameterValue;
                        break;
                    }
                }
            }
        }

    }
}