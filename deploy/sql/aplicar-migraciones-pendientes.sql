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
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE TABLE [leads] (
        [id] uniqueidentifier NOT NULL,
        [nombre] nvarchar(150) NOT NULL,
        [email] nvarchar(254) NOT NULL,
        [telefono_numero] nvarchar(15) NOT NULL,
        [telefono_indicativo] nvarchar(5) NOT NULL,
        [origen] nvarchar(30) NOT NULL,
        [estado] nvarchar(40) NOT NULL,
        [propiedad_de_interes_id] uniqueidentifier NULL,
        [estimacion_minima_monto] numeric(18,2) NULL,
        [estimacion_minima_moneda] nvarchar(3) NULL,
        [estimacion_maxima_monto] numeric(18,2) NULL,
        [estimacion_maxima_moneda] nvarchar(3) NULL,
        [calculo_area_construccion_m2] numeric(12,2) NULL,
        [calculo_tipo_acabado] nvarchar(20) NULL,
        [calculo_municipio] nvarchar(100) NULL,
        [calculo_tipo_proyecto] nvarchar(20) NULL,
        [estimacion_calculada_en] datetimeoffset NULL,
        CONSTRAINT [PK_leads] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE TABLE [propiedades] (
        [id] uniqueidentifier NOT NULL,
        [titulo] nvarchar(200) NOT NULL,
        [descripcion] nvarchar(max) NOT NULL,
        [tipo_inmueble] nvarchar(30) NOT NULL,
        [precio_monto] numeric(18,2) NOT NULL,
        [precio_moneda] nvarchar(3) NOT NULL,
        [estado] nvarchar(30) NOT NULL,
        [direccion] nvarchar(300) NOT NULL,
        [municipio] nvarchar(100) NOT NULL,
        [departamento] nvarchar(100) NOT NULL,
        [latitud] numeric(9,6) NULL,
        [longitud] numeric(9,6) NULL,
        [area_terreno_valor] numeric(12,2) NOT NULL,
        [area_terreno_unidad] nvarchar(20) NOT NULL,
        [area_construida_valor] numeric(12,2) NULL,
        [area_construida_unidad] nvarchar(20) NULL,
        [pendiente_porcentaje] numeric(5,2) NOT NULL,
        [tipo_suelo] nvarchar(20) NOT NULL,
        [topografia] nvarchar(20) NOT NULL,
        [nivel_freatico_metros] numeric(6,2) NULL,
        CONSTRAINT [PK_propiedades] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE TABLE [lead_estimacion_desglose] (
        [id] int NOT NULL IDENTITY,
        [categoria] nvarchar(60) NOT NULL,
        [monto] numeric(18,2) NOT NULL,
        [moneda] nvarchar(3) NOT NULL,
        [lead_id] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_lead_estimacion_desglose] PRIMARY KEY ([id]),
        CONSTRAINT [FK_lead_estimacion_desglose_leads_lead_id] FOREIGN KEY ([lead_id]) REFERENCES [leads] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE TABLE [propiedad_multimedia] (
        [id] uniqueidentifier NOT NULL,
        [url] nvarchar(500) NOT NULL,
        [tipo] nvarchar(20) NOT NULL,
        [orden] int NOT NULL,
        [propiedad_id] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_propiedad_multimedia] PRIMARY KEY ([id]),
        CONSTRAINT [FK_propiedad_multimedia_propiedades_propiedad_id] FOREIGN KEY ([propiedad_id]) REFERENCES [propiedades] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE TABLE [propiedad_retiros_ambientales] (
        [id] int NOT NULL IDENTITY,
        [tipo_fuente] nvarchar(30) NOT NULL,
        [distancia_minima_metros] numeric(8,2) NOT NULL,
        [normativa_aplicable] nvarchar(300) NOT NULL,
        [propiedad_id] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_propiedad_retiros_ambientales] PRIMARY KEY ([id]),
        CONSTRAINT [FK_propiedad_retiros_ambientales_propiedades_propiedad_id] FOREIGN KEY ([propiedad_id]) REFERENCES [propiedades] ([id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_lead_estimacion_desglose_lead_id] ON [lead_estimacion_desglose] ([lead_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_leads_estado] ON [leads] ([estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_leads_origen] ON [leads] ([origen]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_propiedad_multimedia_propiedad_id] ON [propiedad_multimedia] ([propiedad_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_propiedad_retiros_ambientales_propiedad_id] ON [propiedad_retiros_ambientales] ([propiedad_id]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_propiedades_estado] ON [propiedades] ([estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    CREATE INDEX [IX_propiedades_tipo_inmueble] ON [propiedades] ([tipo_inmueble]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260826221952_InitialCreateSqlServer'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260826221952_InitialCreateSqlServer', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260831220515_AgregarNotificacionComercialEnviadaEn'
)
BEGIN
    ALTER TABLE [leads] ADD [notificacion_comercial_enviada_en] datetimeoffset NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260831220515_AgregarNotificacionComercialEnviadaEn'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260831220515_AgregarNotificacionComercialEnviadaEn', N'8.0.10');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901130953_AgregarSolicitudesViabilidadAmbiental'
)
BEGIN
    CREATE TABLE [solicitudes_viabilidad_ambiental] (
        [id] uniqueidentifier NOT NULL,
        [solicitante_nombre] nvarchar(150) NOT NULL,
        [solicitante_email] nvarchar(254) NOT NULL,
        [solicitante_telefono_numero] nvarchar(15) NOT NULL,
        [solicitante_telefono_indicativo] nvarchar(5) NOT NULL,
        [propiedad_id] uniqueidentifier NULL,
        [lote_departamento] nvarchar(100) NULL,
        [lote_municipio] nvarchar(100) NULL,
        [lote_direccion_referencia] nvarchar(250) NULL,
        [monto] numeric(18,2) NOT NULL,
        [moneda] nvarchar(3) NOT NULL,
        [estado] nvarchar(20) NOT NULL,
        [pago_confirmado_en] datetimeoffset NULL,
        CONSTRAINT [PK_solicitudes_viabilidad_ambiental] PRIMARY KEY ([id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901130953_AgregarSolicitudesViabilidadAmbiental'
)
BEGIN
    CREATE INDEX [IX_solicitudes_viabilidad_ambiental_estado] ON [solicitudes_viabilidad_ambiental] ([estado]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260901130953_AgregarSolicitudesViabilidadAmbiental'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260901130953_AgregarSolicitudesViabilidadAmbiental', N'8.0.10');
END;
GO

COMMIT;
GO

