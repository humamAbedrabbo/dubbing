CREATE TABLE [dbo].[costCenters] (
    [costCenterIntno]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]         BIGINT         NOT NULL,
    [costCenterType]    NVARCHAR (50)  NOT NULL,
    [costCenterName]    NVARCHAR (100) NOT NULL,
    [othCostCenterName] NVARCHAR (100) NULL,
    [accountNo]         NVARCHAR (50)  NOT NULL,
    [status]            BIT            NOT NULL,
    CONSTRAINT [PK_costCenters] PRIMARY KEY CLUSTERED ([costCenterIntno] ASC),
    CONSTRAINT [FK_costCenters_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno])
);

