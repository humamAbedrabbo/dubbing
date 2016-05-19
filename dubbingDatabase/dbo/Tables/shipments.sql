CREATE TABLE [dbo].[shipments] (
    [shipmentIntno]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno]     BIGINT        NOT NULL,
    [carrierIntno]         BIGINT        NULL,
    [carrierScheduleIntno] BIGINT        NULL,
    [shipmentMethod]       NVARCHAR (50) NOT NULL,
    [shipmentDate]         DATE          NOT NULL,
    [remarks]              NVARCHAR (50) NULL,
    [status]               NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_shipments] PRIMARY KEY CLUSTERED ([shipmentIntno] ASC),
    CONSTRAINT [FK_shipments_carriers] FOREIGN KEY ([carrierIntno]) REFERENCES [dbo].[carriers] ([carrierIntno]),
    CONSTRAINT [FK_shipments_carrierSchedules] FOREIGN KEY ([carrierScheduleIntno]) REFERENCES [dbo].[carrierSchedules] ([carrierScheduleIntno]),
    CONSTRAINT [FK_shipments_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

