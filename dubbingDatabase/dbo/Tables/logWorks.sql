CREATE TABLE [dbo].[logWorks]
(
	[logIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[clientIntno] [bigint] NOT NULL,
	[clientName] [nvarchar](50) NOT NULL,
	[workIntno] [bigint] NOT NULL,
	[workName] [nvarchar](50) NOT NULL,
	[workType] [nvarchar](50) NOT NULL,
	[workNationality] [nvarchar](50) NOT NULL,
	[contractedDate] [date] NULL,
	[contractedYear] [int] NULL,
	[contractedMonth] [int] NULL,
	[endorsedDate] [date] NULL,
	[endorsedYear] [int] NULL,
	[endorsedMonth] [int] NULL,
	[lastUpdate] [datetime] NOT NULL,
	[updatedBy] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_logWorks] PRIMARY KEY CLUSTERED ([logIntno] ASC)
 );