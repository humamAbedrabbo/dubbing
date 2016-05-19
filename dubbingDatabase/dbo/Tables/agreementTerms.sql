CREATE TABLE [dbo].[agreementTerms] (
    [termIntno]      BIGINT         IDENTITY (1, 1) NOT NULL,
    [agreementIntno] BIGINT         NOT NULL,
    [sortOrder]      SMALLINT       NOT NULL,
    [termDesc]       NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_agreementTerms] PRIMARY KEY CLUSTERED ([termIntno] ASC),
    CONSTRAINT [FK_agreementTerms_agreements] FOREIGN KEY ([agreementIntno]) REFERENCES [dbo].[agreements] ([agreementIntno])
);

