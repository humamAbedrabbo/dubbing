CREATE TABLE [dbo].[tagTemplateDtls]
(
	[tagTemplateDtlIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [tagTemplateHdrIntno] BIGINT NOT NULL, 
    [tagId] INT NOT NULL, 
    [MinScore] INT NOT NULL DEFAULT 2, 
    [Weight] INT NULL DEFAULT 1, 
    CONSTRAINT [FK_tagTemplateDtls_To_tagTemplateHdrs] FOREIGN KEY ([tagTemplateHdrIntno]) REFERENCES [tagTemplateHdrs]([tagTemplateHdrIntno]) on delete cascade, 
    CONSTRAINT [FK_tagTemplateDtls_To_tags] FOREIGN KEY ([tagId]) REFERENCES [tags]([Id])
)
