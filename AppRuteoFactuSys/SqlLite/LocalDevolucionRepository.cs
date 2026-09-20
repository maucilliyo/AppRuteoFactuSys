using AppRuteoFactuSys.Models;

namespace AppRuteoFactuSys.SqlLite
{
    public class LocalDevolucionRepository
    {
        public async Task Nueva(Devolucion notaCredito)
        {
            var conn = await SqlLiteDatabase.GetConnection();
            await conn.RunInTransactionAsync(tran =>
            {
                var id = tran.Insert(notaCredito);

                foreach (var linea in notaCredito.Lineas)
                {
                    linea.IdDevolucion = id;
                    tran.Insert(linea);
                }
            });
        }
        public async Task<List<Devolucion>> GetLista()
        {
            var conn = await SqlLiteDatabase.GetConnection();

            var notas = await conn.Table<Devolucion>().Where(x => x.Enviado == false).ToListAsync();

            return notas;
        }

        public async Task<Devolucion> GetById(int idNota)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            var notas = await conn.Table<Devolucion>().Where(x => x.IdDevolucion == idNota).FirstOrDefaultAsync();

            return notas;
        }
        public async Task Eliminar(Devolucion devolucion)
        {
            var conn = await SqlLiteDatabase.GetConnection();

            await conn.DeleteAsync(devolucion);

            foreach (var linea in devolucion.Lineas)
            {
                await conn.DeleteAsync(linea);
            }
        }
        public async Task EliminarTodas()
        {
            var conn = await SqlLiteDatabase.GetConnection();
            await conn.DeleteAllAsync<Devolucion>();
            await conn.DeleteAllAsync<DevolucionLinea>();
        }
    }
}
