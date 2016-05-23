CREATE TABLE [dbo].[employees] (
    [empIntno]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [empType]     NVARCHAR (50)  NOT NULL,
    [empUID]      NVARCHAR (50)  NULL,
    [fullName]    NVARCHAR (100) NOT NULL,
    [othFullName] NVARCHAR (100) NOT NULL,
    [mobileNo]    NVARCHAR (50)  NOT NULL,
    [landLineNo]  NVARCHAR (50)  NOT NULL,
    [email]       NVARCHAR (50)  NOT NULL,
    [status]      BIT            NOT NULL,
    [accountNo]   NVARCHAR (50)  NULL,
    CONSTRAINT [PK_employees] PRIMARY KEY CLUSTERED ([empIntno] ASC)
);

