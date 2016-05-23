/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET IDENTITY_INSERT [dbo].[dubbDomain] ON 

GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (1, N'langCode', N'en', N'English', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (2, N'langCode', N'ar', N'Arabic', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (3, N'langCode', N'en', N'انكليزي', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (4, N'langCode', N'ar', N'عربي', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (5, N'contactParty', N'01', N'Client', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (6, N'contactParty', N'02', N'Translation', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (7, N'contactParty', N'03', N'Adaptation', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (8, N'contactParty', N'04', N'Music & Sound Effects', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (9, N'contactParty', N'01', N'الزبون', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (10, N'contactParty', N'02', N'الترجمة', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (11, N'contactParty', N'03', N'الإعداد', NULL, N'USR', N'ar', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (12, N'contactParty', N'04', N'موسيقى و مؤثرات صوتية', NULL, N'USR', N'ar', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (13, N'agreementType', N'01', N'Agreement', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (15, N'agreementType', N'02', N'Agreement Contract', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (16, N'agreementType', N'03', N'Stand-Alone Contract', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (17, N'agreementType', N'04', N'single Order', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (18, N'agreementType', N'05', N'Amendement', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (19, N'agreementType', N'01', N'اتفاقية', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (20, N'agreementType', N'02', N'عقد تابع لاتفاقية', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (21, N'agreementType', N'03', N'عقد مستقل', NULL, N'USR', N'ar', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (22, N'agreementType', N'04', N'طلبية منفردة', NULL, N'USR', N'ar', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (23, N'agreementType', N'05', N'إضافات', NULL, N'USR', N'ar', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (24, N'workType', N'01', N'Series', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (25, N'workType', N'02', N'Documentary', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (26, N'workType', N'03', N'Movie', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (27, N'workType', N'04', N'Publicity', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (28, N'workType', N'01', N'مسلسل', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (29, N'workType', N'02', N'وثائقي', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (30, N'workType', N'03', N'فيلم', NULL, N'USR', N'ar', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (31, N'workType', N'04', N'دعاية', NULL, N'USR', N'ar', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (32, N'durationUom', N'01', N'Year', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (33, N'durationUom', N'02', N'Month', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (34, N'durationUom', N'01', N'سنة', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (35, N'durationUom', N'02', N'شهر', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (36, N'salutation', N'01', N'Mr.', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (37, N'salutation', N'02', N'Mrs.', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (38, N'salutation', N'03', N'Ms.', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (39, N'salutation', N'04', N'Dr.', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (40, N'salutation', N'01', N'سيد', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (41, N'salutation', N'02', N'سيدة', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (42, N'salutation', N'03', N'مؤنث غير محدد', NULL, N'USR', N'ar', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (43, N'salutation', N'04', N'دكتور', NULL, N'USR', N'ar', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (44, N'specsType', N'01', N'Video', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (45, N'specsType', N'02', N'Audio', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (46, N'specsType', N'01', N'صورة', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (47, N'specsType', N'02', N'صوت', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (48, N'clientStatus', N'01', N'Active', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (49, N'clientStatus', N'02', N'Suspended', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (50, N'clientStatus', N'03', N'Inactive', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (51, N'clientStatus', N'01', N'فعال', NULL, N'USR', N'ar', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (52, N'clientStatus', N'02', N'معلق', NULL, N'USR', N'ar', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (53, N'clientStatus', N'03', N'غير فعال', NULL, N'USR', N'ar', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (54, N'agreementStatus', N'01', N'Active', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (55, N'agreementStatus', N'02', N'Suspended', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (56, N'agreementStatus', N'03', N'Endorsed', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (57, N'agreementStatus', N'04', N'Canceled', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (60, N'specsType', N'03', N'Others', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (61, N'specsType', N'03', N'مواصفات أخرى', NULL, N'USR', N'ar', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (62, N'videoSpecsSubtype', N'01', N'File Extension', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (63, N'videoSpecsSubtype', N'02', N'File Format', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (64, N'videoSpecsSubtype', N'03', N'Codec', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (65, N'videoSpecsSubtype', N'04', N'Video Depth', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (66, N'videoSpecsSubtype', N'05', N'Standard', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (67, N'videoSpecsSubtype', N'06', N'Data Rate / Bitrate', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (68, N'videoSpecsSubtype', N'07', N'Bitrate Mode', NULL, N'USR', N'en', 7, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (69, N'videoSpecsSubtype', N'08', N'Frame Size', NULL, N'USR', N'en', 8, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (70, N'videoSpecsSubtype', N'09', N'Frame Rate', NULL, N'USR', N'en', 9, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (71, N'videoSpecsSubtype', N'10', N'Chroma Sampling', NULL, N'USR', N'en', 10, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (72, N'videoSpecsSubtype', N'11', N'Bit Depth', NULL, N'USR', N'en', 11, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (73, N'videoSpecsSubtype', N'12', N'Aspect Ratio', NULL, N'USR', N'en', 12, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (74, N'videoSpecsSubtype', N'13', N'Color Space', NULL, N'USR', N'en', 13, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (75, N'videoSpecsSubtype', N'14', N'Scan Type', NULL, N'USR', N'en', 14, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (76, N'audioSpecsSubtype', N'01', N'Fille Extension', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (77, N'audioSpecsSubtype', N'02', N'File Format', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (78, N'audioSpecsSubtype', N'03', N'Bit Depth', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (79, N'audioSpecsSubtype', N'04', N'Bitrate Mode', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (80, N'audioSpecsSubtype', N'05', N'Bitrate', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (81, N'audioSpecsSubtype', N'06', N'Sampling Rate', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (82, N'audioSpecsSubtype', N'07', N'Channels', NULL, N'USR', N'en', 7, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (83, N'audioSpecsSubtype', N'08', N'Mixing', NULL, N'USR', N'en', 8, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (84, N'audioSpecsSubtype', N'09', N'Output Type', NULL, N'USR', N'en', 9, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (85, N'otherSpecsSubtype', N'01', N'others', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (86, N'workNationality', N'01', N'Turkish', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (87, N'workNationality', N'02', N'Hindi', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (88, N'workNationality', N'03', N'Mexican', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (89, N'workNationality', N'04', N'Korian', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (90, N'workNationality', N'05', N'Brazilian', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (91, N'originalLanguage', N'01', N'Native', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (92, N'originalLanguage', N'02', N'English', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (93, N'originalLanguage', N'03', N'Spanish', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (94, N'originalLanguage', N'04', N'Urdu', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (95, N'workStatus', N'01', N'Active', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (96, N'workStatus', N'02', N'Suspended', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (97, N'workStatus', N'03', N'Endorsed', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (98, N'workStatus', N'04', N'Canceled', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (99, N'characterType', N'01', N'Hero', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (100, N'characterType', N'02', N'Primary', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (101, N'characterType', N'03', N'Secondary', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (103, N'characterGender', N'01', N'Male', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (104, N'characterGender', N'02', N'Female', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (105, N'titleType', N'01', N'Production Manager', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (106, N'titleType', N'03', N'Sound Technician', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (107, N'empType', N'01', N'Admin', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (108, N'empType', N'02', N'Studio', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (109, N'workPartyType', N'01', N'Voice Actor', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (110, N'workPartyType', N'02', N'Technician', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (111, N'chargeUom', N'01', N'Episode', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (112, N'chargeUom', N'02', N'Scene', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (113, N'chargeUom', N'03', N'Hour', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (114, N'currencyCode', N'01', N'SYP', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (115, N'currencyCode', N'02', N'USD', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (116, N'currencyCode', N'03', N'EUR', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (117, N'mediaType', N'01', N'Master', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (118, N'mediaType', N'02', N'Script', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (119, N'mediaType', N'03', N'Low-Res', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (120, N'checkType', N'02', N'Pixel', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (121, N'checkType', N'03', N'Low Quality Format', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (122, N'checkType', N'04', N'Black Intervals', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (123, N'checkType', N'05', N'Repeating Scenes', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (125, N'checkType', N'06', N'Wrong Output', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (126, N'checkType', N'07', N'Sound Mismatch', NULL, N'USR', N'en', 7, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (127, N'checkType', N'08', N'Format Transfer Problems', NULL, N'USR', N'en', 8, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (128, N'checkType', N'09', N'Missing Fx', NULL, N'USR', N'en', 9, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (129, N'checkType', N'10', N'Missing Music', NULL, N'USR', N'en', 10, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (130, N'priority', N'01', N'Low', N'Can Wait', N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (131, N'priority', N'02', N'Normal', N'Schedule As per receiving order', N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (132, N'priority', N'03', N'High', N'Immediate Delivery', N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (133, N'orderStatus', N'01', N'New', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (134, N'orderStatus', N'02', N'Accepted', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (135, N'orderStatus', N'04', N'Rejected', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (136, N'orderStatus', N'05', N'Endorsed', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (143, N'checkType', N'01', N'Count Check', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (150, N'studio', N'01', N'Studio 01', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (151, N'studio', N'02', N'studio 02', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (152, N'studio', N'03', N'studio 03', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (153, N'studio', N'04', N'studio 04', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (154, N'studio', N'05', N'studio 05', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (155, N'studio', N'06', N'studio 06', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (156, N'dubbingTrnHdrStatus', N'01', N'Initiated', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (157, N'dubbingTrnHdrStatus', N'02', N'Published', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (158, N'dubbingTrnHdrStatus', N'03', N'Canceled', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (159, N'dubbingTrnHdrStatus', N'04', N'Endorsed', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (160, N'titleType', N'02', N'Supervisor', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (161, N'titleType', N'04', N'Assistant', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (162, N'titleType', N'05', N'Translator', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (163, N'titleType', N'06', N'Adaptor', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (164, N'empType', N'03', N'Contractor', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (165, N'workPartyType', N'03', N'Contractor', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (180, N'rating', N'01', N'Outstanding', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (181, N'rating', N'02', N'Above Average', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (182, N'rating', N'03', N'Below Average', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (183, N'rating', N'04', N'Poor', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (186, N'activityType', N'01', N'Translation', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (187, N'activityType', N'02', N'Adaptation', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (188, N'activityType', N'03', N'Discharging', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (189, N'activityType', N'04', N'Dubbing Supervision', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (191, N'dubbSheetStartRow', N'01', N'3', NULL, N'SYS', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (192, N'dubbSheetStartCol', N'01', N'4', NULL, N'SYS', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (193, N'costCenterType', N'01', N'Voice Actors', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (194, N'costCenterType', N'02', N'Studio Team', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (195, N'costCenterType', N'03', N'Translation', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (196, N'costCenterType', N'04', N'Adaptation', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (197, N'costCenterType', N'05', N'Discharge', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (198, N'orderStatus', N'06', N'Canceled', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (199, N'orderStatus', N'03', N'Active', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (200, N'activityType', N'05', N'Mixage', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (201, N'orderItemStatus', N'01', N'Initiated', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (202, N'orderItemStatus', N'02', N'Accepted', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (203, N'orderItemStatus', N'03', N'Rejected', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (204, N'orderItemStatus', N'04', N'Active', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (205, N'orderItemStatus', N'05', N'Canceled', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (206, N'orderItemStatus', N'06', N'Endorsed', NULL, N'USR', N'en', 6, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (207, N'trnType', N'01', N'Start Translation', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (208, N'trnType', N'02', N'End Translation', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (212, N'departure', N'01', N'Damascus', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (213, N'departure', N'02', N'Beirut', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (214, N'destination', N'01', N'Dubai', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (215, N'destination', N'02', N'Abu Dhabi', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (216, N'destination', N'03', N'Sharjah', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (221, N'shipmentMethod', N'01', N'Carrier', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (222, N'shipmentMethod', N'02', N'Fardous Personnel', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (223, N'shipmentMethod', N'03', N'Courier', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (224, N'carrierType', N'01', N'Airline', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (225, N'carrierType', N'02', N'Courier', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (226, N'shipmentStatus', N'01', N'Active', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (227, N'shipmentStatus', N'02', N'Canceled', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (228, N'shipmentStatus', N'03', N'Endorsed', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (229, N'feedbackType', N'02', N'Shipped Media', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (230, N'feedbackType', N'01', N'Uploaded Media', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (231, N'actionParty', N'01', N'Studio', NULL, N'USR', N'en', 1, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (232, N'actionParty', N'02', N'Mixage', NULL, N'USR', N'en', 2, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (233, N'actionParty', N'03', N'Montage', NULL, N'USR', N'en', 3, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (234, N'actionParty', N'04', N'Management', NULL, N'USR', N'en', 4, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (235, N'actionParty', N'05', N'All', NULL, N'USR', N'en', 5, 0, 1)
GO
INSERT [dbo].[dubbDomain] ([domainIntno], [domainName], [domainCode], [domainValue], [userMessage], [valueUsage], [langCode], [sortOrder], [minAccessLevel], [status]) VALUES (236, N'settings', N'fdw', N'Saturday', N'First Day of the Week', N'SYS', N'en', 1, 0, 1)
GO
SET IDENTITY_INSERT [dbo].[dubbDomain] OFF
GO
