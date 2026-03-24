
USE [RfidAccessDB];
GO

-- 1. Users Table
CREATE TABLE [dbo].[Users] (
    [UserId] INT PRIMARY KEY IDENTITY(1,1),
    [UID] NVARCHAR(50) NOT NULL UNIQUE,
    [FullName] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(255),
    [PhoneNumber] NVARCHAR(20),
    [Role] NVARCHAR(50) NOT NULL,
    [Department] NVARCHAR(100),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME
);

CREATE INDEX [idx_uid] ON [dbo].[Users]([UID]);
CREATE INDEX [idx_active] ON [dbo].[Users]([IsActive]);

-- 2. AccessLogs Table
CREATE TABLE [dbo].[AccessLogs] (
    [LogId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [UID] NVARCHAR(50) NOT NULL,
    [AccessTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [AccessType] NVARCHAR(20) NOT NULL,
    [Status] NVARCHAR(20) NOT NULL,
    [Reason] NVARCHAR(255),
    [IPAddress] NVARCHAR(50),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE
);

CREATE INDEX [idx_uid_log] ON [dbo].[AccessLogs]([UID]);
CREATE INDEX [idx_time] ON [dbo].[AccessLogs]([AccessTime]);
CREATE INDEX [idx_status] ON [dbo].[AccessLogs]([Status]);
CREATE INDEX [idx_userid] ON [dbo].[AccessLogs]([UserId]);

-- 3. ApiKeys Table
CREATE TABLE [dbo].[ApiKeys] (
    [ApiKeyId] INT PRIMARY KEY IDENTITY(1,1),
    [ClientName] NVARCHAR(255) NOT NULL,
    [ApiKey] NVARCHAR(255) NOT NULL UNIQUE,
    [SecretKey] NVARCHAR(500),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [ExpiresAt] DATETIME,
    [LastUsedAt] DATETIME,
    [AllowedIPs] NVARCHAR(500),
    [Description] NVARCHAR(500)
);

CREATE INDEX [idx_apikey] ON [dbo].[ApiKeys]([ApiKey]);
CREATE INDEX [idx_active_key] ON [dbo].[ApiKeys]([IsActive]);

-- 4. AccessRules Table
CREATE TABLE [dbo].[AccessRules] (
    [RuleId] INT PRIMARY KEY IDENTITY(1,1),
    [UserId] INT NOT NULL,
    [AllowedStartTime] TIME,
    [AllowedEndTime] TIME,
    [AllowedDays] NVARCHAR(100),
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedAt] DATETIME,
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE
);

CREATE INDEX [idx_userid_rule] ON [dbo].[AccessRules]([UserId]);

-- 5. AuditLogs Table
CREATE TABLE [dbo].[AuditLogs] (
    [AuditId] INT PRIMARY KEY IDENTITY(1,1),
    [Action] NVARCHAR(100) NOT NULL,
    [EntityType] NVARCHAR(50) NOT NULL,
    [EntityId] INT,
    [OldValues] NVARCHAR(MAX),
    [NewValues] NVARCHAR(MAX),
    [ChangedBy] NVARCHAR(100) NOT NULL,
    [ChangedAt] DATETIME NOT NULL DEFAULT GETDATE(),
    [IP_Address] NVARCHAR(50)
);

CREATE INDEX [idx_changedat] ON [dbo].[AuditLogs]([ChangedAt]);
CREATE INDEX [idx_entitytype] ON [dbo].[AuditLogs]([EntityType]);

-- ===== INSERT DEMO DATA =====

-- Insert Demo Users
INSERT INTO [dbo].[Users] ([UID], [FullName], [Email], [PhoneNumber], [Role], [Department], [IsActive], [CreatedAt])
VALUES
    ('A1B2C3D4', 'John Doe', 'john@example.com', '0917123456', 'Employee', 'IT', 1, GETDATE()),
    ('E5F6G7H8', 'Jane Smith', 'jane@example.com', '0917654321', 'Employee', 'HR', 1, GETDATE()),
    ('I9J0K1L2', 'Bob Johnson', 'bob@example.com', '0918888888', 'Visitor', 'Admin', 1, GETDATE()),
    ('M3N4O5P6', 'Alice Brown', 'alice@example.com', '0919999999', 'Contractor', 'Finance', 1, GETDATE());

-- Insert Demo API Keys
INSERT INTO [dbo].[ApiKeys] ([ClientName], [ApiKey], [SecretKey], [IsActive], [CreatedAt], [Description])
VALUES
    ('Frontend App', 'RFID_ABC123XYZ789', 'SECRET_XYZ789ABC123', 1, GETDATE(), 'Web Dashboard'),
    ('Mobile App', 'RFID_DEF456QWE123', 'SECRET_QWE123DEF456', 1, GETDATE(), 'Mobile Application');

-- Insert Demo Access Rules (9 AM - 5 PM, Monday-Friday)
INSERT INTO [dbo].[AccessRules] ([UserId], [AllowedStartTime], [AllowedEndTime], [AllowedDays], [IsActive], [CreatedAt])
VALUES
    (1, '09:00:00', '17:00:00', 'MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY', 1, GETDATE()),
    (2, '08:00:00', '18:00:00', 'MONDAY,TUESDAY,WEDNESDAY,THURSDAY,FRIDAY', 1, GETDATE());

-- Insert Sample Access Logs
INSERT INTO [dbo].[AccessLogs] ([UserId], [UID], [AccessTime], [AccessType], [Status], [Reason], [IPAddress])
VALUES
    (1, 'A1B2C3D4', DATEADD(HOUR, -3, GETDATE()), 'ENTRY', 'GRANTED', NULL, '192.168.1.10'),
    (2, 'E5F6G7H8', DATEADD(HOUR, -2, GETDATE()), 'ENTRY', 'GRANTED', NULL, '192.168.1.11'),
    (3, 'I9J0K1L2', DATEADD(HOUR, -1, GETDATE()), 'ENTRY', 'GRANTED', NULL, '192.168.1.12'),
    (1, 'A1B2C3D4', DATEADD(MINUTE, -30, GETDATE()), 'EXIT', 'GRANTED', NULL, '192.168.1.10'),
    (-1, 'INVALID01', DATEADD(MINUTE, -15, GETDATE()), 'ENTRY', 'DENIED', 'User not found', '192.168.1.13');

PRINT 'Database setup completed successfully!';
