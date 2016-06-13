CREATE TABLE [dbo].[carriers] (
    [carrierIntno]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [carrierType]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [carrierName]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [othCarrierName] NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [schedule]       NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [status]         BIT            NOT NULL,
    CONSTRAINT [PK_carriers] PRIMARY KEY CLUSTERED ([carrierIntno] ASC)
);

