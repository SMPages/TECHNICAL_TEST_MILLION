/* 
   Crea la base RealEstateDb, su esquema dbo, tablas, FKs, índices y datos de ejemplo.
   Es idempotente: elimina tablas si existen y las vuelve a crear (modo clean).
*/

SET NOCOUNT ON;

------------------------------------------------------------
-- 0) Crear Base de Datos si no existe
------------------------------------------------------------
IF DB_ID(N'RealEstateDb') IS NULL
BEGIN
    PRINT 'Creating database RealEstateDb...';
    CREATE DATABASE RealEstateDb;
END
GO

USE RealEstateDb;
GO

------------------------------------------------------------
-- 1) Eliminar tablas (si existen) para recrear limpio
------------------------------------------------------------
IF OBJECT_ID('dbo.PropertyTrace', 'U') IS NOT NULL DROP TABLE dbo.PropertyTrace;
IF OBJECT_ID('dbo.PropertyImage', 'U') IS NOT NULL DROP TABLE dbo.PropertyImage;
IF OBJECT_ID('dbo.Property',      'U') IS NOT NULL DROP TABLE dbo.Property;
IF OBJECT_ID('dbo.Owner',         'U') IS NOT NULL DROP TABLE dbo.Owner;
GO

------------------------------------------------------------
-- 2) Crear tablas
------------------------------------------------------------

-- Owner
CREATE TABLE dbo.Owner (
    IdOwner      INT IDENTITY(1,1) NOT NULL,
    [Name]       NVARCHAR(150) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Address]    NVARCHAR(300) COLLATE Modern_Spanish_CI_AS NULL,
    Photo        NVARCHAR(300) COLLATE Modern_Spanish_CI_AS NULL,
    Birthday     DATE NULL,
    Email        NVARCHAR(150) COLLATE Modern_Spanish_CI_AS NULL,
    Phone        NVARCHAR(50)  COLLATE Modern_Spanish_CI_AS NULL,
    CreatedAt    DATETIME2(0) NOT NULL CONSTRAINT DF_Owner_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Owner PRIMARY KEY (IdOwner)
);
GO

-- Property
CREATE TABLE dbo.Property (
    IdProperty    INT IDENTITY(1,1) NOT NULL,
    CodeInternal  NVARCHAR(50)  COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Name]        NVARCHAR(150) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Address]     NVARCHAR(300) COLLATE Modern_Spanish_CI_AS NOT NULL,
    City          NVARCHAR(100) COLLATE Modern_Spanish_CI_AS NULL,
    Price         DECIMAL(18,2) NOT NULL,
    [Year]        SMALLINT NULL,
    Bedrooms      TINYINT  NULL,
    Bathrooms     TINYINT  NULL,
    AreaSqFt      FLOAT    NULL,
    IdOwner       INT      NOT NULL,
    CreatedAt     DATETIME2(0) NOT NULL CONSTRAINT DF_Property_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Property PRIMARY KEY (IdProperty),
    CONSTRAINT UQ_Property_CodeInternal UNIQUE (CodeInternal),
    CONSTRAINT FK_Property_Owner FOREIGN KEY (IdOwner)
        REFERENCES dbo.Owner(IdOwner)
);
GO

-- Índices útiles para filtros
CREATE INDEX IX_Property_City  ON dbo.Property(City);
CREATE INDEX IX_Property_Price ON dbo.Property(Price);
CREATE INDEX IX_Property_IdOwner ON dbo.Property(IdOwner);
GO

-- PropertyImage
CREATE TABLE dbo.PropertyImage (
    IdPropertyImage INT IDENTITY(1,1) NOT NULL,
    IdProperty      INT NOT NULL,
    FileUrl         NVARCHAR(300) COLLATE Modern_Spanish_CI_AS NOT NULL,
    Enabled         BIT NOT NULL CONSTRAINT DF_PropertyImage_Enabled DEFAULT (1),
    IsMain          BIT NOT NULL CONSTRAINT DF_PropertyImage_IsMain  DEFAULT (0),
    Caption         NVARCHAR(200) COLLATE Modern_Spanish_CI_AS NULL,
    SortOrder       INT NOT NULL CONSTRAINT DF_PropertyImage_SortOrder DEFAULT (0),
    CONSTRAINT PK_PropertyImage PRIMARY KEY (IdPropertyImage),
    CONSTRAINT FK_PropertyImage_Property FOREIGN KEY (IdProperty)
        REFERENCES dbo.Property(IdProperty)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_PropertyImage_IdProperty ON dbo.PropertyImage(IdProperty);
GO

-- PropertyTrace (histórico)
CREATE TABLE dbo.PropertyTrace (
    IdPropertyTrace INT IDENTITY(1,1) NOT NULL,
    IdProperty      INT NOT NULL,
    DateSale        DATE NULL,
    [Name]          NVARCHAR(100) COLLATE Modern_Spanish_CI_AS NOT NULL,
    [Value]         DECIMAL(18,2) NOT NULL,
    Tax             DECIMAL(18,2) NOT NULL CONSTRAINT DF_PropertyTrace_Tax DEFAULT (0),
    CreatedAt       DATETIME2(0) NOT NULL CONSTRAINT DF_PropertyTrace_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_PropertyTrace PRIMARY KEY (IdPropertyTrace),
    CONSTRAINT FK_PropertyTrace_Property FOREIGN KEY (IdProperty)
        REFERENCES dbo.Property(IdProperty)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_PropertyTrace_IdProperty ON dbo.PropertyTrace(IdProperty);
GO

------------------------------------------------------------
-- 3) Datos de ejemplo
------------------------------------------------------------

-- Owners
INSERT INTO dbo.Owner ([Name], [Address], Photo, Birthday, Email, Phone)
VALUES
(N'John Doe',  N'123 Main St, Miami, FL',    NULL, '1985-05-12', N'john.doe@mail.com',  N'+1-305-555-1001'),
(N'Mary Smith',N'456 Ocean Dr, Miami, FL',   NULL, '1990-09-20', N'mary.smith@mail.com',N'+1-305-555-1002'),
(N'Carlos Pérez', N'Av 30 #15-20, Bogotá',   NULL, '1988-03-15', N'carlos.perez@mail.com', N'3105551003'),
(N'Ana Gómez',    N'Calle 50 #80-90, Medellín', NULL,'1992-11-02', N'ana.gomez@mail.com', N'3105551004'),
(N'Luis Martínez',N'Carrera 7 #45-67, Cali', NULL, '1980-01-25', N'luis.mtz@mail.com',  N'3105551005');

-- Properties (10 ejemplos)
INSERT INTO dbo.Property
(CodeInternal, [Name], [Address], City, Price, [Year], Bedrooms, Bathrooms, AreaSqFt, IdOwner)
VALUES
(N'P-001', N'Apartamento Centro',   N'Cra 10 #20-30',         N'Bogotá',   250000.00, 2015, 2, 1, 75, 1),
(N'P-002', N'Casa Norte',           N'Calle 123 #45-67',      N'Bogotá',   380000.00, 2019, 3, 2, 120, 1),
(N'P-003', N'Loft Chapinero',       N'Calle 60 #9-12',        N'Bogotá',   310000.00, 2020, 1, 1, 55, 3),
(N'P-004', N'Apartamento Poblado',  N'Cra 43A #5-50',         N'Medellín', 420000.00, 2021, 2, 2, 95, 4),
(N'P-005', N'Casa Campestre',       N'Km 5 vía La Calera',    N'Bogotá',   800000.00, 2018, 4, 3, 250, 1),
(N'P-006', N'Penthouse Laureles',   N'Calle 35 #70-45',       N'Medellín', 600000.00, 2022, 3, 3, 180, 4),
(N'P-007', N'Casa Sur',             N'Calle 15 #25-50',       N'Cali',     270000.00, 2016, 3, 2, 130, 5),
(N'P-008', N'Apartaestudio Centro', N'Calle 10 #5-30',        N'Cali',     150000.00, 2017, 1, 1, 45, 5),
(N'P-009', N'Casa Familiar',        N'Calle 80 #120-55',      N'Bogotá',   500000.00, 2020, 5, 4, 280, 3),
(N'P-010', N'Dúplex Chicó',         N'Calle 92 #11-40',       N'Bogotá',   950000.00, 2023, 4, 4, 300, 2);

-- Images
INSERT INTO dbo.PropertyImage (IdProperty, FileUrl, Enabled, IsMain, Caption, SortOrder)
VALUES
(1, N'/uploads/images/apto-centro.jpg', 1, 1, N'Sala principal', 1),
(2, N'/uploads/images/casa-norte.jpg', 1, 1, N'Fachada', 1),
(3, N'/uploads/images/loft-chapinero.jpg', 1, 1, N'Vista interior', 1),
(4, N'/uploads/images/apto-poblado.jpg', 1, 1, N'Balcón panorámico', 1),
(5, N'/uploads/images/casa-campestre.jpg', 1, 1, N'Zona verde', 1),
(6, N'/uploads/images/penthouse-laureles.jpg', 1, 1, N'Terraza', 1),
(7, N'/uploads/images/casa-sur.jpg', 1, 1, N'Jardín', 1),
(8, N'/uploads/images/aparta-centro.jpg', 1, 1, N'Cocina integrada', 1),
(9, N'/uploads/images/casa-familiar.jpg', 1, 1, N'Fachada remodelada', 1),
(10, N'/uploads/images/duplex-chico.jpg', 1, 1, N'Sala doble altura', 1);

-- Traces (históricos de cambios de precio)
INSERT INTO dbo.PropertyTrace (IdProperty, DateSale, [Name], [Value], Tax)
VALUES
(2, NULL, N'Cambio de precio', 400000.00, 0.00),
(3, NULL, N'Cambio de precio', 320000.00, 0.00),
(4, NULL, N'Cambio de precio', 430000.00, 0.00),
(5, NULL, N'Cambio de precio', 850000.00, 0.00),
(6, NULL, N'Cambio de precio', 620000.00, 0.00),
(7, NULL, N'Cambio de precio', 280000.00, 0.00),
(8, NULL, N'Cambio de precio', 160000.00, 0.00),
(9, NULL, N'Cambio de precio', 520000.00, 0.00),
(10, NULL, N'Cambio de precio', 970000.00, 0.00);
GO

------------------------------------------------------------
-- 4) Verificación rápida
------------------------------------------------------------
PRINT 'Owners:';
SELECT COUNT(*) AS Owners FROM dbo.Owner;

PRINT 'Properties:';
SELECT COUNT(*) AS Properties FROM dbo.Property;

PRINT 'Sample properties:';
SELECT TOP (10) IdProperty, CodeInternal, [Name], City, Price, CreatedAt FROM dbo.Property ORDER BY IdProperty;
GO