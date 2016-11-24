CREATE TABLE [dbo].[chrTemplateHdrs]
(
	[chrTemplateHdrIntno] BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY, 
    [workCharacterIntno] BIGINT NOT NULL, 
    [tagTemplateHdrIntno] BIGINT NOT NULL, 
    CONSTRAINT [FK_chrTemplateHdrs_To_workCharacters] FOREIGN KEY ([workCharacterIntno]) REFERENCES [workCharacters]([workCharacterIntno]) on delete cascade, 
    CONSTRAINT [FK_chrTemplateHdrs_To_tagTemplateHdrs] FOREIGN KEY ([tagTemplateHdrIntno]) REFERENCES [tagTemplateHdrs]([tagTemplateHdrIntno]) on delete cascade
)
