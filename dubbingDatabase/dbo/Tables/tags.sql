CREATE TABLE [dbo].[tags]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [TagTypeId] INT NOT NULL , 
    [Name] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_tags_To_tagTypes] FOREIGN KEY ([TagTypeId]) REFERENCES [tagTypes]([Id])
)
