CREATE TABLE [dbo].[workCharacters] (
    [workCharacterIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]          BIGINT         NOT NULL,
    [sortOrder]          SMALLINT       NOT NULL,
    [characterType]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [characterGender]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [characterCode]      NVARCHAR (10)  COLLATE Latin1_General_CI_AS NOT NULL,
    [characterName]      NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [othCharacterName]   NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [nickName]           NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [characterDesc]      NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [remarks]            NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    CONSTRAINT [PK_workCharacters] PRIMARY KEY CLUSTERED ([workCharacterIntno] ASC),
    CONSTRAINT [FK_workCharacters_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno])
);

