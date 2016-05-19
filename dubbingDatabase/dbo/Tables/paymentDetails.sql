CREATE TABLE [dbo].[paymentDetails] (
    [paymentDtlIntno]     BIGINT   IDENTITY (1, 1) NOT NULL,
    [paymentHdrIntno]     BIGINT   NOT NULL,
    [episodeNo]           SMALLINT NOT NULL,
    [totalScenes]         INT      NULL,
    [adjustedTotalScenes] INT      NULL,
    [unitRate]            INT      NOT NULL,
    [amount]              INT      NOT NULL,
    CONSTRAINT [PK_paymentDetails] PRIMARY KEY CLUSTERED ([paymentDtlIntno] ASC),
    CONSTRAINT [FK_paymentDetails_payments] FOREIGN KEY ([paymentHdrIntno]) REFERENCES [dbo].[payments] ([paymentHdrIntno])
);

