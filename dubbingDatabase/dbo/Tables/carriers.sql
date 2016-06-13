CREATE TABLE [dbo].[carriers] (
    [carrierIntno]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [carrierType]    NVARCHAR (50)  NOT NULL,
    [carrierName]    NVARCHAR (50)  NOT NULL,
    [othCarrierName] NVARCHAR (50)  NOT NULL,
    [schedule]       NVARCHAR (MAX) NULL,
    [status]         BIT            NOT NULL,
    CONSTRAINT [PK_carriers] PRIMARY KEY CLUSTERED ([carrierIntno] ASC)
);

