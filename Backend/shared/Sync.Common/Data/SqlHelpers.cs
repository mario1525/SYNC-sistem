using System.Data;
using Microsoft.Data.SqlClient;

namespace Sync.Common.Data
{
    /// <summary>Atajos para construir parámetros y leer columnas de forma segura.</summary>
    public static class Sql
    {
        /// <summary>Crea un parámetro convirtiendo null a DBNull.</summary>
        public static SqlParameter P(string name, object? value) =>
            new(name.StartsWith('@') ? name : "@" + name, value ?? DBNull.Value);

        /// <summary>Convierte cadenas vacías a null (para FKs opcionales).</summary>
        public static string? NullIfEmpty(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value;
    }

    public static class DataRowExtensions
    {
        public static bool Has(this DataRow row, string column) =>
            row.Table.Columns.Contains(column) && row[column] != DBNull.Value;

        public static string Str(this DataRow row, string column) =>
            row.Has(column) ? Convert.ToString(row[column]) ?? string.Empty : string.Empty;

        public static string? StrOrNull(this DataRow row, string column) =>
            row.Has(column) ? Convert.ToString(row[column]) : null;

        public static int Int(this DataRow row, string column) =>
            row.Has(column) ? Convert.ToInt32(row[column]) : 0;

        public static long Long(this DataRow row, string column) =>
            row.Has(column) ? Convert.ToInt64(row[column]) : 0;

        public static bool Bool(this DataRow row, string column) =>
            row.Has(column) && Convert.ToBoolean(row[column]);

        public static DateTime? Date(this DataRow row, string column) =>
            row.Has(column) ? Convert.ToDateTime(row[column]) : null;

        public static TimeSpan Time(this DataRow row, string column) =>
            row.Has(column) ? (TimeSpan)row[column] : TimeSpan.Zero;

        public static List<T> MapRows<T>(this DataTable table, Func<DataRow, T> map) =>
            table.Rows.Cast<DataRow>().Select(map).ToList();
    }
}
