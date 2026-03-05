-- Seed data for App_Segments table
-- Execute this script to add default segments to the database

USE [DonaRogDB-Dev];
GO

SET QUOTED_IDENTIFIER ON;
GO

-- Check if segments already exist
IF NOT EXISTS (SELECT 1 FROM App_Segments WHERE Code = 'VIP')
BEGIN
    INSERT INTO App_Segments (Id, Code, Name, Description, ColorCode, Icon, DisplayOrder, IsActive, IsSystem, ExtraProperties, ConcurrencyStamp, TenantId)
    VALUES 
    (NEWID(), 'VIP', 'VIP Donors', 'Top-tier donors with exceptional giving history', '#FFD700', 'crown', 1, 1, 0, '{}', NEWID(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM App_Segments WHERE Code = 'MAJOR')
BEGIN
    INSERT INTO App_Segments (Id, Code, Name, Description, ColorCode, Icon, DisplayOrder, IsActive, IsSystem, ExtraProperties, ConcurrencyStamp, TenantId)
    VALUES 
    (NEWID(), 'MAJOR', 'Major Donors', 'Significant contributors with high RFM scores', '#FF6B6B', 'star', 2, 1, 0, '{}', NEWID(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM App_Segments WHERE Code = 'REGULAR')
BEGIN
    INSERT INTO App_Segments (Id, Code, Name, Description, ColorCode, Icon, DisplayOrder, IsActive, IsSystem, ExtraProperties, ConcurrencyStamp, TenantId)
    VALUES 
    (NEWID(), 'REGULAR', 'Regular Donors', 'Consistent and reliable donors', '#4ECDC4', 'user', 3, 1, 0, '{}', NEWID(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM App_Segments WHERE Code = 'NEW')
BEGIN
    INSERT INTO App_Segments (Id, Code, Name, Description, ColorCode, Icon, DisplayOrder, IsActive, IsSystem, ExtraProperties, ConcurrencyStamp, TenantId)
    VALUES 
    (NEWID(), 'NEW', 'New Donors', 'First-time donors requiring nurturing', '#95E1D3', 'gift', 4, 1, 0, '{}', NEWID(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM App_Segments WHERE Code = 'LAPSED')
BEGIN
    INSERT INTO App_Segments (Id, Code, Name, Description, ColorCode, Icon, DisplayOrder, IsActive, IsSystem, ExtraProperties, ConcurrencyStamp, TenantId)
    VALUES 
    (NEWID(), 'LAPSED', 'Lapsed Donors', 'Previously active donors who haven''t given recently', '#FFA07A', 'clock-circle', 5, 1, 0, '{}', NEWID(), NULL);
END

IF NOT EXISTS (SELECT 1 FROM App_Segments WHERE Code = 'ATRISK')
BEGIN
    INSERT INTO App_Segments (Id, Code, Name, Description, ColorCode, Icon, DisplayOrder, IsActive, IsSystem, ExtraProperties, ConcurrencyStamp, TenantId)
    VALUES 
    (NEWID(), 'ATRISK', 'At-Risk Donors', 'Donors showing signs of disengagement', '#F38181', 'warning', 6, 1, 0, '{}', NEWID(), NULL);
END

GO

-- Verify inserted data
SELECT Id, Code, Name, DisplayOrder, IsActive 
FROM App_Segments 
ORDER BY DisplayOrder;
GO
