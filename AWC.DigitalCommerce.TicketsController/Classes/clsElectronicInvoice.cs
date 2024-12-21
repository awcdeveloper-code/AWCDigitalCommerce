using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AWC.DigitalCommerce.TicketsController
{
    public class clsFacturaElectronica
    {
        public class FacturaElectronica
        {
            public _Encabezado Encabezado { get; set; }
            public _DetalleServicio DetalleServicio { get; set; }
            public _ResumenFactura ResumenFactura { get; set; }
            public _InformacionReferencia InformacionReferencia { get; set; }
            public _Otros Otros { get; set; }
        }

        #region ENCABEZADO
        public class _Encabezado
        {
            [StringLength(6)]
            public string CodigoActividad { get; set; }
            [StringLength(50)]
            public string Clave { get; set; }
            [StringLength(20)]
            public string NumeroConsecutivo { get; set; }
            public DateTime FechaEmision { get; set; }
            public _Emisor Emisor { get; set; }
            public _Receptor Receptor { get; set; }
            [StringLength(2)]
            public string CondicionVenta { get; set; }
            [StringLength(10)]
            public string PlazoCredito { get; set; }
            [StringLength(2)]
            public List<string> MedioPago { get; set; }
        }
        public class _Emisor
        {
            [StringLength(100)]
            public string Nombre { get; set; }
            public _Identificacion Identificacion { get; set; }
            [StringLength(80)]
            public string NombreComercial { get; set; }
            public _Ubicacion Ubicacion { get; set; }
            public _Telefono Telefono { get; set; }
            public _Fax Fax { get; set; }
            [StringLength(160)]
            public string CorreoElectronico { get; set; }
        }
        public class _Receptor
        {
            [StringLength(100)]
            public string Nombre { get; set; }
            public _Identificacion Identificacion { get; set; }
            [StringLength(80)]
            public string NombreComercial { get; set; }
            public _Ubicacion Ubicacion { get; set; }
            public _Telefono Telefono { get; set; }
            public _Fax Fax { get; set; }
            [StringLength(160)]
            public string CorreoElectronico { get; set; }
        }
        public class _Identificacion
        {
            [StringLength(2)]
            public string Tipo { get; set; }
            [StringLength(12)]
            public string Numero { get; set; }
        }
        public class _Ubicacion
        {
            [StringLength(1)]
            public string Provincia { get; set; }
            [StringLength(2)]
            public string Canton { get; set; }
            [StringLength(2)]
            public string Distrito { get; set; }
            [StringLength(2)]
            public string Barrio { get; set; }
            [StringLength(250)]
            public string OtrasSenas { get; set; }
        }
        public class _Telefono
        {
            public int CodigoPais { get; set; }
            public int NumTelefono { get; set; }
        }
        public class _Fax
        {
            public int CodigoPais { get; set; }
            public int NumTelefono { get; set; }
        }
        #endregion

        #region DETALLESERVICIO
        public class _DetalleServicio
        {
            public List<_LineaDetalle> LineaDetalle { get; set; }
            public _OtrosCargos OtrosCargos { get; set; }
        }
        public class _LineaDetalle
        {
            public int NumeroLinea { get; set; }
            [StringLength(12)]
            public string PartidaArancelaria { get; set; }
            [StringLength(13)]
            public string Codigo { get; set; }
            public List<_CodigoComercial> CodigoComercial { get; set; }
            public decimal Cantidad { get; set; }
            [StringLength(15)]
            public string UnidadMedida { get; set; }
            [StringLength(20)]
            public string UnidadMedidaComercial { get; set; }
            [StringLength(200)]
            public string Detalle { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal MontoTotal { get; set; }
            public List<_Descuento> Descuento { get; set; }
            public decimal SubTotal { get; set; }
            public decimal BaseImponible { get; set; }
            public List<_Impuesto> Impuesto { get; set; }
            public decimal ImpuestoNeto { get; set; }
            public decimal MontoTotalLinea { get; set; }
        }
        public class _CodigoComercial
        {
            [StringLength(2)]
            public string Tipo { get; set; }
            [StringLength(20)]
            public string Codigo { get; set; }
        }
        public class _Descuento
        {
            public decimal MontoDescuento { get; set; }
            [StringLength(80)]
            public string NaturalezaDescuento { get; set; }
        }
        public class _Impuesto
        {
            [StringLength(2)]
            public string Codigo { get; set; }
            [StringLength(2)]
            public string CodigoTarifa { get; set; }
            public decimal Tarifa { get; set; }
            public decimal FactorIVA { get; set; }
            public decimal Monto { get; set; }
            public decimal MontoExportacion { get; set; }
            public _Exoneración Exoneración { get; set; }
        }
        public class _Exoneración
        {
            [StringLength(2)]
            public string Tipodocumento { get; set; }
            [StringLength(40)]
            public string NumeroDocumento { get; set; }
            [StringLength(160)]
            public string NombreInstitucion { get; set; }
            public DateTime FechaEmision { get; set; }
            public int PorcentajeExoneracion { get; set; }
            public decimal MontoExoneracion { get; set; }
        }
        public class _OtrosCargos
        {
            [StringLength(2)]
            public string TipoDocumento { get; set; }
            [StringLength(12)]
            public string NumeroIdentidadTercero { get; set; }
            [StringLength(100)]
            public string NombreTercero { get; set; }
            [StringLength(160)]
            public string Detalle { get; set; }
            public decimal Porcentaje { get; set; }
            public decimal MontoCargo { get; set; }
        }
        #endregion

        #region RESUMEN COMPROBANTE
        public class _ResumenFactura
        {
            public _CodigoTipoMoneda CodigoTipoMoneda { get; set; }
            public decimal TotalServGravados { get; set; }
            public decimal TotalServExentos { get; set; }
            public decimal TotalServExonerado { get; set; }
            public decimal TotalMercanciasGravadas { get; set; }
            public decimal TotalMercanciasExentas { get; set; }
            public decimal TotalMercExonerada { get; set; }
            public decimal TotalGravado { get; set; }
            public decimal TotalExento { get; set; }
            public decimal TotalExonerado { get; set; }
            public decimal TotalVenta { get; set; }
            public decimal TotalDescuentos { get; set; }
            public decimal TotalVentaNeta { get; set; }
            public decimal TotalImpuesto { get; set; }
            public decimal TotalIVADevuelto { get; set; }
            public  decimal TotalOtrosCargos { get; set; }
            public  decimal TotalComprobante { get; set; }
        }
        public class _CodigoTipoMoneda
        {
            [StringLength(3)]
            public string CodigoMoneda { get; set; }
            public decimal TipoCambio { get; set; }
        }
        #endregion

        #region INFORMACION DE REFERENCIA
        public class _InformacionReferencia
        {
            [StringLength(2)]
            public string TipoDoc { get; set; }
            [StringLength(50)]
            public string Numero { get; set; }
            public DateTime FechaEmision { get; set; }
            [StringLength(2)]
            public string Codigo { get; set; }
            [StringLength(180)]
            public string Razon { get; set; }
        }
        #endregion

        #region OTROS
        public class _Otros
        {
            public string OtroTexto { get; set; }
            public string OtroContenido { get; set; }
        }
        #endregion
    }
}
