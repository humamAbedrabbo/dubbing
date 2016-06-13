﻿CREATE TABLE [dbo].[payments] (
    [paymentIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]       BIGINT         NOT NULL,
    [costCenterType]  NVARCHAR (50)  NOT NULL,
    [voiceActorIntno] BIGINT         NULL,
    [empIntno]        BIGINT         NULL,
    [fullName]        NVARCHAR (50)  NULL,
    [episodeNo]       SMALLINT       NULL,
    [totalScenes]     INT            NOT NULL,
    [unitRate]        INT            NOT NULL,
    [paymentDesc]     NVARCHAR (MAX) NULL,
    [totalAmount]     INT            NOT NULL,
    [currencyCode]    NVARCHAR (50)  NOT NULL,
    [deduction]       INT            NOT NULL,
    [paymentDate]     DATE           NULL,
    [accountNo]       NVARCHAR (50)  NOT NULL,
    [status]          BIT            NOT NULL,
    [isPaid]          BIT            NOT NULL,
    [isExported]      BIT            NOT NULL,
    CONSTRAINT [PK_payments] PRIMARY KEY CLUSTERED ([paymentIntno] ASC),
    CONSTRAINT [FK_payments_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_payments_employees] FOREIGN KEY ([empIntno]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_payments_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [dbo].[voiceActors] ([voiceActorIntno])
);

