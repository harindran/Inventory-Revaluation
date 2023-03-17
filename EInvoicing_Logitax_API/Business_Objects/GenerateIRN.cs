
using System.Collections.Generic;

namespace EInvoicing_Logitax_API.Business_Objects
{
    #region Generate_IRN

    public class saplogin
    {
    public string CompanyDB { get; set; }
    public string Password { get; set; }
    public string UserName { get; set; }
}

    public class GenerateIRN
    {
        public string client_code { get; set; }
        public string user_code { get; set; }
        public string password { get; set; }
        public string Url { get; set; }
        public JsonData json_data { get; set; } = new JsonData();
    }

    public class AddlDocDtl
    {
        public string Docs { get; set; }
        public string Info { get; set; }
    }

    public class AttribDtl
    {
        public string Nm { get; set; }
        public string Val { get; set; }
    }

    public class BchDtls
    {
        public string Nm { get; set; }
        public string ExpDt { get; set; }
        public string WrDt { get; set; }
    }

    public class BuyerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Pos { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public int Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class ContrDtl
    {
        public string RecAdvRefr { get; set; }
        public string RecAdvDt { get; set; }
        public string TendRefr { get; set; }
        public string ContrRefr { get; set; }
        public string ExtRefr { get; set; }
        public string ProjRefr { get; set; }
        public string PORefr { get; set; }
        public string PORefDt { get; set; }
    }

    public class DispDtls
    {
        public string Nm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public int Pin { get; set; }
        public string Stcd { get; set; }
    }

    public class DocDtls
    {
        public string Typ { get; set; }
        public string No { get; set; }
        public string Dt { get; set; }
    }

    public class DocPerdDtls
    {
        public string InvStDt { get; set; }
        public string InvEndDt { get; set; }
    }

    public class EwbDtls
    {
        public string TransId { get; set; }
        public string TransName { get; set; }
        public int Distance { get; set; }
        public string TransDocNo { get; set; }
        public string TransDocDt { get; set; }
        public string VehNo { get; set; }
        public string VehType { get; set; }
        public string TransMode { get; set; }
    }

    public class ExpDtls
    {
        public string ShipBNo { get; set; }
        public string ShipBDt { get; set; }
        public string Port { get; set; }
        public string RefClm { get; set; }
        public string ForCur { get; set; }
        public string CntCode { get; set; }
        public object ExpDuty { get; set; }
    }

    public class ItemList
    {
        public string SlNo { get; set; }
        public string PrdDesc { get; set; }
        public string IsServc { get; set; }
        public string HsnCd { get; set; }
        public string Barcde { get; set; }
        public double Qty { get; set; }
        public double FreeQty { get; set; }
        public string Unit { get; set; }
        public double UnitPrice { get; set; }
        public double TotAmt { get; set; }
        public double Discount { get; set; }
        public double PreTaxVal { get; set; }
        public double AssAmt { get; set; }
        public double GstRt { get; set; }
        public double IgstAmt { get; set; }
        public double CgstAmt { get; set; }
        public double SgstAmt { get; set; }
        public double CesRt { get; set; }
        public double CesAmt { get; set; }
        public double CesNonAdvlAmt { get; set; }
        public double StateCesRt { get; set; }
        public double StateCesAmt { get; set; }
        public double StateCesNonAdvlAmt { get; set; }
        public double OthChrg { get; set; }
        public double TotItemVal { get; set; }
        public string OrdLineRef { get; set; }
        public string OrgCntry { get; set; }
        public string PrdSlNo { get; set; }
        public BchDtls BchDtls { get; set; } = new BchDtls();
        public List<AttribDtl> AttribDtls = new List<AttribDtl>();
    }

    public class JsonData
    {
        public string Version { get; set; }
        public TranDtls TranDtls { get; set; } = new TranDtls();
        public DocDtls DocDtls { get; set; } = new DocDtls();
        public SellerDtls SellerDtls { get; set; } = new SellerDtls();
        public BuyerDtls BuyerDtls { get; set; } = new BuyerDtls();
        public DispDtls DispDtls { get; set; } = new DispDtls();
        public ShipDtls ShipDtls { get; set; } = new ShipDtls();
        public List<ItemList> ItemList = new List<ItemList>();
        public ValDtls ValDtls { get; set; } = new ValDtls();
        public PayDtls PayDtls { get; set; } = new PayDtls();
        public RefDtls RefDtls { get; set; } = new RefDtls();
        public List<AddlDocDtl> AddlDocDtls = new List<AddlDocDtl>();
        public ExpDtls ExpDtls { get; set; } = new ExpDtls();
        public EwbDtls EwbDtls { get; set; } = new EwbDtls();
        
    }

    public class PayDtls
    {
        public string Nm { get; set; }
        public string AccDet { get; set; }
        public string Mode { get; set; }
        public string FinInsBr { get; set; }
        public string PayTerm { get; set; }
        public string PayInstr { get; set; }
        public string CrTrn { get; set; }
        public string DirDr { get; set; }
        public int CrDay { get; set; }
        public int PaidAmt { get; set; }
        public int PaymtDue { get; set; }
    }

    public class PrecDocDtl
    {
        public string InvNo { get; set; }
        public string InvDt { get; set; }
        public string OthRefNo { get; set; }
    }

    public class RefDtls
    {
        public string InvRm { get; set; }
        public DocPerdDtls DocPerdDtls { get; set; } = new DocPerdDtls();
        public List<PrecDocDtl> PrecDocDtls = new List<PrecDocDtl>();
        public List<ContrDtl> ContrDtls = new List<ContrDtl>();
    }

    public class SellerDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public int Pin { get; set; }
        public string Stcd { get; set; }
        public string Ph { get; set; }
        public string Em { get; set; }
    }

    public class ShipDtls
    {
        public string Gstin { get; set; }
        public string LglNm { get; set; }
        public string TrdNm { get; set; }
        public string Addr1 { get; set; }
        public string Addr2 { get; set; }
        public string Loc { get; set; }
        public int Pin { get; set; }
        public string Stcd { get; set; }
    }

    public class TranDtls
    {
        public string TaxSch { get; set; }
        public string SupTyp { get; set; }
        public string RegRev { get; set; }
        public object EcmGstin { get; set; }
        public string IgstOnIntra { get; set; }
    }

    public class ValDtls
    {
        public double AssVal { get; set; }
        public double CgstVal { get; set; }
        public double SgstVal { get; set; }
        public double IgstVal { get; set; }
        public double CesVal { get; set; }
        public double StCesVal { get; set; }
        public double Discount { get; set; }
        public double OthChrg { get; set; }
        public double RndOffAmt { get; set; }
        public double TotInvVal { get; set; }
        public double TotInvValFc { get; set; }
    }

    #endregion

    #region Cancel    
    
    public class Cancelledeinvoicelist
    {
        public string Irn { get; set; }
        public string CnlRem { get; set; }
        public int CnlRsn { get; set; }
    }
    public class cancelledewblist
    {
        public string EwbNo { get; set; }
        public string CancelledReason { get; set; }
        public string CancelledRemarks { get; set; }
    }
    public class cancelledeinvoiceewblist
        {
        public string ewbNo { get; set; }
        public string cancelRsnCode { get; set; }
        public string cancelRmrk { get; set; }
     }
public class ClientCred_Cancel
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public List<Cancelledeinvoicelist> cancelledeinvoicelist = new List<Cancelledeinvoicelist>();
        public List<cancelledewblist> cancelledewblist = new List<cancelledewblist>();
        public List<cancelledeinvoiceewblist> cancelledeinvoiceewblist= new List<cancelledeinvoiceewblist>();
    
}


class ClsCancelEInvoice
    {
        public Cancelledeinvoicelist Cancelledeinvoicelist { get; set; }
        public ClientCred_Cancel ClientCred { get; set; }
    }

    #endregion

    #region GetIRN_DocNum

    public class Docdetailslist
    {
        public string DocType { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
    }

    public class ClienCred_GetIRN_DocNum
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public string RequestorGSTIN { get; set; }
        public List<Docdetailslist> docdetailslist = new List<Docdetailslist>();      
    }

    #endregion

    #region GetIRN
    public class Irnlist
    {
        public string irn { get; set; }
    }  
       
    public class GetIRN
    {
        public string USERCODE { get; set; }
        public string CLIENTCODE { get; set; }
        public string PASSWORD { get; set; }
        public string RequestorGSTIN { get; set; }
        public List<Irnlist> irnlist = new List<Irnlist>();
    }
    #endregion

    public class Vehicleupdatelist
    {
        public string ewbNo { get; set; }
        public string vehicleNo { get; set; }
        public string fromPlace { get; set; }
        public string fromState { get; set; }
        public string reasonCode { get; set; }
        public string reasonRem { get; set; }
        public string transDocNo { get; set; }
        public string transDocDate { get; set; }
        public string transMode { get; set; }
        public string vehicleType { get; set; }
    }

    public class Ewbeinvoicelist
    {
        public string Irn { get; set; }
        public string Distance { get; set; }
        public string TransMode { get; set; }
        public string TransId { get; set; }
        public string TransName { get; set; }
        public string TransDocDt { get; set; }
        public string TransDocNo { get; set; }
        public string VehNo { get; set; }
        public string VehType { get; set; }
    }

    public class GetEwayByIRN
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public List<Ewbeinvoicelist> ewbeinvoicelist = new List<Ewbeinvoicelist>();
    }
    public class UpdateEway
    {
        public string CLIENTCODE { get; set; }
        public string USERCODE { get; set; }
        public string PASSWORD { get; set; }
        public List<Vehicleupdatelist> Vehicleupdatelist = new List<Vehicleupdatelist>();
    }


}

