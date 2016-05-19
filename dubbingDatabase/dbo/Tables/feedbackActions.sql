CREATE TABLE [dbo].[feedbackActions] (
    [actionIntno]     BIGINT         NOT NULL,
    [feedbackIntno]   BIGINT         NOT NULL,
    [actionParty]     NVARCHAR (50)  NOT NULL,
    [actionToBeTaken] NVARCHAR (255) NULL,
    [actionDate]      DATE           NULL,
    [checkedBy]       NVARCHAR (50)  NULL,
    [checkedDate]     DATETIME       NULL,
    [status]          BIT            NOT NULL,
    CONSTRAINT [PK_feedbackActions] PRIMARY KEY CLUSTERED ([actionIntno] ASC),
    CONSTRAINT [FK_feedbackActions_deliveryFeedbacks] FOREIGN KEY ([feedbackIntno]) REFERENCES [dbo].[deliveryFeedbacks] ([feedbackIntno])
);

