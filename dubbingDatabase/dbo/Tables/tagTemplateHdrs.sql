CREATE TABLE [dbo].[tagTemplateHdrs]
(
	[tagTemplateHdrIntno] BIGINT NOT NULL PRIMARY KEY, 
    [Title] NVARCHAR(150) NOT NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Text] NVARCHAR(MAX) NULL
)
