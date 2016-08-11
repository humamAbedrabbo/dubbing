CREATE TABLE [dbo].[studioEpisodes] (
    [studioEpisodeIntno] BIGINT IDENTITY (1, 1) NOT NULL,
    [studioIntno]        BIGINT NOT NULL,
    [dubbTrnDtlIntno]    BIGINT NOT NULL,
    [status]             BIT    NOT NULL,
    CONSTRAINT [PK_studioEpisodes] PRIMARY KEY CLUSTERED ([studioEpisodeIntno] ASC),
    CONSTRAINT [FK_studioEpisodes_dubbingTrnDtls] FOREIGN KEY ([dubbTrnDtlIntno]) REFERENCES [dbo].[dubbingTrnDtls] ([dubbTrnDtlIntno]),
    CONSTRAINT [FK_studioEpisodes_studios] FOREIGN KEY ([studioIntno]) REFERENCES [dbo].[studios] ([studioIntno]) ON DELETE CASCADE
);

