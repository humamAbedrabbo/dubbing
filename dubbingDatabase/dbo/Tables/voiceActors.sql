CREATE TABLE [dbo].[voiceActors] (
    [voiceActorIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [fullName]        NVARCHAR (100) NOT NULL,
    [othFullName]     NVARCHAR (100) NOT NULL,
    [mobileNo]        NVARCHAR (50)  NOT NULL,
    [landLineNo]      NVARCHAR (50)  NOT NULL,
    [email]           NVARCHAR (50)  NOT NULL,
    [status]          BIT            NOT NULL,
    [accountNo]       NVARCHAR (50)  NULL,
    CONSTRAINT [PK_voiceActors] PRIMARY KEY CLUSTERED ([voiceActorIntno] ASC)
);

