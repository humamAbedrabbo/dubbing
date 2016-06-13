CREATE TABLE [dbo].[clientClaims] (
    [claimIntno]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno] BIGINT         NOT NULL,
    [clientIntno]      BIGINT         NOT NULL,
    [claimType]        NVARCHAR (50)  NOT NULL,
    [receivedDate]     DATE           NOT NULL,
    [claimRefNo]       NVARCHAR (50)  NULL,
    [claimDesc]        NVARCHAR (MAX) NOT NULL,
    [refLocation]      NVARCHAR (50)  NULL,
    [requiredAction]   NVARCHAR (MAX) NOT NULL,
    [actionDate]       DATE           NULL,
    [status]           BIT            NOT NULL,
    CONSTRAINT [PK_clientClaims] PRIMARY KEY CLUSTERED ([claimIntno] ASC),
    CONSTRAINT [FK_clientClaims_clients] FOREIGN KEY ([clientIntno]) REFERENCES [dbo].[clients] ([clientIntno]),
    CONSTRAINT [FK_clientClaims_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

