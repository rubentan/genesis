
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 01/17/2015 19:07:30
-- Generated from EDMX file: D:\_rtan\Dev\Genesis\Genesis.DataAccess\GenesisDBContext.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Genesis];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[tbl_branch]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_branch];
GO
IF OBJECT_ID(N'[dbo].[tbl_client]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_client];
GO
IF OBJECT_ID(N'[dbo].[tbl_document]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_document];
GO
IF OBJECT_ID(N'[dbo].[tbl_documentType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_documentType];
GO
IF OBJECT_ID(N'[dbo].[tbl_inventory]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_inventory];
GO
IF OBJECT_ID(N'[dbo].[tbl_product]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_product];
GO
IF OBJECT_ID(N'[dbo].[tbl_supplier]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_supplier];
GO
IF OBJECT_ID(N'[dbo].[tbl_transaction]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_transaction];
GO
IF OBJECT_ID(N'[dbo].[tbl_transactionType]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_transactionType];
GO
IF OBJECT_ID(N'[dbo].[tbl_users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[tbl_users];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'tbl_users'
CREATE TABLE [dbo].[tbl_users] (
    [userId] int IDENTITY(1,1) NOT NULL,
    [userName] nvarchar(50)  NOT NULL,
    [passWord] nvarchar(200)  NOT NULL,
    [emailAddress] nvarchar(50)  NULL,
    [firstName] nvarchar(50)  NOT NULL,
    [middleName] nvarchar(50)  NULL,
    [lastName] nvarchar(50)  NOT NULL,
    [branchId] int  NOT NULL,
    [dateLastLogin] datetime  NULL,
    [dateCreated] datetime  NOT NULL,
    [dateLastPasswordChange] datetime  NULL,
    [status] int  NULL,
    [isAdmin] int  NULL
);
GO

-- Creating table 'tbl_branch'
CREATE TABLE [dbo].[tbl_branch] (
    [branchId] int IDENTITY(1,1) NOT NULL,
    [branchCode] nvarchar(50)  NULL,
    [branchName] nvarchar(50)  NOT NULL,
    [branchAddress] nvarchar(100)  NULL,
    [dateCreated] datetime  NULL,
    [createdBy] int  NULL
);
GO

-- Creating table 'tbl_client'
CREATE TABLE [dbo].[tbl_client] (
    [clientId] int IDENTITY(1,1) NOT NULL,
    [clientCode] nvarchar(50)  NOT NULL,
    [clientName] nvarchar(150)  NOT NULL,
    [clientAddress] nvarchar(150)  NULL,
    [clientContactNumber] nvarchar(50)  NULL,
    [clientContactPerson] nvarchar(150)  NULL,
    [branchId] int  NULL,
    [status] int  NOT NULL,
    [dateCreated] datetime  NOT NULL,
    [createdBy] int  NOT NULL
);
GO

-- Creating table 'tbl_product'
CREATE TABLE [dbo].[tbl_product] (
    [productId] int IDENTITY(1,1) NOT NULL,
    [productCode] nvarchar(50)  NULL,
    [productName] nvarchar(150)  NULL,
    [productDescription] nvarchar(150)  NULL,
    [reorderLevel] int  NULL,
    [dateCreated] datetime  NULL,
    [createdBy] int  NULL
);
GO

-- Creating table 'tbl_supplier'
CREATE TABLE [dbo].[tbl_supplier] (
    [supplierId] int IDENTITY(1,1) NOT NULL,
    [supplierCode] nvarchar(50)  NOT NULL,
    [supplierName] nvarchar(150)  NOT NULL,
    [supplierAddress] nvarchar(150)  NULL,
    [supplierContactNumber] nvarchar(50)  NULL,
    [supplierContactPerson] nvarchar(150)  NULL,
    [branchId] int  NULL,
    [status] int  NOT NULL,
    [dateCreated] datetime  NOT NULL,
    [createdBy] int  NOT NULL
);
GO

-- Creating table 'tbl_document'
CREATE TABLE [dbo].[tbl_document] (
    [documentId] int IDENTITY(1,1) NOT NULL,
    [branchId] int  NOT NULL,
    [documentNumber] nvarchar(150)  NOT NULL,
    [documentType] int  NOT NULL,
    [dateCreated] datetime  NOT NULL,
    [createdBy] int  NOT NULL
);
GO

-- Creating table 'tbl_documentType'
CREATE TABLE [dbo].[tbl_documentType] (
    [documentTypeId] int IDENTITY(1,1) NOT NULL,
    [documentTypeName] nvarchar(150)  NOT NULL,
    [documentPrefix] nvarchar(15)  NOT NULL,
    [dateCreated] datetime  NOT NULL,
    [createdBy] int  NOT NULL
);
GO

-- Creating table 'tbl_inventory'
CREATE TABLE [dbo].[tbl_inventory] (
    [inventoryId] int IDENTITY(1,1) NOT NULL,
    [productId] int  NOT NULL,
    [branchId] int  NOT NULL,
    [inventoryCode] varchar(15)  NOT NULL,
    [inventoryDescription] varchar(150)  NOT NULL,
    [beginning] int  NOT NULL,
    [unitPrice] decimal(18,2)  NOT NULL,
    [dateCreated] datetime  NOT NULL,
    [createdBy] int  NOT NULL
);
GO

-- Creating table 'tbl_transaction'
CREATE TABLE [dbo].[tbl_transaction] (
    [transactionId] int IDENTITY(1,1) NOT NULL,
    [inventoryId] int  NOT NULL,
    [documentId] int  NOT NULL,
    [transactionType] int  NOT NULL,
    [quantity] int  NOT NULL,
    [unitPrice] decimal(18,2)  NULL,
    [dateCreated] datetime  NOT NULL
);
GO

-- Creating table 'tbl_transactionType'
CREATE TABLE [dbo].[tbl_transactionType] (
    [transactionTypeId] int IDENTITY(1,1) NOT NULL,
    [transactionTypeName] nvarchar(150)  NOT NULL,
    [transactionOperation] nvarchar(50)  NOT NULL,
    [dateCreated] datetime  NOT NULL,
    [createdBy] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [userId] in table 'tbl_users'
ALTER TABLE [dbo].[tbl_users]
ADD CONSTRAINT [PK_tbl_users]
    PRIMARY KEY CLUSTERED ([userId] ASC);
GO

-- Creating primary key on [branchId] in table 'tbl_branch'
ALTER TABLE [dbo].[tbl_branch]
ADD CONSTRAINT [PK_tbl_branch]
    PRIMARY KEY CLUSTERED ([branchId] ASC);
GO

-- Creating primary key on [clientId] in table 'tbl_client'
ALTER TABLE [dbo].[tbl_client]
ADD CONSTRAINT [PK_tbl_client]
    PRIMARY KEY CLUSTERED ([clientId] ASC);
GO

-- Creating primary key on [productId] in table 'tbl_product'
ALTER TABLE [dbo].[tbl_product]
ADD CONSTRAINT [PK_tbl_product]
    PRIMARY KEY CLUSTERED ([productId] ASC);
GO

-- Creating primary key on [supplierId] in table 'tbl_supplier'
ALTER TABLE [dbo].[tbl_supplier]
ADD CONSTRAINT [PK_tbl_supplier]
    PRIMARY KEY CLUSTERED ([supplierId] ASC);
GO

-- Creating primary key on [documentId] in table 'tbl_document'
ALTER TABLE [dbo].[tbl_document]
ADD CONSTRAINT [PK_tbl_document]
    PRIMARY KEY CLUSTERED ([documentId] ASC);
GO

-- Creating primary key on [documentTypeId] in table 'tbl_documentType'
ALTER TABLE [dbo].[tbl_documentType]
ADD CONSTRAINT [PK_tbl_documentType]
    PRIMARY KEY CLUSTERED ([documentTypeId] ASC);
GO

-- Creating primary key on [inventoryId] in table 'tbl_inventory'
ALTER TABLE [dbo].[tbl_inventory]
ADD CONSTRAINT [PK_tbl_inventory]
    PRIMARY KEY CLUSTERED ([inventoryId] ASC);
GO

-- Creating primary key on [transactionId] in table 'tbl_transaction'
ALTER TABLE [dbo].[tbl_transaction]
ADD CONSTRAINT [PK_tbl_transaction]
    PRIMARY KEY CLUSTERED ([transactionId] ASC);
GO

-- Creating primary key on [transactionTypeId] in table 'tbl_transactionType'
ALTER TABLE [dbo].[tbl_transactionType]
ADD CONSTRAINT [PK_tbl_transactionType]
    PRIMARY KEY CLUSTERED ([transactionTypeId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------