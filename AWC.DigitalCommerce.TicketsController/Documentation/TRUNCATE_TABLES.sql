USE AWCDigitalCommerce

truncate table tbl_ATV
truncate table tbl_BartenderOrder
truncate table tbl_BucketsConfig
truncate table tbl_BucketsDetail
truncate table tbl_CashIncomes
truncate table tbl_CashOnDrawer
truncate table tbl_Categories
truncate table tbl_CustomerID
truncate table tbl_DailyClosing
truncate table tbl_DailyClosingSummary
truncate table tbl_DigitalKeyboardFields
truncate table tbl_SalaryAdvances
truncate table tbl_Expenses
truncate table tbl_InternalOrders
truncate table tbl_InternalOrdersDetail
truncate table tbl_Invoices
truncate table tbl_InvoicesDetail
truncate table tbl_Items
truncate table tbl_ItemsChangePrice
truncate table tbl_ItemsDefective
truncate table tbl_ItemsDeleted
truncate table tbl_ItemsOrders
truncate table tbl_LoyaltyRewards
truncate table tbl_Lunches
truncate table tbl_MealsRelationships
truncate table tbl_MoneyDrawerLog
truncate table tbl_Notes
truncate table tbl_NotesDetail
truncate table tbl_OpenCashDrawerRequest
truncate table tbl_OpenTickets
truncate table tbl_Payments
truncate table tbl_PayMethodChange
truncate table tbl_Prefixes
truncate table tbl_PrintTicketRemotely
truncate table tbl_PromoConfig
truncate table tbl_Providers
truncate table tbl_Tickets
truncate table tbl_TicketsAborted
truncate table tbl_TicketsDetail
truncate table tbl_TicketsDetailAborted
truncate table tbl_TicketsInherited
truncate table tbl_TicketsInheritedDetail
truncate table tbl_TicketsModified
truncate table tbl_TicketsOldCancelled
truncate table tbl_TicketsProforms
truncate table tbl_TicketsReassigned
truncate table tbl_Timecards
truncate table tbl_Users
truncate table tbl_Vouchers

SET IDENTITY_INSERT [dbo].[tbl_CustomerID] ON 
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1001, 3, 0, N'AWCDIGITALCOMMERCE', 0, 0, N'20210430', 0, 0, N'0', N'')
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1002, 3, 0, N'CUENTA ELIMINADA', 0, 0, N'20210430', 0, 0, N'0', N'')
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1003, 3, 0, N'CUENTA SEPARADA', 0, 0, N'20210430', 0, 0, N'0', N'')
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1004, 3, 0, N'VENTA RÁPIDA', 0, 0, N'20210430', 0, 0, N'0', N'')
SET IDENTITY_INSERT [dbo].[tbl_CustomerID] OFF
GO

SET IDENTITY_INSERT [dbo].[tbl_Items] ON
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2001, 9, 0, N'CREDITO (-)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2002, 9, 0, N'CUENTA  PENDIENTE', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2003, 9, 0, N'DESCUENTO (-)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2004, 9, 0, N'EFECTIVO (+)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2005, 9, 0, N'PAGO PARCIAL (-)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
SET IDENTITY_INSERT [dbo].[tbl_Items] OFF

SET IDENTITY_INSERT [dbo].[tbl_Users] ON
INSERT [dbo].[tbl_Users] ([userID], [userDTCreation], [userPIN], [userPW], [userName], [userAccessLevel], [userActive], [userSecurityProfile], [userPowerAdmin]) VALUES (100, CAST(N'2024-09-01T07:43:39.457' AS DateTime), N'12345', N'12345', N'MEMO GRILLO', N'POWER ADMIN', 1, N'1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111', 1)
SET IDENTITY_INSERT [dbo].[tbl_Users] OFF
GO
