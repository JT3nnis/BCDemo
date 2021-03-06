USE [BuildersCapitalDB]
GO
/****** Object:  Table [dbo].[DocStatusView]    Script Date: 9/14/2020 10:08:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DocStatusView](
	[PropertyId] [uniqueidentifier] NOT NULL,
	[Agreement] [bit] NOT NULL,
	[Appraisal] [bit] NOT NULL,
	[SiteMap] [bit] NOT NULL,
	[Resume] [bit] NOT NULL,
	[Paperwork] [bit] NOT NULL,
 CONSTRAINT [PK_DocStatusView] PRIMARY KEY CLUSTERED 
(
	[PropertyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Documents]    Script Date: 9/14/2020 10:08:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Documents](
	[Id] [uniqueidentifier] NOT NULL,
	[PropertyId] [uniqueidentifier] NOT NULL,
	[DocType] [nchar](20) NOT NULL,
	[FileName] [nchar](40) NOT NULL,
	[DocBlob] [varbinary](max) NOT NULL,
 CONSTRAINT [PK_Documents] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
INSERT [dbo].[DocStatusView] ([PropertyId], [Agreement], [Appraisal], [SiteMap], [Resume], [Paperwork]) VALUES (N'a8d5f3ce-94e7-455b-935b-7762330dc81f', 0, 0, 0, 0, 0)
GO
ALTER TABLE [dbo].[Documents]  WITH CHECK ADD FOREIGN KEY([PropertyId])
REFERENCES [dbo].[DocStatusView] ([PropertyId])
GO
