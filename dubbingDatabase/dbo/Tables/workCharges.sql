CREATE TABLE [dbo].[workCharges] (
    [chargeIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]      BIGINT         NOT NULL,
    [workPartyType]  NVARCHAR (50)  NOT NULL,
    [workPartyIntno] BIGINT         NOT NULL,
    [chargeAmount]   BIGINT         NOT NULL,
    [chargeUom]      NVARCHAR (50)  NOT NULL,
    [currencyCode]   NVARCHAR (50)  NOT NULL,
    [fromDate]       DATE           NOT NULL,
    [remarks]        NVARCHAR (MAX) NULL,
    [status]         BIT            NOT NULL,
    CONSTRAINT [PK_workCharges] PRIMARY KEY CLUSTERED ([chargeIntno] ASC)
);

