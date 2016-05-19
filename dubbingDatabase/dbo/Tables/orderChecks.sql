CREATE TABLE [dbo].[orderChecks] (
    [orderCheckIntno]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderIntno]       BIGINT         NOT NULL,
    [orderTrnHdrIntno] BIGINT         NULL,
    [episodeNo]        SMALLINT       NOT NULL,
    [mediaType]        NVARCHAR (50)  NOT NULL,
    [checkType]        NVARCHAR (50)  NOT NULL,
    [isAccepted]       BIT            NOT NULL,
    [rejectReasons]    NVARCHAR (MAX) NULL,
    [remarks]          NVARCHAR (MAX) NULL,
    [updatedBy]        NVARCHAR (50)  NULL,
    [lastUpdated]      DATETIME       NULL,
    CONSTRAINT [PK_orderChecks] PRIMARY KEY CLUSTERED ([orderCheckIntno] ASC),
    CONSTRAINT [FK_orderChecks_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno]),
    CONSTRAINT [FK_orderChecks_workOrders] FOREIGN KEY ([orderIntno]) REFERENCES [dbo].[workOrders] ([orderIntno])
);

