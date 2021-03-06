﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using slls.Models;

namespace slls.ViewModels
{
    public class OPACHomePageViewModel
    {
        public bool ShowWelcomeMessage { get; set; }
        public string WelcomeHeader { get; set; }
        public string WelcomeMessage { get; set; }
        public bool HasTitles { get; set; } 
        public List<UsefulLink> UsefulLinks { get; set; }
        public List<LibraryUserSavedSearch> SavedSearches { get; set; }
        public List<LibraryUserBookmark> UserBookmarks { get; set; }
        public List<Title> NewTitles { get; set; }
        public List<Notification> Notifications { get; set; }
        public bool LibraryStaff { get; set; }
    }
}