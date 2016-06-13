CREATE TABLE [dbo].[dubbingSheetHdrs] (
    [dubbSheetHdrIntno]  BIGINT        IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno]   BIGINT        NOT NULL,
    [workCharacterIntno] BIGINT        NULL,
    [voiceActorIntno]    BIGINT        NOT NULL,
    [actorName]          NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [characterName]      NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_dubbingSheetHdrs] PRIMARY KEY CLUSTERED ([dubbSheetHdrIntno] ASC),
    CONSTRAINT [FK_dubbingSheetHdrs_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno]),
    CONSTRAINT [FK_dubbingSheetHdrs_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [dbo].[voiceActors] ([voiceActorIntno]),
    CONSTRAINT [FK_dubbingSheetHdrs_workCharacters] FOREIGN KEY ([workCharacterIntno]) REFERENCES [dbo].[workCharacters] ([workCharacterIntno])
);

