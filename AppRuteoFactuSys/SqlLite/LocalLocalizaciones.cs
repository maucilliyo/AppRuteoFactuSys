using SQLite;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalLocalizaciones
    {
        public async Task<List<string>> GetProvincias()
        {
            var conn = await SqlLiteDatabase.GetConnection();

            var response = await conn.QueryAsync<Localizacion>(
                """
                SELECT DISTINCT provincia AS Valor
                FROM cliente
                WHERE provincia IS NOT NULL
                  AND provincia <> ''
                ORDER BY provincia
                """);

            var lista = new List<string>();

            foreach (var item in response)
                lista.Add(item.Valor);

            return lista;
        }

        public async Task<List<string>> GetCantones(string provincia)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            var response = await conn.QueryAsync<Localizacion>(
                """
                SELECT DISTINCT canton AS Valor
                FROM cliente
                WHERE provincia = ?
                  AND canton IS NOT NULL
                  AND canton <> ''
                ORDER BY canton
                """,
                provincia);

            var lista = new List<string>();

            foreach (var item in response)
                lista.Add(item.Valor);

            return lista;
        }

        public async Task<List<string>> GetDistritos(string canton)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            var response = await conn.QueryAsync<Localizacion>(
                """
                        SELECT DISTINCT distrito AS Valor
                        FROM cliente
                        WHERE canton = ?
                          AND distrito IS NOT NULL
                          AND distrito <> ''
                        ORDER BY distrito
                        """,
                canton);

            var lista = new List<string>();

            foreach (var item in response)
                lista.Add(item.Valor);

            return lista;
        }
    }

    public class Localizacion
    {
        public string Valor { get; set; } = string.Empty;
    }
}
