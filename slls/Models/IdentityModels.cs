using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using slls.Localization;

namespace slls.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public ApplicationUser()
        {
            //Borrowings = new HashSet<Borrowing>();
            //Circulations = new HashSet<Circulation>();
            //LibraryUserEmailAddresses = new HashSet<LibraryUserEmailAddress>();
            //AuthorisedOrders = new HashSet<OrderDetail>();
            //RequestedOrders = new HashSet<OrderDetail>();
            //Copies = new HashSet<Copy>();
        }
        
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string UserBarcode { get; set; }
        public int? LibraryUserId { get; set; }
        public bool IsLive { get; set; }
        public string AdObjectGuid { get; set; }
        public bool FoundInAd { get; set; }
        public bool IgnoreAd { get; set; }
        public string Position { get; set; }
        public int? DepartmentId { get; set; }
        public int? ClassID { get; set; }
        public int? CohortID { get; set; }
        public int? GenderID { get; set; }
        public int? UserTypeID { get; set; }

        [Column("ReadingGroupID")]
        public int? AudienceID { get; set; }
        public DateTime? DoB { get; set; }

        [Column("LocationId")]
        public int? LocationID { get; set; }

        public bool SelfLoansAllowed { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? InputDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? LastModified { get; set; }
        public string Notes { get; set; }
        public bool CanDelete { get; set; }
        public string TempPassword { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressTownCity { get; set; }
        public string AddressCountyState { get; set; }
        public string AddressPostZipCode { get; set; }
        public string AddressCountry { get; set; }
        public string AltPhoneNumber { get; set; }
        public string AltEmailAddress { get; set; }

        [NotMapped]
        [LocalDisplayName("Users.Fullname", "FieldDisplayName")]
        public string Fullname
        {
            get
            {
                return string.Format("{0} {1}", Firstname, Lastname);
            }
        }

        [NotMapped]
        [LocalDisplayName("Users.Fullname", "FieldDisplayName")]
        public string FullnameRev
        {
            get
            {
                return string.Format("{0}, {1}", Lastname, Firstname);
            }
        }

        [NotMapped]
        [LocalDisplayName("Users.Fullname", "FieldDisplayName")]
        public string FullnameRevDept
        {
            get
            {
                if(!string.IsNullOrEmpty(Department.Department1))
                {
                    return string.Format("{0}, {1} ({3})", Lastname, Firstname, Department.Department1);
                }
            else
                {
                    return string.Format("{0}, {1}", Lastname, Firstname);
                }
                
            }
        }

        [NotMapped]
        public bool HasLoans
        {
            get { return Borrowings.Any(b => b.Returned == null); }
        }

        public virtual ICollection<Borrowing> Borrowings { get; set; }
        public virtual ICollection<Circulation> Circulations { get; set; }
        public virtual Department Department { get; set; }
        public virtual Location Location { get; set; }
        public virtual Class Class { get; set; }
        public virtual Cohort Cohort { get; set; }
        public virtual Gender Gender { get; set; }
        public virtual UserType UserType { get; set; }
        public virtual Audience Audience { get; set; }
        public virtual ICollection<LibraryUserEmailAddress> LibraryUserEmailAddresses { get; set; }
        public virtual ICollection<OrderDetail> AuthorisedOrders { get; set; }
        public virtual ICollection<OrderDetail> RequestedOrders { get; set; }
        public virtual ICollection<Copy> Copies { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("SLLS", throwIfV1Schema: false)
        {
            this.Configuration.LazyLoadingEnabled = true;

        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
