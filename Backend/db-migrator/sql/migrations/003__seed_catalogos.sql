-- ===================================================================================
-- Catálogos base (idempotente)
-- TipoArea modela Producción / Bodega / Patio (ADR-03: reemplaza las tablas Bodega y Patio)
-- Los Id son fijos para que el frontend y los seeds puedan referenciarlos.
-- ===================================================================================
IF NOT EXISTS (SELECT 1 FROM dbo.TipoArea WHERE Id = '00000000-0000-0000-0000-0000000000a1')
    INSERT INTO dbo.TipoArea (Id, Nombre, Descripcion, Estado)
    VALUES ('00000000-0000-0000-0000-0000000000a1', 'Produccion', 'Área productiva donde operan los equipos', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.TipoArea WHERE Id = '00000000-0000-0000-0000-0000000000a2')
    INSERT INTO dbo.TipoArea (Id, Nombre, Descripcion, Estado)
    VALUES ('00000000-0000-0000-0000-0000000000a2', 'Bodega', 'Almacén cubierto de equipos y repuestos', 1);

IF NOT EXISTS (SELECT 1 FROM dbo.TipoArea WHERE Id = '00000000-0000-0000-0000-0000000000a3')
    INSERT INTO dbo.TipoArea (Id, Nombre, Descripcion, Estado)
    VALUES ('00000000-0000-0000-0000-0000000000a3', 'Patio', 'Almacén a cielo abierto', 1);
GO
