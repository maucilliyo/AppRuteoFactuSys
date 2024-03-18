using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppRuteoFactuSys.Service.Interfaces
{
    public interface IBaseService<T>
    {
        Task<List<T>> Listar();
        Task<T> GetById(int id);
        Task Nuevo(T entity);
        Task Actualizar(T entity);
        Task Sincronizar();
    }
}
