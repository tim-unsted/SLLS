using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Westwind.Globalization;

namespace slls.ViewModels
{
    public class DeleteConfirmationViewModel
    {
        public DeleteConfirmationViewModel()
        {
            ButtonText = DbRes.T("Buttons.Confirm_Delete", "Terminology");
            FunctionText = "Confirm Delete";
            ConfirmationHeaderText = "You are about to delete the following ";
            ConfirmationFooterText = "Are you sure you want to continue?";
            ButtonClass = "btn-danger";
            ButtonGlyphicon = "glyphicon-trash";
        }
        
        /// <summary>
        /// Action to perform after user confirms "Delete".
        /// </summary>
        public string PostDeleteAction { get; set; }

        /// <summary>
        /// [OPTIONAL] Controller to look for Post Delete action.
        /// This controller has implementation of Post Delete action
        /// </summary>
        public string PostDeleteController { get; set; }

        public string CallingAction { get; set; }

        public string CallingController { get; set; }

        /// <summary>
        /// While executing POST Delete action, we need id of entity to delete.
        /// </summary>
        public int DeleteEntityId { get; set; }
        public string DeleteEntityIdString { get; set; }

        /// <summary>
        /// Delete Confirmation dialog header text. For example 
        /// For text like "Delete Estimated Effort", Header Text is "Estimated Effort"
        /// </summary>
        public string HeaderText { get; set; }

        public string DetailsText { get; set; }

        public string ButtonText { get; set; }

        public string FunctionText { get; set; }

        public string ConfirmationHeaderText { get; set; }

        public string ConfirmationFooterText { get; set; }

        public string ButtonClass { get; set; }

        public string ButtonGlyphicon { get; set; } 
    }

    public class GenericConfirmationViewModel
    {
        public GenericConfirmationViewModel()
        {
            ConfirmButtonText = "OK";
            ConfirmButtonClass = "btn-success";
            CancelButtonText = "Cancel";
            Glyphicon = "glyphicon-ok";
        }

        public string ConfirmEntityId { get; set; }
        public string PostConfirmController { get; set; }
        public string PostConfirmAction { get; set; }
        public string HeaderText { get; set; }
        [AllowHtml]
        public string DetailsText { get; set; }
        public string ConfirmButtonText { get; set; }
        public string CancelButtonText { get; set; }
        [AllowHtml]
        public string ConfirmationText { get; set; }
        public string ConfirmButtonClass { get; set; }
        public string Glyphicon { get; set; }
        public int Count { get; set; }
        public string Who { get; set; }
    }

    public class GenericHelpViewModel
    {
        public GenericHelpViewModel()
        {
            ConfirmButtonClass = "btn btn-success";
            ConfirmButtonText = " Got It!";
            Glyphicon = "glyphicon-thumbs-up";
        }
        
        public string Title { get; set; }
        public string HelpText { get; set; }
        public string ConfirmButtonClass { get; set; }
        public string ConfirmButtonText { get; set; }
        public string Glyphicon { get; set; }
    }
}