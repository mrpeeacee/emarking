IF EXISTS (SELECT TOP 1 1 FROM [SYS].[OBJECTS] WHERE [OBJECT_ID] = OBJECT_ID(N'[dbo].[QPackRPack_FTPSessionDetails]') AND [TYPE] in (N'U'))
    DROP TABLE [dbo].[QPackRPack_FTPSessionDetails]
GO
CREATE TABLE [dbo].[QPackRPack_FTPSessionDetails](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
    [FTPSessionID] [bigint] NOT NULL,
    [TestCenterID] [bigint] NOT NULL,
    [ScheduleID] [bigint] NOT NULL,
    [PackageGeneratedDate] DATETIME NOT NULL,
    [PackageName] [nvarchar](200) NOT NULL,
 CONSTRAINT [PK_FTPSessionDetails] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/*
ALTER TABLE dbo.FTPSessionDetails
    ADD CONSTRAINT FK_FTPSessionDetails_TestCenter FOREIGN KEY ( TestCenterID ) REFERENCES dbo.TestCenter ( ID )
ON UPDATE  NO ACTION
ON DELETE  NO ACTION
GO

ALTER TABLE dbo.TransferSessionDetails
    ADD CONSTRAINT FK_FTPSessionDetails_FTPSession FOREIGN KEY ( FTPSessionID ) REFERENCES dbo.FTPSession ( ID )
ON UPDATE  NO ACTION
ON DELETE  NO ACTION
GO
ALTER TABLE dbo.TransferSessionDetails
    ADD CONSTRAINT FK_FTPSessionDetails_Schedule FOREIGN KEY ( ScheduleID ) REFERENCES dbo.Schedule ( ScheduleID )
ON UPDATE  NO ACTION
ON DELETE  NO ACTION
GO
*/