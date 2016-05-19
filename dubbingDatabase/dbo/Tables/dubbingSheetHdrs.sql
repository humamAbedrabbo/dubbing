CREATE TABLE [dbo].[dubbingSheetHdrs] (
    [dubbSheetHdrIntno]  BIGINT        IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno]   BIGINT        NOT NULL,
    [workCharacterIntno] BIGINT        NOT NULL,
    [voiceActorIntno]    BIGINT        NOT NULL,
    [anonymActorName]    NVARCHAR (50) NULL,
    CONSTRAINT [PK_dubbingSheetHdrs] PRIMARY KEY CLUSTERED ([dubbSheetHdrIntno] ASC),
    CONSTRAINT [FK_dubbingSheetHdrs_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno]),
    CONSTRAINT [FK_dubbingSheetHdrs_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [dbo].[voiceActors] ([voiceActorIntno]),
    CONSTRAINT [FK_dubbingSheetHdrs_workCharacters] FOREIGN KEY ([workCharacterIntno]) REFERENCES [dbo].[workCharacters] ([workCharacterIntno])
);

