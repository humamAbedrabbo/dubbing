CREATE TABLE [dbo].[clients] (
    [clientIntno]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [clientName]         NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [clientShortName]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [othClientName]      NVARCHAR (100) COLLATE Latin1_General_CI_AS NULL,
    [othClientShortName] NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [status]             NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK_clients] PRIMARY KEY CLUSTERED ([clientIntno] ASC)
);

