CREATE TABLE [dbo].[dubbingSheetDtls] (
    [dubbSheetDtlIntno] BIGINT        IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno]  BIGINT        NOT NULL,
    [dubbSheetHdrIntno] BIGINT        NOT NULL,
    [sceneNo]           SMALLINT      NOT NULL,
    [isTaken]           BIT           NOT NULL,
    [studioNo]          NVARCHAR (50) COLLATE Latin1_General_CI_AS NULL,
    [supervisor]        BIGINT        NULL,
    [soundTechnician]   BIGINT        NULL,
    [assistant]         BIGINT        NULL,
    [takenTimeStamp]    DATETIME      NULL,
    [dubbingDate] DATE NULL, 
    CONSTRAINT [PK_dubbingSheetDtls] PRIMARY KEY CLUSTERED ([dubbSheetDtlIntno] ASC),
    CONSTRAINT [FK_dubbingSheetDtls_dubbingSheetHdrs] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dbo].[dubbingSheetHdrs] ([dubbSheetHdrIntno]) ON DELETE CASCADE,
    CONSTRAINT [FK_dubbingSheetDtls_employees] FOREIGN KEY ([supervisor]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_employees1] FOREIGN KEY ([soundTechnician]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_employees2] FOREIGN KEY ([assistant]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);
GO
