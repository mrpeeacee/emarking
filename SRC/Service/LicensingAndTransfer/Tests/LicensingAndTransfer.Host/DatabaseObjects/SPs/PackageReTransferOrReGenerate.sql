IF  EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PackageReTransferOrReGenerate]'))
BEGIN
	DROP PROCEDURE [dbo].[PackageReTransferOrReGenerate]
END
GO
/*******************************************************************************************************
Created By & Date:
Object Name:[PackageReTransferOrReGenerate]
Purpose:To Re generate or transfer package.
Version:1.4
Remarks:
Reviewed By & Date:
Fix Status:
History
#; Who; Date; Version; Review By; Status
*******************************************************************************************************/
CREATE PROC [dbo].[PackageReTransferOrReGenerate]
(
    @ScheduleDetailID BIGINT = 0,
    @CenterID BIGINT = 0,
    @OperationType INT = 1 -- 1 For Regenerate, 2 For ReTransfer
)
--WITH ENCRYPTION
AS
BEGIN
SET NOCOUNT ON;
BEGIN TRY
    DECLARE @Cnt BIGINT, @TestCenterID BIGINT, @TotalRows BIGINT, @ID BIGINT
	DECLARE @DeliveryMode BIGINT, @strTestCenters NVARCHAR(MAX), @strTestCenterIDs NVARCHAR(MAX)

	SET @DeliveryMode =
	(SELECT (CASE WHEN M.MetadataValue = 1 THEN 1 ELSE 0 END)
	FROM [dbo].[ScheduleDetails] SD WITH(NOLOCK)
		INNER JOIN [dbo].[CalendarEventScheduleMapping] CESM WITH(NOLOCK) ON SD.ScheduleID = CESM.ScheduleID
		INNER JOIN [dbo].[CalendarEvent] CE WITH(NOLOCK) ON CESM.CalendarEventID = CE.ID
		INNER JOIN [dbo].[MetadataInfo] M WITH(NOLOCK) ON M.[ID]=CE.[MetadataID]
	WHERE SD.ScheduleDetailID = @ScheduleDetailID)

    DECLARE @ScheduleTestCenterDetails TABLE
    (
        RowID BIGINT IDENTITY(1,1),
        ScheduleDetailID BIGINT,
        TestCenterID BIGINT
    )

	IF @DeliveryMode = 1
		BEGIN
		    INSERT INTO @ScheduleTestCenterDetails 
            SELECT DISTINCT @ScheduleDetailID,0
		END
    ELSE
    IF (@CenterID IS NOT NULL AND @CenterID > 0)
        BEGIN
            INSERT INTO @ScheduleTestCenterDetails 
            SELECT DISTINCT @ScheduleDetailID,@CenterID
        END
    ELSE
        BEGIN
            INSERT INTO @ScheduleTestCenterDetails 
            SELECT DISTINCT @ScheduleDetailID,SU.[Extention1]
            FROM [ScheduleUser] SU WITH(NOLOCK)
            WHERE SU.ScheduleDetailID = @ScheduleDetailID
        END

    SET @TotalRows = (SELECT MAX(RowID) FROM @ScheduleTestCenterDetails)

    SET @Cnt = 1
    WHILE (@Cnt <= @TotalRows)
        BEGIN
            SELECT @ScheduleDetailID = ScheduleDetailID, @TestCenterID = TestCenterID FROM @ScheduleTestCenterDetails WHERE RowID = @Cnt

			IF @DeliveryMode = 1
				BEGIN
                    SET @strTestCenters = ''
                    SET @strTestCenterIDs = ''

                    SELECT @strTestCenterIDs=@strTestCenterIDs + ISNULL(LTRIM(RTRIM([ID])) + ',', '')
                    FROM [TestCenter]
                    WHERE [ID] IN (SELECT [Extention1] FROM [ScheduleUser] WHERE [ScheduleDetailID]=@ScheduleDetailID)
                    ORDER BY [ID] ASC

                    SELECT @strTestCenters=@strTestCenters + ISNULL(LTRIM(RTRIM([CenterName])) + ',', '')
                    FROM [TestCenter]
                    WHERE [ID] IN (SELECT [Extention1] FROM [ScheduleUser] WHERE [ScheduleDetailID]=@ScheduleDetailID)
                    ORDER BY [ID] ASC
                    IF (LEN(@strTestCenters) <= 0)
                        SET @strTestCenters = 'Data Center'
                    

					INSERT INTO [QPackRPack_PackageStatistics]([ScheduleID],[TestCenterID],[PackageType],[GeneratedDate],[PackageName],[PackagePassword],
						[OrganizationID],[OrganizationName],[DivisionID],[DivisionName],[ProcessID],[ProcessName],[EventID],[EventName],[BatchID],[BatchName],
						[TestCenterName],[ScheduleDate],[ScheduleStartDate],[ScheduleEndDate],
						[RPackToBeSentAtEOD],[DeleteRPackAtEOD],[PackageDeletedStatus]
						,[IsCentralizedPackage],[ScheduleDetailID],[IsPackageGenerated],[Extension1],[Extension2]
						)
					SELECT DISTINCT S.[ScheduleID],0,'QPack',NULL,NULL,NULL,
						O.OrganizationID,O.OrganizationName,
						(SELECT OrganizationID FROM [dbo].[Organization] WITH(NOLOCK) WHERE OrganizationId=OP.OrganizationId AND OrganizationType=2 AND IsDeleted=0) AS DivisionID,
						(SELECT OrganizationName FROM [dbo].[Organization] WITH(NOLOCK) WHERE OrganizationId=OP.OrganizationId AND OrganizationType=2 AND IsDeleted=0) AS DivisionName,
						P.ID,P.ProcessName,CE.ID,CE.EventTitle,G.GroupID,G.GroupName,
						@strTestCenters,(CASE WHEN CHARINDEX('@',S.Extension1,0)>0 THEN SUBSTRING(S.Extension1,0,CHARINDEX('@',S.Extension1,0)) ELSE S.Extension1 END),SD.StartTime,SD.EndTime,0,0,0
						,(CASE WHEN (SELECT TOP 1 [MetadataValue] FROM [MetadataInfo] WHERE [ID]=CE.[MetadataID]) = 1    --  2 for Online; which is centralized
							THEN 1 ELSE 0 END)
						,SD.ScheduleDetailID,0,@strTestCenterIDs
                        ,(SELECT TOP 1 M.MetadataValue FROM [MetadataInfo] M WHERE M.[ID]=CE.[MetadataID])  --  2,distributed online; 3,distributed offline
					FROM [dbo].[Schedule] S WITH(NOLOCK)
						INNER JOIN [dbo].[ScheduleDetails] SD WITH(NOLOCK) ON S.ScheduleID = SD.ScheduleID
						INNER JOIN [dbo].[Group] G WITH(NOLOCK) ON G.GroupId=SD.GroupId
						INNER JOIN [dbo].[CalendarEventScheduleMapping] CESM WITH(NOLOCK) ON S.ScheduleID = CESM.ScheduleID
						INNER JOIN [dbo].[CalendarEvent] CE WITH(NOLOCK) ON CESM.CalendarEventID = CE.ID
						INNER JOIN [dbo].[ProcessCalendarEventMapping] PCEM WITH(NOLOCK) ON PCEM.CalendarEventID = CE.ID
						INNER JOIN [dbo].[Process] P WITH(NOLOCK) ON PCEM.ProcessID = P.ID
						INNER JOIN [dbo].[Organization] O WITH(NOLOCK) ON O.OrganizationID = S.OrganizationID
						INNER JOIN [dbo].[OrganizationProcessMapping] OP WITH(NOLOCK) ON OP.ProcessId=P.Id
					WHERE SD.ScheduleDetailID = @ScheduleDetailID 
				END
			ELSE
				BEGIN
					INSERT INTO [QPackRPack_PackageStatistics]([ScheduleID],[TestCenterID],[PackageType],[GeneratedDate],[PackageName],[PackagePassword],
						[OrganizationID],[OrganizationName],[DivisionID],[DivisionName],[ProcessID],[ProcessName],[EventID],[EventName],[BatchID],[BatchName],
						[TestCenterName],[ScheduleDate],[ScheduleStartDate],[ScheduleEndDate],
						[RPackToBeSentAtEOD],[DeleteRPackAtEOD],[PackageDeletedStatus]
						,[IsCentralizedPackage],[ScheduleDetailID],[IsPackageGenerated],[Extension1],[Extension2]
						)
					SELECT DISTINCT S.[ScheduleID],TC.[ID],'QPack',NULL,NULL,NULL,
						O.OrganizationID,O.OrganizationName,
						(SELECT OrganizationID FROM [dbo].[Organization] WITH(NOLOCK) WHERE OrganizationId=OP.OrganizationId AND OrganizationType=2 AND IsDeleted=0) AS DivisionID,
						(SELECT OrganizationName FROM [dbo].[Organization] WITH(NOLOCK) WHERE OrganizationId=OP.OrganizationId AND OrganizationType=2 AND IsDeleted=0) AS DivisionName,
						P.ID,P.ProcessName,CE.ID,CE.EventTitle,G.GroupID,G.GroupName,
						TC.CenterName,(CASE WHEN CHARINDEX('@',S.Extension1,0)>0 THEN SUBSTRING(S.Extension1,0,CHARINDEX('@',S.Extension1,0)) ELSE S.Extension1 END),SD.StartTime,SD.EndTime,0,0,0
						,(CASE WHEN (SELECT TOP 1 [MetadataValue] FROM [MetadataInfo] WHERE [ID]=CE.[MetadataID]) = 1    --  2 for Online; which is centralized
							THEN 1 ELSE 0 END)
						,SD.ScheduleDetailID,0,TC.[ID]
                        ,(SELECT TOP 1 M.MetadataValue FROM [MetadataInfo] M WHERE M.[ID]=CE.[MetadataID])  --  2,distributed online; 3,distributed offline
					FROM [dbo].[Schedule] S WITH(NOLOCK)
						INNER JOIN [dbo].[ScheduleDetails] SD WITH(NOLOCK) ON S.ScheduleID = SD.ScheduleID
						INNER JOIN [dbo].[ScheduleUser] SU WITH(NOLOCK) ON SU.ScheduleDetailID = SD.ScheduleDetailID
						INNER JOIN [dbo].TestCenter TC WITH(NOLOCK) ON TC.ID=SU.[Extention1]
						INNER JOIN [dbo].[Group] G WITH(NOLOCK) ON G.GroupId=SU.GroupId
						INNER JOIN [dbo].[User] U WITH(NOLOCK) ON U.UserID = SU.UserID
						INNER JOIN [dbo].[CalendarEventScheduleMapping] CESM WITH(NOLOCK) ON S.ScheduleID = CESM.ScheduleID
						INNER JOIN [dbo].[CalendarEvent] CE WITH(NOLOCK) ON CESM.CalendarEventID = CE.ID
						INNER JOIN [dbo].[ProcessCalendarEventMapping] PCEM WITH(NOLOCK) ON PCEM.CalendarEventID = CE.ID
						INNER JOIN [dbo].[Process] P WITH(NOLOCK) ON PCEM.ProcessID = P.ID
						INNER JOIN [dbo].[Organization] O WITH(NOLOCK) ON O.OrganizationID = U.OrganizationID
						INNER JOIN [dbo].[OrganizationProcessMapping] OP WITH(NOLOCK) ON OP.ProcessId=P.Id
					WHERE SD.ScheduleDetailID = @ScheduleDetailID AND TC.ID = @TestCenterID
				END

            SET @ID = SCOPE_IDENTITY()

            UPDATE [QPackRPack_PackageStatistics]
                SET [IsLatest] = 0
            WHERE [ScheduleDetailID]=@ScheduleDetailID AND [TestCenterID]=@TestCenterID
                AND [PackageType] = 'QPack' AND [ID] != @ID

            SET @Cnt = @Cnt + 1
        END
END TRY
BEGIN CATCH 
    DECLARE @ErrorDetail AS NVARCHAR(MAX)
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(''AS NVARCHAR(MAX))+CAST('PackageReTransferOrReGenerate' AS NVARCHAR(MAX))+CAST('  'AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@ScheduleDetailID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@ScheduleDetailID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@CenterID=' AS NVARCHAR(MAX)) + CAST(ISNULL(@CenterID,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    SET @ErrorDetail = CONVERT(NVARCHAR(MAX),CAST(@ErrorDetail AS  NVARCHAR(MAX))+CAST('@OperationType=' AS NVARCHAR(MAX)) + CAST(ISNULL(@OperationType,'NULL') AS NVARCHAR(MAX)) + CAST(','AS NVARCHAR(MAX)))
    EXEC [GenerateErrorHandling] @ErrorDetail

    DECLARE @Exception AS NVARCHAR(MAX)  
    SET @Exception=ERROR_MESSAGE() + '->' + @ErrorDetail 
    RAISERROR (@Exception, 16, 1)
END CATCH
END
GO