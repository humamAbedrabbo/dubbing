CREATE TABLE [dbo].[sceneScentences]
(
	[scentenceIntno] INT NOT NULL Identity(1,1) PRIMARY KEY, 
    [dialogIntno] INT NOT NULL, 
    [scentenceNo] INT NOT NULL, 
    [workCharacterIntno] BIGINT NULL, 
    [characterName] NVARCHAR(100) NULL, 
    [startTime] NVARCHAR(20) NULL, 
    [endTime] NVARCHAR(20) NULL, 
    [scentence] NVARCHAR(500) NOT NULL, 
    CONSTRAINT [FK_sceneScentences_To_sceneDialogs] FOREIGN KEY (dialogIntno) REFERENCES sceneDialogs(dialogIntno), 
    CONSTRAINT [FK_sceneScentences_To_workCharacters] FOREIGN KEY (workCharacterIntno) REFERENCES workCharacters(workCharacterIntno)
)
