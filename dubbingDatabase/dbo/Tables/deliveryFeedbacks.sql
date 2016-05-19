CREATE TABLE [dbo].[deliveryFeedbacks] (
    [feedbackIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno] BIGINT         NOT NULL,
    [feedbackType]     NVARCHAR (50)  NOT NULL,
    [receivedDate]     DATE           NOT NULL,
    [clientRefNo]      NVARCHAR (50)  NOT NULL,
    [feedbackDesc]     NVARCHAR (MAX) NOT NULL,
    [refLocation]      NVARCHAR (50)  NULL,
    [remarks]          NVARCHAR (MAX) NULL,
    [status]           BIT            NOT NULL,
    CONSTRAINT [PK_deliveryFeedbacks] PRIMARY KEY CLUSTERED ([feedbackIntno] ASC),
    CONSTRAINT [FK_deliveryFeedbacks_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

