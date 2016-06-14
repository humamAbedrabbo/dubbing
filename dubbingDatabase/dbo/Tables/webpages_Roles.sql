﻿CREATE TABLE [dbo].[webpages_Roles] (
    [RoleId]   INT            IDENTITY (1, 1) NOT NULL,
    [RoleName] NVARCHAR (256) COLLATE Latin1_General_CI_AS NOT NULL,
    CONSTRAINT [PK__webpages__8AFACE1A1EC48A19] PRIMARY KEY CLUSTERED ([RoleId] ASC),
    CONSTRAINT [UQ__webpages__8A2B616021A0F6C4] UNIQUE NONCLUSTERED ([RoleName] ASC)
);

