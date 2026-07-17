IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Inquilinos] (
    [Id] uniqueidentifier NOT NULL,
    [NombreComercial] nvarchar(200) NOT NULL,
    [DominioRed] nvarchar(250) NOT NULL,
    [PermitirIA] bit NOT NULL DEFAULT CAST(0 AS bit),
    [Estado] nvarchar(50) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [ModifiedAt] datetime2 NULL,
    [ModifiedBy] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_Inquilinos] PRIMARY KEY ([Id])
);

CREATE TABLE [Categorias] (
    [Id] uniqueidentifier NOT NULL,
    [NombreCategoria] nvarchar(150) NOT NULL,
    [Descripcion] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [ModifiedAt] datetime2 NULL,
    [ModifiedBy] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    [InquilinoId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categorias_Inquilinos_InquilinoId] FOREIGN KEY ([InquilinoId]) REFERENCES [Inquilinos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Usuarios] (
    [Id] uniqueidentifier NOT NULL,
    [FullName] nvarchar(200) NOT NULL,
    [Email] nvarchar(320) NOT NULL,
    [UserName] nvarchar(100) NOT NULL,
    [Password] nvarchar(500) NOT NULL,
    [Rol] int NOT NULL,
    [DepartamentoId] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [ModifiedAt] datetime2 NULL,
    [ModifiedBy] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    [InquilinoId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Usuarios_Categorias_DepartamentoId] FOREIGN KEY ([DepartamentoId]) REFERENCES [Categorias] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Usuarios_Inquilinos_InquilinoId] FOREIGN KEY ([InquilinoId]) REFERENCES [Inquilinos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Tickets] (
    [Id] uniqueidentifier NOT NULL,
    [Titulo] nvarchar(200) NOT NULL,
    [DescripcionOriginal] nvarchar(max) NOT NULL,
    [DescripcionSanitizada] nvarchar(max) NULL,
    [Estado] int NOT NULL,
    [Prioridad] int NOT NULL,
    [CategoriaId] uniqueidentifier NULL,
    [ResponsableTecnologiaId] uniqueidentifier NULL,
    [FechaResolucion] datetime2 NULL,
    [FechaAsignacion] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [CreatedBy] uniqueidentifier NULL,
    [ModifiedAt] datetime2 NULL,
    [ModifiedBy] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    [InquilinoId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Tickets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tickets_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Tickets_Inquilinos_InquilinoId] FOREIGN KEY ([InquilinoId]) REFERENCES [Inquilinos] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Tickets_Usuarios_ResponsableTecnologiaId] FOREIGN KEY ([ResponsableTecnologiaId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);

CREATE INDEX [IX_Categorias_InquilinoId] ON [Categorias] ([InquilinoId]);

CREATE UNIQUE INDEX [IX_Categorias_InquilinoId_NombreCategoria] ON [Categorias] ([InquilinoId], [NombreCategoria]);

CREATE UNIQUE INDEX [IX_Inquilinos_DominioRed] ON [Inquilinos] ([DominioRed]);

CREATE INDEX [IX_Inquilinos_NombreComercial] ON [Inquilinos] ([NombreComercial]);

CREATE INDEX [IX_Tickets_CategoriaId] ON [Tickets] ([CategoriaId]);

CREATE INDEX [IX_Tickets_Estado] ON [Tickets] ([Estado]);

CREATE INDEX [IX_Tickets_InquilinoId_CreatedAt] ON [Tickets] ([InquilinoId], [CreatedAt]);

CREATE INDEX [IX_Tickets_ResponsableTecnologiaId] ON [Tickets] ([ResponsableTecnologiaId]);

CREATE INDEX [IX_Usuarios_DepartamentoId] ON [Usuarios] ([DepartamentoId]);

CREATE INDEX [IX_Usuarios_InquilinoId] ON [Usuarios] ([InquilinoId]);

CREATE UNIQUE INDEX [IX_Usuarios_InquilinoId_Email] ON [Usuarios] ([InquilinoId], [Email]);

CREATE UNIQUE INDEX [IX_Usuarios_InquilinoId_UserName] ON [Usuarios] ([InquilinoId], [UserName]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260717035208_InitialCreate', N'10.0.8');

COMMIT;
GO

