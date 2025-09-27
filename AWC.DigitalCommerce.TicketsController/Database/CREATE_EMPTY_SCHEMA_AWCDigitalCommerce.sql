USE [AWCDigitalCommerce]
GO
/****** Object:  Table [dbo].[tbl_Advancements]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Advancements](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[Amount] [int] NOT NULL,
	[RequestedBy] [varchar](10) NOT NULL,
	[ApprovedBy] [varchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_Advancements] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ATV]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ATV](
	[ID] [int] IDENTITY(50000,1) NOT NULL,
	[TicketID] [int] NOT NULL,
	[CustomerName] [varchar](50) NOT NULL,
	[SSN_Type] [int] NOT NULL,
	[SSN] [varchar](50) NOT NULL,
	[CountryCode] [int] NOT NULL,
	[PhoneNumber] [int] NOT NULL,
	[eMailAddress] [varchar](255) NOT NULL,
 CONSTRAINT [PK_tbl_ATV] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_BartenderOrder]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_BartenderOrder](
	[GUID] [varchar](50) NOT NULL,
	[CustomerID] [varchar](50) NOT NULL,
	[BeveragesList] [varchar](5000) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_BucketsConfig]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_BucketsConfig](
	[ID] [int] IDENTITY(7000,1) NOT NULL,
	[ParentID] [int] NOT NULL,
	[ChildID] [int] NOT NULL,
 CONSTRAINT [PK_tbl_BucketsConfig] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_BucketsDetail]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_BucketsDetail](
	[ID] [int] IDENTITY(7000,1) NOT NULL,
	[TicketNumber] [int] NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
 CONSTRAINT [PK_tbl_BucketsDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CashIncomes]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CashIncomes](
	[ID] [int] IDENTITY(3000,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[Shift] [int] NOT NULL,
	[IncomeDescription] [varchar](255) NOT NULL,
	[IncomeAmount] [int] NOT NULL,
	[WhoDidIt] [varchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CashOnDrawer]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CashOnDrawer](
	[ID] [int] IDENTITY(5000,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[Shift] [int] NOT NULL,
	[CashAvailable] [int] NOT NULL,
	[CashWithdrawal] [int] NOT NULL,
	[CashRemaining] [int] NOT NULL,
	[WhoDidIt] [varchar](10) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Categories]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Categories](
	[CategoryID] [int] IDENTITY(10001,1) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[ParentID] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_CustomerID]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_CustomerID](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[Type] [int] NOT NULL,
	[SubType] [int] NULL,
	[CustomerID] [varchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
	[ApplyServiceFee] [bit] NULL,
	[LastPayment] [varchar](8) NOT NULL,
	[FreeOfCharge] [bit] NOT NULL,
	[CreditLimit] [int] NOT NULL,
	[BirthDay] [varchar](50) NULL,
	[MailAddress] [varchar](255) NULL,
	[LoyaltyPoints] [int] NOT NULL,
 CONSTRAINT [PK_tbl_CustomerID] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_DailyAccountantReport]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DailyAccountantReport](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[GrossSales] [int] NOT NULL,
	[NetSales] [int] NOT NULL,
	[Sales_Cash] [int] NOT NULL,
	[Sales_CreditCard] [int] NOT NULL,
	[Sales_Transfer] [int] NOT NULL,
	[Sales_Voucher] [int] NOT NULL,
	[Drawer_Cash] [int] NOT NULL,
	[Drawer_CreditCard] [int] NOT NULL,
	[Drawer_Transfer] [int] NOT NULL,
	[Drawer_Voucher] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_DailyClosing]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DailyClosing](
	[ID] [int] IDENTITY(5000,1) NOT NULL,
	[WorkDay] [varchar](8) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[TicketNumber] [int] NOT NULL,
	[CustomerAKA] [varchar](50) NULL,
 CONSTRAINT [PK_tbl_DailyClosing] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_DailyClosingSummary]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DailyClosingSummary](
	[BusinessDate] [varchar](8) NOT NULL,
	[Shift] [int] NOT NULL,
	[InitialCash] [int] NOT NULL,
	[IncomeCash] [int] NOT NULL,
	[Cash] [int] NOT NULL,
	[CashByOperator] [int] NULL,
	[CreditCard] [int] NOT NULL,
	[CreditCardByOperator] [int] NULL,
	[Transfer] [int] NOT NULL,
	[TransferByOperator] [int] NULL,
	[Voucher] [int] NOT NULL,
	[VoucherByOperator] [int] NULL,
	[AccountsReceivable] [int] NOT NULL,
	[ServiceFee] [int] NOT NULL,
	[GeneralExpenses] [int] NOT NULL,
	[GrossSale] [int] NOT NULL,
	[NetSale] [int] NOT NULL,
	[TotalCashInDrawer] [int] NOT NULL,
	[DailyClosingMatch] [bit] NULL,
	[WhoDidIt] [varchar](10) NULL,
	[CreatedAt] [datetime] NOT NULL,
	[Splited] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_DigitalKeyboardFields]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_DigitalKeyboardFields](
	[ID] [int] IDENTITY(20000,1) NOT NULL,
	[FormName] [varchar](50) NOT NULL,
	[FieldName] [varchar](50) NOT NULL,
	[DigitalKeyboardON] [bit] NOT NULL,
 CONSTRAINT [PK_tbl_DigitalKeyboardFields] PRIMARY KEY NONCLUSTERED 
(
	[FormName] ASC,
	[FieldName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Expenses]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Expenses](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[ExpenseDate] [varchar](50) NOT NULL,
	[Splited] [int] NOT NULL,
	[ExpenseDescription] [varchar](50) NOT NULL,
	[ExpenseAmount] [int] NOT NULL,
	[Shift] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Expenses] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_InternalOrders]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_InternalOrders](
	[OrderDate] [varchar](8) NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[OrderDescription] [varchar](50) NOT NULL,
	[WhoDidIt] [varchar](10) NOT NULL,
	[CreatedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_InternalOrdersDetail]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_InternalOrdersDetail](
	[GUID] [varchar](50) NOT NULL,
	[ItemDescription] [varchar](50) NOT NULL,
	[Qty] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Invoices]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Invoices](
	[InvoiceID] [int] IDENTITY(5000,1) NOT NULL,
	[ProviderID] [int] NOT NULL,
	[InvoiceNumber] [int] NOT NULL,
	[InvoiceDate] [varchar](8) NOT NULL,
	[InvoiceAmount] [numeric](10, 2) NOT NULL,
	[InvoiceGUID] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_InvoicesDetail]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_InvoicesDetail](
	[InvoiceGUID] [varchar](50) NOT NULL,
	[InvoiceDetailID] [int] IDENTITY(5000,1) NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemQty] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Items]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Items](
	[ID] [int] IDENTITY(2000,1) NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemSubType] [int] NOT NULL,
	[IitemCABYS] [varchar](15) NOT NULL,
	[ItemDescription] [varchar](50) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsContainer] [bit] NOT NULL,
	[HideInMenu] [bit] NOT NULL,
	[UnitPrice] [int] NOT NULL,
	[UnitCost] [int] NULL,
	[ItemAvailable] [int] NULL,
	[ItemSold] [int] NULL,
	[ItemDefective] [int] NULL,
	[ItemParent] [int] NULL,
	[ItemParentUnit] [int] NULL,
	[ItemMinimum] [int] NULL,
	[ItemUnitOfMeasurement] [int] NULL,
	[ItemUnitSize] [int] NULL,
	[ItemStock] [int] NULL,
	[DebitNotes] [int] NULL,
	[CreditNotes] [int] NULL,
 CONSTRAINT [PK_tbl_Items_1] PRIMARY KEY CLUSTERED 
(
	[ItemDescription] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ItemsChangePrice]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ItemsChangePrice](
	[ID] [int] IDENTITY(1,1000) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[ItemID] [int] NOT NULL,
	[PreviousPrice] [int] NOT NULL,
	[CurrentPrice] [int] NOT NULL,
	[WhoDidit] [varchar](10) NOT NULL,
	[MadeItAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ItemsDefective]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ItemsDefective](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemQty] [int] NOT NULL,
	[DeclarationDate] [varchar](8) NOT NULL,
	[whoDeclared] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ItemsDeleted]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ItemsDeleted](
	[ID] [int] NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[WhoDeleted] [int] NOT NULL,
	[WhoAuth] [varchar](10) NOT NULL,
	[DeletedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ItemsDeletedFromSystem]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ItemsDeletedFromSystem](
	[ID] [int] IDENTITY(1,1000) NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemDescription] [varchar](50) NOT NULL,
	[WhoDeleted] [int] NOT NULL,
	[WhoDeletedName] [varchar](50) NOT NULL,
	[DeletedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_ItemsOrders]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_ItemsOrders](
	[ID] [int] IDENTITY(20000,1) NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[WhoOrder] [varchar](10) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Qty] [int] NOT NULL,
 CONSTRAINT [PK_tbl_ItemsOrders] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_LoyaltyRewards]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_LoyaltyRewards](
	[ID] [int] IDENTITY(9000,1) NOT NULL,
	[Description] [varchar](225) NOT NULL,
	[Status] [varchar](1) NOT NULL,
	[ItemToQualify] [int] NOT NULL,
	[QtyToQualify] [int] NOT NULL,
	[MaxDaysForReward] [int] NOT NULL,
	[ItemRewarded] [int] NOT NULL,
	[QtyRewarded] [int] NOT NULL,
	[TotalItemsAwarded] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Lunches]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Lunches](
	[ID] [int] IDENTITY(100,1) NOT NULL,
	[LunchDate] [varchar](10) NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[EmployeeName] [varchar](30) NOT NULL,
	[Qty] [int] NOT NULL,
	[MealID] [int] NOT NULL,
 CONSTRAINT [PK_tbl_Lunches] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_MealsRelationships]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_MealsRelationships](
	[ID] [int] IDENTITY(1,50000) NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemFrom] [int] NOT NULL,
	[ItemTo] [int] NOT NULL,
	[Qty] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_tbl_MealsRelationships] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_MoneyDrawerLog]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_MoneyDrawerLog](
	[ID] [int] IDENTITY(2000000,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[EventDateTime] [datetime] NOT NULL,
	[WhoDidIt] [varchar](10) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Notes]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Notes](
	[ID] [int] IDENTITY(3000,1) NOT NULL,
	[NoteDate] [varchar](8) NOT NULL,
	[NoteType] [int] NOT NULL,
	[NoteDescription] [varchar](255) NOT NULL,
	[NoteAmount] [int] NOT NULL,
	[NoteGUID] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbl_Notes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_NotesDetail]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_NotesDetail](
	[ID] [int] IDENTITY(7000,1) NOT NULL,
	[NoteGUID] [varchar](50) NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[ItemQty] [int] NOT NULL,
	[ItemPrice] [int] NOT NULL,
	[ItemTotal] [int] NOT NULL,
 CONSTRAINT [PK_tbl_NotesDetail] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_OpenCashDrawerRequest]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_OpenCashDrawerRequest](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[WhoOpen] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_OpenTickets]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_OpenTickets](
	[ID] [int] NOT NULL,
	[Type] [int] NOT NULL,
	[CustomerID] [varchar](50) NOT NULL,
	[BirthDay] [varchar](4) NULL,
	[Active] [bit] NULL,
	[ApplyServiceFee] [bit] NULL,
	[LastPayment] [varchar](8) NOT NULL,
	[FreeOfCharge] [bit] NULL,
 CONSTRAINT [PK_OpenTickets_CustomerID] PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Payments]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Payments](
	[ID] [int] IDENTITY(5000,1) NOT NULL,
	[PaymentDate] [varchar](8) NOT NULL,
	[Splited] [int] NOT NULL,
	[RandomRef] [varchar](10) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[TicketID] [int] NOT NULL,
	[CurTotalPrice] [int] NOT NULL,
	[PaymentAmount] [int] NOT NULL,
	[Cash] [int] NOT NULL,
	[CreditCard] [int] NOT NULL,
	[Transfer] [int] NOT NULL,
	[NewTotalPrice] [int] NOT NULL,
	[WhoClosed] [int] NOT NULL,
	[Shift] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_PayMethodChange]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PayMethodChange](
	[ID] [int] IDENTITY(10000,1) NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[TicketID] [int] NOT NULL,
	[OrigCash] [int] NOT NULL,
	[OrigCreditCard] [int] NOT NULL,
	[OrigTransfer] [int] NOT NULL,
	[CurrCash] [int] NOT NULL,
	[CurrCreditCard] [int] NOT NULL,
	[CurrTransfer] [int] NOT NULL,
	[WhoDidIt] [varchar](10) NOT NULL,
	[MadeItAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_TicketDate]    Script Date: 20/9/2025 09:56:28 ******/
CREATE CLUSTERED INDEX [IDX_TicketDate] ON [dbo].[tbl_PayMethodChange]
(
	[TicketDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Prefixes]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Prefixes](
	[ID] [int] IDENTITY(100,1) NOT NULL,
	[Type] [int] NOT NULL,
	[Prefix] [varchar](20) NOT NULL,
	[Hits] [int] NULL,
	[LastUpdate] [datetime] NULL,
 CONSTRAINT [PK_tbl_Prefixes] PRIMARY KEY CLUSTERED 
(
	[Type] ASC,
	[Prefix] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_PrintTicketRemotely]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PrintTicketRemotely](
	[GUID] [varchar](50) NOT NULL,
	[TicketForDataGrid] [varchar](5000) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_PromoConfig]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_PromoConfig](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[PromoType] [int] NOT NULL,
	[PromoID] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[Qty] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Providers]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Providers](
	[ID] [int] IDENTITY(100,1) NOT NULL,
	[ProviderName] [varchar](50) NOT NULL,
	[BusinessAddress] [varchar](100) NULL,
	[eMailAddress] [varchar](100) NULL,
	[PaymentMethod] [varchar](50) NULL,
	[PhoneNumber] [varchar](9) NULL,
	[CellularNumber] [varchar](9) NULL,
	[Remarks] [varchar](50) NULL,
 CONSTRAINT [PK_tbl_Providers_1] PRIMARY KEY CLUSTERED 
(
	[ProviderName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_SalaryAdvances]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_SalaryAdvances](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[Requester] [varchar](50) NOT NULL,
	[Approver] [int] NOT NULL,
	[Amount] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Tickets]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Tickets](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[TotalPrice] [int] NOT NULL,
	[ServiceFee] [int] NULL,
	[IVAFee] [int] NULL,
	[Payments] [int] NULL,
	[Cash] [int] NULL,
	[CreditCard] [int] NULL,
	[Transfer] [int] NULL,
	[Voucher] [int] NULL,
	[CashLoan] [int] NULL,
	[CreateAt] [datetime] NOT NULL,
	[CloseAt] [datetime] NULL,
	[PayMethod] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[WhoOpened] [int] NULL,
	[WhoClosed] [int] NULL,
	[Splited] [bit] NOT NULL,
	[customerAKA] [varchar](50) NULL,
	[ApplyServiceFee] [bit] NOT NULL,
	[AbortReason] [varchar](255) NULL,
	[Shift] [int] NOT NULL,
	[ATVStatusCode] [int] NOT NULL,
	[ATVInternalID] [int] NOT NULL,
	[ATVConsecutive] [varchar](50) NOT NULL,
	[ATVKey] [varchar](255) NOT NULL,
	[ATVStateMsj] [varchar](50) NOT NULL,
	[ATVErrorMsj] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbl_Tickets] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsAborted]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsAborted](
	[ID] [int] NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[CustomerID] [int] NOT NULL,
	[TotalPrice] [int] NOT NULL,
	[ServiceFee] [int] NULL,
	[IVAFee] [int] NULL,
	[Payments] [int] NULL,
	[Cash] [int] NULL,
	[CreditCard] [int] NULL,
	[Transfer] [int] NULL,
	[Voucher] [int] NULL,
	[CashLoan] [int] NULL,
	[CreateAt] [datetime] NOT NULL,
	[CloseAt] [datetime] NULL,
	[PayMethod] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[WhoOpened] [int] NULL,
	[WhoClosed] [int] NULL,
	[Splited] [bit] NOT NULL,
	[customerAKA] [varchar](50) NULL,
	[ApplyServiceFee] [bit] NOT NULL,
	[AbortReason] [varchar](255) NULL,
	[Shift] [int] NOT NULL,
	[ATVStatusCode] [int] NOT NULL,
	[ATVInternalID] [int] NOT NULL,
	[ATVConsecutive] [varchar](50) NOT NULL,
	[ATVKey] [varchar](255) NOT NULL,
	[ATVStateMsj] [varchar](50) NOT NULL,
	[ATVErrorMsj] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbl_TicketsAborted] PRIMARY KEY CLUSTERED 
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsDetail]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsDetail](
	[ID] [int] IDENTITY(10000,1) NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[Qty] [int] NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[UnitPrice] [int] NOT NULL,
	[UnitCost] [int] NOT NULL,
	[TotalPrice] [int] NOT NULL,
	[TotalCost] [int] NOT NULL,
	[CreatedAt] [varchar](50) NOT NULL,
	[WhoUpdated] [int] NOT NULL,
	[Splited] [bit] NOT NULL,
	[Remarks] [varchar](512) NOT NULL,
	[GUIDBucket] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_ByGUID]    Script Date: 20/9/2025 09:56:28 ******/
CREATE CLUSTERED INDEX [IDX_ByGUID] ON [dbo].[tbl_TicketsDetail]
(
	[GUID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsDetailAborted]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsDetailAborted](
	[ID] [int] NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[Qty] [int] NOT NULL,
	[ItemType] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[UnitPrice] [int] NOT NULL,
	[UnitCost] [int] NOT NULL,
	[TotalPrice] [int] NOT NULL,
	[TotalCost] [int] NOT NULL,
	[CreatedAt] [varchar](50) NOT NULL,
	[WhoUpdated] [int] NOT NULL,
	[Splited] [bit] NOT NULL,
	[Remarks] [varchar](512) NOT NULL,
	[GUIDBucket] [varchar](50) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsInherited]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsInherited](
	[ID] [int] IDENTITY(20000,1) NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[TicketID] [int] NOT NULL,
	[TicketGUID] [varchar](50) NOT NULL,
	[FromCustomer] [varchar](50) NOT NULL,
	[ToCustomer] [varchar](50) NOT NULL,
	[WhoMakeIt] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_TicketsInherited] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsInheritedDetail]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsInheritedDetail](
	[ID] [int] IDENTITY(30000,1) NOT NULL,
	[GUID] [varchar](50) NOT NULL,
	[Qty] [int] NOT NULL,
	[ItemID] [int] NOT NULL,
	[UnitPrice] [int] NOT NULL,
	[TotalPrice] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsModified]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsModified](
	[ID] [int] NOT NULL,
	[origTicketDate] [varchar](8) NOT NULL,
	[origCustomerID] [int] NOT NULL,
	[origTotalPrice] [int] NOT NULL,
	[origServiceFee] [int] NOT NULL,
	[origPayments] [int] NOT NULL,
	[origCash] [int] NOT NULL,
	[origCreditCard] [int] NOT NULL,
	[origTransfer] [int] NOT NULL,
	[origVoucher] [int] NOT NULL,
	[origCreatedAt] [varchar](50) NOT NULL,
	[modTicketDate] [varchar](8) NOT NULL,
	[modCustomerID] [int] NOT NULL,
	[modTotalPrice] [int] NOT NULL,
	[modServiceFee] [int] NOT NULL,
	[modPayments] [int] NOT NULL,
	[modCash] [int] NOT NULL,
	[modCreditCard] [int] NOT NULL,
	[modTransfer] [int] NOT NULL,
	[modVoucher] [int] NOT NULL,
	[modCreatedAt] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsOldCancelled]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsOldCancelled](
	[ID] [int] IDENTITY(10001,1) NOT NULL,
	[PayDate] [varchar](8) NOT NULL,
	[Splited] [int] NOT NULL,
	[TicketID] [int] NOT NULL,
	[TotalPrice] [int] NOT NULL,
	[Shift] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsProforms]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsProforms](
	[ID] [int] IDENTITY(1000,1) NOT NULL,
	[TicketNumber] [int] NOT NULL,
	[TicketDetailID] [int] NOT NULL,
	[CustomerAKA] [varchar](50) NOT NULL,
	[ItemID] [int] NOT NULL,
	[Qty] [int] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_TicketsReassigned]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_TicketsReassigned](
	[ID] [int] IDENTITY(30000,1) NOT NULL,
	[TicketDate] [varchar](8) NOT NULL,
	[TicketID] [int] NOT NULL,
	[FromCustomer] [varchar](50) NOT NULL,
	[ToCustomer] [varchar](50) NOT NULL,
	[WhoMakeit] [varchar](50) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
 CONSTRAINT [PK_tbl_TicketsReassigned] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Timecards]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Timecards](
	[BusinessDate] [varchar](8) NULL,
	[userPIN] [varchar](10) NOT NULL,
	[EventType] [bit] NOT NULL,
	[EventDatetime] [datetime] NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Users]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Users](
	[userID] [int] IDENTITY(100,1) NOT NULL,
	[userDTCreation] [datetime] NOT NULL,
	[userPIN] [varchar](10) NOT NULL,
	[userPW] [varchar](10) NOT NULL,
	[userName] [varchar](50) NOT NULL,
	[userAccessLevel] [varchar](20) NOT NULL,
	[userActive] [bit] NOT NULL,
	[userSecurityProfile] [varchar](100) NOT NULL,
	[userPowerAdmin] [bit] NOT NULL,
	[userFingerprint] [varchar](max) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[tbl_Vouchers]    Script Date: 20/9/2025 09:56:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbl_Vouchers](
	[ID] [int] IDENTITY(10000,1) NOT NULL,
	[BusinessDate] [varchar](8) NOT NULL,
	[IssueBy] [varchar](10) NOT NULL,
	[Amount] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
	[ExpireAt] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tbl_Vouchers] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_BusinessDate]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_BusinessDate] ON [dbo].[tbl_Advancements]
(
	[BusinessDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_All]    Script Date: 20/9/2025 09:56:28 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IDX_All] ON [dbo].[tbl_DailyClosing]
(
	[WorkDay] ASC,
	[CustomerID] ASC,
	[TicketNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_TicketNumber]    Script Date: 20/9/2025 09:56:28 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IDX_TicketNumber] ON [dbo].[tbl_DailyClosing]
(
	[TicketNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_TicketDate_WhoOrder]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_TicketDate_WhoOrder] ON [dbo].[tbl_ItemsOrders]
(
	[TicketDate] ASC,
	[WhoOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_ByCustomerID]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_ByCustomerID] ON [dbo].[tbl_Tickets]
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_ByID]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_ByID] ON [dbo].[tbl_Tickets]
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_ByTicketDate]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_ByTicketDate] ON [dbo].[tbl_Tickets]
(
	[TicketDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IDX_ByItemID]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_ByItemID] ON [dbo].[tbl_TicketsDetail]
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_TicketDate]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_TicketDate] ON [dbo].[tbl_TicketsInherited]
(
	[TicketDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_TicketDate]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_TicketDate] ON [dbo].[tbl_TicketsReassigned]
(
	[TicketDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IDX_BusinessDate]    Script Date: 20/9/2025 09:56:28 ******/
CREATE NONCLUSTERED INDEX [IDX_BusinessDate] ON [dbo].[tbl_Vouchers]
(
	[BusinessDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[tbl_Advancements] ADD  CONSTRAINT [DF_tbl_Advancements_Amount]  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[tbl_Advancements] ADD  CONSTRAINT [DF_tbl_Advancements_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_BartenderOrder] ADD  CONSTRAINT [DF_tbl_BartenderOrder_TicketNumber]  DEFAULT ((0)) FOR [CustomerID]
GO
ALTER TABLE [dbo].[tbl_BartenderOrder] ADD  CONSTRAINT [DF_Table_1_Content]  DEFAULT ('') FOR [BeveragesList]
GO
ALTER TABLE [dbo].[tbl_BucketsConfig] ADD  CONSTRAINT [DF_tbl_BucketsConfig_ItemID]  DEFAULT ((0)) FOR [ParentID]
GO
ALTER TABLE [dbo].[tbl_BucketsConfig] ADD  CONSTRAINT [DF_tbl_BucketsConfig_ItemsList]  DEFAULT ((0)) FOR [ChildID]
GO
ALTER TABLE [dbo].[tbl_BucketsDetail] ADD  CONSTRAINT [DF_tbl_BucketsDetail_TicketNumber]  DEFAULT ((0)) FOR [TicketNumber]
GO
ALTER TABLE [dbo].[tbl_BucketsDetail] ADD  CONSTRAINT [DF_tbl_BucketsDetail_GUID]  DEFAULT ('') FOR [GUID]
GO
ALTER TABLE [dbo].[tbl_BucketsDetail] ADD  CONSTRAINT [DF_tbl_BucketsDetail_BucketItemsList]  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[tbl_BucketsDetail] ADD  CONSTRAINT [DF_tbl_BucketsDetail_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[tbl_CashIncomes] ADD  CONSTRAINT [DF_tbl_CashIncomes_BusinessDate]  DEFAULT (getdate()) FOR [BusinessDate]
GO
ALTER TABLE [dbo].[tbl_CashIncomes] ADD  CONSTRAINT [DF_tbl_CashIncomes_Shift]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_CashIncomes] ADD  CONSTRAINT [DF_tbl_CashIncomes_IncomeDescription]  DEFAULT ('') FOR [IncomeDescription]
GO
ALTER TABLE [dbo].[tbl_CashIncomes] ADD  CONSTRAINT [DF_tbl_CashIncomes_IncomeAmount]  DEFAULT ((0)) FOR [IncomeAmount]
GO
ALTER TABLE [dbo].[tbl_CashIncomes] ADD  CONSTRAINT [DF_tbl_CashIncomes_WhoDidIt]  DEFAULT ('') FOR [WhoDidIt]
GO
ALTER TABLE [dbo].[tbl_CashIncomes] ADD  CONSTRAINT [DF_tbl_CashIncomes_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_CashOnDrawer] ADD  CONSTRAINT [DF_tbl_CashOnDrawer_BusinessDate]  DEFAULT ('') FOR [BusinessDate]
GO
ALTER TABLE [dbo].[tbl_CashOnDrawer] ADD  CONSTRAINT [DF_tbl_CashOnDrawer_Shift]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_CashOnDrawer] ADD  CONSTRAINT [DF_tbl_CashOnDrawer_CashAvailable]  DEFAULT ((0)) FOR [CashAvailable]
GO
ALTER TABLE [dbo].[tbl_CashOnDrawer] ADD  CONSTRAINT [DF_tbl_CashOnDrawer_CashWithdrawal]  DEFAULT ((0)) FOR [CashWithdrawal]
GO
ALTER TABLE [dbo].[tbl_CashOnDrawer] ADD  CONSTRAINT [DF_tbl_CashOnDrawer_CashRemaining]  DEFAULT ((0)) FOR [CashRemaining]
GO
ALTER TABLE [dbo].[tbl_CashOnDrawer] ADD  CONSTRAINT [DF_tbl_CashOnDrawer_WhoDidIt]  DEFAULT ('') FOR [WhoDidIt]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_SubType]  DEFAULT ((0)) FOR [SubType]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_ApplyServiceFee]  DEFAULT ((0)) FOR [ApplyServiceFee]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_customerFOC]  DEFAULT ((0)) FOR [FreeOfCharge]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_CreditLimit]  DEFAULT ((0)) FOR [CreditLimit]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_BirthDay]  DEFAULT ('') FOR [BirthDay]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_MailAddress]  DEFAULT ('') FOR [MailAddress]
GO
ALTER TABLE [dbo].[tbl_CustomerID] ADD  CONSTRAINT [DF_tbl_CustomerID_LotaltyPoints]  DEFAULT ((0)) FOR [LoyaltyPoints]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Busin__68D28DBC]  DEFAULT ('') FOR [BusinessDate]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Gross__69C6B1F5]  DEFAULT ((0)) FOR [GrossSales]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__NetSa__6ABAD62E]  DEFAULT ((0)) FOR [NetSales]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Sales__6BAEFA67]  DEFAULT ((0)) FOR [Sales_Cash]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Sales__6CA31EA0]  DEFAULT ((0)) FOR [Sales_CreditCard]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Sales__6D9742D9]  DEFAULT ((0)) FOR [Sales_Transfer]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Sales__6E8B6712]  DEFAULT ((0)) FOR [Sales_Voucher]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Drawe__6F7F8B4B]  DEFAULT ((0)) FOR [Drawer_Cash]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Drawe__7073AF84]  DEFAULT ((0)) FOR [Drawer_CreditCard]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Drawe__7167D3BD]  DEFAULT ((0)) FOR [Drawer_Transfer]
GO
ALTER TABLE [dbo].[tbl_DailyAccountantReport] ADD  CONSTRAINT [DF__tbl_Daily__Drawe__725BF7F6]  DEFAULT ((0)) FOR [Drawer_Voucher]
GO
ALTER TABLE [dbo].[tbl_DailyClosing] ADD  CONSTRAINT [DF_tbl_DailyClosing_CustomerAKA]  DEFAULT ('') FOR [CustomerAKA]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_Shift]  DEFAULT ((0)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_InitialCash]  DEFAULT ((0)) FOR [InitialCash]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_IncomeCash]  DEFAULT ((0)) FOR [IncomeCash]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_Cash]  DEFAULT ((0)) FOR [Cash]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_CashByOperator]  DEFAULT ((0)) FOR [CashByOperator]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_CreditCard]  DEFAULT ((0)) FOR [CreditCard]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_CreditCardByOperator]  DEFAULT ((0)) FOR [CreditCardByOperator]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_TransferByOperator]  DEFAULT ((0)) FOR [TransferByOperator]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_Voucher]  DEFAULT ((0)) FOR [Voucher]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_AccountsReceivable]  DEFAULT ((0)) FOR [AccountsReceivable]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_FeeService]  DEFAULT ((0)) FOR [ServiceFee]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_GeneralExpenses]  DEFAULT ((0)) FOR [GeneralExpenses]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_GrossSale]  DEFAULT ((0)) FOR [GrossSale]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_NetSale]  DEFAULT ((0)) FOR [NetSale]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_TotalCashInDrawer]  DEFAULT ((0)) FOR [TotalCashInDrawer]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_DailyClosingMatch]  DEFAULT ((0)) FOR [DailyClosingMatch]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_WhoDidIt]  DEFAULT ('NA') FOR [WhoDidIt]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_DailyClosingSummary] ADD  CONSTRAINT [DF_tbl_DailyClosingSummary_Splited]  DEFAULT ((1)) FOR [Splited]
GO
ALTER TABLE [dbo].[tbl_Expenses] ADD  CONSTRAINT [DF_tbl_Expenses_Shift]  DEFAULT ((0)) FOR [Splited]
GO
ALTER TABLE [dbo].[tbl_Expenses] ADD  CONSTRAINT [DF_tbl_Expenses_ExpenseDescription]  DEFAULT ((0)) FOR [ExpenseDescription]
GO
ALTER TABLE [dbo].[tbl_Expenses] ADD  CONSTRAINT [DF_tbl_Expenses_Shift_1]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_InternalOrders] ADD  CONSTRAINT [DF_tbl_InternalOrders_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_InternalOrdersDetail] ADD  CONSTRAINT [DF_tbl_InternalOrdersDetail_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemSubType]  DEFAULT ((0)) FOR [ItemSubType]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_IitemCABYS]  DEFAULT ('') FOR [IitemCABYS]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_Active]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_IsContainer]  DEFAULT ((0)) FOR [IsContainer]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_HideInMenu]  DEFAULT ((0)) FOR [HideInMenu]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_UnitPrice]  DEFAULT ((0)) FOR [UnitPrice]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_UnitCost]  DEFAULT ((0)) FOR [UnitCost]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemAvailable]  DEFAULT ((0)) FOR [ItemAvailable]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemSold]  DEFAULT ((0)) FOR [ItemSold]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemDefective]  DEFAULT ((0)) FOR [ItemDefective]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemParent]  DEFAULT ((0)) FOR [ItemParent]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemParentUnit]  DEFAULT ((0)) FOR [ItemParentUnit]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemAvailableMinimun]  DEFAULT ((0)) FOR [ItemMinimum]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF__tbl_Items__ItemU__4CA06362]  DEFAULT ((0)) FOR [ItemUnitOfMeasurement]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF__tbl_Items__ItemU__4D94879B]  DEFAULT ((1)) FOR [ItemUnitSize]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_ItemStock]  DEFAULT ((0)) FOR [ItemStock]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_DebitNotes]  DEFAULT ((0)) FOR [DebitNotes]
GO
ALTER TABLE [dbo].[tbl_Items] ADD  CONSTRAINT [DF_tbl_Items_CreditNotes]  DEFAULT ((0)) FOR [CreditNotes]
GO
ALTER TABLE [dbo].[tbl_ItemsChangePrice] ADD  CONSTRAINT [DF_tbl_ItemsChangePrice_MadeItAt]  DEFAULT (getdate()) FOR [MadeItAt]
GO
ALTER TABLE [dbo].[tbl_ItemsDeleted] ADD  CONSTRAINT [DF_tbl_ItemsDeleted_TicketDate]  DEFAULT ('') FOR [TicketDate]
GO
ALTER TABLE [dbo].[tbl_ItemsDeleted] ADD  CONSTRAINT [DF_tbl_ItemsDeleted_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[tbl_ItemsDeleted] ADD  CONSTRAINT [DF_tbl_ItemsDeleted_WhoAuth]  DEFAULT ('') FOR [WhoAuth]
GO
ALTER TABLE [dbo].[tbl_ItemsDeleted] ADD  CONSTRAINT [DF_tbl_ItemsDeleted_DeletedAt]  DEFAULT (getdate()) FOR [DeletedAt]
GO
ALTER TABLE [dbo].[tbl_ItemsDeletedFromSystem] ADD  CONSTRAINT [DF_tbl_ItemsDeletedFromSystem_TicketDate]  DEFAULT ('') FOR [TicketDate]
GO
ALTER TABLE [dbo].[tbl_ItemsDeletedFromSystem] ADD  CONSTRAINT [DF_tbl_ItemsDeletedFromSystem_WhoAuth]  DEFAULT ('') FOR [WhoDeleted]
GO
ALTER TABLE [dbo].[tbl_ItemsDeletedFromSystem] ADD  CONSTRAINT [DF_tbl_ItemsDeletedFromSystem_DeletedAt]  DEFAULT (getdate()) FOR [DeletedAt]
GO
ALTER TABLE [dbo].[tbl_LoyaltyRewards] ADD  CONSTRAINT [DF_tbl_Loyalty_Status]  DEFAULT ('A') FOR [Status]
GO
ALTER TABLE [dbo].[tbl_LoyaltyRewards] ADD  CONSTRAINT [DF_tbl_LoyaltyRewards_TimeForReward]  DEFAULT ((0)) FOR [MaxDaysForReward]
GO
ALTER TABLE [dbo].[tbl_LoyaltyRewards] ADD  CONSTRAINT [DF_tbl_LoyaltyRewards_TotalItemsAwarded]  DEFAULT ((0)) FOR [TotalItemsAwarded]
GO
ALTER TABLE [dbo].[tbl_MealsRelationships] ADD  CONSTRAINT [DF_tbl_MealsRelationships_ItemType]  DEFAULT ((0)) FOR [ItemType]
GO
ALTER TABLE [dbo].[tbl_MealsRelationships] ADD  CONSTRAINT [DF_tbl_MealsRelationships_ItemFrom]  DEFAULT ((0)) FOR [ItemFrom]
GO
ALTER TABLE [dbo].[tbl_MealsRelationships] ADD  CONSTRAINT [DF_tbl_MealsRelationships_ItemTo]  DEFAULT ((0)) FOR [ItemTo]
GO
ALTER TABLE [dbo].[tbl_MealsRelationships] ADD  CONSTRAINT [DF_tbl_MealsRelationships_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[tbl_MealsRelationships] ADD  CONSTRAINT [DF_tbl_MealsRelationships_IsActive]  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[tbl_MoneyDrawerLog] ADD  CONSTRAINT [DF_tbl_MoneyDrawerLog_EventDateTime]  DEFAULT (getdate()) FOR [EventDateTime]
GO
ALTER TABLE [dbo].[tbl_MoneyDrawerLog] ADD  CONSTRAINT [DF_tbl_MoneyDrawerLog_WhoDidIt]  DEFAULT ('') FOR [WhoDidIt]
GO
ALTER TABLE [dbo].[tbl_OpenCashDrawerRequest] ADD  CONSTRAINT [DF_tbl_OpenCashDrawerRequest_BusinessDate]  DEFAULT ('') FOR [BusinessDate]
GO
ALTER TABLE [dbo].[tbl_OpenCashDrawerRequest] ADD  CONSTRAINT [DF_tbl_OpenCashDrawerRequest_WhoOpen]  DEFAULT ((0)) FOR [WhoOpen]
GO
ALTER TABLE [dbo].[tbl_OpenCashDrawerRequest] ADD  CONSTRAINT [DF_tbl_OpenCashDrawerRequest_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_OpenTickets] ADD  CONSTRAINT [DF__tbl_OpenT__Birth__1AD3FDA4]  DEFAULT ('0000') FOR [BirthDay]
GO
ALTER TABLE [dbo].[tbl_OpenTickets] ADD  CONSTRAINT [DF__tbl_OpenT__Activ__1BC821DD]  DEFAULT ((0)) FOR [Active]
GO
ALTER TABLE [dbo].[tbl_OpenTickets] ADD  CONSTRAINT [DF__tbl_OpenT__Apply__1CBC4616]  DEFAULT ((0)) FOR [ApplyServiceFee]
GO
ALTER TABLE [dbo].[tbl_OpenTickets] ADD  CONSTRAINT [DF__tbl_OpenT__FreeO__1DB06A4F]  DEFAULT ((0)) FOR [FreeOfCharge]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_PaymentDate]  DEFAULT ('') FOR [PaymentDate]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_Shift]  DEFAULT ((0)) FOR [Splited]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_CustomerID]  DEFAULT ((0)) FOR [CustomerID]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_TicketID]  DEFAULT ((0)) FOR [TicketID]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_CurTotalPrice]  DEFAULT ((0)) FOR [CurTotalPrice]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_PaymentAmount]  DEFAULT ((0)) FOR [PaymentAmount]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_Cash]  DEFAULT ((0)) FOR [Cash]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_CreditCard]  DEFAULT ((0)) FOR [CreditCard]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_Transfer]  DEFAULT ((0)) FOR [Transfer]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_NewTotalPrice]  DEFAULT ((0)) FOR [NewTotalPrice]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_WhoClosed]  DEFAULT ((0)) FOR [WhoClosed]
GO
ALTER TABLE [dbo].[tbl_Payments] ADD  CONSTRAINT [DF_tbl_Payments_Shift_1]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_TicketDate]  DEFAULT ('') FOR [TicketDate]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_TicketID]  DEFAULT ((0)) FOR [TicketID]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_OrigCash]  DEFAULT ((0)) FOR [OrigCash]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_OrigCreditCard]  DEFAULT ((0)) FOR [OrigCreditCard]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_OrigTransfer]  DEFAULT ((0)) FOR [OrigTransfer]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_CurrCash]  DEFAULT ((0)) FOR [CurrCash]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_WhoDidIt]  DEFAULT ('') FOR [WhoDidIt]
GO
ALTER TABLE [dbo].[tbl_PayMethodChange] ADD  CONSTRAINT [DF_tbl_PayMethodChange_MadeItAt]  DEFAULT (getdate()) FOR [MadeItAt]
GO
ALTER TABLE [dbo].[tbl_Prefixes] ADD  CONSTRAINT [DF_tbl_Prefixes_Hits]  DEFAULT ((1)) FOR [Hits]
GO
ALTER TABLE [dbo].[tbl_Prefixes] ADD  CONSTRAINT [DF_tbl_Prefixes_LastUpdate]  DEFAULT (getdate()) FOR [LastUpdate]
GO
ALTER TABLE [dbo].[tbl_PromoConfig] ADD  CONSTRAINT [DF_tbl_PromoConfig_Type]  DEFAULT ((1)) FOR [PromoType]
GO
ALTER TABLE [dbo].[tbl_PromoConfig] ADD  CONSTRAINT [DF_tbl_PromoConfig_PromoID]  DEFAULT ((0)) FOR [PromoID]
GO
ALTER TABLE [dbo].[tbl_PromoConfig] ADD  CONSTRAINT [DF_tbl_PromoConfig_ItemID]  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[tbl_PromoConfig] ADD  CONSTRAINT [DF_tbl_PromoConfig_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[tbl_SalaryAdvances] ADD  CONSTRAINT [DF_tbl_SalaryAdvances_BusinessDate]  DEFAULT ('') FOR [BusinessDate]
GO
ALTER TABLE [dbo].[tbl_SalaryAdvances] ADD  CONSTRAINT [DF_tbl_SalaryAdvances_Requester]  DEFAULT ('') FOR [Requester]
GO
ALTER TABLE [dbo].[tbl_SalaryAdvances] ADD  CONSTRAINT [DF_tbl_SalaryAdvances_Approver]  DEFAULT ((0)) FOR [Approver]
GO
ALTER TABLE [dbo].[tbl_SalaryAdvances] ADD  CONSTRAINT [DF_tbl_SalaryAdvances_Amount]  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[tbl_SalaryAdvances] ADD  CONSTRAINT [DF_tbl_SalaryAdvances_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ServiceFee]  DEFAULT ((0)) FOR [ServiceFee]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_IVAFee]  DEFAULT ((0)) FOR [IVAFee]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_Payments]  DEFAULT ((0)) FOR [Payments]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_Cash]  DEFAULT ((0)) FOR [Cash]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_CreditCard]  DEFAULT ((0)) FOR [CreditCard]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_Transfer]  DEFAULT ((0)) FOR [Transfer]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_Voucher]  DEFAULT ((0)) FOR [Voucher]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_CashLoan]  DEFAULT ((0)) FOR [CashLoan]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_CreateAt]  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_Splited]  DEFAULT ((0)) FOR [Splited]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_customerAKA]  DEFAULT ('ND') FOR [customerAKA]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ApplyServiceFee]  DEFAULT ((0)) FOR [ApplyServiceFee]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_AbortReason]  DEFAULT ('') FOR [AbortReason]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_Shift]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ATV]  DEFAULT ((0)) FOR [ATVStatusCode]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ATVInternalID]  DEFAULT ((0)) FOR [ATVInternalID]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ATVConsecutivo]  DEFAULT ('') FOR [ATVConsecutive]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ATVClase]  DEFAULT ('') FOR [ATVKey]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ATVEstado]  DEFAULT ('') FOR [ATVStateMsj]
GO
ALTER TABLE [dbo].[tbl_Tickets] ADD  CONSTRAINT [DF_tbl_Tickets_ATVErrorMsj]  DEFAULT ('') FOR [ATVErrorMsj]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ServiceFee]  DEFAULT ((0)) FOR [ServiceFee]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_IVAFee]  DEFAULT ((0)) FOR [IVAFee]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_Payments]  DEFAULT ((0)) FOR [Payments]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_Cash]  DEFAULT ((0)) FOR [Cash]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_CreditCard]  DEFAULT ((0)) FOR [CreditCard]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_Transfer]  DEFAULT ((0)) FOR [Transfer]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_CashLoan]  DEFAULT ((0)) FOR [CashLoan]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_CreateAt]  DEFAULT (getdate()) FOR [CreateAt]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_Splited]  DEFAULT ((0)) FOR [Splited]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_customerAKA]  DEFAULT ('ND') FOR [customerAKA]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ApplyServiceFee]  DEFAULT ((0)) FOR [ApplyServiceFee]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_AbortReason]  DEFAULT ('') FOR [AbortReason]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_Shift]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ATV]  DEFAULT ((0)) FOR [ATVStatusCode]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ATVInternalID]  DEFAULT ((0)) FOR [ATVInternalID]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ATVConsecutivo]  DEFAULT ('') FOR [ATVConsecutive]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ATVClase]  DEFAULT ('') FOR [ATVKey]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ATVEstado]  DEFAULT ('') FOR [ATVStateMsj]
GO
ALTER TABLE [dbo].[tbl_TicketsAborted] ADD  CONSTRAINT [DF_tbl_TicketsAborted_ATVErrorMsj]  DEFAULT ('') FOR [ATVErrorMsj]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_ItemType]  DEFAULT ((1)) FOR [ItemType]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_UnitCost]  DEFAULT ((0)) FOR [UnitCost]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_TotalCost]  DEFAULT ((0)) FOR [TotalCost]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_WhoUpdated]  DEFAULT ('') FOR [WhoUpdated]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_Remarks]  DEFAULT ('') FOR [Remarks]
GO
ALTER TABLE [dbo].[tbl_TicketsDetail] ADD  CONSTRAINT [DF_tbl_TicketsDetail_GUIDBucket]  DEFAULT ('') FOR [GUIDBucket]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_ItemType]  DEFAULT ((0)) FOR [ItemType]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_UnitCost]  DEFAULT ((0)) FOR [UnitCost]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_TotalCost]  DEFAULT ((0)) FOR [TotalCost]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_WhoUpdated]  DEFAULT ('') FOR [WhoUpdated]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_Remarks]  DEFAULT ('') FOR [Remarks]
GO
ALTER TABLE [dbo].[tbl_TicketsDetailAborted] ADD  CONSTRAINT [DF_tbl_TicketsDetailAborted_GUIDBucket]  DEFAULT ('') FOR [GUIDBucket]
GO
ALTER TABLE [dbo].[tbl_TicketsInherited] ADD  CONSTRAINT [DF_tbl_TicketsInherited_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_TicketsModified] ADD  CONSTRAINT [DF_tbl_TicketsModified_origVoucher]  DEFAULT ((0)) FOR [origVoucher]
GO
ALTER TABLE [dbo].[tbl_TicketsModified] ADD  CONSTRAINT [DF_tbl_TicketsModified_modVoucher]  DEFAULT ((0)) FOR [modVoucher]
GO
ALTER TABLE [dbo].[tbl_TicketsOldCancelled] ADD  CONSTRAINT [DF_tbl_TicketsOldCancelled_Splited]  DEFAULT ((0)) FOR [Splited]
GO
ALTER TABLE [dbo].[tbl_TicketsOldCancelled] ADD  CONSTRAINT [DF_tbl_TicketsOldCancelled_Shift]  DEFAULT ((1)) FOR [Shift]
GO
ALTER TABLE [dbo].[tbl_TicketsProforms] ADD  CONSTRAINT [DF_tbl_TicketsProforms_TicketNumber]  DEFAULT ((0)) FOR [TicketNumber]
GO
ALTER TABLE [dbo].[tbl_TicketsProforms] ADD  CONSTRAINT [DF_tbl_TicketsProforms_TicketDetailID]  DEFAULT ((0)) FOR [TicketDetailID]
GO
ALTER TABLE [dbo].[tbl_TicketsProforms] ADD  CONSTRAINT [DF_tbl_TicketsProforms_CustomerID]  DEFAULT ('') FOR [CustomerAKA]
GO
ALTER TABLE [dbo].[tbl_TicketsProforms] ADD  CONSTRAINT [DF_tbl_TicketsProforms_ItemID]  DEFAULT ((0)) FOR [ItemID]
GO
ALTER TABLE [dbo].[tbl_TicketsProforms] ADD  CONSTRAINT [DF_tbl_TicketsProforms_Qty]  DEFAULT ((0)) FOR [Qty]
GO
ALTER TABLE [dbo].[tbl_TicketsReassigned] ADD  CONSTRAINT [DF_tbl_TicketsReassigned_CreateAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_Timecards] ADD  CONSTRAINT [DF_tbl_Timecards_EventDatetime]  DEFAULT (getdate()) FOR [EventDatetime]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userDTCreation]  DEFAULT (getdate()) FOR [userDTCreation]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userPW]  DEFAULT ('') FOR [userPW]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userName]  DEFAULT ('') FOR [userName]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userAccessLevel]  DEFAULT ('') FOR [userAccessLevel]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userActive]  DEFAULT ((1)) FOR [userActive]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userSecurityProfile]  DEFAULT ('11111111111111111111111111111111111111111111111111') FOR [userSecurityProfile]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_PowerAdmin]  DEFAULT ((0)) FOR [userPowerAdmin]
GO
ALTER TABLE [dbo].[tbl_Users] ADD  CONSTRAINT [DF_tbl_Users_userFingerprint]  DEFAULT ('') FOR [userFingerprint]
GO
ALTER TABLE [dbo].[tbl_Vouchers] ADD  CONSTRAINT [DF_tbl_Vouchers_Amount]  DEFAULT ((0)) FOR [Amount]
GO
ALTER TABLE [dbo].[tbl_Vouchers] ADD  CONSTRAINT [DF_tbl_Vouchers_CreatedAt]  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[tbl_Vouchers] ADD  CONSTRAINT [DF_tbl_Vouchers_ExpireAt]  DEFAULT (getdate()) FOR [ExpireAt]
GO
