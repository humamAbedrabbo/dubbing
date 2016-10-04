CREATE TABLE [dbo].[audioSampleDtls]
(
	[audioSampleDtlIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [audioSampleHdrIntno] BIGINT NOT NULL, 
    [tagId] INT NOT NULL, 
    [TagScore] INT NOT NULL, 
    [Match] FLOAT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_audioSampleDtls_To_audioSampleHdrs] FOREIGN KEY ([audioSampleHdrIntno]) REFERENCES [audioSampleHdrs]([audioSampleHdrIntno]) on delete cascade, 
    CONSTRAINT [FK_audioSampleDtls_To_tags] FOREIGN KEY ([tagId]) REFERENCES [tags](Id) 
)
