CREATE TABLE [dbo].[workCharacters] (
    [workCharacterIntno] BIGINT         IDENTITY (1, 1) NOT NULL,
    [workIntno]          BIGINT         NOT NULL,
    [sortOrder]          SMALLINT       NOT NULL,
    [characterType]      NVARCHAR (50)  NOT NULL,
    [characterGender]    NVARCHAR (50)  NOT NULL,
    [characterCode]      NVARCHAR (10)  NOT NULL,
    [characterName]      NVARCHAR (100) NOT NULL,
    [othCharacterName]   NVARCHAR (100) NULL,
    [nickName]           NVARCHAR (100) NULL,
    [characterDesc]      NVARCHAR (MAX) NULL,
    [remarks]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_workCharacters] PRIMARY KEY CLUSTERED ([workCharacterIntno] ASC),
    CONSTRAINT [FK_workCharacters_agreementWorks] FOREIGN KEY ([workIntno]) REFERENCES [dbo].[agreementWorks] ([workIntno])
);

