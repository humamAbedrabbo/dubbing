CREATE TABLE [dbo].[contacts] (
    [contactIntno]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [contactParty]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [partyIntno]       BIGINT         NOT NULL,
    [salutation]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [contactName]      NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [othContactName]   NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [contactTitle]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [contactPhoneNbr]  NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [contactEmailAddr] NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [mailingAddr]      NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [remarks]          NVARCHAR (MAX) COLLATE Latin1_General_CI_AS NULL,
    [status]           BIT            NOT NULL,
    CONSTRAINT [PK_contacts] PRIMARY KEY CLUSTERED ([contactIntno] ASC)
);

