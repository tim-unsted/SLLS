﻿using System.Collections.Generic;
using System.Web.Mvc;
using Westwind.Globalization;

namespace slls.ViewModels
{
    public class ListBoxViewModel
    {
        public ListBoxViewModel()
        {
            OkButtonText = DbRes.T("Buttons.Ok", "Terminology");
        }
        
        //A list of the selected items (id's)
        public IEnumerable<string> SelectedItems { get; set; }

        public IEnumerable<SelectListItem> AvailableItems { get; set; }

        /// Action to perform after user confirms "Select".
        public string PostSelectAction { get; set; }

        /// This controller that has implementation of Post Select action
        public string PostSelectController { get; set; }

        public string HeaderText { get; set; }

        public string DetailsHeader { get; set; }

        public string DetailsText { get; set; }

        public string SelectLabel { get; set; }

        public string OkButtonText { get; set; }

        public int PostSelectId { get; set; }

        public string PostSelectIdString { get; set; }

        public string AvailableItemsType { get; set; }
  
    }

    public class SelectPopupViewModel
    {
        public SelectPopupViewModel()
        {
            OkButtonText = DbRes.T("Buttons.Ok", "Terminology");
        }

        //A list of items to select from
        public IEnumerable<SelectListItem> AvailableItems { get; set; }

        //A single selected item (id)
        public string SelectedItem { get; set; }
        public string SelectedItemString { get; set; }

        //A list of options to select from
        public IEnumerable<SelectListItem> AdditionalOptions { get; set; }

        // Any other selected option 
        public string SelectedOption { get; set; }

        //This controller that has implementation of Post Select action
        public string PostSelectController { get; set; }

        // Action to perform after user confirms "Select".
        public string PostSelectAction { get; set; }
        
        public string HeaderText { get; set; }

        public string DetailsHeader { get; set; }

        public string DetailsText { get; set; }

        public string SelectLabel { get; set; }

        public string SelectText { get; set; }

        public string OptionLabel { get; set; }

        public string OptionText { get; set; }

        public string OkButtonText { get; set; }

        public int PostSelectId { get; set; }

        public string PostSelectIdString { get; set; } 

    }

    public class Select2PopupViewModel
    {
        public Select2PopupViewModel()
        {
            OkButtonText = DbRes.T("Buttons.Ok", "Terminology");
        }

        //A list of items to select from
        public IEnumerable<SelectListItem> AvailableItems1 { get; set; }

        //A list of items to select from
        public IEnumerable<SelectListItem> AvailableItems2 { get; set; }

        //A single selected item (id)
        public string SelectedItem1 { get; set; }

        //A single selected item (id)
        public string SelectedItem2 { get; set; }

        //A list of options to select from
        public IEnumerable<SelectListItem> AdditionalOptions { get; set; }

        // Any other selected option 
        public string SelectedOption { get; set; }

        //This controller that has implementation of Post Select action
        public string PostSelectController { get; set; }

        // Action to perform after user confirms "Select".
        public string PostSelectAction { get; set; }

        public string HeaderText { get; set; }

        public string DetailsHeader { get; set; }

        public string DetailsText { get; set; }

        public string SelectLabel1 { get; set; }

        public string SelectLabel2 { get; set; }

        public string SelectText1 { get; set; }

        public string SelectText2 { get; set; }

        public string OkButtonText { get; set; }

        public int PostSelectId { get; set; }

    }

    public class TickBoxViewModel
    {
        public TickBoxViewModel()
        {
            OkButtonText = DbRes.T("Buttons.Ok", "Terminology");
        }

        //A list of the selected items (id's)
        public IEnumerable<string> SelectedItems { get; set; }

        public IEnumerable<SelectListItem> AvailableItems { get; set; }

        /// Action to perform after user confirms "Select".
        public string PostSelectAction { get; set; }

        /// This controller that has implementation of Post Select action
        public string PostSelectController { get; set; }

        public string HeaderText { get; set; }

        public string DetailsHeader { get; set; }

        public string DetailsText { get; set; }

        public string SelectLabel { get; set; }

        public string OkButtonText { get; set; }

        public int PostSelectId { get; set; }

        public string PostSelectIdString { get; set; }

        public string AvailableItemsType { get; set; }

    }
}