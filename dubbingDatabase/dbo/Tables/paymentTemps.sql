CREATE TABLE [dbo].[paymentTemps] (
    [tempIntno]       BIGINT        IDENTITY (1, 1) NOT NULL,
    [personIntno]     BIGINT        NOT NULL,
    [personName]      NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [workIntno]       BIGINT        NOT NULL,
    [workName]        NVARCHAR (50) COLLATE Latin1_General_CI_AS NOT NULL,
    [trnDate]         DATE          NOT NULL,
    [totalUnits]      INT           NOT NULL,
    [refPaymentIntno] BIGINT        NULL,
    CONSTRAINT [PK_paymentTemps] PRIMARY KEY CLUSTERED ([tempIntno] ASC)
);

