CREATE TABLE [dbo].[adaptationSubtitles]
(
	[subtitleIntno] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	[dialogIntno] INT NOT NULL,
	[subtitleNo] smallint NULL,
	[scentence] nvarchar(500),
	[startTime] nvarchar(20),
	[endTime] nvarchar(20), 
    CONSTRAINT [FK_adaptationSubtitles_To_adaptationDialogs] FOREIGN KEY ([dialogIntno]) REFERENCES [adaptationDialogs]([dialogIntno]) on delete cascade
)
