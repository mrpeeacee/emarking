IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SearchPackageStatistics]'))
BEGIN
	DROP PROCEDURE [dbo].[SearchPackageStatistics]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath    16/Jan/2010
Object Name:[SearchPackageStatistics]
Purpose:To Search Package Statistics.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [dbo].[SearchPackageStatistics]
(
	@IDs NVARCHAR(MAX) = NULL
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    IF ( (@IDs IS NULL) OR (LEN(LTRIM(RTRIM(@IDs))) <= 0) )
        RETURN

    DECLARE @Tbl_IDs TABLE (ID INT)
    SET @IDs = @IDs + ',' -- Append comma
    DECLARE @Pos1 INT, @pos2 INT -- Indexes to keep the position of searching
    SET @Pos1=1 -- Start from first character
    SET @Pos2=1
    WHILE @Pos1<Len(@IDs)
        BEGIN
            SET @Pos1 = CharIndex(',',@IDs,@Pos1)
            INSERT @Tbl_IDs
            SELECT CAST(SUBSTRING(@IDs,@Pos2,@Pos1-@Pos2) AS BIGINT)
            SET @Pos2=@Pos1+1 -- Go to next non comma character
            SET @Pos1 = @Pos1+1 -- Search from the next charcater
        END

    SELECT PS.[ID],PS.[ScheduleID],PS.[TestCenterID],PS.[PackageType],PS.[GeneratedDate],
        PS.[TransferredToDataExchangeServer],PS.[TransferredToDataExchangeServerOn],PS.[TransferredToTestCenter],PS.[TransferredToTestCenterOn],
        PS.[TransferredToDataCenterDistributed],PS.[TransferredToDataCenterDistributedOn],
        PS.[TransferredToDataCenterCentralized],PS.[TransferredToDataCenterCentralizedOn],
        PS.[RecievedFromDataExchangeServer],PS.[RecievedFromDataExchangeServerOn],PS.[RecievedFromTestCenter],PS.[RecievedFromTestCenterOn],
        PS.[RecievedFromDataCenterDistributed],PS.[RecievedFromDataCenterDistributedOn],
        PS.[RecievedFromDataCenterCentralized],PS.[RecievedFromDataCenterCentralizedOn],
        PS.[PackageName],PS.[PackagePassword],PS.[PackagePath],PS.[OrganizationID],PS.[OrganizationName],PS.[DivisionID],PS.[DivisionName],
        PS.[ProcessID],PS.[ProcessName],PS.[EventID],PS.[EventName],PS.[BatchID],PS.[BatchName],PS.[TestCenterName],
        PS.[ScheduleDate],PS.[ScheduleStartDate],PS.[ScheduleEndDate],
        PS.[LeadTimeForQPackDispatchInMinutes],PS.[DeleteQPackAfterExamination],PS.[RPackToBeSentImmediatelyAfterExamination],
        PS.[RPackToBeSentAtEOD],PS.[DeleteRPackAfterExamination],PS.[DeleteRPackAtEOD],PS.[PackageDeletedStatus],PS.[IsCentralizedPackage],
        PS.[Extension1],PS.[Extension2],PS.[Extension3],PS.[Extension4],PS.[Extension5],PS.[ScheduleDetailID],
        PS.[LoadedDateTestCenter],PS.[IsPackageGenerated],PS.[IsLatest],PS.[LoadedDateCentralized],PS.[LoadedDateDistributed]
    FROM [QPackRPack_PackageStatistics] PS
    WHERE PS.[ID] IN (SELECT [ID] FROM @Tbl_IDs)
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('SearchPackageStatistics' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@IDs=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@IDs+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO