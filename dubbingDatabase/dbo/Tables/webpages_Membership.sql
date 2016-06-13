CREATE TABLE [dbo].[webpages_Membership] (
    [UserId]                                  INT            NOT NULL,
    [CreateDate]                              DATETIME       NULL,
    [ConfirmationToken]                       NVARCHAR (128) COLLATE Latin1_General_CI_AS NULL,
    [IsConfirmed]                             BIT            CONSTRAINT [DF__webpages___IsCon__1AF3F935] DEFAULT ((0)) NULL,
    [LastPasswordFailureDate]                 DATETIME       NULL,
    [PasswordFailuresSinceLastSuccess]        INT            CONSTRAINT [DF__webpages___Passw__1BE81D6E] DEFAULT ((0)) NOT NULL,
    [Password]                                NVARCHAR (128) COLLATE Latin1_General_CI_AS NOT NULL,
    [PasswordChangedDate]                     DATETIME       NULL,
    [PasswordSalt]                            NVARCHAR (128) COLLATE Latin1_General_CI_AS NOT NULL,
    [PasswordVerificationToken]               NVARCHAR (128) COLLATE Latin1_General_CI_AS NULL,
    [PasswordVerificationTokenExpirationDate] DATETIME       NULL,
    CONSTRAINT [PK__webpages__1788CC4C190BB0C3] PRIMARY KEY CLUSTERED ([UserId] ASC)
);

