CREATE TABLE [dbo].[tagTemplateHdrs]
(
	[tagTemplateHdrIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Title] NVARCHAR(150) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Text] NVARCHAR(MAX) NULL
)
