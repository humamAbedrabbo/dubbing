CREATE TABLE [dbo].[chrCastListDtls]
(
	[chrCastListDtlIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [chrCastListHdrIntno] BIGINT NOT NULL, 
    [voiceActorIntno] BIGINT NOT NULL, 
    [Match] FLOAT NOT NULL, 
    [Rank] INT NULL, 
    [Comments] NVARCHAR(MAX) NULL, 
    [ClientRank] INT NULL, 
    [ClientComments] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_chrCastListDtls_To_chrCastListHdrs] FOREIGN KEY ([chrCastListHdrIntno]) REFERENCES [chrCastListHdrs]([chrCastListHdrIntno]) on delete cascade, 
    CONSTRAINT [FK_chrCastListDtls_To_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [voiceActors]([voiceActorIntno]) on delete cascade
)
