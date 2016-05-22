CREATE TABLE [dbo].[workOrders] (
    [orderIntno]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [clientIntno]          BIGINT         NOT NULL,
    [workIntno]            BIGINT         NOT NULL,
    [fromEpisode]          SMALLINT       NOT NULL,
    [thruEpisode]          SMALLINT       NOT NULL,
    [receivedDate]         DATE           NOT NULL,
    [priority]             NVARCHAR (50)  NULL,
    [expectedDeliveryDate] DATE           NULL,
    [allowFirstDubbing]    BIT            NULL,
    [remarks]              NVARCHAR (MAX) NULL,
    [updatedBy]            NVARCHAR (50)  NULL,
    [lastUpdated]          DATETIME       NULL,
    [status]               NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_workOrders] PRIMARY KEY CLUSTERED ([orderIntno] ASC),
    CONSTRAINT [FK_workOrders_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_workOrders_clients] FOREIGN KEY ([clientIntno]) REFERENCES [dbo].[clients] ([clientIntno])
);

