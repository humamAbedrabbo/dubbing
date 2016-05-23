CREATE TABLE [dbo].[sceneDialogs]
(
	[dialogIntno] INT NOT NULL Identity(1,1) PRIMARY KEY, 
    [sceneIntno] INT NOT NULL, 
    [dialogNo] INT NOT NULL, 
    CONSTRAINT [FK_sceneDialogs_To_adaptationScenes] FOREIGN KEY (sceneIntno) REFERENCES adaptationScenes(sceneIntno)
)
