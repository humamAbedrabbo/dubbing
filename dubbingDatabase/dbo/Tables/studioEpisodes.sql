CREATE TABLE [dbo].[studioEpisodes] (
    [studioEpisodeIntno] BIGINT IDENTITY (1, 1) NOT NULL,
    [studioIntno]        BIGINT NOT NULL,
    [orderTrnDtlIntno]    BIGINT NOT NULL,
    CONSTRAINT [PK_studioEpisodes] PRIMARY KEY CLUSTERED ([studioEpisodeIntno] ASC),
    CONSTRAINT [FK_studioEpisodes_orderTrnDtls] FOREIGN KEY ([orderTrnDtlIntno]) REFERENCES [dbo].[orderTrnDtls] ([orderTrnDtlIntno]),
    CONSTRAINT [FK_studioEpisodes_studios] FOREIGN KEY ([studioIntno]) REFERENCES [dbo].[studios] ([studioIntno]) ON DELETE CASCADE
);

