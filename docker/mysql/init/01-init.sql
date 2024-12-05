CREATE DATABASE pdfsolutions;
GO

USE pdfsolutions;
GO

-- Create Usuarios table
CREATE TABLE Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(MAX) NOT NULL,
    Correo NVARCHAR(MAX) NOT NULL,
    Password NVARCHAR(MAX) NOT NULL
);

-- Create detalles_suscripciones table
CREATE TABLE detalles_suscripciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    tipo_suscripcion NVARCHAR(MAX) NOT NULL,
    fecha_inicio DATETIME2(6) NULL,
    fecha_final DATETIME2(6) NULL,
    precio DECIMAL(10,2) NULL,
    operaciones_realizadas INT NOT NULL,
    usuario_id INT NOT NULL,
    CONSTRAINT FK_detalles_suscripciones_Usuarios_usuario_id 
        FOREIGN KEY (usuario_id) REFERENCES Usuarios (Id) ON DELETE CASCADE
);

-- Create operaciones_pdf table
CREATE TABLE operaciones_pdf (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    usuario_id INT NOT NULL,
    TipoOperacion NVARCHAR(MAX) NOT NULL,
    fecha_operacion DATETIME2(6) NOT NULL,
    CONSTRAINT FK_operaciones_pdf_Usuarios_usuario_id 
        FOREIGN KEY (usuario_id) REFERENCES Usuarios (Id) ON DELETE CASCADE
);

GO