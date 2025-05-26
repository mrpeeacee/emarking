ALTER TABLE [TestCenter] ADD [MacID] NVARCHAR(MAX)
GO

IF EXISTS (SELECT TOP 1 1 FROM [SYS].[OBJECTS] WHERE [OBJECT_ID] = OBJECT_ID(N'[dbo].[RegisteredTestCenters]') AND [TYPE] in (N'U'))
    DROP TABLE [dbo].[RegisteredTestCenters]
GO
CREATE TABLE [dbo].[RegisteredTestCenters](
	[TestCenterID] [bigint],
    [RegisteredOn] DATETIME CONSTRAINT DF_DateTime DEFAULT(GETUTCDATE()),
 CONSTRAINT [PK_ExportedTestCenters] PRIMARY KEY CLUSTERED 
(
	[TestCenterID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/*
ALTER TABLE [dbo].[RegisteredTestCenters]
    ADD CONSTRAINT FK_RegisteredTestCenters_TestCenter FOREIGN KEY ( TestCenterID ) REFERENCES dbo.TestCenter ( ID )
ON UPDATE  NO ACTION
ON DELETE  NO ACTION
GO
*/