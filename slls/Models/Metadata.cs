using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace slls.Models
{
    public class AuthorMetadata
    {
        //[Display(Name = "Display Name")]
        public string DisplayName;
    }

    public class LibraryUserMetadata
    {
        //[Display(Name = "Login")]
        public string Username;

        //[Display(Name = "Firstname(s)")]
        public string Firstnames;

        //[Display(Name = "Lastname")]
        public string Lastnames;
    }
}