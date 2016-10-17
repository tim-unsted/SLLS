using System;
using System.Web.Optimization;

namespace slls
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;
            BundleTable.EnableOptimizations = true; //force optimization while debugging

            // JQuery ...
            var jqueryBundle = new ScriptBundle("~/bundles/jquery",
                "https://ajax.googleapis.com/ajax/libs/jquery/2.2.3/jquery.min.js")
            {
                CdnFallbackExpression = "window.jquery"
            };
            jqueryBundle.Include("~/Scripts/jquery-2.2.3.min.js");
            //var jqueryBundle = new ScriptBundle("~/bundles/jquery").Include("~/Scripts/jquery-2.2.3.min.js");
            bundles.Add(jqueryBundle);


            // JQuery Validation ...
            var jqueryValBundle = new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js"
                );
            bundles.Add(jqueryValBundle);


            // Modernizr ...
            var modernizrBundle = new ScriptBundle("~/bundles/modernizr",
                "https://cdnjs.cloudflare.com/ajax/libs/modernizr/2.8.3/modernizr.min.js")
            {
                CdnFallbackExpression = "window.jquery"
            };
            modernizrBundle.Include("~/Scripts/modernizr-2.8.3.js");
            //var modernizrBundle = new ScriptBundle("~/bundles/modernizr").Include("~/Scripts/modernizr-2.8.3.js");
            bundles.Add(modernizrBundle);


            // Bootstrap JS ...
            //var bootstrapJsBundle = new ScriptBundle("~/bundles/bootstrapjs",
            //    "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js")
            //{
            //    CdnFallbackExpression = "window.jquery"
            //};
            //bootstrapJsBundle.Include("~/Scripts/bootstrap.min.js");
            var bootstrapJsBundle = new ScriptBundle("~/bundles/bootstrapjs").Include("~/Scripts/bootstrap.min.js");
            bundles.Add(bootstrapJsBundle);


            // Other Bootstrap and local JS stuff ...
            var bootstrapExtrasBundle = new ScriptBundle("~/bundles/bootstrapextras").Include(
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/DatePickerReady.js",
                "~/Scripts/locales/bootstrap-datepicker.en-GB.min.js",
                "~/Scripts/modal.min.js");
            bundles.Add(bootstrapExtrasBundle);


            // Options for DataTables ...
            var datatableOptionsBundle = new ScriptBundle("~/bundles/datatables").Include(
                "~/DataTables/options.js");
            bundles.Add(datatableOptionsBundle);


            // JQueryUI ...
            var jqueryUiBundle = new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.12.1.js");
            bundles.Add(jqueryUiBundle);


            // Boostrap CSS ...
            //var bootstrapCssBundle = new StyleBundle("~/Content/bootstrap",
            //    "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css")
            //{
            //    CdnFallbackExpression = "window.jquery"
            //};
            //bootstrapCssBundle.Include("~/Content/bootstrap.min.css");
            var bootstrapCssBundle = new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.min.css");
            bundles.Add(bootstrapCssBundle);


            // Other local CSS stuff ...
            var otherCssBundle = new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap-datepicker.min.css.map",
                "~/Content/jquery-ui.css",
                "~/Content/jquery-ui.theme.css"); 
                //,"~/Content/site.css");
            bundles.Add(otherCssBundle);


            // Microsoft signalR messaging ...
            var signalRBundle = new ScriptBundle("~/bundles/signalR").Include(
                "~/Scripts/jquery.signalR-2.2.0.min.js",
                "~/Scripts/signal-r.js");
            bundles.Add(signalRBundle);
            
        }
    }
}