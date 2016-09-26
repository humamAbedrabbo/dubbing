CREATE VIEW [dbo].[paymentsDueVW]
AS
SELECT     dbo.orderTrnHdrs.workIntno, dbo.agreementWorks.workName, dbo.dubbingSheetHdrs.voiceActorIntno, dbo.dubbingSheetHdrs.actorName, 
                      dbo.dubbingSheetDtls.dubbingDate, COUNT(dbo.dubbingSheetDtls.sceneNo) AS totalUnits
FROM         dbo.dubbingSheetDtls INNER JOIN
                      dbo.dubbingSheetHdrs ON dbo.dubbingSheetDtls.dubbSheetHdrIntno = dbo.dubbingSheetHdrs.dubbSheetHdrIntno INNER JOIN
                      dbo.orderTrnHdrs ON dbo.dubbingSheetDtls.orderTrnHdrIntno = dbo.orderTrnHdrs.orderTrnHdrIntno AND 
                      dbo.dubbingSheetHdrs.orderTrnHdrIntno = dbo.orderTrnHdrs.orderTrnHdrIntno INNER JOIN
                      dbo.agreementWorks ON dbo.orderTrnHdrs.workIntno = dbo.agreementWorks.workIntno
WHERE     (dbo.agreementWorks.status = N'01' COLLATE SQL_Latin1_General_CP1_CI_AS) AND (dbo.dubbingSheetDtls.dubbingDate NOT IN
                          (SELECT     dbo.paymentDetails.dubbingDate
                            FROM          dbo.paymentDetails INNER JOIN
                                                   dbo.payments ON dbo.payments.paymentIntno = dbo.paymentDetails.paymentIntno
                            WHERE      (dbo.payments.voiceActorIntno = dbo.dubbingSheetHdrs.voiceActorIntno) AND (dbo.payments.fullName = dbo.dubbingSheetHdrs.actorName COLLATE SQL_Latin1_General_CP1_CI_AS)))
GROUP BY dbo.orderTrnHdrs.workIntno, dbo.agreementWorks.workName, dbo.dubbingSheetHdrs.voiceActorIntno, dbo.dubbingSheetHdrs.actorName, 
                      dbo.dubbingSheetDtls.dubbingDate