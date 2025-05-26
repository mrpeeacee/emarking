IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertFTPSessionDetails]'))
BEGIN
	DROP PROCEDURE [dbo].[InsertFTPSessionDetails]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath  11/Jan/2010
Object Name:[InsertFTPSessionDetails]
Purpose:To Insert FTP Session Details.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [dbo].[InsertFTPSessionDetails]
(
	@FTPSessionID BIGINT = NULL,
	@TestCenterID BIGINT = NULL,
	@ScheduleID BIGINT = NULL,
	@PackageGeneratedDate DATETIME = NULL,
	@PackageName NVARCHAR(MAX) = NULL,
	@PackagePassword NVARCHAR(MAX) = NULL,
    @IsCentralizedPackage BIT = NULL,
    @ID BIGINT = 0 OUTPUT
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    DECLARE @PackageType NVARCHAR(MAX), @ServerType NVARCHAR(MAX), @Operation NVARCHAR(MAX)
    (SELECT TOP 1 @PackageType=[OperationType], @ServerType=[ServerType] FROM [QPackRPack_FTPSession] WHERE [ID]=@FTPSessionID)

    SET @Operation = @PackageType
    IF (CHARINDEX('QPack',@PackageType) > 0)
        SET @PackageType = 'QPack'
    IF (CHARINDEX('RPack',@PackageType) > 0)
        SET @PackageType = 'RPack'

    INSERT INTO [dbo].[QPackRPack_FTPSessionDetails] ([FTPSessionID],[TestCenterID],[ScheduleID],[PackageGeneratedDate],[PackageName])
    VALUES (@FTPSessionID,@TestCenterID,@ScheduleID,@PackageGeneratedDate,@PackageName)
    
    SET @ID = SCOPE_IDENTITY()
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('InsertFTPSessionDetails' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@FTPSessionID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@FTPSessionID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TestCenterID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TestCenterID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageGeneratedDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@PackageGeneratedDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackageName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackagePassword=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackagePassword+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@IsCentralizedPackage=' AS NVARCHAR(MAX)) + CAST(ISNULL(@IsCentralizedPackage,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO