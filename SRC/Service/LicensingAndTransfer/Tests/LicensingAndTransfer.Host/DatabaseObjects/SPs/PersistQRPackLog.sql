IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersistQRPackLog]'))
BEGIN
	DROP PROCEDURE [dbo].[PersistQRPackLog]
END
GO
/*******************************************************************************************************
Created By & Date:
Object Name:[PersistQRPackLog]
Purpose:To Persist QPackRPack Logs
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
1; Manohar.PS; 29/Apr/2010; 1.4.0; Manohar. V; Completed
*******************************************************************************************************/
CREATE PROC [dbo].[PersistQRPackLog]
(
    @Date DATETIME = NULL,
    @MessageDescription NVARCHAR(MAX) = NULL,
    @ErrorMessage NVARCHAR(MAX) = NULL,
    @ErrorStackTrace NVARCHAR(MAX) = NULL,
    @SessionID NVARCHAR(MAX) = NULL,
    @ApplicationIdentifier NVARCHAR(MAX) = NULL
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    INSERT INTO QPackRPack_Logs ([Date],[MessageDescription],[ErrorMessage],[ErrorStackTrace],[SessionID],[ApplicationIdentifier])
    VALUES (@Date,@MessageDescription,@ErrorMessage,@ErrorStackTrace,@SessionID,@ApplicationIdentifier)
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('PersistQRPackLog' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Date=' AS NVARCHAR(MAX)) + CAST(ISNULL(@Date,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@MessageDescription=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@MessageDescription+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ErrorMessage=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ErrorMessage+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ErrorStackTrace=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ErrorStackTrace+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@SessionID=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@SessionID+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ApplicationIdentifier=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ApplicationIdentifier+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO