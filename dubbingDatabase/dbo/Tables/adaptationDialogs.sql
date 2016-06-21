CREATE TABLE [dbo].[adaptationDialogs]
(
	[dialogIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [dubbSheetHdrIntno] bigint NOT NULL,
	[sceneNo] smallint NOT NULL,
	dialogNo smallint NULL, 
    [startTimeCode] NVARCHAR(20) NULL, 
    [endTimeCode] NVARCHAR(20) NULL, 
    [isTaken] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_adaptationDialogs_To_dubbingSheetDtls] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dubbingSheetHdrs]([dubbSheetHdrIntno])	on delete cascade
)
