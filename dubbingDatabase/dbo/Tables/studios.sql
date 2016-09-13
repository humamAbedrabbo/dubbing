CREATE TABLE [dbo].[studios] (
    [studioIntno]     BIGINT        IDENTITY (1, 1) NOT NULL,
    [dubbTrnHdrIntno] BIGINT        NOT NULL,
    [workIntno]       BIGINT        NOT NULL,
    [studioNo]        NVARCHAR (50) COLLATE Latin1_General_CI_AS NULL,
    [sound]           BIGINT        NULL,
    CONSTRAINT [PK_studios] PRIMARY KEY CLUSTERED ([studioIntno] ASC),
    CONSTRAINT [FK_studios_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno]),
    CONSTRAINT [FK_studios_dubbingTrnHdrs] FOREIGN KEY ([dubbTrnHdrIntno]) REFERENCES [dbo].[dubbingTrnHdrs] ([dubbTrnHdrIntno]) ON DELETE CASCADE,
    CONSTRAINT [FK_studios_employees_sound] FOREIGN KEY ([sound]) REFERENCES [dbo].[employees] ([empIntno])
);

