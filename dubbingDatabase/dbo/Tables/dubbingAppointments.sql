CREATE TABLE [dbo].[dubbingAppointments] (
    [dubbAppointIntno]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [dubbTrnDtlIntno]   BIGINT         NOT NULL,
    [dubbSheetHdrIntno] BIGINT         NOT NULL,
    [appointmentDate]   DATE           NULL,
    [fromTime]          DATETIME       NULL,
    [thruTime]          DATETIME       NULL,
    [totalScenes]       INT            NULL,
    [actualFromTime]    DATETIME       NULL,
    [actualThruTime]    DATETIME       NULL,
    [remarks]           NVARCHAR (MAX) NULL,
    [totalMinutes]      INT            NOT NULL,
    CONSTRAINT [PK_dubbingAppointments] PRIMARY KEY CLUSTERED ([dubbAppointIntno] ASC),
    CONSTRAINT [FK_dubbingAppointments_dubbingSheetHdrs1] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dbo].[dubbingSheetHdrs] ([dubbSheetHdrIntno]),
    CONSTRAINT [FK_dubbingAppointments_dubbingTrnDtls] FOREIGN KEY ([dubbTrnDtlIntno]) REFERENCES [dbo].[dubbingTrnDtls] ([dubbTrnDtlIntno])
);

