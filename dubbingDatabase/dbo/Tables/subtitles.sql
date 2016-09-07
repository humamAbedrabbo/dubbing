CREATE TABLE [dbo].[subtitles]
(
	[subtitleIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [dialogIntno] BIGINT NOT NULL, 
    [subtitleNo] SMALLINT NOT NULL, 
    [dubbSheetHdrIntno] BIGINT NOT NULL, 
    [scentence] NVARCHAR(500) NOT NULL, 
    [startTimeCode] NVARCHAR(20) NULL, 
    [endTimeCode] NVARCHAR(20) NULL, 
    [startSecond] BIGINT NOT NULL DEFAULT 0, 
    [endSecond] BIGINT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_subtitles_To_dialogs] FOREIGN KEY ([dialogIntno]) REFERENCES [dialogs]([dialogIntno]) on delete cascade, 
    CONSTRAINT [FK_subtitles_To_dubbingSheetHdrs] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dubbingSheetHdrs]([dubbSheetHdrIntno])
)
