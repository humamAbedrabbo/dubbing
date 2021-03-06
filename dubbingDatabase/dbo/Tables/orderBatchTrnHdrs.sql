CREATE TABLE [dbo].[orderBatchTrnHdrs] (
    [orderBatchTrnIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderIntno]         BIGINT         NOT NULL,
    [workIntno]          BIGINT         NOT NULL,
    [trnType]            NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [fromEpisode]        INT            NOT NULL,
    [thruEpisode]        INT            NOT NULL,
    [trnDate]            DATE           NOT NULL,
    [empIntno]           BIGINT         NOT NULL,
    [trnDesc]            NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [remarks]            NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [updatedBy]          NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [lastUpdated]        DATETIME       NOT NULL,
    [status]             BIT            NOT NULL,
    CONSTRAINT [PK_orderBatchTrnHdrs] PRIMARY KEY CLUSTERED ([orderBatchTrnIntno] ASC),
    CONSTRAINT [FK_orderBatchTrnHdrs_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_orderBatchTrnHdrs_employees] FOREIGN KEY ([empIntno]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_orderBatchTrnHdrs_workOrders] FOREIGN KEY ([orderIntno]) REFERENCES [dbo].[workOrders] ([orderIntno])
);

