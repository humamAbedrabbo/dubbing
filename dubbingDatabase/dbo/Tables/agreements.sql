CREATE TABLE [dbo].[agreements] (
    [agreementIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [clientIntno]       BIGINT         NOT NULL,
    [agreementType]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [agreementName]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [othAgreementName]  NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [refAgreementIntno] BIGINT         NULL,
    [fromDate]          DATE           NOT NULL,
    [agreementDuration] SMALLINT       NOT NULL,
    [durationUom]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [thruDate]          DATE           NULL,
    [remarks]           NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [status]            NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_agreements] PRIMARY KEY CLUSTERED ([agreementIntno] ASC),
    CONSTRAINT [FK_agreements_agreements] FOREIGN KEY ([refAgreementIntno]) REFERENCES [dbo].[agreements] ([agreementIntno]),
    CONSTRAINT [FK_agreements_clients] FOREIGN KEY ([clientIntno]) REFERENCES [dbo].[clients] ([clientIntno])
);

