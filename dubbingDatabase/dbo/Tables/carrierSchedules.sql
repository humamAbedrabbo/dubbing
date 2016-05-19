CREATE TABLE [dbo].[carrierSchedules] (
    [carrierScheduleIntno] BIGINT        IDENTITY (1, 1) NOT NULL,
    [weekDay]              NVARCHAR (50) NOT NULL,
    [carrierIntno]         BIGINT        NOT NULL,
    [flightTime]           TIME (0)      NOT NULL,
    [departure]            NVARCHAR (50) NOT NULL,
    [destination]          NVARCHAR (50) NOT NULL,
    [status]               BIT           NOT NULL,
    CONSTRAINT [PK_carrierSchedules] PRIMARY KEY CLUSTERED ([carrierScheduleIntno] ASC),
    CONSTRAINT [FK_carrierSchedules_carriers] FOREIGN KEY ([carrierIntno]) REFERENCES [dbo].[carriers] ([carrierIntno])
);

