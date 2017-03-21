using System;
using System.Web.Optimization;

namespace slls
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Modernizr ...
            var modernizrBundle = new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-2.8.3.js");
            bundles.Add(modernizrBundle);


            // JQuery ...
            var jqueryBundle = new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-2.2.3.min.js");
            bundles.Add(jqueryBundle);


            // JQuery Validation ...
            var jqueryValBundle = new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.validate.min.js",
                "~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js"
                );
            bundles.Add(jqueryValBundle);


            // Options for DataTables ...
            var datatableOptionsBundle = new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/datatables.min.js",
                "~/DataTables/options.js"
                );
            bundles.Add(datatableOptionsBundle);
            

            
            // Bootstrap CSS, etc ...
            var cssBundle = new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-datepicker.min.css.map",
                "~/Content/jquery-ui.css",
                "~/Content/jquery-ui.theme.css",
                "~/Content/bootstrap-select.min.css",
                "~/Content/datatables.min.css",
                "~/Content/site.css"
                );
            bundles.Add(cssBundle);


            // Bootstrap JS ...
            var bootstrapJsBundle = new ScriptBundle("~/bundles/bootstrapjs").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/bootstrap-select.min.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/DatePickerReady.js",
                "~/Scripts/locales/bootstrap-datepicker.en-GB.min.js",
                "~/Scripts/modal.min.js"
                );
            bundles.Add(bootstrapJsBundle);
            

            // JQueryUI ...
            var jqueryUiBundle = new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-1.12.1.js"
                );
            bundles.Add(jqueryUiBundle);

            
            // Microsoft signalR messaging ...
            var signalRBundle = new ScriptBundle("~/bundles/signalR").Include(
                "~/Scripts/jquery.signalR-2.2.0.min.js",
                "~/Scripts/signal-r.js"
                );
            bundles.Add(signalRBundle);
            
        }
    }
}