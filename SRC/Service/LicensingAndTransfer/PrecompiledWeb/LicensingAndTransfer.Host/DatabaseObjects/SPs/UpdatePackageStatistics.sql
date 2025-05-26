IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePackageStatistics]'))
BEGIN
	DROP PROCEDURE [dbo].[UpdatePackageStatistics]
END
GO
/*******************************************************************************************************
Created By & Date:Bharath   12/Jan/2010
Object Name:[UpdatePackageStatistics]
Purpose:To Update Package Statistics.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [UpdatePackageStatistics]
(
	@ID BIGINT = NULL,
	@ScheduleID BIGINT = NULL,
	@TestCenterID BIGINT = NULL,
	@PackageType NVARCHAR(MAX) = NULL,
	@GeneratedDate DATETIME = NULL,
	@TransferredToDataExchangeServer BIT = NULL,
	@TransferredToDataExchangeServerOn DATETIME = NULL,
	@TransferredToTestCenter BIT = NULL,
	@TransferredToTestCenterOn DATETIME = NULL,
	@TransferredToDataCenterDistributed BIT = NULL,
	@TransferredToDataCenterDistributedOn DATETIME = NULL,
	@TransferredToDataCenterCentralized BIT = NULL,
	@TransferredToDataCenterCentralizedOn DATETIME = NULL,
	@RecievedFromDataExchangeServer INT = NULL,
	@RecievedFromDataExchangeServerOn DATETIME = NULL,
	@RecievedFromTestCenter INT = NULL,
	@RecievedFromTestCenterOn DATETIME = NULL,
	@RecievedFromDataCenterDistributed INT = NULL,
	@RecievedFromDataCenterDistributedOn DATETIME = NULL,
    @RecievedFromDataCenterCentralized INT = NULL,
    @RecievedFromDataCenterCentralizedOn DATETIME = NULL,
	@PackageName NVARCHAR(MAX) = NULL,
	@PackagePassword NVARCHAR(MAX) = NULL,
	@PackagePath NVARCHAR(MAX) = NULL,
	@OrganizationID BIGINT = NULL,
	@OrganizationName NVARCHAR(MAX) = NULL,
	@DivisionID BIGINT = NULL,
	@DivisionName NVARCHAR(MAX) = NULL,
	@ProcessID BIGINT = NULL,
	@ProcessName NVARCHAR(MAX) = NULL,
	@EventID BIGINT = NULL,
	@EventName NVARCHAR(MAX) = NULL,
	@BatchID BIGINT = NULL,
	@BatchName NVARCHAR(MAX) = NULL,
	@TestCenterName NVARCHAR(MAX) = NULL,
	@ScheduleDate DATETIME = NULL,
	@ScheduleStartDate DATETIME = NULL,
	@ScheduleEndDate DATETIME = NULL,
    @LeadTimeForQPackDispatchInMinutes INT = NULL,
    @DeleteQPackAfterExamination BIT = NULL,
    @RPackToBeSentImmediatelyAfterExamination BIT = NULL,
    @RPackToBeSentAtEOD BIT = NULL,
    @DeleteRPackAfterExamination BIT = NULL,
    @DeleteRPackAtEOD BIT = NULL,
    @PackageDeletedStatus BIT = NULL,
    @IsCentralizedPackage BIT = NULL,
    @Extension1 NVARCHAR(MAX) = NULL,
    @Extension2 NVARCHAR(MAX) = NULL,
    @Extension3 NVARCHAR(MAX) = NULL,
    @Extension4 NVARCHAR(MAX) = NULL,
    @Extension5 NVARCHAR(MAX) = NULL,
	@ScheduleDetailID BIGINT = 0,
    @LoadedDateTestCenter DATETIME = NULL,
    @IsPackageGenerated BIT = NULL,
    @IsLatest BIT = NULL,
    @LoadedDateCentralized DATETIME = NULL,
    @LoadedDateDistributed DATETIME = NULL
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    DECLARE @RowCount BIGINT
	/* Truncating the white spaces at left & right positions Starts */
	SET @PackageType = LTRIM(RTRIM(@PackageType))
	SET @PackageName = LTRIM(RTRIM(@PackageName))
	SET @PackagePassword = LTRIM(RTRIM(@PackagePassword))
	SET @PackagePath = LTRIM(RTRIM(@PackagePath))
	SET @OrganizationName = LTRIM(RTRIM(@OrganizationName))
	SET @DivisionName = LTRIM(RTRIM(@DivisionName))
	SET @ProcessName = LTRIM(RTRIM(@ProcessName))
	SET @EventName = LTRIM(RTRIM(@EventName))
	SET @BatchName = LTRIM(RTRIM(@BatchName))
	SET @TestCenterName = LTRIM(RTRIM(@TestCenterName))
	SET @Extension1 = LTRIM(RTRIM(@Extension1))
	SET @Extension2 = LTRIM(RTRIM(@Extension2))
	SET @Extension3 = LTRIM(RTRIM(@Extension3))
	SET @Extension4 = LTRIM(RTRIM(@Extension4))
	SET @Extension5 = LTRIM(RTRIM(@Extension5))
	/** Truncating the white spaces at left & right positions Ends **/

	UPDATE [QPackRPack_PackageStatistics]
	SET [GeneratedDate]=ISNULL(@GeneratedDate,[GeneratedDate]),
        [TransferredToDataExchangeServer]=ISNULL(@TransferredToDataExchangeServer,[TransferredToDataExchangeServer]),
        [TransferredToDataExchangeServerOn]=ISNULL(@TransferredToDataExchangeServerOn,[TransferredToDataExchangeServerOn]),
        [TransferredToTestCenter]=ISNULL(@TransferredToTestCenter,[TransferredToTestCenter]),
        [TransferredToTestCenterOn]=ISNULL(@TransferredToTestCenterOn,[TransferredToTestCenterOn]),
        [TransferredToDataCenterDistributed]=ISNULL(@TransferredToDataCenterDistributed,[TransferredToDataCenterDistributed]),
        [TransferredToDataCenterDistributedOn]=ISNULL(@TransferredToDataCenterDistributedOn,[TransferredToDataCenterDistributedOn]),
        [TransferredToDataCenterCentralized]=ISNULL(@TransferredToDataCenterCentralized,[TransferredToDataCenterCentralized]),
        [TransferredToDataCenterCentralizedOn]=ISNULL(@TransferredToDataCenterCentralizedOn,[TransferredToDataCenterCentralizedOn]),
        [RecievedFromDataExchangeServer]=ISNULL(@RecievedFromDataExchangeServer,[RecievedFromDataExchangeServer]),
        [RecievedFromDataExchangeServerOn]=ISNULL(@RecievedFromDataExchangeServerOn,[RecievedFromDataExchangeServerOn]),
        [RecievedFromTestCenter]=ISNULL(@RecievedFromTestCenter,[RecievedFromTestCenter]),
        [RecievedFromTestCenterOn]=ISNULL(@RecievedFromTestCenterOn,[RecievedFromTestCenterOn]),
        [RecievedFromDataCenterDistributed]=ISNULL(@RecievedFromDataCenterDistributed,[RecievedFromDataCenterDistributed]),
        [RecievedFromDataCenterDistributedOn]=ISNULL(@RecievedFromDataCenterDistributedOn,[RecievedFromDataCenterDistributedOn]),
        [RecievedFromDataCenterCentralized]=ISNULL(@RecievedFromDataCenterCentralized,[RecievedFromDataCenterCentralized]),
        [RecievedFromDataCenterCentralizedOn]=ISNULL(@RecievedFromDataCenterCentralizedOn,[RecievedFromDataCenterCentralizedOn]),
        [PackagePassword]=ISNULL(@PackagePassword,[PackagePassword]),
        [PackagePath]=ISNULL(@PackagePath,[PackagePath]),
        [OrganizationID]=ISNULL(@OrganizationID,[OrganizationID]),
        [OrganizationName]=ISNULL(@OrganizationName,[OrganizationName]),
        [DivisionID]=ISNULL(@DivisionID,[DivisionID]),
        [DivisionName]=ISNULL(@DivisionName,[DivisionName]),
        [ProcessID]=ISNULL(@ProcessID,[ProcessID]),
        [ProcessName]=ISNULL(@ProcessName,[ProcessName]),
        [EventID]=ISNULL(@EventID,[EventID]),
        [EventName]=ISNULL(@EventName,[EventName]),
        [BatchID]=ISNULL(@BatchID,[BatchID]),
        [BatchName]=ISNULL(@BatchName,[BatchName]),
        [TestCenterName]=ISNULL(@TestCenterName,[TestCenterName]),
        [ScheduleDate]=ISNULL(@ScheduleDate,[ScheduleDate]),
        [ScheduleStartDate]=ISNULL(@ScheduleStartDate,[ScheduleStartDate]),
        [ScheduleEndDate]=ISNULL(@ScheduleEndDate,[ScheduleEndDate]),
        [LeadTimeForQPackDispatchInMinutes]=ISNULL(@LeadTimeForQPackDispatchInMinutes,[LeadTimeForQPackDispatchInMinutes]),
        [DeleteQPackAfterExamination]=ISNULL(@DeleteQPackAfterExamination,[DeleteQPackAfterExamination]),
        [RPackToBeSentImmediatelyAfterExamination]=ISNULL(@RPackToBeSentImmediatelyAfterExamination,[RPackToBeSentImmediatelyAfterExamination]),
        [RPackToBeSentAtEOD]=ISNULL(@RPackToBeSentAtEOD,[RPackToBeSentAtEOD]),
        [DeleteRPackAfterExamination]=ISNULL(@DeleteRPackAfterExamination,[DeleteRPackAfterExamination]),
        [DeleteRPackAtEOD]=ISNULL(@DeleteRPackAtEOD,[DeleteRPackAtEOD]),
        [PackageDeletedStatus]=ISNULL(@PackageDeletedStatus,[PackageDeletedStatus]),
        [IsCentralizedPackage]=ISNULL(@IsCentralizedPackage,[IsCentralizedPackage]),
        [Extension1]=ISNULL(@Extension1,[Extension1]),
        [Extension2]=ISNULL(@Extension2,[Extension2]),
        [Extension3]=ISNULL(@Extension3,[Extension3]),
        [Extension4]=ISNULL(@Extension4,[Extension4]),
        [Extension5]=ISNULL(@Extension5,[Extension5]),
        [LoadedDateTestCenter]=ISNULL(@LoadedDateTestCenter,[LoadedDateTestCenter]),
        [IsPackageGenerated]=ISNULL(@IsPackageGenerated,[IsPackageGenerated]),
        [IsLatest]=ISNULL(@IsLatest,[IsLatest]),
        [LoadedDateCentralized]=ISNULL(@LoadedDateCentralized,[LoadedDateCentralized]),
        [LoadedDateDistributed]=ISNULL(@LoadedDateDistributed,[LoadedDateDistributed])
    WHERE [ScheduleID]=@ScheduleID AND [TestCenterID]=@TestCenterID AND [PackageName]=@PackageName
    AND DATEDIFF(ss,[GeneratedDate],@GeneratedDate)=0
    AND [PackageType]=@PackageType AND [ScheduleDetailID]=@ScheduleDetailID AND [IsLatest]=1

	SET @RowCount = @@ROWCOUNT
	PRINT @RowCount
    IF ( (@RowCount <= 0) AND NOT EXISTS (SELECT TOP 1 1 FROM [QPackRPack_PackageStatistics]
            WHERE [ScheduleID]=@ScheduleID AND [TestCenterID]=@TestCenterID AND [PackageName]=@PackageName AND [PackageType]=@PackageType
            AND [ScheduleDetailID]=@ScheduleDetailID AND [IsLatest]=1 AND DATEDIFF(ss,[GeneratedDate],@GeneratedDate)=0) )
		BEGIN
			INSERT INTO [QPackRPack_PackageStatistics] ([ScheduleID],[TestCenterID],[PackageType],[GeneratedDate],[TransferredToDataExchangeServer],[TransferredToDataExchangeServerOn],
                [TransferredToTestCenter],[TransferredToTestCenterOn],[TransferredToDataCenterDistributed],[TransferredToDataCenterDistributedOn],
                [TransferredToDataCenterCentralized],[TransferredToDataCenterCentralizedOn],[RecievedFromDataExchangeServer],[RecievedFromDataExchangeServerOn],
                [RecievedFromTestCenter],[RecievedFromTestCenterOn],[RecievedFromDataCenterDistributed],[RecievedFromDataCenterDistributedOn],
                [RecievedFromDataCenterCentralized],[RecievedFromDataCenterCentralizedOn],
                [PackageName],[PackagePassword],[PackagePath],[OrganizationID],[OrganizationName],[DivisionID],[DivisionName],[ProcessID],[ProcessName],
                [EventID],[EventName],[BatchID],[BatchName],[TestCenterName],[ScheduleDate],[ScheduleStartDate],[ScheduleEndDate],
                [LeadTimeForQPackDispatchInMinutes],[DeleteQPackAfterExamination],[RPackToBeSentImmediatelyAfterExamination],[RPackToBeSentAtEOD],
                [DeleteRPackAfterExamination],[DeleteRPackAtEOD],
                [PackageDeletedStatus],[IsCentralizedPackage],
                [Extension1],[Extension2],[Extension3],[Extension4],[Extension5],[ScheduleDetailID],
                [LoadedDateTestCenter],[IsPackageGenerated],[IsLatest],[LoadedDateCentralized],[LoadedDateDistributed])
            VALUES (@ScheduleID,@TestCenterID,@PackageType,@GeneratedDate,@TransferredToDataExchangeServer,@TransferredToDataExchangeServerOn,
                @TransferredToTestCenter,@TransferredToTestCenterOn,@TransferredToDataCenterDistributed,@TransferredToDataCenterDistributedOn,
                @TransferredToDataCenterCentralized,@TransferredToDataCenterCentralizedOn,@RecievedFromDataExchangeServer,@RecievedFromDataExchangeServerOn,
                @RecievedFromTestCenter,@RecievedFromTestCenterOn,@RecievedFromDataCenterDistributed,@RecievedFromDataCenterDistributedOn,
                @RecievedFromDataCenterCentralized,@RecievedFromDataCenterCentralizedOn,
                @PackageName,@PackagePassword,@PackagePath,@OrganizationID,@OrganizationName,@DivisionID,@DivisionName,@ProcessID,@ProcessName,
                @EventID,@EventName,@BatchID,@BatchName,@TestCenterName,@ScheduleDate,@ScheduleStartDate,@ScheduleEndDate,
                @LeadTimeForQPackDispatchInMinutes,@DeleteQPackAfterExamination,@RPackToBeSentImmediatelyAfterExamination,@RPackToBeSentAtEOD,
                @DeleteRPackAfterExamination,@DeleteRPackAtEOD,
                @PackageDeletedStatus,@IsCentralizedPackage,
                @Extension1,@Extension2,@Extension3,@Extension4,@Extension5,@ScheduleDetailID,
                @LoadedDateTestCenter,@IsPackageGenerated,@IsLatest,@LoadedDateCentralized,@LoadedDateDistributed)
		END
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('UpdatePackageStatistics' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TestCenterID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TestCenterID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageType=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackageType+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@GeneratedDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@GeneratedDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToDataExchangeServer=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToDataExchangeServer,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToDataExchangeServerOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToDataExchangeServerOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToTestCenter=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToTestCenter,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToTestCenterOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToTestCenterOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToDataCenterDistributed=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToDataCenterDistributed,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToDataCenterDistributedOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToDataCenterDistributedOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToDataCenterCentralized=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToDataCenterCentralized,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TransferredToDataCenterCentralizedOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@TransferredToDataCenterCentralizedOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromDataExchangeServer=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromDataExchangeServer,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromDataExchangeServerOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromDataExchangeServerOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromTestCenter=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromTestCenter,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromTestCenterOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromTestCenterOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromDataCenterDistributed=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromDataCenterDistributed,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromDataCenterDistributedOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromDataCenterDistributedOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromDataCenterCentralized=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromDataCenterCentralized,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RecievedFromDataCenterCentralizedOn=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RecievedFromDataCenterCentralizedOn,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackageName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackagePassword=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackagePassword+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackagePath=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@PackagePath+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@OrganizationID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@OrganizationID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@OrganizationName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@OrganizationName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@DivisionID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@DivisionID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@DivisionName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@DivisionName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ProcessID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ProcessID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ProcessName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@ProcessName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@EventID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@EventID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@EventName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@EventName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@BatchID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@BatchID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@BatchName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@BatchName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@TestCenterName=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@TestCenterName+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleStartDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleStartDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleEndDate=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleEndDate,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@LeadTimeForQPackDispatchInMinutes=' AS NVARCHAR(MAX)) + CAST(ISNULL(@LeadTimeForQPackDispatchInMinutes,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@DeleteQPackAfterExamination=' AS NVARCHAR(MAX)) + CAST(ISNULL(@DeleteQPackAfterExamination,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RPackToBeSentImmediatelyAfterExamination=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RPackToBeSentImmediatelyAfterExamination,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@RPackToBeSentAtEOD=' AS NVARCHAR(MAX)) + CAST(ISNULL(@RPackToBeSentAtEOD,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@DeleteRPackAfterExamination=' AS NVARCHAR(MAX)) + CAST(ISNULL(@DeleteRPackAfterExamination,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@DeleteRPackAtEOD=' AS NVARCHAR(MAX)) + CAST(ISNULL(@DeleteRPackAtEOD,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@PackageDeletedStatus=' AS NVARCHAR(MAX)) + CAST(ISNULL(@PackageDeletedStatus,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@IsCentralizedPackage=' AS NVARCHAR(MAX)) + CAST(ISNULL(@IsCentralizedPackage,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Extension1=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Extension1+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Extension2=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Extension2+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Extension3=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Extension3+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Extension4=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Extension4+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@Extension5=' AS NVARCHAR(MAX)) + CAST(ISNULL(''''+@Extension5+'''','NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleDetailID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleDetailID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@LoadedDateTestCenter=' AS NVARCHAR(MAX)) + CAST(ISNULL(@LoadedDateTestCenter,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@IsPackageGenerated=' AS NVARCHAR(MAX)) + CAST(ISNULL(@IsPackageGenerated,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@IsLatest=' AS NVARCHAR(MAX)) + CAST(ISNULL(@IsLatest,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@LoadedDateCentralized=' AS NVARCHAR(MAX)) + CAST(ISNULL(@LoadedDateCentralized,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@LoadedDateDistributed=' AS NVARCHAR(MAX)) + CAST(ISNULL(@LoadedDateDistributed,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO