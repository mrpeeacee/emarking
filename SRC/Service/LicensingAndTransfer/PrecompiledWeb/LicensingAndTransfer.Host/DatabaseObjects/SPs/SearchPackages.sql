IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SearchPackages]'))
BEGIN
	DROP PROCEDURE [dbo].[SearchPackages]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath  11/Jan/2010
Object Name:[SearchPackages]
Purpose:To Search Packages.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
1; Manohar.P.S; 24-Apr-2010; 1.4.0; Manohar.V; Completed
2; Manohar.P.S; 12-May-2010; 1.4.0; Manohar.PS; Completed
*******************************************************************************************************/
CREATE PROC [dbo].[SearchPackages]
(
    @LoadPackagesFromDCDToDCC BIT = 0,
	@MacID NVARCHAR(MAX) = NULL,
    @ServerType NVARCHAR(MAX) = NULL,
	@PackageType NVARCHAR(MAX) = NULL
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    IF (@ServerType = 'TestCenter')
        BEGIN
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
            WHERE PS.[TestCenterID] IN (SELECT [ID] FROM [TestCenter] WHERE [MacID]=@MacID)
				AND ISNULL(PS.[RecievedFromTestCenter],0) IN (0,2)
				AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=0 AND PS.[IsLatest] = 1
                AND ( PS.[LeadTimeForQPackDispatchInMinutes] <= 0 OR PS.[LeadTimeForQPackDispatchInMinutes] >= DATEDIFF(mi,GETUTCDATE(),PS.[ScheduleStartDate]) )
        END

    IF (@ServerType = 'DataServer' OR @ServerType = 'DataCenter')
        BEGIN
            DECLARE @IsCentralized BIT
            SET @IsCentralized = (SELECT [IsCentralized] FROM [QPackRPack_ServerConfigurationDetails] WHERE [MacID]=@MacID)
			IF (@PackageType = 'QPack')
				BEGIN
                    IF (@IsCentralized = 0)
                        BEGIN
                            --  Retrieving all the R-Packs recieved at Distributed Data Center
			                SELECT PS.[ScheduleID],PS.[TestCenterID],PS.[ScheduleDetailID]
                                INTO #RPack
			                FROM [QPackRPack_PackageStatistics] PS
			                WHERE PS.[PackageType]='RPack'

                            --  To load all the distributed packages
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
					        WHERE PS.[IsLatest] = 1 AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=0
                            AND ISNULL(PS.[RecievedFromDataCenterDistributed],0) IN (0,2)
                            AND PS.[LoadedDateTestCenter] IS NOT NULL

                            DROP TABLE #RPack
                        END
                    ELSE IF (@IsCentralized = 1)
                        BEGIN
                            IF (@LoadPackagesFromDCDToDCC = 0)
                                BEGIN
                                    --  To load all the centralized packages
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
					                WHERE PS.[IsLatest] = 1 AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=1
                                        AND ISNULL(PS.[RecievedFromDataCenterCentralized],0) IN (0,2)
                                        AND PS.[RecievedFromDataExchangeServer]=1
                                END
                            ELSE
                                BEGIN
                                    --  Retrieving all the R-Packs recieved at Distributed Data Center
					                SELECT PS.[ScheduleID],PS.[TestCenterID],PS.[ScheduleDetailID]
                                        INTO #RPacks
					                FROM [QPackRPack_PackageStatistics] PS
					                WHERE PS.[PackageType]='RPack'

                                    --  To load all the distributed packages, in Centralized Server
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
                                    INNER JOIN #RPacks RS ON PS.[ScheduleID] = RS.[ScheduleID] AND PS.[TestCenterID] = RS.[TestCenterID]
                                        AND PS.[ScheduleDetailID] = RS.[ScheduleDetailID]
					                WHERE PS.[IsLatest] = 1 AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=0
                                        AND ISNULL(PS.[RecievedFromDataCenterCentralized],0) IN (0,2)
                                        AND PS.[LoadedDateDistributed] IS NOT NULL

                                    DROP TABLE #RPacks
                                END
                        END
				END
			ELSE
				BEGIN
                    IF (@IsCentralized = 0)
                        BEGIN
                            --  To load all the distributed packages
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
					        WHERE PS.[IsLatest] = 1 AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=0
                                AND ISNULL(PS.[RecievedFromDataCenterDistributed],0) IN (0,2)
                                AND PS.[RecievedFromDataExchangeServer]=1
                        END
                    ELSE IF (@IsCentralized = 1)
                        BEGIN
                            IF (@LoadPackagesFromDCDToDCC = 0)
                                BEGIN
                                    --  To load all the centralized packages
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
					                WHERE PS.[IsLatest] = 1 AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=1
                                        AND ISNULL(PS.[RecievedFromDataCenterCentralized],0) IN (0,2)
                                        AND PS.[RecievedFromDataExchangeServer]=1
                                END
                            ELSE
                                BEGIN
                                    --  To load all the distributed packages, in Centralized Server
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
					                WHERE PS.[IsLatest] = 1 AND PS.[PackageType]=@PackageType AND PS.[IsCentralizedPackage]=0
                                        AND ISNULL(PS.[RecievedFromDataCenterCentralized],0) IN (0,2)
                                        AND PS.[LoadedDateDistributed] IS NOT NULL
                                END
                        END
				END
        END
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('SearchPackages' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@LoadPackagesFromDCDToDCC=' AS NVARCHAR(MAX)) + CAST(ISNULL(@LoadPackagesFromDCDToDCC,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@MacID=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@MacID+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ServerType=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ServerType+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageType=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackageType+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO