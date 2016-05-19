CREATE TABLE [dbo].[clients] (
    [clientIntno]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [clientName]         NVARCHAR (100) NOT NULL,
    [clientShortName]    NVARCHAR (50)  NULL,
    [othClientName]      NVARCHAR (100) NULL,
    [othClientShortName] NVARCHAR (50)  NULL,
    [status]             NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_clients] PRIMARY KEY CLUSTERED ([clientIntno] ASC)
);

