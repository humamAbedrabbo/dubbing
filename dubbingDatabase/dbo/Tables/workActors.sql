CREATE TABLE [dbo].[workActors] (
    [workActorIntno]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]          BIGINT         NOT NULL,
    [workCharacterIntno] BIGINT         NOT NULL,
    [voiceActorIntno]    BIGINT         NOT NULL,
    [fromDate]           DATE           NOT NULL,
    [thruDate]           DATE           NULL,
    [remarks]            NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [status]             BIT            NOT NULL,
    [scenesPerHour]      INT            NOT NULL,
    CONSTRAINT [PK_workActors] PRIMARY KEY CLUSTERED ([workActorIntno] ASC),
    CONSTRAINT [FK_workActors_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_workActors_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [dbo].[voiceActors] ([voiceActorIntno]),
    CONSTRAINT [FK_workActors_workCharacters] FOREIGN KEY ([workCharacterIntno]) REFERENCES [dbo].[workCharacters] ([workCharacterIntno])
);

