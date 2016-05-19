CREATE TABLE [dbo].[payments] (
    [paymentHdrIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [dubbTrnHdrIntno] BIGINT         NOT NULL,
    [workIntno]       BIGINT         NOT NULL,
    [costCenterType]  NVARCHAR (50)  NULL,
    [voiceActorIntno] BIGINT         NULL,
    [empIntno]        BIGINT         NULL,
    [anonymousName]   NVARCHAR (50)  NULL,
    [paymentDesc]     NVARCHAR (MAX) NULL,
    [totalAmount]     INT            NOT NULL,
    [currencyCode]    NVARCHAR (50)  NOT NULL,
    [paymentDate]     DATE           NULL,
    [remarks]         NVARCHAR (MAX) NULL,
    [status]          BIT            NOT NULL,
    CONSTRAINT [PK_payments] PRIMARY KEY CLUSTERED ([paymentHdrIntno] ASC),
    CONSTRAINT [FK_payments_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_payments_dubbingTrnHdrs] FOREIGN KEY ([dubbTrnHdrIntno]) REFERENCES [dbo].[dubbingTrnHdrs] ([dubbTrnHdrIntno]),
    CONSTRAINT [FK_payments_employees] FOREIGN KEY ([empIntno]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_payments_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [dbo].[voiceActors] ([voiceActorIntno])
);

