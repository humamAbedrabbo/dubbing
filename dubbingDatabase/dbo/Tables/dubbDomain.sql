CREATE TABLE [dbo].[dubbDomain] (
    [domainIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [domainName]     NVARCHAR (50)  NOT NULL,
    [domainCode]     NVARCHAR (10)  NOT NULL,
    [domainValue]    NVARCHAR (100) NOT NULL,
    [userMessage]    NVARCHAR (50)  NULL,
    [valueUsage]     NVARCHAR (50)  NOT NULL,
    [langCode]       NVARCHAR (50)  NOT NULL,
    [sortOrder]      SMALLINT       NOT NULL,
    [minAccessLevel] SMALLINT       NOT NULL,
    [status]         BIT            NOT NULL,
    CONSTRAINT [PK_dubbDomain] PRIMARY KEY CLUSTERED ([domainIntno] ASC)
);

