using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWC.DigitalCommerce.TicketsController
{
    public class ElectronicDoc
    {
        public DocElectronico DocElectronico { get; set; }
    }
    public class DocElectronico
    {
        public string Token { get; set; }
        public int User { get; set; }
        public string TipoDoc { get; set; }
        public int internal_id { get; set; }
        public int CodigoActividad { get; set; }
        public int Cliente { get; set; }
        public WhoReceive Receptor { get; set; }
        public int CondicionVenta { get; set; }
        public string MedioPago { get; set; }
        public ServiceDetail DetalleServicio { get; set; }
        public OtherCharges OtrosCargos { get; set; }
        public TicketSummary ResumenFactura { get; set; }
    }
    public class WhoReceive
    {
        public string Nombre { get; set; }
        public SSN Identificacion { get; set; }
        public PhoneNumber Telefono { get; set; }
        public string CorreoElectronico { get; set; }
    }
    public class SSN
    {
        public int Tipo { get; set; }
        public int Numero { get; set; }
    }
    public class PhoneNumber
    {
        public int CodigoPais { get; set; }
        public int NumTelefono { get; set; }
    }
    public class ServiceDetail
    {
        public List<LineDetail> LineaDetalle { get; set; }
    }
    public class LineDetail
    {
        public int NumeroLinea { get; set; }
        public long Codigo { get; set; }
        public ComercialCode CodigoComercial { get; set; }
        public int Cantidad { get; set; }
        public string UnidadMedida { get; set; }
        public string Detalle { get; set; }
        public decimal PrecioUnitario { get; set; }
        public Discount Descuento { get; set; }
        public decimal SubTotal { get; set; }
        public Tax Impuesto { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal MontoTotalLinea { get; set; }
    }
    public class ComercialCode
    {
        public int Tipo { get; set; }
        public int Codigo { get; set; }
    }
    public class Discount
    {
        public int MontoDescuento { get; set; }
        public string NaturalezaDescuento { get; set; }
    }
    public class Tax
    {
        public int Codigo { get; set; }
        public int CodigoTarifa { get; set; }
        public int Tarifa { get; set; }
        public decimal Monto { get; set; }
    }
    public class OtherCharges
    {
        public int TipoDocumento { get; set; }
        public string Detalle { get; set; }
        public decimal MontoCargo { get; set; }
    }
    public class TicketSummary
    {
        public CurrencyTypeCode CodigoTipoMoneda { get; set; }
    }
    public class CurrencyTypeCode
    {
        public string CodigoMoneda { get; set; }
        public int TipoCambio { get; set; }
    }
    public class ATVResponse
    {
        public int ID { get; set; }
        public int cod { get; set; }
        public int internal_id { get; set; }
        public string consecutivo { get; set; }
        public string clave { get; set; }
        public string estado { get; set; }
        public string msj { get; set; }
        public string mensaje { get; set; }
        public string email { get; set; }
    }
    public class ATVQuery
    {
        public int ID { get; set; }
        public int TicketID { get; set; }
        public string CustomerName { get; set; }
        public int SSN_Type { get; set; }
        public int SSN { get; set; }
        public int CountryCode { get; set; }
        public int PhoneNumber { get; set; }
        public string eMailAddress { get; set; }
    }
}
