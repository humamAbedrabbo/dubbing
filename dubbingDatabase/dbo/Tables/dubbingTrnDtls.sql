CREATE TABLE [dbo].[dubbingTrnDtls] (
    [dubbTrnDtlIntno]  BIGINT        IDENTITY (1, 1) NOT NULL,
    [dubbTrnHdrIntno]  BIGINT        NOT NULL,
    [orderTrnHdrIntno] BIGINT        NOT NULL,
    [dubbingDate]      DATE          NULL,
    [studioNo]         NVARCHAR (50) NULL,
    [supervisor]       BIGINT        NULL,
    [soundTechnician]  BIGINT        NULL,
    [assistant]        BIGINT        NULL,
    [status]           NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_dubbingTrnDtls] PRIMARY KEY CLUSTERED ([dubbTrnDtlIntno] ASC),
    CONSTRAINT [FK_dubbingTrnDtls_dubbingTrnHdrs] FOREIGN KEY ([dubbTrnHdrIntno]) REFERENCES [dbo].[dubbingTrnHdrs] ([dubbTrnHdrIntno]),
    CONSTRAINT [FK_dubbingTrnDtls_employees] FOREIGN KEY ([supervisor]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingTrnDtls_employees1] FOREIGN KEY ([soundTechnician]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingTrnDtls_employees2] FOREIGN KEY ([assistant]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingTrnDtls_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);

