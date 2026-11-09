IF DB_ID('MaquinaRefrescosDb') IS NULL
BEGIN
    CREATE DATABASE MaquinaRefrescosDb;
END
GO

USE MaquinaRefrescosDb;
GO

IF OBJECT_ID('dbo.Ventas', 'U') IS NOT NULL DROP TABLE dbo.Ventas;
IF OBJECT_ID('dbo.Monedero', 'U') IS NOT NULL DROP TABLE dbo.Monedero;
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
GO

CREATE TABLE dbo.Productos
(
    Id          INT IDENTITY(1,1) NOT NULL,
    Nombre      NVARCHAR(50)      NOT NULL,
    Precio      DECIMAL(10,2)     NOT NULL,
    Existencia  INT               NOT NULL,

    CONSTRAINT PK_Productos PRIMARY KEY (Id),
    CONSTRAINT UQ_Productos_Nombre UNIQUE (Nombre),
    CONSTRAINT CK_Productos_Precio CHECK (Precio > 0),
    CONSTRAINT CK_Productos_Existencia CHECK (Existencia >= 0)
);
GO

CREATE TABLE dbo.Monedero
(
    Denominacion DECIMAL(10,2) NOT NULL,
    Cantidad     INT           NOT NULL,

    CONSTRAINT PK_Monedero PRIMARY KEY (Denominacion),
    CONSTRAINT CK_Monedero_Denominacion CHECK (Denominacion IN (1.00, 2.00, 5.00, 10.00)),
    CONSTRAINT CK_Monedero_Cantidad CHECK (Cantidad >= 0)
);
GO

CREATE TABLE dbo.Ventas
(
    Id                 INT IDENTITY(1,1) NOT NULL,
    ProductoId         INT               NOT NULL,
    Precio             DECIMAL(10,2)     NOT NULL,
    MontoIngresado     DECIMAL(10,2)     NOT NULL,
    Cambio             DECIMAL(10,2)     NOT NULL,
    MonedasIngresadas  NVARCHAR(200)     NOT NULL,
    MonedasCambio      NVARCHAR(200)     NOT NULL CONSTRAINT DF_Ventas_MonedasCambio DEFAULT (''),
    Fecha              DATETIME2(0)      NOT NULL CONSTRAINT DF_Ventas_Fecha DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Ventas PRIMARY KEY (Id),
    CONSTRAINT FK_Ventas_Productos FOREIGN KEY (ProductoId) REFERENCES dbo.Productos (Id),
    CONSTRAINT CK_Ventas_Montos CHECK (MontoIngresado >= Precio AND Cambio = MontoIngresado - Precio)
);
GO

CREATE INDEX IX_Ventas_Fecha ON dbo.Ventas (Fecha DESC);
GO

INSERT INTO dbo.Productos (Nombre, Precio, Existencia) VALUES
    (N'Coca-Cola',      17.00, 10),
    (N'Fanta',          15.00, 10),
    (N'Pepsi',          16.00, 10),
    (N'Yoli',           14.00, 10),
    (N'Coca-Cola Zero', 18.00,  0);
GO

INSERT INTO dbo.Monedero (Denominacion, Cantidad) VALUES
    (1.00,  5),
    (2.00,  5),
    (5.00,  5),
    (10.00, 5);
GO

SELECT Id, Nombre, Precio, Existencia FROM dbo.Productos;
SELECT Denominacion, Cantidad FROM dbo.Monedero;
GO
