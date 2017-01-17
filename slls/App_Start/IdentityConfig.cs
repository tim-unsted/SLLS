using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using slls.App_Settings;

namespace slls.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public class CustomPasswordValidator : PasswordValidator
        {
            public int MaxLength { get; set; }

            public override async Task<IdentityResult> ValidateAsync(string item)
            {
                IdentityResult result = await base.ValidateAsync(item);

                var errors = result.Errors.ToList();

                if (string.IsNullOrEmpty(item) || item.Length > MaxLength)
                {
                    errors.Add(string.Format("Password length cannot exceed {0} characters.", MaxLength));
                }
                
                return await Task.FromResult(!errors.Any()
                 ? IdentityResult.Success
                 : IdentityResult.Failed(errors.ToArray()));
            }
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = Settings.GetParameterValue("Security.Logins.RequireUniqueEmail", "true", "Specifies whether user email addresses must be unique (Recommended)", dataType: "bool") == "false"
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new CustomPasswordValidator
            {
                RequiredLength = int.Parse(Settings.GetParameterValue("Security.Passwords.MinimumLength", "8", "Specifies the minimum required length of a password.", dataType: "int")),
                RequireNonLetterOrDigit = Settings.GetParameterValue("Security.Passwords.RequireNonLetterOrDigit", "false", "Specifies whether the password requires at least one non-alphanumeric character (e.g. '$', '~', '?', etc.)", dataType: "bool") == "true",
                RequireDigit = Settings.GetParameterValue("Security.Passwords.RequireDigit", "false", "Specifies whether the password requires at least one digit (0-9).", dataType: "bool") == "true",
                RequireLowercase = Settings.GetParameterValue("Security.Passwords.RequireLowercase", "false", "Specifies whether the password requires at least one lower-case letter.", dataType: "bool") == "true",
                RequireUppercase = Settings.GetParameterValue("Security.Passwords.RequireUppercase", "false", "Specifies whether the password requires at least one upper-case letter", dataType: "bool") == "true",
                MaxLength = int.Parse(Settings.GetParameterValue("Security.Passwords.MaximumLength", "999", "Specifies the maximum required length of a password.", dataType: "int"))
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = Settings.GetParameterValue("Security.Logins.UserLockoutEnabled", "true", "Specifies whether the user lockout is enabled when users are created.", dataType: "bool") == "true";
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(int.Parse(Settings.GetParameterValue("Security.Logins.AccountLockoutTimeSpan", "5", "Sets the default amount of time that a user is locked out for after MaxFailedAccessAttemptsBeforeLockout is reached.", dataType: "int")));
            manager.MaxFailedAccessAttemptsBeforeLockout = int.Parse(Settings.GetParameterValue("Security.Logins.MaxFailedAccessAttemptsBeforeLockout", "5", "Sets the maximum number of access attempts allowed before a user is locked out (if lockout is enabled).", dataType: "int"));

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            Messaging.EmailService.SendDbMail(destination:message.Destination,subject:message.Subject, body:message.Body);

            return Task.FromResult(0);
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            const string name = "admin@example.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new ApplicationRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser {UserName = name, Email = name};
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }
        }
    }

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            :
                base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager) UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options,
            IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}