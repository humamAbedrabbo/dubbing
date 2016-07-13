CREATE TABLE [dbo].[subtitles]
(
	[subtitleIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [dialogIntno] BIGINT NOT NULL, 
    [subtitleNo] SMALLINT NOT NULL, 
    [workCharacterIntno] BIGINT NULL, 
    [characterName] NVARCHAR(100) NULL, 
    [scentence] NVARCHAR(500) NOT NULL, 
    [startTimeCode] NVARCHAR(20) NULL, 
    [endTimeCode] NVARCHAR(20) NULL, 
    CONSTRAINT [FK_subtitles_To_dialogs] FOREIGN KEY ([dialogIntno]) REFERENCES [dialogs]([dialogIntno]) on delete cascade, 
    CONSTRAINT [FK_subtitles_To_workCharacters] FOREIGN KEY ([workCharacterIntno]) REFERENCES [workCharacters]([workCharacterIntno])
)
