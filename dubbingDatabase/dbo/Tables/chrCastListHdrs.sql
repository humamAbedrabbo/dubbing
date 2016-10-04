CREATE TABLE [dbo].[chrCastListHdrs]
(
	[chrCastListHdrIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [workCharacterIntno] BIGINT NOT NULL, 
	[clientIntno] BIGINT NOT NULL, 
    [Title] NVARCHAR(150) NOT NULL, 
    [Status] NVARCHAR(10) NOT NULL, 
    CONSTRAINT [FK_chrCastListHdrs_To_workCharacters] FOREIGN KEY ([workCharacterIntno]) REFERENCES [workCharacters]([workCharacterIntno]) on delete cascade, 
    CONSTRAINT [FK_chrCastListHdrs_To_clients] FOREIGN KEY ([clientIntno]) REFERENCES [clients]([clientIntno]) on delete cascade
)
