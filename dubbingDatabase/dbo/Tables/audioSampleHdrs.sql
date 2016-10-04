CREATE TABLE [dbo].[audioSampleHdrs]
(
	[audioSampleHdrIntno] BIGINT NOT NULL IDENTITY(1,1) PRIMARY KEY, 
    [Title] NVARCHAR(250) NOT NULL, 
	[VoiceActorIntno] bigint NOT NULL,
	[tagTemplateHdrIntno] bigint NOT NULL,
    [SubmitDate] DATETIME NOT NULL DEFAULT getdate(), 
    [FileName] NVARCHAR(250) NULL, 
    [FileUrl] NVARCHAR(MAX) NULL, 
    [Studio] NVARCHAR(10) NULL, 
    [Description] NVARCHAR(MAX) NULL, 
    [Status] NVARCHAR(10) NULL, 
    CONSTRAINT [FK_audioSampleHdrs_To_tagTemplateHdrs] FOREIGN KEY ([tagTemplateHdrIntno]) REFERENCES [tagTemplateHdrs]([tagTemplateHdrIntno]), 
    CONSTRAINT [FK_audioSampleHdrs_To_voiceActors] FOREIGN KEY ([VoiceActorIntno]) REFERENCES [voiceActors]([VoiceActorIntno]) on delete cascade
)
