CREATE TABLE [dbo].[orderTrnDtls] (
    [orderTrnDtlIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno] BIGINT         NOT NULL,
    [activityType]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [empIntno]         BIGINT         NOT NULL,
    [providedService]  NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [totalMinutes]     INT            NULL,
    [timeRating]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [qualityRating]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [fromTimeCode] NVARCHAR(50) NULL, 
    [thruTimeCode] NVARCHAR(50) NULL, 
    [status] BIT NOT NULL, 
    [assignedDate] DATE NULL, 
    [forDueDate] DATE NULL, 
    CONSTRAINT [PK_orderTrnDtls] PRIMARY KEY CLUSTERED ([orderTrnDtlIntno] ASC),
    CONSTRAINT [FK_orderTrnDtls_employees] FOREIGN KEY ([empIntno]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_orderTrnDtls_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

