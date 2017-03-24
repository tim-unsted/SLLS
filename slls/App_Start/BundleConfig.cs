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
            var jqueryBundle = new ScriptBundle("~/bundles/jquery");
            jqueryBundle.Include("~/Scripts/jquery-2.2.3.js");
            bundles.Add(jqueryBundle);


            // JQuery-Ui ...
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-1.12.1.js"));
            

            // JQuery Validation ...
            var jqueryValBundle = new ScriptBundle("~/bundles/jqueryval");
            jqueryValBundle.Include("~/Scripts/jquery.validate.min.js");
            jqueryValBundle.Include("~/Scripts/jquery.validate.unobtrusive.min.js");
            jqueryValBundle.Include("~/Scripts/jquery.unobtrusive-ajax.min.js");
            bundles.Add(jqueryValBundle);


            // Options for DataTables ...
            var datatablesBundle = new ScriptBundle("~/bundles/datatables");
            datatablesBundle.Include("~/Scripts/datatables.min.js");
            datatablesBundle.Include("~/DataTables/options.js");
            bundles.Add(datatablesBundle);
            
            
            // Bootstrap CSS, etc ...
            var cssBundle = new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-datepicker.min.css.map",
                "~/Content/bootstrap-select.min.css",
                "~/Content/datatables.min.css",
                "~/Content/site.css"
                );
            bundles.Add(cssBundle);


            // Site CSS ...
            var siteCssBundle = new StyleBundle("~/Content/sitecss").Include(
                "~/Content/site.css"
                );
            bundles.Add(siteCssBundle);


            // JQuery-ui CSS ...
            var jqueryuicssBundle = new StyleBundle("~/Content/jqueryuicss").Include(
                "~/Content/jquery-ui.css",
                "~/Content/jquery-ui-structure.css",
                "~/Content/jquery-ui.theme.css"
                );
            bundles.Add(jqueryuicssBundle);


            // Bootstrap JS ...
            var bootstrapJsBundle = new ScriptBundle("~/bundles/otherjavascripts").Include(
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/datatables.min.js",
                "~/DataTables/options.js",
                "~/Scripts/respond.js",
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/DatePickerReady.js",
                "~/Scripts/locales/bootstrap-datepicker.en-GB.min.js",
                "~/Scripts/modal.min.js",
                "~/Scripts/slls.js"
                );
            bundles.Add(bootstrapJsBundle);
            

            // Microsoft signalR messaging ...
            var signalRBundle = new ScriptBundle("~/bundles/signalR").Include(
                "~/Scripts/jquery.signalR-2.2.0.min.js",
                "~/Scripts/signal-r.js"
                );
            bundles.Add(signalRBundle);


            //Stuff for the special select used in various places ...
            var bootstrapSelect = new ScriptBundle("~/bundles/selectPicker").Include(
                "~/Scripts/bootstrap-select.min.js"
                );
            bundles.Add(bootstrapSelect);


            //Script for the partial Select Title ...
            var selectTitle = new ScriptBundle("~/bundles/selectTitle").Include(
                "~/Scripts/_SelectTitle.js"
                );
            bundles.Add(selectTitle);


            //Script for the modal edit pop-up ...
            var modalAddEditHeader = new ScriptBundle("~/bundles/modalAddEditHeader");
            modalAddEditHeader.Include("~/Scripts/jquery.validate.min.js");
            modalAddEditHeader.Include("~/Scripts/jquery.validate.unobtrusive.min.js");
            modalAddEditHeader.Include("~/Scripts/jquery.unobtrusive-ajax.min.js");
            modalAddEditHeader.Include("~/Scripts/DatePickerReady.js");
            modalAddEditHeader.Include("~/Scripts/_ModalAddEditHeader.js");
            bundles.Add(modalAddEditHeader);
            

            //Stuff for the multi-column autocomplete ...
            var mcAutoComplete = new ScriptBundle("~/bundles/mcAutoComplete").Include(
                "~/Scripts/jquery.mcautocomplete.js"
                );
            bundles.Add(mcAutoComplete);


            //Scripts to use when adding or editing titles  ...
            var titlesAddEdit = new ScriptBundle("~/bundles/titlesAddEdit").Include(
                "~/Scripts/titles-add-classmarks.js",
                "~/Scripts/titles-add-media.js",
                "~/Scripts/titles-add-frequency.js",
                "~/Scripts/titles-add-publisher.js",
                "~/Scripts/titles-add-language.js"
                );
            bundles.Add(titlesAddEdit);


            //Scripts to use when adding titles  ...
            var titlesAutoCatPartial = new ScriptBundle("~/bundles/titlesAutoCatPartial").Include(
                "~/Scripts/titles-autocat-partial.js"
                );
            bundles.Add(titlesAutoCatPartial);

        }
    }
}