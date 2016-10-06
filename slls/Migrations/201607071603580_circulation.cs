namespace slls.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class circulation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        LibraryUserId = c.Int(),
                        IsLive = c.Boolean(nullable: false),
                        AdObjectGuid = c.String(),
                        FoundInAd = c.Boolean(nullable: false),
                        IgnoreAd = c.Boolean(nullable: false),
                        Position = c.String(),
                        DepartmentId = c.Int(),
                        LocationId = c.Int(),
                        SelfLoansAllowed = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Notes = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.Departments", t => t.DepartmentId)
                .Index(t => t.DepartmentId)
                .Index(t => t.LocationId)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        OrderNo = c.String(maxLength: 50),
                        OrderDate = c.DateTime(storeType: "smalldatetime"),
                        SupplierID = c.Int(),
                        TitleID = c.Int(nullable: false),
                        Item = c.String(maxLength: 255),
                        NumCopies = c.Int(),
                        Price = c.Decimal(storeType: "money"),
                        VAT = c.Decimal(storeType: "money"),
                        OnApproval = c.Boolean(nullable: false),
                        Expected = c.DateTime(storeType: "smalldatetime"),
                        AccountYearID = c.Int(),
                        OrderCategoryID = c.Int(),
                        BudgetCodeID = c.Int(),
                        Cancelled = c.DateTime(storeType: "smalldatetime"),
                        Chased = c.DateTime(storeType: "smalldatetime"),
                        Report = c.String(maxLength: 255),
                        ReceivedDate = c.DateTime(storeType: "smalldatetime"),
                        Accepted = c.Boolean(nullable: false),
                        ReturnedDate = c.DateTime(storeType: "smalldatetime"),
                        InvoiceRef = c.String(maxLength: 255),
                        Passed = c.DateTime(storeType: "smalldatetime"),
                        MonthSubDue = c.Int(),
                        InvoiceDate = c.DateTime(storeType: "smalldatetime"),
                        Link = c.String(maxLength: 1000),
                        Notes = c.String(),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                        AuthoriserUser_Id = c.String(maxLength: 128),
                        RequesterUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.AccountYears", t => t.AccountYearID)
                .ForeignKey("dbo.AspNetUsers", t => t.AuthoriserUser_Id)
                .ForeignKey("dbo.BudgetCodes", t => t.BudgetCodeID)
                .ForeignKey("dbo.OrderCategories", t => t.OrderCategoryID)
                .ForeignKey("dbo.AspNetUsers", t => t.RequesterUser_Id)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .Index(t => t.SupplierID)
                .Index(t => t.TitleID)
                .Index(t => t.AccountYearID)
                .Index(t => t.OrderCategoryID)
                .Index(t => t.BudgetCodeID)
                .Index(t => t.AuthoriserUser_Id)
                .Index(t => t.RequesterUser_Id);
            
            CreateTable(
                "dbo.AccountYears",
                c => new
                    {
                        AccountYearID = c.Int(nullable: false, identity: true),
                        AccountYear = c.String(nullable: false, maxLength: 255),
                        YearStartDate = c.DateTime(nullable: false, storeType: "smalldatetime"),
                        YearEndDate = c.DateTime(nullable: false, storeType: "smalldatetime"),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.AccountYearID);
            
            CreateTable(
                "dbo.BudgetCodes",
                c => new
                    {
                        BudgetCodeID = c.Int(nullable: false, identity: true),
                        BudgetCode = c.String(maxLength: 255),
                        AllocationSubs = c.Decimal(nullable: false, storeType: "money"),
                        AllocationOneOffs = c.Decimal(nullable: false, storeType: "money"),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.BudgetCodeID);
            
            CreateTable(
                "dbo.OrderCategories",
                c => new
                    {
                        OrderCategoryID = c.Int(nullable: false, identity: true),
                        OrderCategory = c.String(maxLength: 255),
                        Annual = c.Boolean(nullable: false),
                        Sub = c.Boolean(nullable: false),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.OrderCategoryID);
            
            CreateTable(
                "dbo.Suppliers",
                c => new
                    {
                        SupplierID = c.Int(nullable: false, identity: true),
                        SupplierName = c.String(maxLength: 255),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.SupplierID);
            
            CreateTable(
                "dbo.SupplierAddresses",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        SupplierID = c.Int(),
                        Division = c.String(maxLength: 255),
                        Address1 = c.String(maxLength: 255),
                        Address2 = c.String(maxLength: 255),
                        TownCity = c.String(name: "Town/City", maxLength: 255),
                        County = c.String(maxLength: 255),
                        Postcode = c.String(maxLength: 50),
                        Country = c.String(maxLength: 255),
                        DX = c.String(maxLength: 50),
                        MainTel = c.String(maxLength: 255),
                        MainFax = c.String(maxLength: 255),
                        Account = c.String(maxLength: 255),
                        ActivityCode = c.Int(),
                        URL = c.String(maxLength: 1000),
                        WebPassword = c.String(maxLength: 255),
                        Notes = c.String(),
                        Email = c.String(maxLength: 255),
                        Phone1 = c.String(maxLength: 255),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.Suppliers", t => t.SupplierID)
                .Index(t => t.SupplierID);
            
            CreateTable(
                "dbo.Titles",
                c => new
                    {
                        TitleID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 450),
                        NonFilingChars = c.Int(nullable: false),
                        MediaID = c.Int(nullable: false),
                        ClassmarkID = c.Int(nullable: false),
                        PublisherID = c.Int(nullable: false),
                        FrequencyID = c.Int(nullable: false),
                        LanguageID = c.Int(nullable: false),
                        Series = c.String(maxLength: 255),
                        Edition = c.String(maxLength: 255),
                        PlaceofPublication = c.String(maxLength: 50),
                        Year = c.String(maxLength: 50),
                        Price = c.Decimal(storeType: "money"),
                        ISBN10 = c.String(maxLength: 50),
                        ISBN13 = c.String(maxLength: 50),
                        Citation = c.String(maxLength: 255),
                        Source = c.String(maxLength: 255),
                        Description = c.String(maxLength: 255),
                        Notes = c.String(),
                        DateCatalogued = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.TitleID)
                .ForeignKey("dbo.Classmarks", t => t.ClassmarkID, cascadeDelete: true)
                .ForeignKey("dbo.FrequencyTypes", t => t.FrequencyID, cascadeDelete: true)
                .ForeignKey("dbo.Languages", t => t.LanguageID, cascadeDelete: true)
                .ForeignKey("dbo.MediaTypes", t => t.MediaID, cascadeDelete: true)
                .ForeignKey("dbo.Publishers", t => t.PublisherID, cascadeDelete: true)
                .Index(t => t.MediaID)
                .Index(t => t.ClassmarkID)
                .Index(t => t.PublisherID)
                .Index(t => t.FrequencyID)
                .Index(t => t.LanguageID);
            
            CreateTable(
                "dbo.Classmarks",
                c => new
                    {
                        ClassmarkID = c.Int(nullable: false, identity: true),
                        Classmark = c.String(maxLength: 255),
                        Code = c.String(maxLength: 50),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.ClassmarkID);
            
            CreateTable(
                "dbo.Copies",
                c => new
                    {
                        CopyID = c.Int(nullable: false, identity: true),
                        TitleID = c.Int(nullable: false),
                        CopyNumber = c.Int(nullable: false),
                        AcquisitionsNo = c.String(maxLength: 50),
                        LocationID = c.Int(),
                        StatusID = c.Int(),
                        AcquisitionsList = c.Boolean(nullable: false),
                        PrintLabel = c.Boolean(nullable: false),
                        Holdings = c.String(),
                        Bind = c.Boolean(nullable: false),
                        Commenced = c.DateTime(storeType: "smalldatetime"),
                        Cancellation = c.DateTime(storeType: "smalldatetime"),
                        AccountYearID = c.Int(),
                        Saving = c.Decimal(storeType: "money"),
                        Notes = c.String(),
                        OnLoan = c.Boolean(nullable: false),
                        Circulated = c.Boolean(nullable: false),
                        StandingOrder = c.Boolean(nullable: false),
                        CirculationMsgID = c.Int(),
                        AddedToAcquisitions = c.DateTime(storeType: "smalldatetime"),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                        CancelledByUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CopyID)
                .ForeignKey("dbo.AccountYears", t => t.AccountYearID)
                .ForeignKey("dbo.AspNetUsers", t => t.CancelledByUser_Id)
                .ForeignKey("dbo.CirculationMessages", t => t.CirculationMsgID)
                .ForeignKey("dbo.Locations", t => t.LocationID)
                .ForeignKey("dbo.StatusTypes", t => t.StatusID)
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .Index(t => t.TitleID)
                .Index(t => t.LocationID)
                .Index(t => t.StatusID)
                .Index(t => t.AccountYearID)
                .Index(t => t.CirculationMsgID)
                .Index(t => t.CancelledByUser_Id);
            
            CreateTable(
                "dbo.CirculationMessages",
                c => new
                    {
                        CirculationMsgID = c.Int(nullable: false, identity: true),
                        CirculationMsg = c.String(),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        ListPos = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CirculationMsgID);
            
            CreateTable(
                "dbo.Circulation",
                c => new
                    {
                        CirculationID = c.Int(nullable: false, identity: true),
                        CopyID = c.Int(),
                        SortOrder = c.Int(),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                        RecipientUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.CirculationID)
                .ForeignKey("dbo.Copies", t => t.CopyID)
                .ForeignKey("dbo.AspNetUsers", t => t.RecipientUser_Id)
                .Index(t => t.CopyID)
                .Index(t => t.RecipientUser_Id);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationID = c.Int(nullable: false, identity: true),
                        ParentLocationID = c.Int(),
                        Location = c.String(),
                        LocationHier = c.String(maxLength: 128),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                        Location2_LocationID = c.Int(),
                    })
                .PrimaryKey(t => t.LocationID)
                .ForeignKey("dbo.Locations", t => t.Location2_LocationID)
                .Index(t => t.Location2_LocationID);
            
            CreateTable(
                "dbo.PartsReceived",
                c => new
                    {
                        PartID = c.Int(nullable: false, identity: true),
                        CopyID = c.Int(),
                        PartReceived = c.String(maxLength: 255),
                        DateReceived = c.DateTime(storeType: "smalldatetime"),
                        PrintList = c.Boolean(nullable: false),
                        Returned = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.PartID)
                .ForeignKey("dbo.Copies", t => t.CopyID)
                .Index(t => t.CopyID);
            
            CreateTable(
                "dbo.StatusTypes",
                c => new
                    {
                        StatusID = c.Int(nullable: false, identity: true),
                        Status = c.String(maxLength: 255),
                        Opac = c.Boolean(nullable: false),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.StatusID);
            
            CreateTable(
                "dbo.Volumes",
                c => new
                    {
                        VolumeID = c.Int(nullable: false, identity: true),
                        CopyID = c.Int(nullable: false),
                        LabelText = c.String(maxLength: 255),
                        OnLoan = c.Boolean(nullable: false),
                        PrintLabel = c.Boolean(nullable: false),
                        RefOnly = c.Boolean(nullable: false),
                        Barcode = c.String(maxLength: 50),
                        IsBarcodeEdited = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.VolumeID)
                .ForeignKey("dbo.Copies", t => t.CopyID, cascadeDelete: true)
                .Index(t => t.CopyID);
            
            CreateTable(
                "dbo.Borrowing",
                c => new
                    {
                        BorrowID = c.Int(nullable: false, identity: true),
                        VolumeID = c.Int(nullable: false),
                        UserID = c.String(),
                        Borrowed = c.DateTime(storeType: "smalldatetime"),
                        ReturnDue = c.DateTime(storeType: "smalldatetime"),
                        Returned = c.DateTime(storeType: "smalldatetime"),
                        Renewal = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                        BorrowerUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BorrowID)
                .ForeignKey("dbo.AspNetUsers", t => t.BorrowerUser_Id)
                .ForeignKey("dbo.Volumes", t => t.VolumeID, cascadeDelete: true)
                .Index(t => t.VolumeID)
                .Index(t => t.BorrowerUser_Id);
            
            CreateTable(
                "dbo.vwVolumesWithLoans",
                c => new
                    {
                        VolumeID = c.Int(nullable: false, identity: true),
                        CopyID = c.Int(nullable: false),
                        CopyNumber = c.Int(nullable: false),
                        Barcode = c.String(),
                        LabelText = c.String(),
                        OnLoan = c.Boolean(nullable: false),
                        RefOnly = c.Boolean(nullable: false),
                        PrintLabel = c.Boolean(nullable: false),
                        BorrowID = c.Int(),
                        UserID = c.String(),
                        Borrowed = c.DateTime(),
                        ReturnDue = c.DateTime(),
                        EmailAddress = c.String(),
                        Fullname = c.String(),
                    })
                .PrimaryKey(t => t.VolumeID)
                .ForeignKey("dbo.Copies", t => t.CopyID, cascadeDelete: true)
                .Index(t => t.CopyID);
            
            CreateTable(
                "dbo.FrequencyTypes",
                c => new
                    {
                        FrequencyID = c.Int(nullable: false, identity: true),
                        Frequency = c.String(maxLength: 255),
                        Days = c.Int(nullable: false),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.FrequencyID);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        LanguageID = c.Int(nullable: false, identity: true),
                        Language = c.String(nullable: false, maxLength: 255),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.LanguageID);
            
            CreateTable(
                "dbo.MediaTypes",
                c => new
                    {
                        MediaID = c.Int(nullable: false, identity: true),
                        Media = c.String(nullable: false, maxLength: 255),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.MediaID);
            
            CreateTable(
                "dbo.Publishers",
                c => new
                    {
                        PublisherID = c.Int(nullable: false, identity: true),
                        PublisherName = c.String(maxLength: 255),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.PublisherID);
            
            CreateTable(
                "dbo.SubjectIndex",
                c => new
                    {
                        SubjectIndexID = c.Int(nullable: false, identity: true),
                        TitleID = c.Int(nullable: false),
                        KeywordID = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SubjectIndexID)
                .ForeignKey("dbo.Keywords", t => t.KeywordID, cascadeDelete: true)
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .Index(t => t.TitleID)
                .Index(t => t.KeywordID);
            
            CreateTable(
                "dbo.Keywords",
                c => new
                    {
                        KeywordID = c.Int(nullable: false, identity: true),
                        KeywordTerm = c.String(maxLength: 255),
                        ParentKeywordID = c.Int(),
                        KeywordHier = c.String(maxLength: 255),
                        Deleted = c.Boolean(nullable: false),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.KeywordID)
                .ForeignKey("dbo.Keywords", t => t.ParentKeywordID)
                .Index(t => t.ParentKeywordID);
            
            CreateTable(
                "dbo.TitleAdditionalFieldData",
                c => new
                    {
                        RecID = c.Int(nullable: false, identity: true),
                        FieldID = c.Int(nullable: false),
                        TitleID = c.Int(nullable: false),
                        FieldData = c.String(),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.RecID)
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .ForeignKey("dbo.TitleAdditionalFieldDefs", t => t.FieldID, cascadeDelete: true)
                .Index(t => t.FieldID)
                .Index(t => t.TitleID);
            
            CreateTable(
                "dbo.TitleAdditionalFieldDefs",
                c => new
                    {
                        FieldID = c.Int(nullable: false, identity: true),
                        FieldName = c.String(nullable: false, maxLength: 128),
                        IsDate = c.Boolean(nullable: false),
                        IsNumeric = c.Boolean(nullable: false),
                        IsBoolean = c.Boolean(nullable: false),
                        IsLongText = c.Boolean(nullable: false),
                        ShowOnOPAC = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.FieldID);
            
            CreateTable(
                "dbo.TitleAuthors",
                c => new
                    {
                        TitleAuthorId = c.Int(nullable: false, identity: true),
                        TitleId = c.Int(nullable: false),
                        AuthorId = c.Int(nullable: false),
                        OrderSeq = c.Int(),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.TitleAuthorId)
                .ForeignKey("dbo.Authors", t => t.AuthorId, cascadeDelete: true)
                .ForeignKey("dbo.Titles", t => t.TitleId, cascadeDelete: true)
                .Index(t => t.TitleId)
                .Index(t => t.AuthorId);
            
            CreateTable(
                "dbo.Authors",
                c => new
                    {
                        AuthorID = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 100),
                        DisplayName = c.String(maxLength: 255),
                        Firstnames = c.String(maxLength: 255),
                        Lastnames = c.String(maxLength: 255),
                        AuthType = c.String(maxLength: 1),
                        Notes = c.String(),
                        Deleted = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.AuthorID);
            
            CreateTable(
                "dbo.TitleEditors",
                c => new
                    {
                        TitleEditorID = c.Int(nullable: false, identity: true),
                        TitleID = c.Int(nullable: false),
                        AuthorID = c.Int(nullable: false),
                        OrderSeq = c.Int(),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.TitleEditorID)
                .ForeignKey("dbo.Authors", t => t.AuthorID, cascadeDelete: true)
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .Index(t => t.TitleID)
                .Index(t => t.AuthorID);
            
            CreateTable(
                "dbo.TitleImages",
                c => new
                    {
                        TitleImageId = c.Int(nullable: false, identity: true),
                        ImageId = c.Int(nullable: false),
                        TitleId = c.Int(nullable: false),
                        Alt = c.String(),
                        HoverText = c.String(),
                        IsPrimary = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.TitleImageId)
                .ForeignKey("dbo.Titles", t => t.TitleId, cascadeDelete: true)
                .Index(t => t.TitleId);
            
            CreateTable(
                "dbo.TitleLinks",
                c => new
                    {
                        TitleLinkID = c.Int(nullable: false, identity: true),
                        TitleID = c.Int(nullable: false),
                        URL = c.String(nullable: false),
                        HoverTip = c.String(maxLength: 1000),
                        DisplayText = c.String(maxLength: 255),
                        Login = c.String(maxLength: 70),
                        Password = c.String(maxLength: 20),
                        IsValid = c.Boolean(nullable: false),
                        LinkStatus = c.String(maxLength: 50),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        Deleted = c.Boolean(nullable: false),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.TitleLinkID)
                .ForeignKey("dbo.Titles", t => t.TitleID, cascadeDelete: true)
                .Index(t => t.TitleID);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Departments",
                c => new
                    {
                        DepartmentID = c.Int(nullable: false, identity: true),
                        Department = c.String(maxLength: 255),
                        CanUpdate = c.Boolean(nullable: false),
                        CanDelete = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        ListPos = c.Int(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        RowVersion = c.Binary(fixedLength: true, timestamp: true, storeType: "timestamp"),
                    })
                .PrimaryKey(t => t.DepartmentID);
            
            CreateTable(
                "dbo.LibraryUserEmailAddresses",
                c => new
                    {
                        EmailID = c.Int(nullable: false, identity: true),
                        UserID = c.String(),
                        EmailAddress = c.String(maxLength: 255),
                        IsPrimary = c.Boolean(nullable: false),
                        Deleted = c.Boolean(nullable: false),
                        InputDate = c.DateTime(storeType: "smalldatetime"),
                        LastModified = c.DateTime(storeType: "smalldatetime"),
                        LibraryUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.EmailID)
                .ForeignKey("dbo.AspNetUsers", t => t.LibraryUser_Id)
                .Index(t => t.LibraryUser_Id);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LibraryUserEmailAddresses", "LibraryUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "DepartmentId", "dbo.Departments");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.TitleLinks", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.TitleImages", "TitleId", "dbo.Titles");
            DropForeignKey("dbo.TitleAuthors", "TitleId", "dbo.Titles");
            DropForeignKey("dbo.TitleEditors", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.TitleEditors", "AuthorID", "dbo.Authors");
            DropForeignKey("dbo.TitleAuthors", "AuthorId", "dbo.Authors");
            DropForeignKey("dbo.TitleAdditionalFieldData", "FieldID", "dbo.TitleAdditionalFieldDefs");
            DropForeignKey("dbo.TitleAdditionalFieldData", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.SubjectIndex", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.SubjectIndex", "KeywordID", "dbo.Keywords");
            DropForeignKey("dbo.Keywords", "ParentKeywordID", "dbo.Keywords");
            DropForeignKey("dbo.Titles", "PublisherID", "dbo.Publishers");
            DropForeignKey("dbo.Orders", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.Titles", "MediaID", "dbo.MediaTypes");
            DropForeignKey("dbo.Titles", "LanguageID", "dbo.Languages");
            DropForeignKey("dbo.Titles", "FrequencyID", "dbo.FrequencyTypes");
            DropForeignKey("dbo.vwVolumesWithLoans", "CopyID", "dbo.Copies");
            DropForeignKey("dbo.Volumes", "CopyID", "dbo.Copies");
            DropForeignKey("dbo.Borrowing", "VolumeID", "dbo.Volumes");
            DropForeignKey("dbo.Borrowing", "BorrowerUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Copies", "TitleID", "dbo.Titles");
            DropForeignKey("dbo.Copies", "StatusID", "dbo.StatusTypes");
            DropForeignKey("dbo.PartsReceived", "CopyID", "dbo.Copies");
            DropForeignKey("dbo.Locations", "Location2_LocationID", "dbo.Locations");
            DropForeignKey("dbo.AspNetUsers", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Copies", "LocationID", "dbo.Locations");
            DropForeignKey("dbo.Circulation", "RecipientUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Circulation", "CopyID", "dbo.Copies");
            DropForeignKey("dbo.Copies", "CirculationMsgID", "dbo.CirculationMessages");
            DropForeignKey("dbo.Copies", "CancelledByUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Copies", "AccountYearID", "dbo.AccountYears");
            DropForeignKey("dbo.Titles", "ClassmarkID", "dbo.Classmarks");
            DropForeignKey("dbo.SupplierAddresses", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Orders", "SupplierID", "dbo.Suppliers");
            DropForeignKey("dbo.Orders", "RequesterUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "OrderCategoryID", "dbo.OrderCategories");
            DropForeignKey("dbo.Orders", "BudgetCodeID", "dbo.BudgetCodes");
            DropForeignKey("dbo.Orders", "AuthoriserUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "AccountYearID", "dbo.AccountYears");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.LibraryUserEmailAddresses", new[] { "LibraryUser_Id" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.TitleLinks", new[] { "TitleID" });
            DropIndex("dbo.TitleImages", new[] { "TitleId" });
            DropIndex("dbo.TitleEditors", new[] { "AuthorID" });
            DropIndex("dbo.TitleEditors", new[] { "TitleID" });
            DropIndex("dbo.TitleAuthors", new[] { "AuthorId" });
            DropIndex("dbo.TitleAuthors", new[] { "TitleId" });
            DropIndex("dbo.TitleAdditionalFieldData", new[] { "TitleID" });
            DropIndex("dbo.TitleAdditionalFieldData", new[] { "FieldID" });
            DropIndex("dbo.Keywords", new[] { "ParentKeywordID" });
            DropIndex("dbo.SubjectIndex", new[] { "KeywordID" });
            DropIndex("dbo.SubjectIndex", new[] { "TitleID" });
            DropIndex("dbo.vwVolumesWithLoans", new[] { "CopyID" });
            DropIndex("dbo.Borrowing", new[] { "BorrowerUser_Id" });
            DropIndex("dbo.Borrowing", new[] { "VolumeID" });
            DropIndex("dbo.Volumes", new[] { "CopyID" });
            DropIndex("dbo.PartsReceived", new[] { "CopyID" });
            DropIndex("dbo.Locations", new[] { "Location2_LocationID" });
            DropIndex("dbo.Circulation", new[] { "RecipientUser_Id" });
            DropIndex("dbo.Circulation", new[] { "CopyID" });
            DropIndex("dbo.Copies", new[] { "CancelledByUser_Id" });
            DropIndex("dbo.Copies", new[] { "CirculationMsgID" });
            DropIndex("dbo.Copies", new[] { "AccountYearID" });
            DropIndex("dbo.Copies", new[] { "StatusID" });
            DropIndex("dbo.Copies", new[] { "LocationID" });
            DropIndex("dbo.Copies", new[] { "TitleID" });
            DropIndex("dbo.Titles", new[] { "LanguageID" });
            DropIndex("dbo.Titles", new[] { "FrequencyID" });
            DropIndex("dbo.Titles", new[] { "PublisherID" });
            DropIndex("dbo.Titles", new[] { "ClassmarkID" });
            DropIndex("dbo.Titles", new[] { "MediaID" });
            DropIndex("dbo.SupplierAddresses", new[] { "SupplierID" });
            DropIndex("dbo.Orders", new[] { "RequesterUser_Id" });
            DropIndex("dbo.Orders", new[] { "AuthoriserUser_Id" });
            DropIndex("dbo.Orders", new[] { "BudgetCodeID" });
            DropIndex("dbo.Orders", new[] { "OrderCategoryID" });
            DropIndex("dbo.Orders", new[] { "AccountYearID" });
            DropIndex("dbo.Orders", new[] { "TitleID" });
            DropIndex("dbo.Orders", new[] { "SupplierID" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUsers", new[] { "LocationId" });
            DropIndex("dbo.AspNetUsers", new[] { "DepartmentId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.LibraryUserEmailAddresses");
            DropTable("dbo.Departments");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.TitleLinks");
            DropTable("dbo.TitleImages");
            DropTable("dbo.TitleEditors");
            DropTable("dbo.Authors");
            DropTable("dbo.TitleAuthors");
            DropTable("dbo.TitleAdditionalFieldDefs");
            DropTable("dbo.TitleAdditionalFieldData");
            DropTable("dbo.Keywords");
            DropTable("dbo.SubjectIndex");
            DropTable("dbo.Publishers");
            DropTable("dbo.MediaTypes");
            DropTable("dbo.Languages");
            DropTable("dbo.FrequencyTypes");
            DropTable("dbo.vwVolumesWithLoans");
            DropTable("dbo.Borrowing");
            DropTable("dbo.Volumes");
            DropTable("dbo.StatusTypes");
            DropTable("dbo.PartsReceived");
            DropTable("dbo.Locations");
            DropTable("dbo.Circulation");
            DropTable("dbo.CirculationMessages");
            DropTable("dbo.Copies");
            DropTable("dbo.Classmarks");
            DropTable("dbo.Titles");
            DropTable("dbo.SupplierAddresses");
            DropTable("dbo.Suppliers");
            DropTable("dbo.OrderCategories");
            DropTable("dbo.BudgetCodes");
            DropTable("dbo.AccountYears");
            DropTable("dbo.Orders");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
        }
    }
}
