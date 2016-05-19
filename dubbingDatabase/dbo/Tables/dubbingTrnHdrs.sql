CREATE TABLE [dbo].[dubbingTrnHdrs] (
    [dubbTrnHdrIntno] BIGINT IDENTITY (1, 1) NOT NULL,
    [fromDate]        DATE   NOT NULL,
    [thruDate]        DATE   NOT NULL,
    [isPaid]          BIT    NOT NULL,
    [status]          BIT    NOT NULL,
    CONSTRAINT [PK_dubbingTrnHdrs] PRIMARY KEY CLUSTERED ([dubbTrnHdrIntno] ASC)
);

