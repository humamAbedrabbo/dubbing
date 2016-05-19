CREATE TABLE [dbo].[orderTrnDtls] (
    [orderTrnDtlIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno] BIGINT         NOT NULL,
    [activityType]     NVARCHAR (50)  NOT NULL,
    [empIntno]         BIGINT         NOT NULL,
    [providedService]  NVARCHAR (MAX) NULL,
    [totalMinutes]     INT            NULL,
    [timeRating]       NVARCHAR (50)  NULL,
    [qualityRating]    NVARCHAR (50)  NULL,
    CONSTRAINT [PK_orderTrnDtls] PRIMARY KEY CLUSTERED ([orderTrnDtlIntno] ASC),
    CONSTRAINT [FK_orderTrnDtls_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

