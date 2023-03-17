using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EInvoicing_Logitax_API.Business_Objects
{
    class Generate_EWay
    {
        public string version { get; set; }
        public string USERCODE { get; set; }
        public string CLIENTCODE { get; set; }
        public string PASSWORD { get; set; }       
        public string frompincode { get; set; }       
        public string topincode { get; set; }       
        
        public List<EwayList> billLists = new List<EwayList>();

        public partial class EwayList
        {
            public string userGstin { get; set; }
            public string supplyType { get; set; }
            public string subSupplyType { get; set; }
            public string subSupplyTypeDesc { get; set; }
            public string docType { get; set; }
            public string docNo { get; set; }
            public string docDate { get; set; }
            public string TransType { get; set; }
            public string fromGstin { get; set; }
            public string fromTrdName { get; set; }
            public string fromAddr1 { get; set; }
            public string fromAddr2 { get; set; }
            public string fromPlace { get; set; }
            public string fromPincode { get; set; }
            public string fromStateCode { get; set; }
            public string actualFromStateCode { get; set; }
            public string toGstin { get; set; }
            public string toTrdName { get; set; }
            public string toAddr1 { get; set; }
            public string toAddr2 { get; set; }
            public string toPlace { get; set; }
            public string toPincode { get; set; }
            public string actualToStateCode { get; set; }
            public string toStateCode { get; set; }
            public string totalValue { get; set; }
            public string cgstValue { get; set; }
            public string sgstValue { get; set; }
            public string igstValue { get; set; }
            public string cessValue { get; set; }
            public string TotNonAdvolVal { get; set; }
            public string OthValue { get; set; }
            public string transDocNo { get; set; }
            public string transDocDate { get; set; }
            public string totInvValue { get; set; }
            public string transporterId { get; set; }
            public string transporterName { get; set; }
            public string transMode { get; set; }
            public string transDistance { get; set; }
            public string vehicleNo { get; set; }
            public string vehicleType { get; set; }
            public string shipToGSTIN { get; set; }
            public string shipToTradeName { get; set; }
            public string dispatchFromGSTIN { get; set; }
            public string dispatchFromTradeName { get; set; }
            public string portPin { get; set; }
            public string portName { get; set; }
            public List<Ewayitemlist> itemList = new List<Ewayitemlist>();
        }

        public partial class Ewayitemlist
        {
            public int itemNo { get; set; }
            public string productName { get; set; }
            public string productDesc { get; set; }
            public string hsnCode { get; set; }
            public string quantity { get; set; }
            public string qtyUnit { get; set; }
            public string taxableAmount { get; set; }
            public decimal cgstRate { get; set; }
            public decimal sgstRate { get; set; }
            public decimal igstRate { get; set; }
            public decimal cessRate { get; set; }
            public string cessNonAdvol { get; set; }
        }

    }

    class GenerateEwaybyIRN
        {
            public string CLIENTCODE { get; set; }
            public string USERCODE { get; set; }
            public string PASSWORD { get; set; }
            public List<Ewbeinvoicelist> ewbeinvoicelist = new List<Ewbeinvoicelist>();

        public class DispDtls
        {
            public string Nm { get; set; }
            public string Addr1 { get; set; }
            public string Addr2 { get; set; }
            public string Loc { get; set; }
            public int Pin { get; set; }
            public string Stcd { get; set; }
        }

        public class Ewbeinvoicelist
        {
            public string Irn { get; set; }
            public int Distance { get; set; }
            public string TransMode { get; set; }
            public string TransId { get; set; }
            public string TransName { get; set; }
            public string TransDocDt { get; set; }
            public string TransDocNo { get; set; }
            public string VehNo { get; set; }
            public string VehType { get; set; }
            public ExpShipDtls ExpShipDtls { get; set; } = new ExpShipDtls();
            public DispDtls DispDtls { get; set; } = new DispDtls();
        }

        public class ExpShipDtls
        {
            public string Addr1 { get; set; }
            public string Addr2 { get; set; }
            public string Loc { get; set; }
            public int Pin { get; set; }
            public string Stcd { get; set; }
        }
    }

    class GetDistance
        {
            public string USERCODE { get; set; }
            public string CLIENTCODE { get; set; }
            public string PASSWORD { get; set; }
            public int topincode { get; set; }
            public int frompincode { get; set; }
        }

    
}
