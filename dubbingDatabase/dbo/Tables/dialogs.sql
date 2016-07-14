CREATE TABLE [dbo].[dialogs]
(
	[dialogIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [sceneIntno] BIGINT NOT NULL, 
    [dialogNo] SMALLINT NOT NULL, 
    [startTimeCode] NVARCHAR(20) NULL, 
    [endTimeCode] NVARCHAR(20) NULL, 
    [isTaken] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_dialogs_To_scenes] FOREIGN KEY ([sceneIntno]) REFERENCES [scenes]([sceneIntno]) on delete cascade
)
