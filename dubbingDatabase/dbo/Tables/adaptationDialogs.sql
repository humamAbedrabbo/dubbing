CREATE TABLE [dbo].[adaptationDialogs]
(
	[dialogIntno] INT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [dubbSheetHdrIntno] bigint NOT NULL,
	[sceneNo] smallint NOT NULL,
	dialogNo smallint NULL, 
    CONSTRAINT [FK_adaptationDialogs_To_dubbingSheetDtls] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dubbingSheetHdrs]([dubbSheetHdrIntno])	on delete cascade
)
