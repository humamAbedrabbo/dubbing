CREATE TABLE [dbo].[dialogs]
(
	[dialogIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [sceneIntno] BIGINT NOT NULL,
	[dubbSheetHdrIntno] BIGINT NOT NULL,  
    [dialogNo] SMALLINT NOT NULL, 
    [startTimeCode] NVARCHAR(20) NULL, 
    [endTimeCode] NVARCHAR(20) NULL, 
    [isTaken] BIT NOT NULL DEFAULT 0, 
    [takenTimeStamp] DATETIME NULL, 
    [studioNo] NVARCHAR(50) NULL, 
    [supervisor] BIGINT NULL, 
    CONSTRAINT [FK_dialogs_To_scenes] FOREIGN KEY ([sceneIntno]) REFERENCES [scenes]([sceneIntno]) on delete cascade,
	CONSTRAINT [FK_dialogs_To_dubbingSheetHdrs] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dubbingSheetHdrs]([dubbSheetHdrIntno]) on delete cascade,
	CONSTRAINT [FK_dialogs_To_employees] FOREIGN KEY ([supervisor]) REFERENCES [employees]([empIntno]) on delete cascade
)
