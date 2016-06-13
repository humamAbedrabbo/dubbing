CREATE TABLE [dbo].[shipmentDetails] (
    [shipmentDtlIntno] BIGINT IDENTITY (1, 1) NOT NULL,
    [shipmentIntno]    BIGINT NOT NULL,
    [orderTrnHdrIntno] BIGINT NOT NULL,
    CONSTRAINT [PK_shipmentDetails] PRIMARY KEY CLUSTERED ([shipmentDtlIntno] ASC),
    CONSTRAINT [FK_shipmentDetails_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno]),
    CONSTRAINT [FK_shipmentDetails_shipments] FOREIGN KEY ([shipmentIntno]) REFERENCES [dbo].[shipments] ([shipmentIntno])
);

