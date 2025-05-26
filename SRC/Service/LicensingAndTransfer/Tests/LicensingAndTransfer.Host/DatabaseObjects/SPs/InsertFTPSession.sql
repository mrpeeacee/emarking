IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFTPSession]'))
BEGIN
	DROP PROCEDURE [dbo].[InsertFTPSession]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath   11/Jan/2010
Object Name:[InsertFTPSession]
Purpose:To Insert FTP Session.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [dbo].[InsertFTPSession]
(
    @ID BIGINT = 0 OUTPUT,
	@GUID NVARCHAR(MAX) = NULL,
	@MacID NVARCHAR(MAX) = NULL,
	@StartTime DATETIME = NULL,
	@EndTime DATETIME = NULL,
	@OperationType NVARCHAR(MAX) = NULL,
	@ServerType NVARCHAR(MAX) = NULL
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    IF NOT EXISTS (SELECT TOP 1 1 FROM [QPackRPack_FTPSession] WHERE [GUID]=@GUID)
        BEGIN
			INSERT INTO [QPackRPack_FTPSession] ([GUID],[MacID],[StartTime],[EndTime],[OperationType],[ServerType])
			VALUES (@GUID,@MacID,@StartTime,@EndTime,@OperationType,@ServerType)
            SET @ID = SCOPE_IDENTITY()
        END
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('InsertFTPSession' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@GUID=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@GUID+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@MacID=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@MacID+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@StartTime=' AS NVARCHAR(MAX)) + CAST(ISNULL(@StartTime,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@EndTime=' AS NVARCHAR(MAX)) + CAST(ISNULL(@EndTime,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@OperationType=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@OperationType+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ServerType=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ServerType+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO