CREATE TABLE [dbo].[agreementSpecs] (
    [specsIntno]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [agreementIntno] BIGINT         NOT NULL,
    [specsType]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [specsSubtype]   NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [specsValue]     NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_agreementSpecs] PRIMARY KEY CLUSTERED ([specsIntno] ASC),
    CONSTRAINT [FK_agreementSpecs_agreements] FOREIGN KEY ([agreementIntno]) REFERENCES [dbo].[agreements] ([agreementIntno])
);

