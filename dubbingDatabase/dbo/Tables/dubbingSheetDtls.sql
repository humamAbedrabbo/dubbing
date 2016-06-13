CREATE TABLE [dbo].[dubbingSheetDtls] (
    [dubbSheetDtlIntno] BIGINT        IDENTITY (1, 1) NOT NULL,
    [orderTrnHdrIntno]  BIGINT        NOT NULL,
    [dubbSheetHdrIntno] BIGINT        NOT NULL,
    [sceneNo]           SMALLINT      NOT NULL,
    [startTimeCode]     NVARCHAR (50) NOT NULL,
    [isTaken]           BIT           NOT NULL,
    [studioNo]          NVARCHAR (50) NULL,
    [supervisor]        BIGINT        NULL,
    [soundTechnician]   BIGINT        NULL,
    [assistant]         BIGINT        NULL,
    [takenTimeStamp]    DATETIME      NULL,
    CONSTRAINT [PK_dubbingSheetDtls] PRIMARY KEY CLUSTERED ([dubbSheetDtlIntno] ASC),
    CONSTRAINT [FK_dubbingSheetDtls_dubbingSheetHdrs] FOREIGN KEY ([dubbSheetHdrIntno]) REFERENCES [dbo].[dubbingSheetHdrs] ([dubbSheetHdrIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_employees] FOREIGN KEY ([supervisor]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_employees1] FOREIGN KEY ([soundTechnician]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_employees2] FOREIGN KEY ([assistant]) REFERENCES [dbo].[employees] ([empIntno]),
    CONSTRAINT [FK_dubbingSheetDtls_orderTrnHdrs] FOREIGN KEY ([orderTrnHdrIntno]) REFERENCES [dbo].[orderTrnHdrs] ([orderTrnHdrIntno])
);


GO
CREATE TRIGGER [dbo].[sceneTakenTrigger]
   ON  dbo.dubbingSheetDtls 
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	if UPDATE (isTaken)
	begin
		declare @isTaken bit;
		declare @orderTrnHdrIntno bigint;
		declare @cnt int;
		
		select @isTaken = i.isTaken, @orderTrnHdrIntno = i.orderTrnHdrIntno from inserted i;
		if (@isTaken = 1)
		begin
			/* episode start dubbing */
			select @cnt = COUNT(*)
			from dbo.dubbingSheetDtls
			where orderTrnHdrIntno = @orderTrnHdrIntno and isTaken = 1;
			
			if (@cnt = 1) 
			begin
				begin transaction;									
				update  dbo.orderTrnHdrs
				set		startDubbing = SYSDATETIME()
				where	orderTrnHdrIntno = @orderTrnHdrIntno;
				commit;
			end;
			
			/* episode finished dubbing */
			select @cnt = COUNT(*)
			from dbo.dubbingSheetDtls
			where orderTrnHdrIntno = @orderTrnHdrIntno and isTaken = 0;
			
			if (@cnt = 0)
			begin
				begin transaction;									
				update	dbo.orderTrnHdrs
				set		endDubbing = SYSDATETIME(), startMixage = SYSDATETIME()
				where	orderTrnHdrIntno = @orderTrnHdrIntno;
				commit;
			end;
			
			/* check if the whole dubbing schedule (for a week) is finished */
			declare @dubbTrnHdrIntno bigint;
			declare schedulesList cursor forward_only for
				select dubbTrnHdrIntno
				from dbo.dubbingTrnDtls
				where orderTrnHdrIntno = @orderTrnHdrIntno;
			open schedulesList;
			fetch next from schedulesList into @dubbTrnHdrIntno;
			while @@FETCH_STATUS = 0
			begin
				select @cnt = COUNT(*)
				from dbo.dubbingTrnDtls
				where dubbTrnHdrIntno = @dubbTrnHdrIntno;
				if (@cnt = 0)
				begin
					begin transaction;
					update dbo.dubbingTrnHdrs
					set status = 0
					where dubbTrnHdrIntno = @dubbTrnHdrIntno;
					commit;
				end;
				fetch next from schedulesList into @dubbTrnHdrIntno;
			end;
			close schedulesList;
			deallocate schedulesList;
		end;
	end;
END
