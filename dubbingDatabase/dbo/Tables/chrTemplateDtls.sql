CREATE TABLE [dbo].[chrTemplateDtls]
(
	[chrTemplateDtlIntno] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [chrTemplateHdrIntno] BIGINT NOT NULL, 
    [tagId] INT NOT NULL, 
    [Weight] INT NOT NULL DEFAULT 1, 
    CONSTRAINT [FK_chrTemplateDtls_To_chrTemplateHdrs] FOREIGN KEY ([chrTemplateHdrIntno]) REFERENCES [chrTemplateHdrs]([chrTemplateHdrIntno]) on delete cascade, 
    CONSTRAINT [FK_chrTemplateDtls_To_tags] FOREIGN KEY ([tagId]) REFERENCES [tags]([Id])
)
