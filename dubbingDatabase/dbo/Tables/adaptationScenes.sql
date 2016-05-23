CREATE TABLE [dbo].[adaptationScenes]
(
	[sceneIntno] INT NOT NULL Identity(1,1) PRIMARY KEY, 
    [orderTrnHdrIntno] BIGINT NOT NULL, 
    [sceneNo] INT NOT NULL, 
    [summary] NVARCHAR(150) NULL, 
    CONSTRAINT [FK_adaptationScenes_To_orderAdaptations] FOREIGN KEY (orderTrnHdrIntno) REFERENCES orderAdaptations(orderTrnHdrIntno)
)
