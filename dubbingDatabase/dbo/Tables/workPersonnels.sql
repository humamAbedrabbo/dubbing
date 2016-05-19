CREATE TABLE [dbo].[workPersonnels] (
    [workPersonnelIntno] BIGINT        IDENTITY (1, 1) NOT NULL,
    [workIntno]          BIGINT        NOT NULL,
    [empIntno]           BIGINT        NOT NULL,
    [titleType]          NVARCHAR (50) NOT NULL,
    [fromDate]           DATE          NOT NULL,
    [thruDate]           DATE          NULL,
    [status]             BIT           NOT NULL,
    CONSTRAINT [PK_workPersonnels] PRIMARY KEY CLUSTERED ([workPersonnelIntno] ASC),
    CONSTRAINT [FK_workPersonnels_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_workPersonnels_employees] FOREIGN KEY ([empIntno]) REFERENCES [dbo].[employees] ([empIntno])
);

