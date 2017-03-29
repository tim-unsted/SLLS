using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace slls.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class DbEntities : IdentityDbContext<ApplicationUser>
    {
        public DbEntities()
            : base("name=SLLS",false)
        {
        }

        public virtual DbSet<AccountYear> AccountYears { get; set; }
        public virtual DbSet<ActivityType> ActivityTypes { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Borrowing> Borrowings { get; set; }
        public virtual DbSet<BudgetCode> BudgetCodes { get; set; }
        public virtual DbSet<Circulation> Circulations { get; set; }
        public virtual DbSet<CirculationMessage> CirculationMessages { get; set; }
        public virtual DbSet<Classmark> Classmarks { get; set; }
        public virtual DbSet<CommMethodType> CommMethodTypes { get; set; }
        public virtual DbSet<Copy> Copies { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<EmailLog> EmailLogs { get; set; }
        public virtual DbSet<EmailLogAttachment> EmailLogAttachments { get; set; }
        public virtual DbSet<ErrorLog> ErrorLogs { get; set; }
        public virtual DbSet<Frequency> Frequencies { get; set; }
        public virtual DbSet<HelpText> HelpTexts { get; set; }
        public virtual DbSet<IpAddress> IpAddresses { get; set; }
        public virtual DbSet<CoverImage> Images { get; set; }
        public virtual DbSet<Keyword> Keywords { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LibraryUserEmailAddress> LibraryUserEmailAddresses { get; set; }
        public virtual DbSet<Localization> Localizations { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<MediaType> MediaTypes { get; set; }
        public virtual DbSet<Menu> Menus { get; set; }
        //public virtual DbSet<OPACAdvancedSearch> OPACAdvancedSearches { get; set; }
        //public virtual DbSet<OPACAdvSearchOperator> OPACAdvSearchOperators { get; set; }
        //public virtual DbSet<OPACBookmark> OPACBookmarks { get; set; }
        //public virtual DbSet<OPACControlType> OPACControlTypes { get; set; }
        //public virtual DbSet<OPACCSSElement> OPACCSSElements { get; set; }
        //public virtual DbSet<OPACCSSTheme> OPACCSSThemes { get; set; }
        //public virtual DbSet<OPACCSSValue> OPACCSSValues { get; set; }
        //public virtual DbSet<OPACIpAddress> OPACIpAddresses { get; set; }
        //public virtual DbSet<OPACLoginAttempt> OPACLoginAttempts { get; set; }
        //public virtual DbSet<OPACSavedSearch> OPACSavedSearches { get; set; }
        //public virtual DbSet<OPACSearchFilter> OPACSearchFilters { get; set; }
        //public virtual DbSet<OPACSearchHeader> OPACSearchHeaders { get; set; }
        //public virtual DbSet<OPACSearchOrderType> OPACSearchOrderTypes { get; set; }
        //public virtual DbSet<OPACSearchResult> OPACSearchResults { get; set; }
        //public virtual DbSet<OPACUserControl> OPACUserControls { get; set; }
        public virtual DbSet<OrderCategory> OrderCategories { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<OrderLink> OrderLinks { get; set; }
        public virtual DbSet<Parameter> Parameters { get; set; }
        public virtual DbSet<PartsReceived> PartsReceiveds { get; set; }
        public virtual DbSet<Publisher> Publishers { get; set; }
        public virtual DbSet<ReleaseHeader> ReleaseHeaders { get; set; }
        public virtual DbSet<ReleaseNote> ReleaseNotes { get; set; }
        public virtual DbSet<ReportType> ReportTypes { get; set; }
        public virtual DbSet<SearchOperatorType> SearchOperatorTypes { get; set; }
        public virtual DbSet<StatusType> StatusTypes { get; set; }
        public virtual DbSet<SubjectIndex> SubjectIndexes { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<SupplierAddress> SupplierAddresses { get; set; }
        public virtual DbSet<SupplierPeople> SupplierPeoples { get; set; }
        public virtual DbSet<SupplierPeopleComm> SupplierPeopleComms { get; set; }
        public virtual DbSet<Title> Titles { get; set; }
        public virtual DbSet<TitleAdditionalFieldData> TitleAdditionalFieldDatas { get; set; }
        public virtual DbSet<TitleAdditionalFieldDef> TitleAdditionalFieldDefs { get; set; }
        public virtual DbSet<TitleAuthor> TitleAuthors { get; set; }
        public virtual DbSet<TitleEditor> TitleEditors { get; set; }
        public virtual DbSet<TitleImage> TitleImages { get; set; }
        public virtual DbSet<TitleLink> TitleLinks { get; set; }
        public virtual DbSet<UsersADMaintenanceLog> UsersADMaintenanceLogs { get; set; }
        public virtual DbSet<Volume> Volumes { get; set; }
        public virtual DbSet<CopacLanguage> CopacLanguages { get; set; }
        public virtual DbSet<CopacLibrary> CopacLibraries { get; set; }
        public virtual DbSet<LibraryUserBookmark> LibraryUserBookmarks { get; set; }
        public virtual DbSet<LibraryUserSavedSearch> LibraryUserSavedSearches { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<LoanType> LoanTypes { get; set; }
        public virtual DbSet<DefaultValue> DefaultValues { get; set; }
        public virtual DbSet<SearchField> SearchFields { get; set; }
        public virtual DbSet<SearchOrderType> SearchOrderTypes { get; set; }
        public virtual DbSet<DashboardGadget> DashboardGadgets { get; set; }
        public virtual DbSet<HostedFile> HostedFiles { get; set; }
        //public virtual DbSet<OpacSearchResults> OpacSearchResults { get; set; }

        //Views
        
        public virtual DbSet<vwVolumesWithLoans> vwVolumesWithLoans { get; set; }
        public virtual DbSet<vwSelectTitle> vwSelectTitles { get; set; }
        public virtual DbSet<vwSelectCopy> vwSelectCopies { get; set; }
        public virtual DbSet<vwSelectCopyWithCheckins> vwSelectCopiesWithCheckins { get; set; }
        public virtual DbSet<vwSelectCirculatedCopy> vwSelectCirculatedCopies { get; set; }
        public virtual DbSet<vwSelectTitleWithCopy> vwSelectTitlesWithCopies { get; set; }
        public virtual DbSet<vwSelectSupplier> vwSelectSuppliers { get; set; }
        public virtual DbSet<vwSelectOrder> vwSelectOrders { get; set; }
        public virtual DbSet<vwSelectUserByFirstname> vwSelectUsersByFirstnames { get; set; }
        public virtual DbSet<vwSelectUserByLastname> vwSelectUsersByLastnames { get; set; }
        public virtual DbSet<vwSelectTitleToBorrow> VwSelectTitlesToBorrow { get; set; }
        public virtual DbSet<vwSelectTitleToRenewReturn> VwSelectTitlesToRenewReturn { get; set; }
        public virtual DbSet<vwTitleSummary> VwTitlesSummary { get; set; }

        //public virtual DbSet<vwBinding> vwBinding { get; set; } 
        //public virtual DbSet<vwBorrowing> vwBorrowing { get; set; }
        //public virtual DbSet<vwItemsOnLoan> vwItemsOnLoan { get; set; }
        //public virtual DbSet<vwAccountQry> vwAccountQry { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            //modelBuilder.Entity<ApplicationUser>().ToTable("dbo.AspNetUsers");

            //modelBuilder.Entity<IdentityUserLogin>()
            //    .HasKey<string>(l => l.UserId);

            //modelBuilder.Entity<ApplicationRole>()
            //    .HasKey<string>(r => r.Id);

            //modelBuilder.Entity<IdentityUserRole>()
            //    .HasKey(r => new { r.RoleId, r.UserId });

            modelBuilder.Entity<AccountYear>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<ActivityType>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Author>()
                .Property(e => e.AuthType)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Author>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Author>()
                .HasMany(e => e.TitleAuthors)
                .WithRequired(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Author>()
                .HasMany(e => e.TitleEditors)
                .WithRequired(e => e.Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Borrowing>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<BudgetCode>()
                .Property(e => e.AllocationSubs)
                .HasPrecision(19, 4);

            modelBuilder.Entity<BudgetCode>()
                .Property(e => e.AllocationOneOffs)
                .HasPrecision(19, 4);

            modelBuilder.Entity<BudgetCode>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Circulation>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Circulation>()
                .HasRequired(c => c.Copy)
                .WithMany(a => a.Circulations);
            
            modelBuilder.Entity<Classmark>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Classmark>()
                .HasMany(e => e.Titles)
                .WithRequired(e => e.Classmark)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CommMethodType>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<CommMethodType>()
                .HasMany(e => e.SupplierPeopleComms)
                .WithRequired(e => e.CommMethodType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Copy>()
                .Property(e => e.AcquisitionsNo)
                .IsUnicode(false);

            modelBuilder.Entity<Copy>()
                .Property(e => e.Saving)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Copy>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Department>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.From)
                .IsUnicode(false);

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.To)
                .IsUnicode(false);

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.CC)
                .IsUnicode(false);

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.BCC)
                .IsUnicode(false);

            modelBuilder.Entity<EmailLog>()
                .Property(e => e.Subject)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ErrType)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ErrURL)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ErrMessage)
                .IsUnicode(false);

            modelBuilder.Entity<ErrorLog>()
                .Property(e => e.ErrStackTrace)
                .IsUnicode(false);

            modelBuilder.Entity<Frequency>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Frequency>()
                .HasMany(e => e.Titles)
                .WithRequired(e => e.Frequency)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<HelpText>()
                .Property(e => e.HelpId)
                .IsUnicode(false);

            modelBuilder.Entity<Keyword>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Keyword>()
                .HasMany(e => e.Keyword1)
                .WithOptional(e => e.Keyword2)
                .HasForeignKey(e => e.ParentKeywordID);

            modelBuilder.Entity<Keyword>()
                .HasMany(e => e.SubjectIndexes)
                .WithRequired(e => e.Keyword)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Language>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Language>()
                .HasMany(e => e.Titles)
                .WithRequired(e => e.Language)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<LibraryUser>()
            //    .Property(e => e.RowVersion)
            //    .IsFixedLength();

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(e => e.Borrowings)
            //    .WithRequired(e => e.BorrowerUser)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(e => e.AuthorisedOrders)
            //    .WithOptional(e => e.AuthoriserUser)
            //    .HasForeignKey(e => e.Authority);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(e => e.RequestedOrders)
            //    .WithOptional(e => e.RequesterUser)
            //    .HasForeignKey(e => e.RequestedBy);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(e => e.Copies)
            //    .WithOptional(e => e.CancelledByUser)
            //    .HasForeignKey(e => e.UserID);

            //modelBuilder.Entity<ApplicationUser>()
            //    .HasMany(e => e.Circulations)
            //    .WithOptional(e => e.Recipient)
            //    .HasForeignKey(e => e.UserID);

            modelBuilder.Entity<Location>()
                .Property(e => e.LocationHier)
                .IsUnicode(false);

            modelBuilder.Entity<Location>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Location>()
                .HasMany(e => e.SubLocations)
                .WithOptional(e => e.ParentLocation)
                .HasForeignKey(e => e.ParentLocationID);

            modelBuilder.Entity<MediaType>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<MediaType>()
                .HasMany(e => e.Titles)
                .WithRequired(e => e.MediaType)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Menu>()
                .Property(e => e.Title)
                .IsUnicode(false);

            modelBuilder.Entity<Menu>()
                .Property(e => e.Controller)
                .IsUnicode(false);

            modelBuilder.Entity<Menu>()
                .Property(e => e.Action)
                .IsUnicode(false);

            modelBuilder.Entity<Menu>()
                .Property(e => e.Roles)
                .IsUnicode(false);

            modelBuilder.Entity<Menu>()
                .HasMany(e => e.Menu1)
                .WithOptional(e => e.Menu2)
                .HasForeignKey(e => e.ParentID);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField1)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchType1)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.Boolean1)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField2)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchType2)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.Boolean2)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField3)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchType3)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.Boolean3)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField4)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchType4)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.Boolean4)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField5)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchType5)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.LocationIDString)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.ClassmarkIDString)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.MediaIDString)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.PublisherIDString)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.OrderBy)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.OrderByDesc)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField1Alias)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField2Alias)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField3Alias)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField4Alias)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvancedSearch>()
            //    .Property(e => e.SearchField5Alias)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvSearchOperator>()
            //    .Property(e => e.OpValue)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvSearchOperator>()
            //    .Property(e => e.OpText)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACAdvSearchOperator>()
            //    .Property(e => e.OpDefaultText)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACBookmark>()
            //    .Property(e => e.Type)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.ControlID)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.ControlGroup)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.ControlType)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.ControlDesc)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.ControlText)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.ToolTip)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACControlType>()
            //    .Property(e => e.Width)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACCSSElement>()
            //    .Property(e => e.ElementName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACCSSElement>()
            //    .HasMany(e => e.OpaccssValues)
            //    .WithRequired(e => e.OPACCSSElement)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OPACCSSTheme>()
            //    .Property(e => e.ThemeName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACCSSTheme>()
            //    .Property(e => e.Description)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACCSSTheme>()
            //    .HasMany(e => e.OPACCSSValues)
            //    .WithRequired(e => e.OPACCSSTheme)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OPACIpAddress>()
            //    .Property(e => e.IPAddress)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACIpAddress>()
            //    .Property(e => e.LastAccessed)
            //    .IsFixedLength();

            //modelBuilder.Entity<OPACLoginAttempt>()
            //    .Property(e => e.logipaddress)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACLoginAttempt>()
            //    .Property(e => e.logusername)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACLoginAttempt>()
            //    .Property(e => e.logpassword)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACLoginAttempt>()
            //    .Property(e => e.timestamp)
            //    .IsFixedLength();

            //modelBuilder.Entity<OPACSavedSearch>()
            //    .Property(e => e.Target)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchFilter>()
            //    .Property(e => e.FilterColumn)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchFilter>()
            //    .Property(e => e.FilterType)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchHeader>()
            //    .Property(e => e.Target)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchHeader>()
            //    .Property(e => e.BoolType)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchHeader>()
            //    .HasMany(e => e.OPACSavedSearches)
            //    .WithRequired(e => e.OPACSearchHeader)
            //    .HasForeignKey(e => e.OrigSearchID)
            //    .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OPACSearchOrderType>()
            //    .Property(e => e.OrderTypeSQL)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchOrderType>()
            //    .Property(e => e.OrderTypeFriendly)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACSearchOrderType>()
            //    .Property(e => e.Scope)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACUserControl>()
            //    .Property(e => e.ControlName)
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACUserControl>()
            //    .Property(e => e.Visible)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<OPACUserControl>()
            //    .Property(e => e.Scope)
            //    .IsUnicode(false);

            modelBuilder.Entity<OrderCategory>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.OrderNo)
                .IsUnicode(false);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.VAT)
                .HasPrecision(19, 4);

            modelBuilder.Entity<OrderDetail>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            //modelBuilder.Entity<OrderDetail>()
            //    .HasOptional(o => o.AuthoriserUser)
            //        .WithMany(o => o.AuthorisedOrders)
            //        .HasForeignKey(o => o.AuthoriserUser)
            //        .WillCascadeOnDelete(false);

            //modelBuilder.Entity<OrderDetail>()
            //    .HasOptional(o => o.RequesterUser)
            //        .WithMany(o => o.RequestedOrders)
            //        .HasForeignKey(o => o.RequesterUser)
            //        .WillCascadeOnDelete(false);

            modelBuilder.Entity<Parameter>()
                .Property(e => e.ParameterID)
                .IsUnicode(false);

            modelBuilder.Entity<Parameter>()
                .Property(e => e.ParamUsage)
                .IsUnicode(false);

            modelBuilder.Entity<Parameter>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<PartsReceived>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            //modelBuilder.Entity<PenWebSearch>()
            //    .Property(e => e.schdate)
            //    .IsUnicode(false);

            //modelBuilder.Entity<PenWebSearch>()
            //    .Property(e => e.schtime)
            //    .IsUnicode(false);

            //modelBuilder.Entity<PenWebSearch>()
            //    .Property(e => e.schtype)
            //    .IsUnicode(false);

            modelBuilder.Entity<Publisher>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Publisher>()
                .HasMany(e => e.Titles)
                .WithRequired(e => e.Publisher)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ReportType>()
                .Property(e => e.FriendlyName)
                .IsUnicode(false);

            modelBuilder.Entity<ReportType>()
                .Property(e => e.ReportName)
                .IsUnicode(false);

            modelBuilder.Entity<ReportType>()
                .Property(e => e.ReportArea)
                .IsUnicode(false);

            modelBuilder.Entity<SearchOperatorType>()
                .Property(e => e.EnglishPhrase)
                .IsUnicode(false);

            modelBuilder.Entity<SearchOperatorType>()
                .Property(e => e.Operator)
                .IsUnicode(false);

            modelBuilder.Entity<SearchOperatorType>()
                .Property(e => e.Prefix)
                .IsUnicode(false);

            modelBuilder.Entity<SearchOperatorType>()
                .Property(e => e.Suffix)
                .IsUnicode(false);

            modelBuilder.Entity<StatusType>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Supplier>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SupplierAddress>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SupplierPeople>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<SupplierPeople>()
                .HasMany(e => e.SupplierPeopleComms)
                .WithRequired(e => e.SupplierPeople)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<SysReleaseHeader>()
            //    .Property(e => e.release)
            //    .IsUnicode(false);

            //modelBuilder.Entity<SysReleaseHeader>()
            //    .Property(e => e.releasedate)
            //    .IsUnicode(false);

            modelBuilder.Entity<Title>()
                .Property(e => e.Price)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Title>()
                .Property(e => e.ISBN10)
                .IsUnicode(false);

            modelBuilder.Entity<Title>()
                .Property(e => e.ISBN13)
                .IsUnicode(false);

            modelBuilder.Entity<Title>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Title>()
                .HasMany(e => e.Copies)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.OrderDetails)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.SubjectIndexes)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.TitleAuthors)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.TitleAdditionalFieldDatas)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.TitleEditors)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.TitleImages)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Title>()
                .HasMany(e => e.TitleLinks)
                .WithRequired(e => e.Title)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TitleAdditionalFieldData>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<TitleAdditionalFieldDef>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<TitleAdditionalFieldDef>()
                .HasMany(e => e.TitleAdditionalFieldDatas)
                .WithRequired(e => e.TitleAdditionalFieldDef)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TitleAuthor>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<TitleEditor>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<TitleImage>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            //modelBuilder.Entity<TitleLabel>()
            //    .Property(e => e.LabelNo)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<TitleLabel>()
            //    .Property(e => e.Title)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<TitleLabel>()
            //    .Property(e => e.LabelText)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<TitleLabel>()
            //    .Property(e => e.Location)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            modelBuilder.Entity<TitleLink>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Volume>()
                .Property(e => e.Barcode)
                .IsUnicode(false);

            modelBuilder.Entity<Volume>()
                .Property(e => e.RowVersion)
                .IsFixedLength();

            modelBuilder.Entity<Volume>()
                .HasMany(e => e.Borrowings)
                .WithRequired(e => e.Volume)
                .WillCascadeOnDelete(false);

            //modelBuilder.Entity<VolumesLabel>()
            //    .Property(e => e.LabelNo)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<VolumesLabel>()
            //    .Property(e => e.Title)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<VolumesLabel>()
            //    .Property(e => e.LabelText)
            //    .IsFixedLength()
            //    .IsUnicode(false);

            //modelBuilder.Entity<VolumesLabel>()
            //    .Property(e => e.Location)
            //    .IsFixedLength()
            //    .IsUnicode(false);
        }


        //public System.Data.Entity.DbSet<slls.Models.Localization> Localizations { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.LocalizationIndexViewModel> LocalizationViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.LocalizationDetailsViewModel> LocalizationDetailsViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.LocalizationEditViewModel> LocalizationEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.TitleAuthorAddViewModel> TitleAuthorAddViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.TitleLinksAddViewModel> TitleLinksAddViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.TitleLinksEditViewModel> TitleLinksEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.TitleAdditionalFieldDefEdit> TitleAdditionalFieldDefEdits { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.TitleCustomDataIndexViewModel> TitleCustomDataIndexViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.LinkedFileEditViewModel> LinkedFileEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.OrderDetailsAddViewModel> OrderDetailsAddViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.OrderDetailsDeleteViewModel> OrderDetailsDeleteViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.OrderReceiptsViewModel> OrderReceiptsViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.SuppliersEditViewModel> SuppliersEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.SupplierPeopleAddViewModel> SupplierPeopleAddViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.NewTitlesListViewModel> NewTitlesListViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.PartsReceivedIndexViewModel> PartsReceivedIndexViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.PartsReceivedEditViewModel> PartsReceivedEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.PartsOverdueViewModel> PartsOverdues { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.CopySummaryViewModel> CopySummaryViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.PartsReceivedSubFormViewModel> PartsReceivedSubFormViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.CirculationListSummaryViewModel> CirculationListSummaryViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.CirculationListViewModel> CirculationListViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.CirculationSlipViewModel> CirculationSlipViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.AddTitleWithAutoCatViewModel> AddTitleWithAutoCatViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.CopacSearchCriteria> CopacSearchCriterias { get; set; }

        //public System.Data.Entity.DbSet<AutoCat.AutoCat.ViewModels.CopacRecordViewModel> CopacRecords { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.LibraryUserBookmarkViewModel> LibraryUserBookmarksViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.Models.UsefulLink> UsefulLinks { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.UsefulLinksAddEditViewModel> UsefulLinksAddEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.NotificationsViewModel> NotificationsViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.LibraryUserSavedSearchViewModel> LibraryUserSavedSearchViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.ParametersAddEditViewModel> ParametersAddEditViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.SelectClassmarkEditorViewModel> SelectClassmarkEditorViewModels { get; set; }

        public System.Data.Entity.DbSet<slls.ViewModels.NewEmailViewModel> NewEmailViewModels { get; set; }
    }
}
