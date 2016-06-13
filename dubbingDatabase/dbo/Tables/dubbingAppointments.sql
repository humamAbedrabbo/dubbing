CREATE TABLE [dbo].[dubbingAppointments] (
    [dubbAppointIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [voiceActorIntno]  BIGINT         NOT NULL,
    [actorName]        NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [studioIntno]      BIGINT         NOT NULL,
    [appointmentDate]  DATE           NOT NULL,
    [fromTime]         DATETIME       NULL,
    [thruTime]         DATETIME       NULL,
    [workIntno]        BIGINT         NOT NULL,
    [totalScenes]      INT            NULL,
    [actualFromTime]   DATETIME       NULL,
    [actualThruTime]   DATETIME       NULL,
    [remarks]          NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [totalMinutes]     INT            NOT NULL,
    CONSTRAINT [PK_dubbingAppointments] PRIMARY KEY CLUSTERED ([dubbAppointIntno] ASC),
    CONSTRAINT [FK_dubbingAppointments_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_dubbingAppointments_studios] FOREIGN KEY ([studioIntno]) REFERENCES [dbo].[studios] ([studioIntno]),
    CONSTRAINT [FK_dubbingAppointments_voiceActors] FOREIGN KEY ([voiceActorIntno]) REFERENCES [dbo].[voiceActors] ([voiceActorIntno])
);

