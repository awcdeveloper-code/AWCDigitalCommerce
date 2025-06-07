USE [AWCDigitalCommerce]
GO
SET IDENTITY_INSERT [dbo].[tbl_CustomerID] ON 
GO
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1001, 3, 0, N'AWCDIGITALCOMMERCE', 0, 0, N'20230611', 0, 0, N'0', N'')
GO
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1002, 3, 0, N'CUENTA ELIMINADA', 0, 0, N'20230510', 0, 0, N'0', N'')
GO
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1003, 3, 0, N'CUENTA SEPARADA', 0, 0, N'20230510', 0, 0, N'0', N'')
GO
INSERT [dbo].[tbl_CustomerID] ([ID], [Type], [SubType], [CustomerID], [Active], [ApplyServiceFee], [LastPayment], [FreeOfCharge], [CreditLimit], [BirthDay], [MailAddress]) VALUES (1004, 3, 0, N'VENTA RÁPIDA', 0, 0, N'20230510', 0, 0, N'0', N'')
GO
SET IDENTITY_INSERT [dbo].[tbl_CustomerID] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Items] ON
GO
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2001, 9, 0, N'CREDITO (-)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
GO
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2002, 9, 0, N'CUENTA  PENDIENTE', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
GO
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2003, 9, 0, N'DESCUENTO (-)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
GO
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2004, 9, 0, N'EFECTIVO (+)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
GO
INSERT [dbo].[tbl_Items] ([ID], [ItemType], [ItemSubType], [ItemDescription], [UnitPrice], [UnitCost], [ItemAvailable], [ItemSold], [ItemDefective], [ItemParent], [ItemParentUnit], [ItemMinimum], [ItemUnitOfMeasurement], [ItemUnitSize], [ItemStock], [DebitNotes], [CreditNotes]) VALUES (2005, 9, 0, N'PAGO PARCIAL (-)', 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0)
GO
GO
SET IDENTITY_INSERT [dbo].[tbl_Items] OFF
GO
SET IDENTITY_INSERT [dbo].[tbl_Users] ON
INSERT [dbo].[tbl_Users] ([userID], [userDTCreation], [userPIN], [userPW], [userName], [userAccessLevel], [userActive], [userSecurityProfile], [userPowerAdmin]) VALUES (100, CAST(N'2024-09-01T07:43:39.457' AS DateTime), N'12345', N'12345', N'MEMO GRILLO', N'POWER ADMIN', 1, N'1111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111111', 1)
SET IDENTITY_INSERT [dbo].[tbl_Users] OFF
