USE [DUBBDB]
GO

/****** Object:  Table [dbo].[tagTypes]    Script Date: 04/10/2016 11:16:00 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tagTypes](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


/****** Object:  Table [dbo].[tags]    Script Date: 04/10/2016 11:16:23 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tags](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TagTypeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tags]  WITH CHECK ADD  CONSTRAINT [FK_tags_To_tagTypes] FOREIGN KEY([TagTypeId])
REFERENCES [dbo].[tagTypes] ([Id])
GO

ALTER TABLE [dbo].[tags] CHECK CONSTRAINT [FK_tags_To_tagTypes]
GO


/****** Object:  Table [dbo].[tagTemplateHdrs]    Script Date: 04/10/2016 11:16:43 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tagTemplateHdrs](
	[tagTemplateHdrIntno] [bigint] NOT NULL IDENTITY(1,1),
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Text] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[tagTemplateHdrIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

/****** Object:  Table [dbo].[tagTemplateDtls]    Script Date: 04/10/2016 11:16:56 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tagTemplateDtls](
	[tagTemplateDtlIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[tagTemplateHdrIntno] [bigint] NOT NULL,
	[tagId] [int] NOT NULL,
	[MinScore] [int] NOT NULL,
	[Weight] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[tagTemplateDtlIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tagTemplateDtls] ADD  DEFAULT ((2)) FOR [MinScore]
GO

ALTER TABLE [dbo].[tagTemplateDtls] ADD  DEFAULT ((1)) FOR [Weight]
GO

ALTER TABLE [dbo].[tagTemplateDtls]  WITH CHECK ADD  CONSTRAINT [FK_tagTemplateDtls_To_tags] FOREIGN KEY([tagId])
REFERENCES [dbo].[tags] ([Id])
GO

ALTER TABLE [dbo].[tagTemplateDtls] CHECK CONSTRAINT [FK_tagTemplateDtls_To_tags]
GO

ALTER TABLE [dbo].[tagTemplateDtls]  WITH CHECK ADD  CONSTRAINT [FK_tagTemplateDtls_To_tagTemplateHdrs] FOREIGN KEY([tagTemplateHdrIntno])
REFERENCES [dbo].[tagTemplateHdrs] ([tagTemplateHdrIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tagTemplateDtls] CHECK CONSTRAINT [FK_tagTemplateDtls_To_tagTemplateHdrs]
GO


/****** Object:  Table [dbo].[audioSampleHdrs]    Script Date: 04/10/2016 11:17:18 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[audioSampleHdrs](
	[audioSampleHdrIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](250) NOT NULL,
	[voiceActorIntno] [bigint] NOT NULL,
	[tagTemplateHdrIntno] [bigint] NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
	[FileName] [nvarchar](250) NULL,
	[FileUrl] [nvarchar](max) NULL,
	[Studio] [nvarchar](10) NULL,
	[Description] [nvarchar](max) NULL,
	[Status] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[audioSampleHdrIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[audioSampleHdrs] ADD  DEFAULT (getdate()) FOR [SubmitDate]
GO

ALTER TABLE [dbo].[audioSampleHdrs]  WITH CHECK ADD  CONSTRAINT [FK_audioSampleHdrs_To_tagTemplateHdrs] FOREIGN KEY([tagTemplateHdrIntno])
REFERENCES [dbo].[tagTemplateHdrs] ([tagTemplateHdrIntno])
GO

ALTER TABLE [dbo].[audioSampleHdrs] CHECK CONSTRAINT [FK_audioSampleHdrs_To_tagTemplateHdrs]
GO

ALTER TABLE [dbo].[audioSampleHdrs]  WITH CHECK ADD  CONSTRAINT [FK_audioSampleHdrs_To_voiceActors] FOREIGN KEY([voiceActorIntno])
REFERENCES [dbo].[voiceActors] ([voiceActorIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[audioSampleHdrs] CHECK CONSTRAINT [FK_audioSampleHdrs_To_voiceActors]
GO

/****** Object:  Table [dbo].[audioSampleDtls]    Script Date: 04/10/2016 11:17:38 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[audioSampleDtls](
	[audioSampleDtlIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[audioSampleHdrIntno] [bigint] NOT NULL,
	[tagId] [int] NOT NULL,
	[TagScore] [int] NOT NULL,
	[Match] [float] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[audioSampleDtlIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[audioSampleDtls] ADD  DEFAULT ((0)) FOR [Match]
GO

ALTER TABLE [dbo].[audioSampleDtls]  WITH CHECK ADD  CONSTRAINT [FK_audioSampleDtls_To_audioSampleHdrs] FOREIGN KEY([audioSampleHdrIntno])
REFERENCES [dbo].[audioSampleHdrs] ([audioSampleHdrIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[audioSampleDtls] CHECK CONSTRAINT [FK_audioSampleDtls_To_audioSampleHdrs]
GO

ALTER TABLE [dbo].[audioSampleDtls]  WITH CHECK ADD  CONSTRAINT [FK_audioSampleDtls_To_tags] FOREIGN KEY([tagId])
REFERENCES [dbo].[tags] ([Id])
GO

ALTER TABLE [dbo].[audioSampleDtls] CHECK CONSTRAINT [FK_audioSampleDtls_To_tags]
GO


/****** Object:  Table [dbo].[chrTemplateHdrs]    Script Date: 04/10/2016 11:17:55 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[chrTemplateHdrs](
	[chrTemplateHdrIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[workCharacterIntno] [bigint] NOT NULL,
	[tagTemplateHdrIntno] [bigint] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[chrTemplateHdrIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[chrTemplateHdrs]  WITH CHECK ADD  CONSTRAINT [FK_chrTemplateHdrs_To_tagTemplateHdrs] FOREIGN KEY([tagTemplateHdrIntno])
REFERENCES [dbo].[tagTemplateHdrs] ([tagTemplateHdrIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrTemplateHdrs] CHECK CONSTRAINT [FK_chrTemplateHdrs_To_tagTemplateHdrs]
GO

ALTER TABLE [dbo].[chrTemplateHdrs]  WITH CHECK ADD  CONSTRAINT [FK_chrTemplateHdrs_To_workCharacters] FOREIGN KEY([workCharacterIntno])
REFERENCES [dbo].[workCharacters] ([workCharacterIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrTemplateHdrs] CHECK CONSTRAINT [FK_chrTemplateHdrs_To_workCharacters]
GO

/****** Object:  Table [dbo].[chrTemplateDtls]    Script Date: 04/10/2016 11:18:06 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[chrTemplateDtls](
	[chrTemplateDtlIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[chrTemplateHdrIntno] [bigint] NOT NULL,
	[tagId] [int] NOT NULL,
	[Weight] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[chrTemplateDtlIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[chrTemplateDtls] ADD  DEFAULT ((1)) FOR [Weight]
GO

ALTER TABLE [dbo].[chrTemplateDtls]  WITH CHECK ADD  CONSTRAINT [FK_chrTemplateDtls_To_chrTemplateHdrs] FOREIGN KEY([chrTemplateHdrIntno])
REFERENCES [dbo].[chrTemplateHdrs] ([chrTemplateHdrIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrTemplateDtls] CHECK CONSTRAINT [FK_chrTemplateDtls_To_chrTemplateHdrs]
GO

ALTER TABLE [dbo].[chrTemplateDtls]  WITH CHECK ADD  CONSTRAINT [FK_chrTemplateDtls_To_tags] FOREIGN KEY([tagId])
REFERENCES [dbo].[tags] ([Id])
GO

ALTER TABLE [dbo].[chrTemplateDtls] CHECK CONSTRAINT [FK_chrTemplateDtls_To_tags]
GO


/****** Object:  Table [dbo].[chrCastListHdrs]    Script Date: 04/10/2016 11:18:28 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[chrCastListHdrs](
	[chrCastListHdrIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[workCharacterIntno] [bigint] NOT NULL,
	[clientIntno] [bigint] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Status] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[chrCastListHdrIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[chrCastListHdrs]  WITH CHECK ADD  CONSTRAINT [FK_chrCastListHdrs_To_clients] FOREIGN KEY([clientIntno])
REFERENCES [dbo].[clients] ([clientIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrCastListHdrs] CHECK CONSTRAINT [FK_chrCastListHdrs_To_clients]
GO

ALTER TABLE [dbo].[chrCastListHdrs]  WITH CHECK ADD  CONSTRAINT [FK_chrCastListHdrs_To_workCharacters] FOREIGN KEY([workCharacterIntno])
REFERENCES [dbo].[workCharacters] ([workCharacterIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrCastListHdrs] CHECK CONSTRAINT [FK_chrCastListHdrs_To_workCharacters]
GO

/****** Object:  Table [dbo].[chrCastListDtls]    Script Date: 04/10/2016 11:18:38 PM ******/
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[chrCastListDtls](
	[chrCastListDtlIntno] [bigint] IDENTITY(1,1) NOT NULL,
	[chrCastListHdrIntno] [bigint] NOT NULL,
	[voiceActorIntno] [bigint] NOT NULL,
	[Match] [float] NOT NULL,
	[Rank] [int] NULL,
	[Comments] [nvarchar](max) NULL,
	[ClientRank] [int] NULL,
	[ClientComments] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[chrCastListDtlIntno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

ALTER TABLE [dbo].[chrCastListDtls]  WITH CHECK ADD  CONSTRAINT [FK_chrCastListDtls_To_chrCastListHdrs] FOREIGN KEY([chrCastListHdrIntno])
REFERENCES [dbo].[chrCastListHdrs] ([chrCastListHdrIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrCastListDtls] CHECK CONSTRAINT [FK_chrCastListDtls_To_chrCastListHdrs]
GO

ALTER TABLE [dbo].[chrCastListDtls]  WITH CHECK ADD  CONSTRAINT [FK_chrCastListDtls_To_voiceActors] FOREIGN KEY([voiceActorIntno])
REFERENCES [dbo].[voiceActors] ([voiceActorIntno])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[chrCastListDtls] CHECK CONSTRAINT [FK_chrCastListDtls_To_voiceActors]
GO

SET IDENTITY_INSERT [dbo].[dubbDomain] ON
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (240, N'vlScore', N'1', N'Not Avaiable', NULL, N'USR', N'en', 1, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (241, N'vlScore', N'2', N'Below Expectation', NULL, N'USR', N'en', 2, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (242, N'vlScore', N'3', N'Good', NULL, N'USR', N'en', 3, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (243, N'vlScore', N'4', N'Outstanding', NULL, N'USR', N'en', 4, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (244, N'vlWeight', N'0', N'Not Used', NULL, N'USR', N'en', 1, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (245, N'vlWeight', N'1', N'Normal', NULL, N'USR', N'en', 2, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (246, N'vlWeight', N'2', N'High', NULL, N'USR', N'en', 3, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (247, N'vlWeight', N'3', N'Very High', NULL, N'USR', N'en', 4, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (248, N'vlAudioSample', N'1', N'Not Evaluated', NULL, N'USR', N'en', 1, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (249, N'vlAudioSample', N'2', N'Evaluated', NULL, N'USR', N'en', 2, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (250, N'vlCastList', N'1', N'Draft', NULL, N'USR', N'en', 1, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (251, N'vlCastList', N'2', N'Published', NULL, N'USR', N'en', 2, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (252, N'vlCastList', N'3', N'Reviewed', NULL, N'USR', N'en', 3, 0, 1)
INSERT INTO [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (253, N'vlCastList', N'4', N'Cancelled', NULL, N'USR', N'en', 4, 0, 1)
SET IDENTITY_INSERT [dbo].[dubbDomain] OFF

-- Default tag types
INSERT INTO TagTypes (Id, Name) VALUES (10, N'Gender');
INSERT INTO TagTypes (Id, Name) VALUES (20, N'Genre');
INSERT INTO TagTypes (Id, Name) VALUES (30, N'Characterstics');
INSERT INTO TagTypes (Id, Name) VALUES (40, N'Roles');
INSERT INTO TagTypes (Id, Name) VALUES (50, N'Age');
INSERT INTO TagTypes (Id, Name) VALUES (60, N'Language and Accent');
INSERT INTO TagTypes (Id, Name) VALUES (90, N'Unclassified');

-- Gender default tags
INSERT INTO Tags (TagTypeId, Name) VALUES (10, N'Male');
INSERT INTO Tags (TagTypeId, Name) VALUES (10, N'Female');

-- Genre default tags
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Animation and Character');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Announcement');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Audiobook');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Commercial');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Corporate');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Dialog');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Documentary');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Education');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Dialog');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Film');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Government');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Impressions');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Narration');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Podcast');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Promo and Trailer');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Promotional');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'Song');
INSERT INTO Tags (TagTypeId, Name) VALUES (20, N'TV');

-- Characterstics defaults tags
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'arrogant');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'attitude');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'believable');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'calming');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'casual');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'charming');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'cheerful');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'classy');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'clear');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'cool');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'corporate');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'cute');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'deep');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'easy-going');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'educational');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'elegant');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'enthusiastic');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'exciting');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'friendly');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'fun');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'gravely');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'happy');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'informative');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'innocent');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'inviting');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'intelligent');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'luxurious');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'neighborly');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'optimistic');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'playful');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'powerfull');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'raw');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'relaxing');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'rough');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'sarcastic');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'sesual');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'sexy');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'silly');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'smooth');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'sweet');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'tough');
INSERT INTO Tags (TagTypeId, Name) VALUES (30, N'warm');

-- Roles default tags
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'father');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'mother');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'grandfather');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'grnadmother');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'friend');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'boss');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'son');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'daughter');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'student');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'teacher');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'manager');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'police');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'security');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'salesman');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'driver');
INSERT INTO Tags (TagTypeId, Name) VALUES (40, N'Father');


-- Age default tags
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'Child');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'Preteen');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'Teenager');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'20s');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'30s');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'40s');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'50s');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'60s');
INSERT INTO Tags (TagTypeId, Name) VALUES (50, N'Senior');

-- Language and Accent
INSERT INTO Tags (TagTypeId, Name) VALUES (60, N'Arabic');
INSERT INTO Tags (TagTypeId, Name) VALUES (60, N'English');
INSERT INTO Tags (TagTypeId, Name) VALUES (60, N'Hindi');
INSERT INTO Tags (TagTypeId, Name) VALUES (60, N'Urdo');
