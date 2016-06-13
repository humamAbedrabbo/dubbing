CREATE TABLE [dbo].[dubbingTrnDtls] (
    [dubbTrnDtlIntno]  BIGINT IDENTITY (1, 1) NOT NULL,
    [dubbTrnHdrIntno]  BIGINT NOT NULL,
    [workIntno]        BIGINT NOT NULL,
    [orderTrnHdrIntno] BIGINT NOT NULL,
    [episodeNo]        INT    NOT NULL,
    CONSTRAINT [PK_dubbingTrnDtls] PRIMARY KEY CLUSTERED ([dubbTrnDtlIntno] ASC),
    CONSTRAINT [FK_dubbingTrnDtls_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_dubbingTrnDtls_dubbingTrnHdrs] FOREIGN KEY ([dubbTrnHdrIntno]) REFERENCES [dbo].[dubbingTrnHdrs] ([dubbTrnHdrIntno]),
    CONSTRAINT [FK_dubbingTrnDtls_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

