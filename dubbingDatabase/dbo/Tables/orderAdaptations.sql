CREATE TABLE [dbo].[orderAdaptations]
(
	[orderTrnHdrIntno] BIGINT NOT NULL PRIMARY KEY, 
    [createdOn] DATETIME NOT NULL DEFAULT GETDATE(), 
    [status] NVARCHAR(10) NOT NULL DEFAULT '01', 
    CONSTRAINT [FK_orderAdaptations_To_orderTrnHdrs] FOREIGN KEY (orderTrnHdrIntno) REFERENCES orderTrnHdrs(orderTrnHdrIntno)
)
