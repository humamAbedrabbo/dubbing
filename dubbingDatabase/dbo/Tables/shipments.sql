CREATE TABLE [dbo].[shipments] (
    [shipmentIntno] BIGINT        IDENTITY (1, 1) NOT NULL,
    [carrierIntno]  BIGINT        NULL,
    [shipmentDate]  DATE          NOT NULL,
    [remarks]       NVARCHAR (50) NULL,
    [status]        BIT           NOT NULL,
    [clientIntno]   BIGINT        NOT NULL,
    CONSTRAINT [PK_shipments] PRIMARY KEY CLUSTERED ([shipmentIntno] ASC),
    CONSTRAINT [FK_shipments_carriers] FOREIGN KEY ([carrierIntno]) REFERENCES [dbo].[carriers] ([carrierIntno]),
    CONSTRAINT [FK_shipments_clients] FOREIGN KEY ([clientIntno]) REFERENCES [dbo].[clients] ([clientIntno])
);

