CREATE TABLE [dbo].[orderChecks] (
    [orderCheckIntno]  BIGINT        IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno] BIGINT        NOT NULL,
    [checkType]        NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [isAccepted]       BIT           NOT NULL,
    CONSTRAINT [PK_orderChecks] PRIMARY KEY CLUSTERED ([orderCheckIntno] ASC),
    CONSTRAINT [FK_orderChecks_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

