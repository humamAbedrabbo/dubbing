CREATE TABLE [dbo].[webpages_OAuthMembership] (
    [Provider]       NVARCHAR (30)  COLLATE Latin1_General_CI_AS NOT NULL,
    [ProviderUserId] NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [UserId]         INT            NOT NULL,
    CONSTRAINT [PK__webpages__F53FC0ED153B1FDF] PRIMARY KEY CLUSTERED ([Provider] ASC, [ProviderUserId] ASC)
);

