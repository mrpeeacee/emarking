IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ValidateMacID]'))
BEGIN
	DROP PROCEDURE [dbo].[ValidateMacID] 
END
GO
/******************************************************************************************************
Created By & Date: 
Object Name:[ValidateMacID]
Purpose:This sp is to Validate the MacId  
Version:1.4.0
Remarks:
Reviewed By & Date:
Fix Status: 
History 
#;Who;Date;Version;Review By; Status
******************************************************************************************************/ 
CREATE PROC [dbo].[ValidateMacID]
(
    @MacID NVARCHAR(MAX) = NULL,
    @ServerType NVARCHAR(MAX) = NULL,
    @Status NVARCHAR(MAX) = NULL OUTPUT
)
--With Encryption
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
	SET @Status = 'S000'
	IF (@ServerType = 'TestCenter')
		BEGIN
			IF EXISTS (SELECT TOP 1 1 FROM [TestCenter] WHERE [MacID]=@MacID)
				BEGIN
					SET @Status = 'S001'
				END
			ELSE
				BEGIN
					SET @Status = 'S004'
				END
		END
	IF (@ServerType = 'ControllerOfExamination')
		BEGIN
			IF EXISTS (SELECT TOP 1 1 FROM [QPackRPack_ServerConfigurationDetails] WHERE [MacID]=@MacID)
				BEGIN
					SET @Status = 'S001'
				END
			ELSE
				BEGIN
					SET @Status = 'S004'
				END
		END
	IF (@ServerType = 'DataCenter' OR @ServerType = 'DataServer')
		BEGIN
			IF EXISTS (SELECT TOP 1 1 FROM [QPackRPack_ServerConfigurationDetails] WHERE [MacID]=@MacID)
				BEGIN
					SET @Status = 'S001'
				END
			ELSE
				BEGIN
					SET @Status = 'S004'
				END
		END
END TRY
BEGIN CATCH
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
	SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('ValidateMacID' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
	SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@MacID=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@MacID+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
	SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ServerType=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ServerType+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
	SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Status=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Status+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
	EXEC [GenerateErrorHandling] @ErrorDetail
	
	DECLARE @Exception AS NVARCHAR(MAX)  
	SET @Exception=ERROR_MESSAGE() + ' -> ' + @ErrorDetail 
	RAISERROR (@Exception, 16, 1)
END CATCH
SET NOCOUNT OFF;
END
GO