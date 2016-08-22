CREATE TABLE [dbo].[logActors]
(
	[logIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[logYear] [int] NOT NULL,
	[logMonth] [int] NOT NULL,
	[clientIntno] [bigint] NOT NULL,
	[clientName] [nvarchar](50) NOT NULL,
	[workIntno] [bigint] NOT NULL,
	[workName] [nvarchar](50) NOT NULL,
	[workType] [nvarchar](50) NOT NULL,
	[totalMainActors] [int] NOT NULL DEFAULT 0,
	[totalMainActorsCharges] [int] NOT NULL DEFAULT 0,
	[highestMainActorCharges] [int] NOT NULL DEFAULT 0,
	[lowestMainActorCharges] [int] NOT NULL DEFAULT 0,
	[totalOtherActors] [int] NOT NULL DEFAULT 0,
	[totalOtherActorsCharges] [int] NOT NULL DEFAULT 0,
	[avgOtherActorCharges] [int] NOT NULL DEFAULT 0,
	[lastUpdate] [int] NOT NULL,
 CONSTRAINT [PK_logActors] PRIMARY KEY CLUSTERED ([logIntno] ASC)
);
