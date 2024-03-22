using AppRuteoFactuSys.Models;
using AppRuteoFactuSys.MySql;
using AppRuteoFactuSys.Service.Interfaces;
using AppRuteoFactuSys.SqlLite;

namespace AppRuteoFactuSys.Service
{
    public class ProductoService : IProductoService
    {
        private readonly ProductoRepository _productoRepository;
        private readonly LocalProductoRepository _sqlLiteProductoRepository;
        public ProductoService(ProductoRepository productoRepository , LocalProductoRepository sqlLiteProductoRepository)
        {
             _productoRepository = productoRepository;
            _sqlLiteProductoRepository= sqlLiteProductoRepository;
        }
        public Task Actualizar(Producto entity)
        {
            throw new NotImplementedException();
        }

        public Task<Producto> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Producto>> Listar()
        {
          return   await _sqlLiteProductoRepository.GetProductos();
        }

        public Task Nuevo(Producto entity)
        {
            throw new NotImplementedException();
        }

        public async Task Sincronizar()
        {
            //traemos la lista de productos del sistema
            var productosSistema = await _productoRepository.GetProductos();
            //lista de productos de la app
            var productosApp = await _sqlLiteProductoRepository.GetProductos();
            //recorremos la lista de productos del sistema a ver si hay cambios para actualizar los de la app
            foreach (var producto in productosSistema)
            {
                var productoApp = await _sqlLiteProductoRepository.GetProductoByCodigo(producto.CodPro);

                var noExiste = !productosApp.Any(p=> p.CodPro == producto.CodPro);
                //el producto no existe
                if (noExiste)
                {
                    try
                    {
                        //agregamos el producto a la base de datos de la app
                        await _sqlLiteProductoRepository.Agregar(producto);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                //el producto existe pero la fecha de la app es mas vieja que la del sistema
                else if (productoApp.FechaUpdate < producto.FechaUpdate)
                {
                    try
                    {
                        //actualizamos el prodcto en la base de datos de la app
                        await _sqlLiteProductoRepository.Actualizar(producto);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            }
        }
    }
}
