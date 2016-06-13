CREATE TABLE [dbo].[paymentDetails] (
    [paymentDtlIntno] BIGINT IDENTITY (1, 1) NOT NULL,
    [paymentIntno]    BIGINT NOT NULL,
    [dubbingDate]     DATE   NOT NULL,
    [totalUnits]      INT    NOT NULL,
    CONSTRAINT [PK_paymentDetails] PRIMARY KEY CLUSTERED ([paymentDtlIntno] ASC),
    CONSTRAINT [FK_paymentDetails_payments] FOREIGN KEY ([paymentIntno]) REFERENCES [dbo].[payments] ([paymentIntno])
);

