-- ================================================
-- Template generated from Template Explorer using:
-- Create Procedure (New Menu).SQL
--
-- Use the Specify Values for Template Parameters 
-- command (Ctrl-Shift-M) to fill in the parameter 
-- values below.
--
-- This block of comments will not be included in
-- the definition of the procedure.
-- ================================================
USE [DUBBDB]
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Wael Alshatti
-- Create date: 09 Aug 2016
-- Description:	Archive Endorsed Work
-- =============================================
ALTER PROCEDURE archiveEndorsedWork 
	-- Add the parameters for the stored procedure here
	@workIntno bigint = 0,
	@respMsg nvarchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    begin try
		begin transaction
			-- delete studio and related: cascade delete appointments and studio episodes
			delete from studios where workIntno = @workIntno;
			
			-- delete schedule
			delete from dubbingTrnHdrs where dubbTrnHdrIntno not in
					(select dubbTrnHdrIntno from studios);
			
			-- delete scenes and related: cascade delete dialogs and subtitles
			delete from scenes where orderTrnHdrIntno in
					(select orderTrnHdrIntno from orderTrnHdrs where workIntno = @workIntno);
					
			-- delete dubbing sheets and related: cascade delete dubbingSheetDtls
			delete from dubbingSheetHdrs where orderTrnHdrIntno in
					(select orderTrnHdrIntno from orderTrnHdrs where workIntno = @workIntno);
			
			-- endorse the work(contract) by changing its status to "03"
			update agreementWorks
			set status = '03'
			where workIntno = @workIntno;
			
			-- finally log work endorsement
			update logWorks
			set endorsedDate = GETDATE(), endorsedYear = YEAR(GETDATE()), endorsedMonth = MONTH(GETDATE()),
				lastUpdate = GETDATE(), updatedBy = @respMsg
			where workIntno = @workIntno;
			
			set @respMsg = null;
		commit
    end try
    
    begin catch
		select @respMsg = ERROR_NUMBER() + '-' + ERROR_MESSAGE();
	end catch
END
GO
