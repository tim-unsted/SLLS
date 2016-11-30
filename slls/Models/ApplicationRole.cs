using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace slls.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
         
        public ApplicationRole(string name)
            : base(name)
        {
            
        }
        
        public string Packages { get; set; }
    }
}