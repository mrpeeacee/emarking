IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PersistTestCenter]'))
BEGIN
	DROP PROCEDURE [dbo].[PersistTestCenter]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath   11/Jan/2010
Object Name:[PersistTestCenter]
Purpose:This sp is to Persist Test center.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [dbo].[PersistTestCenter]
(
    @ID BIGINT = NULL,
	@MACID NVARCHAR(MAX) = NULL,
	@CenterName NVARCHAR(MAX) = NULL,
	@CenterCode NVARCHAR(MAX) = NULL,
	@IsAvailable BIT = NULL,
	@AddressID BIGINT = NULL,
	@CreatedBy BIGINT = NULL,
	@CreatedDate DATETIME = NULL,
	@ModifiedBy BIGINT = NULL,
	@ModifiedDate DATETIME = NULL,
	@ParentID BIGINT = NULL,
	@LocationID BIGINT = NULL,
    @Status NVARCHAR(MAX) = NULL OUTPUT
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
    SET @Status = 'RTC00'
    BEGIN TRY
	    IF NOT EXISTS (SELECT TOP 1 1 FROM [TestCenter] WHERE [ID]=@ID)
		    BEGIN
                SET IDENTITY_INSERT [dbo].[TestCenter] ON;
			    INSERT INTO [TestCenter] ([ID],[CenterName],[CenterCode],[IsAvailable],[AddressID],[IsDeleted],[CreatedBy],[CreatedDate],
                    [ModifiedBy],[ModifiedDate],[ParentID],[LocationID],[MacID])
			    VALUES (@ID,@CenterName,@CenterCode,@IsAvailable,@AddressID,0,@CreatedBy,@CreatedDate,
                    @ModifiedBy,@ModifiedDate,@ParentID,@LocationID,@MACID)
                SET IDENTITY_INSERT [dbo].[TestCenter] OFF;
                SET @Status = 'RTC01'
		    END
        ELSE
            BEGIN
			    UPDATE [TestCenter]
			    SET [CenterName]=ISNULL(@CenterName,[CenterName]),[CenterCode]=ISNULL(@CenterCode,[CenterCode]),
                    [IsAvailable]=ISNULL(@IsAvailable,[IsAvailable]),[AddressID]=ISNULL(@AddressID,[AddressID]),
                    [CreatedBy]=ISNULL(@CreatedBy,[CreatedBy]),[CreatedDate]=ISNULL(@CreatedDate,[CreatedDate]),
                    [ModifiedBy]=ISNULL(@ModifiedBy,[ModifiedBy]),[ModifiedDate]=ISNULL(@ModifiedDate,[ModifiedDate]),
                    [ParentID]=ISNULL(@ParentID,[ParentID]),[LocationID]=ISNULL(@LocationID,[LocationID]),
                    [MacID]=ISNULL(@MACID,[MacID])
                WHERE [ID]=@ID
                SET @Status = 'RTC05'
            END
    END TRY
    BEGIN CATCH 
        DECLARE @ErrorDetail AS NVARCHAR(MAX)
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('PersistTestCenter' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@MACID=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@MACID+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@CenterName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@CenterName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@CenterCode=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@CenterCode+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@IsAvailable=' AS NVARCHAR(MAX)) + CAST(ISNULL(@IsAvailable,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@AddressID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@AddressID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@CreatedBy=' AS NVARCHAR(MAX)) + CAST(ISNULL(@CreatedBy,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@CreatedDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@CreatedDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ModifiedBy=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ModifiedBy,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ModifiedDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ModifiedDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ParentID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ParentID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@LocationID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@LocationID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Status=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Status+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
        EXEC [GenerateErrorHandling] @ErrorDetail

        DECLARE @Exception AS NVARCHAR(MAX)  
        SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
        RAISERROR (@Exception, 16, 1)
    END CATCH
END
GO