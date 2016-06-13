CREATE TABLE [dbo].[clientClaims] (
    [claimIntno]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno] BIGINT         NOT NULL,
    [clientIntno]      BIGINT         NOT NULL,
    [claimType]        NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [receivedDate]     DATE           NOT NULL,
    [claimRefNo]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [claimDesc]        NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NOT NULL,
    [refLocation]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [requiredAction]   NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NOT NULL,
    [actionDate]       DATE           NULL,
    [status]           BIT            NOT NULL,
    CONSTRAINT [PK_clientClaims] PRIMARY KEY CLUSTERED ([claimIntno] ASC),
    CONSTRAINT [FK_clientClaims_clients] FOREIGN KEY ([clientIntno]) REFERENCES [dbo].[clients] ([clientIntno]),
    CONSTRAINT [FK_clientClaims_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

