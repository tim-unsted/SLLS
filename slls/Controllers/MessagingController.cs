﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using slls.App_Settings;
using slls.Models;
using slls.Utils;
using slls.ViewModels;

namespace slls.Controllers
{
    public class MessagingController : sllsBaseController
    {
        private readonly DbEntities _db = new DbEntities();
        
        public ActionResult NewEmail(string to = "", string subject = "")
        {
            var userId = PublicFunctions.GetUserId();
            var emailFrom = "";
            if (userId != null)
            {
                emailFrom = _db.Users.Find(userId).Email;
            }

            var viewModel = new NewEmailViewModel
            {
                To = to,
                From = emailFrom,
                Subject = subject
            };

            ViewBag.Title = "New Email";
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult NewEmail(NewEmailViewModel viewModel, bool captchaValid)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("reCaptcha", "Please verify that you are human!");
                return View(viewModel);
            }
            
            Messaging.EmailService.SendDbMail(destination:viewModel.To, from:viewModel.From,cc:viewModel.Cc,bcc:viewModel.Bcc,subject:viewModel.Subject,body:viewModel.Message);
            TempData["SuccessDialogMsg"] = "Your Enquiry had been sent.";

            if (!string.IsNullOrEmpty(viewModel.RedirectAction))
            {
                return RedirectToAction(viewModel.RedirectAction, viewModel.RedirectController, new { success = true });
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult NewEmailPopup(string to = "", string subject = "")
        {
            var userId = PublicFunctions.GetUserId();
            var emailFrom = "";
            if (userId != null)
            {
                emailFrom = _db.Users.Find(userId).Email;
            }

            var viewModel = new NewEmailViewModel
            {
                To = to,
                From = emailFrom,
                Subject = subject
            };

            ViewBag.Title = "New Email";
            return PartialView("NewEmailPopup", viewModel);
        }

        [HttpPost]
        public ActionResult NewEmailPopup(NewEmailViewModel viewModel)
        {
            Messaging.EmailService.SendDbMail(destination: viewModel.To, from: viewModel.From, cc: viewModel.Cc, bcc: viewModel.Bcc, subject: viewModel.Subject, body: viewModel.Message);
            return Json(new { success = true });
        }

        public ActionResult _NewEmail(string to = "", string subject = "")
        {
            var userId = PublicFunctions.GetUserId();
            var emailFrom = "";
            if (userId != null)
            {
                emailFrom = _db.Users.Find(userId).Email;
            }

            var viewModel = new NewEmailViewModel
            {
                To = to,
                From = emailFrom,
                Subject = subject
            };
            return PartialView("_NewEmail", viewModel);
        }

        [HttpPost]
        public ActionResult SendEmail(NewEmailViewModel viewModel, bool captchaValid)
        {
            if (!captchaValid)
            {
                ModelState.AddModelError("reCaptcha", "Please verify you are human! ");
                TempData["ErrorDialogMsg"] = "Verify that you are human and not a robot!";

                var enquiryTypes = new Dictionary<string, string>
                {
                    {"info@baileysolutions.co.uk", "General Enquiry"},
                    {"support@baileysolutions.co.uk", "Support Request"}
                };
                ViewBag.EnquiryTypes = enquiryTypes;
                ViewBag.Title = viewModel.Title;
                if (!string.IsNullOrEmpty(viewModel.RedirectAction))
                {
                    if (viewModel.RedirectAction == "Contact")
                    {
                        return View(viewModel.RedirectAction, viewModel);
                    }
                }
                return View("NewEmail", viewModel);
            }

            //Attempt to send the message ...
            var success = Messaging.EmailService.SendDbMail(destination: viewModel.To, from: viewModel.From, cc: viewModel.Cc, bcc: viewModel.Bcc, subject: viewModel.Subject, body: viewModel.Message);

            if (success == false)
            {
                TempData["ErrorDialogMsg"] = "Sorry, your message has not been sent. Please try again.";

                var enquiryTypes = new Dictionary<string, string>
                {
                    {"info@baileysolutions.co.uk", "General Enquiry"},
                    {"support@baileysolutions.co.uk", "Support Request"}
                };
                ViewBag.EnquiryTypes = enquiryTypes;
                ViewBag.Title = viewModel.Title;
                if (!string.IsNullOrEmpty(viewModel.RedirectAction))
                {
                    if (viewModel.RedirectAction == "Contact")
                    {
                        return View(viewModel.RedirectAction, viewModel);
                    }
                }
                return View("NewEmail", viewModel);
            }

            //Else return success ...
            TempData["SuccessDialogMsg"] = "Your message had been sent.";
            if (!string.IsNullOrEmpty(viewModel.RedirectAction))
            {
                return RedirectToAction(viewModel.RedirectAction, viewModel.RedirectController, new { success = true });
            }
            return RedirectToAction("Index", "Home", new { success = true });
        }

        public ActionResult ErrorEmail()
        {
            var userId = PublicFunctions.GetUserId();
            var emailFrom = "";
            if (userId != null)
            {
                emailFrom = _db.Users.Find(userId).Email;
            }
            else
            {
                emailFrom = App_Settings.Settings.GetParameterValue("EmailSettings.EmailFromAddress",
                    "client@slls.online.co.uk",
                    "The default 'from' address to use, unless one has been explicitly provided.");
                if (string.IsNullOrEmpty(emailFrom))
                {
                    emailFrom = "unknown.client@slls.online";
                }
            }

            var errorInfo = (System.Web.Mvc.HandleErrorInfo)TempData["ErrorInfo"];

            if(errorInfo != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
                sb.AppendLine("<table>");
                sb.AppendLine("<tr><td><strong>Exception Message: </strong></td><td>" + errorInfo.Exception.Message + "</td></tr>");
                sb.AppendLine("<tr><td><strong>Controller: </strong></td><td>" + errorInfo.ControllerName + "</td></tr>");
                sb.AppendLine("<tr><td><strong>Action: </strong></td><td>" + errorInfo.ActionName + "</td></tr>");
                sb.AppendLine("<tr><td><strong>Stack Trace: </strong></td><td>&nbsp;</td></tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("<p>" + errorInfo.Exception.StackTrace + "</p>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                var message = sb.ToString();
                var success = Messaging.EmailService.SendDbMail(destination: "support@baileysolutions.co.uk", from: emailFrom, subject: "SLLS Unhandled Error", body: message);

                TempData["SuccessDialogMsg"] = "Thank you. Your message had been sent to support@baileysolutions.co.uk.";
                
            }
            return RedirectToAction("Index", "Home", new { success = true });
            
        }
    }
}