CREATE TABLE [dbo].[dubbDomain] (
    [domainIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [domainName]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [domainCode]     NVARCHAR (10)  COLLATE Latin1_General_CI_AS NOT NULL,
    [domainValue]    NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [userMessage]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [valueUsage]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [langCode]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [sortOrder]      SMALLINT       NOT NULL,
    [minAccessLevel] SMALLINT       NOT NULL,
    [status]         BIT            NOT NULL,
    CONSTRAINT [PK_dubbDomain] PRIMARY KEY CLUSTERED ([domainIntno] ASC)
);

