INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220814', 1440, 60692, N'MEMO GRILLO')
INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220815', 1440, 60718, N'MEMO GRILLO')
INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220816', 1440, 60747, N'MEMO GRILLO')
INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220817', 1440, 60777, N'MEMO GRILLO')
INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220818', 1440, 60800, N'MEMO GRILLO')
INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220819', 1440, 60863, N'MEMO GRILLO')
INSERT [dbo].[tbl_DailyClosing] ([WorkDay], [CustomerID], [TicketNumber], [CustomerAKA]) VALUES (N'20220820', 1440, 60958, N'MEMO GRILLO')

SELECT * FROM [tbl_DailyClosing] WHERE [CustomerID] = 1440

UPDATE tbl_Tickets SET STATUS = 1 WHERE ID in (60692,60718,60747,60777,60800,60863,60958)