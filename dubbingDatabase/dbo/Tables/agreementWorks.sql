CREATE TABLE [dbo].[agreementWorks] (
    [workIntno]            BIGINT         IDENTITY (1, 1) NOT NULL,
    [agreementIntno]       BIGINT         NOT NULL,
    [workType]             NVARCHAR (50)  NOT NULL,
    [workName]             NVARCHAR (100) NOT NULL,
    [othWorkName]          NVARCHAR (100) NULL,
    [workNationality]      NVARCHAR (50)  NOT NULL,
    [workOriginalLanguage] NVARCHAR (50)  NOT NULL,
    [firstEpisodeShowDate] DATE           NULL,
    [totalNbrEpisodes]     SMALLINT       NOT NULL,
    [totalWeekNbrEpisodes] SMALLINT       NOT NULL,
    [contactIntno]         BIGINT         NULL,
    [status]               NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_agreementWorks] PRIMARY KEY CLUSTERED ([workIntno] ASC),
    CONSTRAINT [FK_agreementWorks_agreements] FOREIGN KEY ([agreementIntno]) REFERENCES [dbo].[agreements] ([agreementIntno]),
    CONSTRAINT [FK_agreementWorks_contacts] FOREIGN KEY ([contactIntno]) REFERENCES [dbo].[contacts] ([contactIntno])
);

