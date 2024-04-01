namespace AppRuteoFactuSys.SqlLite
{
    public static class SQLiteInitialization
    {

        public static void InitializeDatabase()
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Crear la tabla proforma
                        string createProformaTable = @"
                        CREATE TABLE IF NOT EXISTS proforma (
                            LocalID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                            Nproforma,
                            cedcliente TEXT,
                            fecha TEXT,
                            condicionventa TEXT,
                            formapago TEXT,
                            totalcomprobante REAL,
                            totaldescuento REAL,
                            totalgrabado REAL,
                            totalexento REAL,
                            totalimpuesto REAL,
                            totalventa REAL,
                            codvendedor TEXT,
                            codigomoneda TEXT,
                            totalservgravados REAL,
                            totalservexentos REAL,
                            diasplazo INTEGER,
                            estado TEXT,
                            notas TEXT,
                            serviciosexonerados REAL,
                            mercanciasexoneradas REAL,
                            totalmercanciasgravadas REAL,
                            totalmercanciasexentas REAL,
                            totalventaneta REAL,
                            totalexonerado REAL,
                            nombre_cliente TEXT,
                            terminal TEXT,
                            id_usuario INTEGER,
                            fecha_update TEXT,
                            entregado INTEGER
                        );";
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = createProformaTable;
                            command.ExecuteNonQuery();
                        }

                        // Crear la tabla lineasproforma
                        string createLineasProformaTable = @"
                        CREATE TABLE IF NOT EXISTS lineasproforma (
                            id_linea INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                            Local_ID INTEGER,
                            n_proforma INTEGER,
                            linea INTEGER,
                            codpro TEXT,
                            unidadmedida TEXT,
                            detalle TEXT,
                            preciounidad REAL,
                            cantidad REAL,
                            subtotal REAL,
                            descuento REAL,
                            impuesto REAL,
                            totallinea REAL,
                            montoexonerado REAL,
                            porimpuesto REAL,
                            subtotaldescuento REAL,
                            impuestoneto REAL,
                            porexonerado REAL,
                            codigo_impuesto TEXT,
                            codigo_tarifa TEXT,
                            codecabys TEXT,
                            FOREIGN KEY (Local_ID) REFERENCES proforma (LocalID)
                        );";
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = createLineasProformaTable;
                            command.ExecuteNonQuery();
                        }

                        //Crear la tabla Clientes
                        string createClientesTable = @"
                                CREATE TABLE IF NOT EXISTS clientes (
                                  cedula TEXT PRIMARY KEY,
                                  tipocedula INTEGER,
                                  nombre TEXT,
                                  apellido TEXT,
                                  tel INTEGER,
                                  email TEXT,
                                  provincia TEXT,
                                  canton TEXT,
                                  distrito TEXT,
                                  otrassenas TEXT,
                                  contacto TEXT,
                                  credito INTEGER,
                                  tipocliente INTEGER,
                                  pordescuento INTEGER,
                                  diascredito INTEGER,
                                  tipo_cliente_impuesto TEXT,
                                  tipodocpreferido TEXT,
                                  tipo_precio TEXT DEFAULT 'C',
                                  fecha_update DATETIME DEFAULT CURRENT_TIMESTAMP
                                );";
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = createClientesTable;
                            command.ExecuteNonQuery();
                        }

                        //crear la tabla producto
                        string createProductoTable = @"
                        CREATE TABLE IF NOT EXISTS productos (
                            Nombre TEXT,
                            CodPro TEXT PRIMARY KEY,
                            Cod_Proveedor TEXT,
                            Detalle TEXT,
                            CodBarras TEXT,
                            Granel INTEGER,
                            ExitenciaMinima REAL,
                            Lote INTEGER,
                            UnidadMedida TEXT,
                            Descripcion TEXT,
                            PorcientoImpuesto REAL,
                            VenderNegativo INTEGER,
                            ExistenciaMaxima REAL,
                            CodigoImpuesto TEXT,
                            CodigoTarifa TEXT,
                            CodigoCabys TEXT,
                            UsaInventario INTEGER,
                            Agroinsumo INTEGER,
                            Oferta INTEGER,
                            IdDepartamento TEXT,
                            PrecioCompra REAL,
                            Ganacia REAL,
                            PrecioVenta REAL,
                            PrecioVentaA REAL,
                            PrecioVentaB REAL,
                            PorGanacia REAL,
                            Stock REAL,
                            Bodega TEXT,
                            Pasillo TEXT,
                            Estante TEXT,
                            Ubicacion TEXT,
                            Caja TEXT,
                            Activo INTEGER,
                            Imagen BLOB,
                            TipoUpdateProduc TEXT,
                            ImpreBodegaCocina INTEGER,
                            FechaUpdate TEXT,
                            FechaUltimaVenta TEXT
                        );";
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = createProductoTable;
                            command.ExecuteNonQuery();
                        }

                        // Commit de la transacción

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback en caso de error
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }

        }
        public static void DeleteDataBase()
        {
            using (var connection = SqlLiteConexion.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Borrar la tabla proforma si existe
                        string dropProformaTable = @"DROP TABLE IF EXISTS lineasproforma;
                                                     DROP TABLE IF EXISTS clientes;
                                                     DROP TABLE IF EXISTS proforma;";
                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = dropProformaTable;
                            command.ExecuteNonQuery();
                        }

                        // Commit de la transacción
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback en caso de error
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }
    }
}
