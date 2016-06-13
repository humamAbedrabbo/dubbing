CREATE TABLE [dbo].[workCharges] (
    [chargeIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]      BIGINT         NOT NULL,
    [workPartyType]  NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [workPartyIntno] BIGINT         NOT NULL,
    [chargeAmount]   BIGINT         NOT NULL,
    [chargeUom]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [currencyCode]   NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [fromDate]       DATE           NOT NULL,
    [remarks]        NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [status]         BIT            NOT NULL,
    CONSTRAINT [PK_workCharges] PRIMARY KEY CLUSTERED ([chargeIntno] ASC)
);

