CREATE TABLE [dbo].[contacts] (
    [contactIntno]     BIGINT         IDENTITY (1, 1) NOT NULL,
    [contactParty]     NVARCHAR (50)  NOT NULL,
    [partyIntno]       BIGINT         NOT NULL,
    [salutation]       NVARCHAR (50)  NOT NULL,
    [contactName]      NVARCHAR (100) NOT NULL,
    [othContactName]   NVARCHAR (100) NULL,
    [contactTitle]     NVARCHAR (50)  NULL,
    [contactPhoneNbr]  NVARCHAR (50)  NULL,
    [contactEmailAddr] NVARCHAR (50)  NULL,
    [mailingAddr]      NVARCHAR (MAX) NULL,
    [remarks]          NVARCHAR (MAX) NULL,
    [status]           BIT            NOT NULL,
    CONSTRAINT [PK_contacts] PRIMARY KEY CLUSTERED ([contactIntno] ASC)
);

