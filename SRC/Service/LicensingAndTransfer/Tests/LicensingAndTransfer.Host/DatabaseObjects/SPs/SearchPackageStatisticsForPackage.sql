IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SearchPackageStatisticsForPackage]'))
BEGIN
	DROP PROCEDURE [dbo].[SearchPackageStatisticsForPackage]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath  16/Jan/2010
Object Name:[SearchPackageStatisticsForPackage]
Purpose:To Search Package Statistics For Package
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [dbo].[SearchPackageStatisticsForPackage]
(
	@PackageName NVARCHAR(MAX) = NULL,
    @PackagePassword NVARCHAR(MAX) = NULL
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
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
    WHERE PS.[PackageName] = @PackageName AND PS.[PackagePassword] = ISNULL(@PackagePassword,PS.[PackagePassword])
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('SearchPackageStatisticsForPackage' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackageName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackagePassword=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackagePassword+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO