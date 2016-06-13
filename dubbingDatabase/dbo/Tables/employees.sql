CREATE TABLE [dbo].[employees] (
    [empIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [empType]     NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [empUID]      NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    [fullName]    NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [othFullName] NVARCHAR (100) COLLATE Latin1_General_CI_AS NOT NULL,
    [mobileNo]    NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [landLineNo]  NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [email]       NVARCHAR (50)  COLLATE Latin1_General_CI_AS NOT NULL,
    [status]      BIT            NOT NULL,
    [accountNo]   NVARCHAR (50)  COLLATE Latin1_General_CI_AS NULL,
    CONSTRAINT [PK_employees] PRIMARY KEY CLUSTERED ([empIntno] ASC)
);

