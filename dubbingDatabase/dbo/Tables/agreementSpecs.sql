CREATE TABLE [dbo].[agreementSpecs] (
    [specsIntno]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [agreementIntno] BIGINT         NOT NULL,
    [specsType]      NVARCHAR (50)  NOT NULL,
    [specsSubtype]   NVARCHAR (50)  NOT NULL,
    [specsValue]     NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_agreementSpecs] PRIMARY KEY CLUSTERED ([specsIntno] ASC),
    CONSTRAINT [FK_agreementSpecs_agreements] FOREIGN KEY ([agreementIntno]) REFERENCES [dbo].[agreements] ([agreementIntno])
);

