
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/30/2016 23:37:05
-- Generated from EDMX file: D:\GIT\AdaniServices\CoreLib\CoreLib\Entity\EntityDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [AdaniBase];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_UserSessionKey]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[SessionKeys] DROP CONSTRAINT [FK_UserSessionKey];
GO
IF OBJECT_ID(N'[dbo].[FK_DeviceGroupDeviceSettings]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Devices] DROP CONSTRAINT [FK_DeviceGroupDeviceSettings];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[SessionKeys]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SessionKeys];
GO
IF OBJECT_ID(N'[dbo].[DeviceGroups]', 'U') IS NOT NULL
    DROP TABLE [dbo].[DeviceGroups];
GO
IF OBJECT_ID(N'[dbo].[Devices]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Devices];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NULL,
    [Login] nvarchar(max)  NOT NULL,
    [PasswordHash] nvarchar(max)  NOT NULL,
    [AccessLevel] bigint  NOT NULL
);
GO

-- Creating table 'SessionKeys'
CREATE TABLE [dbo].[SessionKeys] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [ExpirationTime] datetime  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'DeviceGroups'
CREATE TABLE [dbo].[DeviceGroups] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Devices'
CREATE TABLE [dbo].[Devices] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DeviceGroupId] int  NOT NULL,
    [ConnectionType] nvarchar(max)  NOT NULL,
    [GeneratorType] int  NOT NULL,
    [NormalVoltage] int  NOT NULL,
    [HighVoltage] int  NOT NULL,
    [NormalCurrent] int  NOT NULL,
    [HighCurrent] int  NOT NULL,
    [HighMode] int  NOT NULL,
    [ReseasonDate] float  NOT NULL,
    [WorkTime] float  NOT NULL,
    [XRayTime] float  NOT NULL,
    [LastWorkedDate] float  NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SessionKeys'
ALTER TABLE [dbo].[SessionKeys]
ADD CONSTRAINT [PK_SessionKeys]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'DeviceGroups'
ALTER TABLE [dbo].[DeviceGroups]
ADD CONSTRAINT [PK_DeviceGroups]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Devices'
ALTER TABLE [dbo].[Devices]
ADD CONSTRAINT [PK_Devices]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [User_Id] in table 'SessionKeys'
ALTER TABLE [dbo].[SessionKeys]
ADD CONSTRAINT [FK_UserSessionKey]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_UserSessionKey'
CREATE INDEX [IX_FK_UserSessionKey]
ON [dbo].[SessionKeys]
    ([User_Id]);
GO

-- Creating foreign key on [DeviceGroupId] in table 'Devices'
ALTER TABLE [dbo].[Devices]
ADD CONSTRAINT [FK_DeviceGroupDeviceSettings]
    FOREIGN KEY ([DeviceGroupId])
    REFERENCES [dbo].[DeviceGroups]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceGroupDeviceSettings'
CREATE INDEX [IX_FK_DeviceGroupDeviceSettings]
ON [dbo].[Devices]
    ([DeviceGroupId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------