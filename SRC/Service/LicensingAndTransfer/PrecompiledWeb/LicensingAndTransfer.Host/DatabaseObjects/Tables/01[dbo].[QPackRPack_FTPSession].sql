IF EXISTS (SELECT TOP 1 1 FROM [SYS].[OBJECTS] WHERE [OBJECT_ID] = OBJECT_ID(N'[dbo].[QPackRPack_FTPSession]') AND [TYPE] in (N'U'))
    DROP TABLE [dbo].[QPackRPack_FTPSession]
GO
CREATE TABLE [dbo].[QPackRPack_FTPSession](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[GUID] [nvarchar](50) NOT NULL,
	[MacID] [nvarchar](50) NOT NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NULL,
	[OperationType] [nvarchar](20) NOT NULL,
	[ServerType] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_QPackRPack_FTPSession] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO