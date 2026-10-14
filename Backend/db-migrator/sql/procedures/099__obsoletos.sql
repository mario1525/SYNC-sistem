-- ===================================================================================
-- Limpieza de procedimientos obsoletos (convenciones antiguas y tablas inexistentes).
-- Se ejecuta en cada despliegue; es idempotente.
-- ===================================================================================
DROP PROCEDURE IF EXISTS dbo.db_Sp_Bodega_Get, dbo.db_Sp_Bodega_Set, dbo.db_Sp_Bodega_Del, dbo.db_Sp_Bodega_Active;
DROP PROCEDURE IF EXISTS dbo.db_Sp_Patio_Get, dbo.db_Sp_Patio_Set, dbo.db_Sp_Patio_Del, dbo.db_Sp_Patio_Active;
DROP PROCEDURE IF EXISTS dbo.db_Sp_Guia_Act_Equipo_Get, dbo.db_Sp_Guia_Act_Equipo_Set, dbo.db_Sp_Guia_Act_Equipo_Del, dbo.db_Sp_Guia_Act_Equipo_Active;
DROP PROCEDURE IF EXISTS dbo.db_Sp_Act_Equipo_Get, dbo.db_Sp_Act_Equipo_Set, dbo.db_Sp_Act_Equipo_Del, dbo.db_Sp_Act_Equipo_Active;
DROP PROCEDURE IF EXISTS dbo.db_Sp_Guia_Equipo_Get, dbo.db_Sp_Guia_Equipo_Set, dbo.db_Sp_Guia_Equipo_Del, dbo.db_Sp_Guia_Equipo_Active;
DROP PROCEDURE IF EXISTS dbo.db_Sp_CompEsp_Get, dbo.db_Sp_CompEsp_Set, dbo.db_Sp_CompEsp_Del, dbo.db_Sp_CompEsp_Active;
GO
