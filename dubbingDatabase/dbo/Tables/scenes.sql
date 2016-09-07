CREATE TABLE [dbo].[scenes]
(
	[sceneIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [orderTrnHdrIntno] BIGINT NOT NULL, 
    [sceneNo] SMALLINT NOT NULL, 
    [startTimeCode] NVARCHAR(20) NULL, 
    [endTimeCode] NVARCHAR(20) NULL, 
    [isTaken] BIT NOT NULL DEFAULT 0, 
    [startSecond] BIGINT NOT NULL DEFAULT 0, 
    [endSecond] BIGINT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_scenes_To_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [orderTrnHdrs]([orderTrnHdrIntno])
)
