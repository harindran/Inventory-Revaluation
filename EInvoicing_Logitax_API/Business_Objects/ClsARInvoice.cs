using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EInvoicing_Logitax_API.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Data;
using System.Globalization;
using System.Net.Http;

namespace EInvoicing_Logitax_API.Business_Objects
{
    class ClsARInvoice : clsAddon
    {
        private SAPbouiCOM.Form oForm;
        private string strSQL;
        private SAPbobsCOM.Recordset objRs;
        SAPbouiCOM.ButtonCombo buttonCombo;

        #region ITEM EVENT
        public override void Item_Event(string oFormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        {
            try
            {
                oForm = clsModule.objaddon.objapplication.Forms.Item(oFormUID);
                ClsARInvoice.EinvoiceMethod einvoiceMethod = ClsARInvoice.EinvoiceMethod.Default;
                string DocEntry = "";
                string TransType = "";
                string Type = "";
                SAPbouiCOM.ButtonCombo buttonCombo=null;
                if (pVal.BeforeAction)
                {
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            Create_Customize_Fields(oFormUID);
                            break;
                        case SAPbouiCOM.BoEventTypes.et_DOUBLE_CLICK:                           
                            break;
                        case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                             buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                            break;
                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            EnabledMenu(pVal.FormType.ToString());
                            break;
                    }
                }
                else
                {
                    switch (pVal.EventType)
                    {                       
                        case SAPbouiCOM.BoEventTypes.et_CLICK:
                            if (pVal.ItemUID == "einv")
                            {
                                oForm.PaneLevel = 26;
                            }
                            break;
                        case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                            switch (pVal.FormType)
                            {
                                
                                case 133:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("OINV").GetValue("DocEntry", 0);
                                        TransType = "INV";
                                         buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {
                                            string irn = oForm.DataSources.DBDataSources.Item("OINV").GetValue("U_IRNNo", 0);
                                            string eway = oForm.DataSources.DBDataSources.Item("INV26").GetValue("EWayBillNo", 0);
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                            Type = eway == "" ? Type : "Update E-way";
                                            einvoiceMethod = eway == "" ? einvoiceMethod : ClsARInvoice.EinvoiceMethod.UpdateEway;
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelEway;
                                            Type = "E-way";
                                        }                                      
                                    }
                                    break;
                                case 179:
                                    if (pVal.ItemUID == "btneinv" && (oForm.Mode == SAPbouiCOM.BoFormMode.fm_OK_MODE | oForm.Mode == SAPbouiCOM.BoFormMode.fm_UPDATE_MODE))
                                    {
                                        DocEntry = oForm.DataSources.DBDataSources.Item("ORIN").GetValue("DocEntry", 0);
                                        TransType = "CRN";
                                         buttonCombo = (SAPbouiCOM.ButtonCombo)oForm.Items.Item("btneinv").Specific;
                                        if (buttonCombo.Selected.Value == "Create IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CreateIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Create Eway")
                                        {                                         
                                            string irn = oForm.DataSources.DBDataSources.Item("ORIN").GetValue("U_IRNNo", 0);
                                            einvoiceMethod = irn == "" ? ClsARInvoice.EinvoiceMethod.CreateEway : ClsARInvoice.EinvoiceMethod.GetEwayByIRN;
                                            Type = irn == "" ? "E-way" : "E-way IRN";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel IRN")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelIRN;
                                            Type = "E-Invoice";
                                        }
                                        else if (buttonCombo.Selected.Value == "Cancel Eway")
                                        {
                                            einvoiceMethod = ClsARInvoice.EinvoiceMethod.CancelEway;
                                            Type = "E-way";
                                        }
                                     
                                    }
                                    break;
                            }
                            if (DocEntry != "" && TransType != "" && Type != "")
                            {
                                DataTable dt = new DataTable();
                                Generate_Cancel_IRN(einvoiceMethod, DocEntry, TransType, Type, ref dt);
                                buttonCombo.Caption = "Generate E-invoice";
                                if (dt.Rows.Count > 0)
                                {
                                    if (dt.Rows[0]["flag"].ToString() == "True")
                                    {   
                                        oForm.Items.Item("1").Click();
                                        clsModule.objaddon.objapplication.Menus.Item("1304").Activate();
                                    }
                                }
                            }
                           
                            break;
                    }
                }
              
            }
            catch (Exception Ex)
            {
                return;
            }
            finally
            {

            }
        }
        #endregion

        public void EnabledMenu(string oFormUID,bool Penable=false,string UDFormID="")
        {            
            switch (oFormUID)
            {                
                case "133":
                case "179":
                    oForm.Items.Item("txtIrn").Enabled = Penable;
                    oForm.Items.Item("txtqrcode").Enabled = Penable;
                    oForm.Items.Item("txtAckNo").Enabled = Penable;
                    break;
                case "-133":
                case "-179":
                    SAPbouiCOM.Form oUDFForm;
                    if (UDFormID=="")
                    {
                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(oForm.UDFFormUID);
                    }
                    if (UDFormID != "")
                    {
                        oUDFForm = clsModule.objaddon.objapplication.Forms.Item(UDFormID);
                        oUDFForm.Items.Item("U_IRNNo").Enabled = Penable;
                        oUDFForm.Items.Item("U_QRCode").Enabled = Penable;
                        oUDFForm.Items.Item("U_AckDate").Enabled = Penable;
                        oUDFForm.Items.Item("U_AckNo").Enabled = Penable;

                    }
                    break;
            }

        }
        #region FORM DATA EVENT
        public override void FormData_Event(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        {
            try
            {

                if (BusinessObjectInfo.BeforeAction)
                {
                    switch (BusinessObjectInfo.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_UPDATE:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                            break;
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            break;
                    }
                }
                else
                {
                    switch (BusinessObjectInfo.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_DATA_LOAD:
                            break;
                    }
                }
            }
            catch (Exception Ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(Ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                BubbleEvent = false;
                return;
            }
            finally
            {
                // oForm.Freeze(false);
            }
        }
        #endregion

        public string GetInvoiceData(string DocEntry, string TransType)
        {
            int HSNCode = Convert.ToInt32(clsModule.objaddon.objglobalmethods.getSingleValue(@"SELECT ""U_HSNL"" FROM ""@ATEICFG"""));

            Querycls qcls = new Querycls();
            qcls.HSNLength = HSNCode;
            switch (TransType)
            {
                case "INV":
                    strSQL = qcls.InvoiceQuery(DocEntry);
                    break;
                case "CRN":
                    strSQL = qcls.CreditNoteQuery(DocEntry);
                    break;
            }
            if (!clsModule.objaddon.HANA) 
            {
                strSQL = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strSQL);
            }
                return strSQL;
        }
        public string GetFrightData(string DocEntry)
        {
            
                strSQL = @" Select 'Freight' as Dscription,1 as Quantity,'9965' as HSN,TF.""VatPrcnt"",TF.""LineTotal"",TF.""GrsAmount"" as ""Total Value"",";
                strSQL += @" IFNULL((select sum(""TaxSum"") from INV4 where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = '-100' AND ""ExpnsCode"" <> '-1'),0) as CGSTAmt,IFNULL((select sum(""TaxSum"") from INV4 where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = -110 and ""ExpnsCode"" <> '-1'),0) as SGSTAmt,";
                strSQL += @"IFNULL((select sum(""TaxSum"") from INV4 where ""DocEntry"" = TF.""DocEntry"" and ""LineNum"" = TF.""LineNum"" and ""staType"" = '-120' AND  ""ExpnsCode"" <> '-1'),0) as IGSTAmt from INV3 TF where TF.""DocEntry"" = " + DocEntry + @" and TF.""ExpnsCode"" <> '-1'";

            
            if (!clsModule.objaddon.HANA)
            {
                strSQL = clsModule.objaddon.objglobalmethods.ChangeHANAtoSql(strSQL);
            }

            return strSQL;
        }
        public enum EinvoiceMethod
        {
            Default = 0,
            CreateIRN = 1,
            CancelIRN = 2,
            GetIrnByDocnum = 3,
            GETIRNDetails = 4,
            CreateEway = 5,
            CancelEway = 6,
            GetEwayByIRN = 7,
            UpdateEway=8

        }

        private void Create_Customize_Fields(string oFormUID)
        {
            oForm = clsModule.objaddon.objapplication.Forms.Item(oFormUID);
            try
            {
                switch (oForm.TypeEx)
                {
                    case "133":
                        break;
                    case "179":
                        break;
                    default:
                        return;
                }

                SAPbouiCOM.Item oItem;


                try
                {
                    if (oForm.Items.Item("btneinv").UniqueID == "btneinv")
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    //throw;
                }
                SAPbouiCOM.Folder objfolder;
                oItem = oForm.Items.Add("einv", SAPbouiCOM.BoFormItemTypes.it_FOLDER);
                objfolder = (SAPbouiCOM.Folder)oItem.Specific;
                oItem.AffectsFormMode = false;
                objfolder.Caption = "E-Invoice Details";
                objfolder.GroupWith("1320002137");
                objfolder.Pane = 26;
                oItem.Width = 125;

                oItem.Visible = true;
                oForm.PaneLevel = 1;
                oItem.Left = oForm.Items.Item("1320002137").Left + oForm.Items.Item("1320002137").Width;
                oItem.Enabled = true;


                oItem = oForm.Items.Add("btneinv", SAPbouiCOM.BoFormItemTypes.it_BUTTON_COMBO);
                buttonCombo = (SAPbouiCOM.ButtonCombo)oItem.Specific;
                buttonCombo.Caption = "Generate E-invoice";
                oItem.Left = oForm.Items.Item("2").Left + oForm.Items.Item("2").Width + 5;
                oItem.Top = oForm.Items.Item("2").Top;
                oItem.Height = oForm.Items.Item("2").Height;
                oItem.LinkTo = "2";
                Size Fieldsize = System.Windows.Forms.TextRenderer.MeasureText("Generate E-Invoice", new Font("Arial", 12.0f));
                oItem.Width = Fieldsize.Width;
                buttonCombo.ValidValues.Add("Create IRN", "Create E-invoice");
                buttonCombo.ValidValues.Add("Create Eway", "Create Eway");
                buttonCombo.ValidValues.Add("Cancel IRN", "Cancel E-invoice");
                buttonCombo.ValidValues.Add("Cancel Eway", "Cancel Eway");

                buttonCombo.ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
                oForm.Items.Item("btneinv").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Add), SAPbouiCOM.BoModeVisualBehavior.mvb_False);
                oForm.Items.Item("btneinv").SetAutoManagedAttribute(SAPbouiCOM.BoAutoManagedAttr.ama_Editable, Convert.ToInt32(SAPbouiCOM.BoAutoFormMode.afm_Find), SAPbouiCOM.BoModeVisualBehavior.mvb_False);

                SAPbouiCOM.Item newTextBox;
                SAPbouiCOM.EditText otxt;
                SAPbouiCOM.StaticText olbl;
                string tablename = "";
                oForm.Freeze(true);

                switch (oForm.TypeEx)
                {
                    case "133":
                        tablename = "OINV";
                        break;
                    case "179":
                        tablename = "ORIN";
                        break;
                    default:
                        return;

                }
                #region "IRN"
                newTextBox = oForm.Items.Add("lblIrn", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("112").Top + 25;
                newTextBox.Width = 250;                
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("lblIrn").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "IRN No";
                

                newTextBox = oForm.Items.Add("txtIrn", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("lblIrn").Left + 80;
                newTextBox.Top = oForm.Items.Item("112").Top + 25;
                newTextBox.Width = 500;                
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtIrn").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_IRNNo");
                #endregion
                #region "QRCode"
                newTextBox = oForm.Items.Add("lblQrcode", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("txtIrn").Top + oForm.Items.Item("txtIrn").Height + 2;
                newTextBox.Width = 250;
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("lblQrcode").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "Qrcode";

                newTextBox = oForm.Items.Add("txtqrcode", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("lblQrcode").Left + 80;
                newTextBox.Top = oForm.Items.Item("txtIrn").Top + oForm.Items.Item("txtIrn").Height + 2;
                newTextBox.Width = 500;                
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtqrcode").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_QRCode");
                #endregion
                #region "Ack No"
                newTextBox = oForm.Items.Add("lblAckNo", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("112").Left + 20;
                newTextBox.Top = oForm.Items.Item("txtqrcode").Top + oForm.Items.Item("txtqrcode").Height + 2;
                newTextBox.Width = 250;
                olbl = (SAPbouiCOM.StaticText)oForm.Items.Item("lblAckNo").Specific;
                ((SAPbouiCOM.StaticText)(olbl.Item.Specific)).Caption = "Ack No";

                newTextBox = oForm.Items.Add("txtAckNo", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                newTextBox.FromPane = 26;
                newTextBox.ToPane = 26;
                newTextBox.Left = oForm.Items.Item("lblAckNo").Left + 80;
                newTextBox.Top = oForm.Items.Item("txtqrcode").Top + oForm.Items.Item("txtqrcode").Height + 2;
                newTextBox.Width = 200;                
                otxt = (SAPbouiCOM.EditText)oForm.Items.Item("txtAckNo").Specific;
                otxt.DataBind.SetBound(true, tablename, "U_AckNo");
                #endregion

                oForm.Freeze(false);

            }
            catch (Exception ex)
            {
            }
        }

        public bool Generate_Cancel_IRN(EinvoiceMethod Create_Cancel, string DocEntry, string TransType, string Type, ref DataTable datatable)
        {
            string requestParams;
            string SapMessage;
            try
            {
                

                SAPbobsCOM.Recordset invrecordset, Freightrecset;
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                Freightrecset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                if (Create_Cancel == EinvoiceMethod.CreateIRN)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Invoice. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    GenerateIRN GenerateIRNGetJson = new GenerateIRN();

                    strSQL = GetInvoiceData(DocEntry, TransType);
                    clsModule.objaddon.objapplication.StatusBar.SetText("Getting Data.... Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    invrecordset.DoQuery(strSQL);
                    if (invrecordset.RecordCount > 0)
                    {
                        strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Generate IRN' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";
                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Create IRN\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        if (string.IsNullOrEmpty(invrecordset.Fields.Item("BpGSTN").Value.ToString()))
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("GST No is Missing for \"Create GSTNo\"... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                       string AssignEinvunit= invrecordset.Fields.Item("Unit").Value.ToString();
                       

                        strSQL = "SELECT \"U_GUnitCod\"  FROM \"@UOMMAP\" u WHERE u.\"U_UOMCod\" ='" + AssignEinvunit + "'";
                        DataTable dt1 = new DataTable();
                        dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                        if (dt1.Rows.Count > 0)
                        {
                            AssignEinvunit = dt1.Rows[0]["U_GUnitCod"].ToString();
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Unit(" + AssignEinvunit + ") Not Mapped please Map Unit... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        if (Convert.ToDouble(invrecordset.Fields.Item("Distance").Value) > 0)
                        {
                            Generate_EWay distanceEway = new Generate_EWay();
                            distanceEway.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                            distanceEway.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                            distanceEway.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                            distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                            distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();

                            DataTable deway = Get_API_Response(JsonConvert.SerializeObject(distanceEway), objRs.Fields.Item("BaseURL").Value.ToString() + "/TransactionAPI/GetPincodeDistance");

                            if (deway.Rows.Count > 0)
                            {
                                if (Convert.ToDouble(deway.Rows[0]["Distance"].ToString()) < Convert.ToDouble(invrecordset.Fields.Item("Distance").Value))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Distance Must be Less than " + deway.Rows[0]["Distance"].ToString() + " ... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                            }
                        }

                        GenerateIRNGetJson.client_code = objRs.Fields.Item("U_ClientCode").Value.ToString();
                        GenerateIRNGetJson.user_code = objRs.Fields.Item("U_UserCode").Value.ToString();
                        GenerateIRNGetJson.password = objRs.Fields.Item("U_Password").Value.ToString();
                        GenerateIRNGetJson.json_data.Version = "1.1";
                        GenerateIRNGetJson.json_data.TranDtls.TaxSch = invrecordset.Fields.Item("TaxSch").Value.ToString();
                        GenerateIRNGetJson.json_data.TranDtls.SupTyp = invrecordset.Fields.Item("SupTyp").Value.ToString();
                        GenerateIRNGetJson.json_data.TranDtls.RegRev = "N";
                        GenerateIRNGetJson.json_data.TranDtls.EcmGstin = "";
                        GenerateIRNGetJson.json_data.TranDtls.IgstOnIntra = "";

                        GenerateIRNGetJson.json_data.DocDtls.Typ = invrecordset.Fields.Item("Type").Value.ToString();
                        GenerateIRNGetJson.json_data.DocDtls.No = invrecordset.Fields.Item("Inv_No").Value.ToString();
                        GenerateIRNGetJson.json_data.DocDtls.Dt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString());



                        GenerateIRNGetJson.json_data.SellerDtls.Gstin = invrecordset.Fields.Item("Seller GSTN").Value.ToString(); //"32ACRPV8768P1Z8";
                        GenerateIRNGetJson.json_data.SellerDtls.LglNm = invrecordset.Fields.Item("Seller_Legal Name").Value.ToString();
                        GenerateIRNGetJson.json_data.SellerDtls.Addr1 = invrecordset.Fields.Item("Seller_Addr1").Value.ToString();
                        GenerateIRNGetJson.json_data.SellerDtls.Loc = invrecordset.Fields.Item("Seller Location Name").Value.ToString();
                        GenerateIRNGetJson.json_data.SellerDtls.Pin = (invrecordset.Fields.Item("Seller_PIN code").Value.ToString() == "") ? 0 : Convert.ToInt32(invrecordset.Fields.Item("Seller_PIN code").Value.ToString());// 695001;
                        GenerateIRNGetJson.json_data.SellerDtls.Stcd = invrecordset.Fields.Item("Seller_State_code").Value.ToString();//"32";

                        GenerateIRNGetJson.json_data.BuyerDtls.Gstin = invrecordset.Fields.Item("Buyer GSTN").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.LglNm = invrecordset.Fields.Item("Buyer_Legal Name").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Pos = invrecordset.Fields.Item("Bill to State Code").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Addr1 = invrecordset.Fields.Item("BBuilding").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Loc = invrecordset.Fields.Item("BCity").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Stcd = invrecordset.Fields.Item("Bill to State Code").Value.ToString();
                        GenerateIRNGetJson.json_data.BuyerDtls.Pin = (invrecordset.Fields.Item("BZipCode").Value.ToString() == "") ? 0 : Convert.ToInt32(invrecordset.Fields.Item("BZipCode").Value.ToString());


                        GenerateIRNGetJson.json_data.DispDtls.Nm = invrecordset.Fields.Item("Buyer_Legal Name").Value.ToString();
                        GenerateIRNGetJson.json_data.DispDtls.Addr1 = invrecordset.Fields.Item("SBuilding").Value.ToString();
                        GenerateIRNGetJson.json_data.DispDtls.Loc = invrecordset.Fields.Item("SCity").Value.ToString();
                        GenerateIRNGetJson.json_data.DispDtls.Pin = (invrecordset.Fields.Item("SZipCode").Value.ToString() == "") ? 0 : Convert.ToInt32(invrecordset.Fields.Item("SZipCode").Value.ToString()); //Convert.ToInt32(invrecordset.Fields.Item("SZipCode").Value.ToString());
                        GenerateIRNGetJson.json_data.DispDtls.Stcd = invrecordset.Fields.Item("Shipp to State Code").Value.ToString();

                        GenerateIRNGetJson.json_data.ShipDtls.Gstin = invrecordset.Fields.Item("Buyer GSTN").Value.ToString();
                        GenerateIRNGetJson.json_data.ShipDtls.LglNm = invrecordset.Fields.Item("Buyer_Legal Name").Value.ToString();
                        GenerateIRNGetJson.json_data.ShipDtls.Addr1 = invrecordset.Fields.Item("SBuilding").Value.ToString();
                        GenerateIRNGetJson.json_data.ShipDtls.Loc = invrecordset.Fields.Item("SCity").Value.ToString();
                        GenerateIRNGetJson.json_data.ShipDtls.Pin = (invrecordset.Fields.Item("SZipCode").Value.ToString() == "") ? 0 : Convert.ToInt32(invrecordset.Fields.Item("SZipCode").Value.ToString()); //Convert.ToInt32(invrecordset.Fields.Item("SZipCode").Value.ToString());
                        GenerateIRNGetJson.json_data.ShipDtls.Stcd = invrecordset.Fields.Item("Shipp to State Code").Value.ToString();

                        for (int i = 0; i < invrecordset.RecordCount; i++)
                        {
                            GenerateIRNGetJson.json_data.ItemList.Add(new ItemList
                            {
                                SlNo = invrecordset.Fields.Item("SINo").Value.ToString(),
                                PrdDesc = invrecordset.Fields.Item("Dscription").Value.ToString(),
                                IsServc = invrecordset.Fields.Item("IsServc").Value.ToString(),
                                HsnCd = invrecordset.Fields.Item("HSN").Value.ToString(),//"9965" for Service Invoice,
                                Qty = Convert.ToDouble(invrecordset.Fields.Item("Quantity").Value.ToString()),
                                Discount = Convert.ToDouble(invrecordset.Fields.Item("LineDiscountAmt").Value.ToString()),//LineDisc
                                Unit = AssignEinvunit,
                                UnitPrice = Convert.ToDouble(invrecordset.Fields.Item("UnitPrice").Value.ToString()),
                                TotAmt = Convert.ToDouble(invrecordset.Fields.Item("Tot Amt").Value.ToString()),
                                AssAmt = Convert.ToDouble(invrecordset.Fields.Item("AssAmt").Value.ToString()),//AssAmt
                                GstRt = Convert.ToDouble(invrecordset.Fields.Item("GSTRATE").Value.ToString()),
                                TotItemVal = Convert.ToDouble(invrecordset.Fields.Item("Total Item Value").Value.ToString()),
                                CgstAmt = Convert.ToDouble(invrecordset.Fields.Item("CGSTAmt").Value.ToString()),
                                SgstAmt = Convert.ToDouble(invrecordset.Fields.Item("SGSTAmt").Value.ToString()),
                                IgstAmt = Convert.ToDouble(invrecordset.Fields.Item("IGSTAmt").Value.ToString()),
                                BchDtls = new BchDtls()
                                {
                                    Nm = invrecordset.Fields.Item("BatchNum").Value.ToString(),
                                    ExpDt = "",
                                    WrDt = ""
                                },
                                AttribDtls = { new AttribDtl() { Nm = "", Val = "" } }
                            });
                            invrecordset.MoveNext();
                        }

                        strSQL = GetFrightData(DocEntry);
                        Freightrecset.DoQuery(strSQL);
                        if (Freightrecset.RecordCount > 0)
                        {
                            for (int i = 0; i < Freightrecset.RecordCount; i++)
                            {
                                int row = GenerateIRNGetJson.json_data.ItemList.Count + 1;
                                GenerateIRNGetJson.json_data.ItemList.Add(new ItemList
                                {
                                    SlNo = Convert.ToString(row),
                                    PrdDesc = Freightrecset.Fields.Item("Dscription").Value.ToString(),
                                    IsServc = "Y",
                                    HsnCd = Freightrecset.Fields.Item("HSN").Value.ToString(),
                                    Qty = Convert.ToDouble(Freightrecset.Fields.Item("Quantity").Value.ToString()),
                                    UnitPrice = Convert.ToDouble(Freightrecset.Fields.Item("LineTotal").Value.ToString()),
                                    TotAmt = Convert.ToDouble(Freightrecset.Fields.Item("LineTotal").Value.ToString()),
                                    AssAmt = Convert.ToDouble(Freightrecset.Fields.Item("LineTotal").Value.ToString()),//AssAmt
                                    GstRt = Convert.ToDouble(Freightrecset.Fields.Item("VatPrcnt").Value.ToString()),
                                    TotItemVal = Convert.ToDouble(Freightrecset.Fields.Item("Total Value").Value.ToString()),
                                    CgstAmt = Convert.ToDouble(Freightrecset.Fields.Item("CGSTAmt").Value.ToString()),
                                    SgstAmt = Convert.ToDouble(Freightrecset.Fields.Item("SGSTAmt").Value.ToString()),
                                    IgstAmt = Convert.ToDouble(Freightrecset.Fields.Item("IGSTAmt").Value.ToString()),

                                });
                                Freightrecset.MoveNext();
                            }
                        }


                        invrecordset.MoveFirst();
                        GenerateIRNGetJson.json_data.ValDtls.CgstVal = Convert.ToDouble(invrecordset.Fields.Item("CGSTVal").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.SgstVal = Convert.ToDouble(invrecordset.Fields.Item("SGSTVal").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.IgstVal = Convert.ToDouble(invrecordset.Fields.Item("IGSTVal").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.AssVal = Convert.ToDouble(invrecordset.Fields.Item("AssValN").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.TotInvVal = Convert.ToDouble(invrecordset.Fields.Item("Doc Total").Value.ToString());
                        GenerateIRNGetJson.json_data.ValDtls.RndOffAmt = Convert.ToDouble(invrecordset.Fields.Item("RoundDif").Value.ToString());

                        GenerateIRNGetJson.json_data.PayDtls.Nm = invrecordset.Fields.Item("CAcctName").Value.ToString();
                        GenerateIRNGetJson.json_data.PayDtls.AccDet = invrecordset.Fields.Item("CAccount").Value.ToString();
                        GenerateIRNGetJson.json_data.PayDtls.FinInsBr = invrecordset.Fields.Item("CIFSCNo").Value.ToString();

                        GenerateIRNGetJson.json_data.RefDtls.InvRm = "";
                        GenerateIRNGetJson.json_data.RefDtls.DocPerdDtls.InvStDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString());
                        GenerateIRNGetJson.json_data.RefDtls.DocPerdDtls.InvStDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString());
                        GenerateIRNGetJson.json_data.RefDtls.DocPerdDtls.InvEndDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv Due Date").Value.ToString());


                        GenerateIRNGetJson.json_data.RefDtls.PrecDocDtls.Add(new PrecDocDtl
                        {
                            InvNo = invrecordset.Fields.Item("Inv_No").Value.ToString(),
                            InvDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString()),
                            OthRefNo = ""

                        });
                        GenerateIRNGetJson.json_data.RefDtls.ContrDtls.Add(new ContrDtl
                        {
                            ContrRefr = ""
                        });

                        GenerateIRNGetJson.json_data.AddlDocDtls.Add(new AddlDocDtl
                        {
                            Docs = ""
                        });
                        GenerateIRNGetJson.json_data.ExpDtls.CntCode = "";

                        if (Convert.ToDouble(invrecordset.Fields.Item("Distance").Value) > 0)
                        {

                            GenerateIRNGetJson.json_data.EwbDtls.Distance = Convert.ToInt32(invrecordset.Fields.Item("Distance").Value);
                            GenerateIRNGetJson.json_data.EwbDtls.TransDocDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString());
                            GenerateIRNGetJson.json_data.EwbDtls.TransDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString();
                            GenerateIRNGetJson.json_data.EwbDtls.TransId = invrecordset.Fields.Item("TransID").Value.ToString();
                            GenerateIRNGetJson.json_data.EwbDtls.TransMode = invrecordset.Fields.Item("TransMode").Value.ToString();
                            GenerateIRNGetJson.json_data.EwbDtls.TransName = invrecordset.Fields.Item("TransName").Value.ToString();
                            GenerateIRNGetJson.json_data.EwbDtls.VehNo = invrecordset.Fields.Item("VehicleNo").Value.ToString();
                            GenerateIRNGetJson.json_data.EwbDtls.VehType = invrecordset.Fields.Item("VehicleTyp").Value.ToString();

                        }

                        requestParams = JsonConvert.SerializeObject(GenerateIRNGetJson);

                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                        string msg = datatable.Rows[0]["message"].ToString();
                        if (datatable.Rows[0]["error_log_ids"].ToString() == string.Empty)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("No data found for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    GenerateIRNGetJson = null;

                }
                else if (Create_Cancel == EinvoiceMethod.CreateEway)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Way. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    Generate_EWay GenerateIRNGetJson = new Generate_EWay();
                    strSQL = GetInvoiceData(DocEntry, TransType);
                    invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    invrecordset.DoQuery(strSQL);

                    DataTable dt = new DataTable();
                    dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                    if (invrecordset.RecordCount > 0)
                    {
                        strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T0.""U_UATUrl"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01' and T1.""U_URLType"" ='Generate IRN' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";
                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Create IRN\". Please up  in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        if (Convert.ToDouble(invrecordset.Fields.Item("Distance").Value) < 0)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Distance Must be Greater than Zero ... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        Generate_EWay distanceEway = new Generate_EWay();
                        distanceEway.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                        distanceEway.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                        distanceEway.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                        distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                        distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();

                        DataTable deway = Get_API_Response(JsonConvert.SerializeObject(distanceEway), objRs.Fields.Item("BaseURL").Value.ToString() + "/TransactionAPI/GetPincodeDistance");

                        if (deway.Rows.Count > 0)
                        {
                            if (Convert.ToDouble(deway.Rows[0]["Distance"].ToString()) < Convert.ToDouble(invrecordset.Fields.Item("Distance").Value))
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Distance Must be Less than " + deway.Rows[0]["Distance"].ToString() + " ... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                return false;
                            }
                        }


                        string AssignEwayunit = invrecordset.Fields.Item("Unit").Value.ToString();

                        strSQL = "SELECT \"U_GUnitCod\"  FROM \"@UOMMAP\" u WHERE u.\"U_UOMCod\" ='"+ AssignEwayunit+"'";
                        DataTable dt1 = new DataTable();
                        dt1 = clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                        if (dt1.Rows.Count>0)
                        {
                            AssignEwayunit = dt1.Rows[0]["U_GUnitCod"].ToString();
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Unit("+AssignEwayunit+") Not Mapped please Map Unit... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            return false;
                        }

                        GenerateIRNGetJson.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                        GenerateIRNGetJson.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                        GenerateIRNGetJson.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                        GenerateIRNGetJson.version = "1.1";
                        GenerateIRNGetJson.billLists.Add(new Generate_EWay.EwayList
                        {
                            userGstin = invrecordset.Fields.Item("Seller GSTN").Value.ToString(),
                            supplyType = invrecordset.Fields.Item("SuplyType").Value.ToString(),
                            subSupplyType = invrecordset.Fields.Item("SubSplyTyp").Value.ToString(),
                            subSupplyTypeDesc = invrecordset.Fields.Item("SubtypeDescription").Value.ToString(),
                            docType = invrecordset.Fields.Item("EDocType").Value.ToString(),
                            docNo = invrecordset.Fields.Item("Inv_No").Value.ToString(),
                            docDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("Inv_Doc_Date").Value.ToString()),
                            TransType = invrecordset.Fields.Item("TransType").Value.ToString(),
                            fromGstin = invrecordset.Fields.Item("FrmGSTN").Value.ToString(),
                            fromTrdName = invrecordset.Fields.Item("FrmTraName").Value.ToString(),
                            fromAddr1 = invrecordset.Fields.Item("FrmAddres1").Value.ToString(),
                            fromAddr2 = invrecordset.Fields.Item("FrmAddres2").Value.ToString(),
                            fromPlace = invrecordset.Fields.Item("FrmPlace").Value.ToString(),
                            fromPincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString(),
                            fromStateCode = invrecordset.Fields.Item("ActFrmStat").Value.ToString(),
                            actualFromStateCode = invrecordset.Fields.Item("ActFrmStat").Value.ToString(),
                            toGstin = invrecordset.Fields.Item("ToGSTN").Value.ToString(),
                            toTrdName = invrecordset.Fields.Item("ToTraName").Value.ToString(),
                            toAddr1 = invrecordset.Fields.Item("ToAddres1").Value.ToString(),
                            toAddr2 = invrecordset.Fields.Item("ToAddres2").Value.ToString(),
                            toPlace = invrecordset.Fields.Item("ToPlace").Value.ToString(),
                            toPincode = invrecordset.Fields.Item("ToZipCode").Value.ToString(),
                            actualToStateCode = invrecordset.Fields.Item("ActToState").Value.ToString(),
                            toStateCode = invrecordset.Fields.Item("ActToState").Value.ToString(),
                            totalValue = invrecordset.Fields.Item("AssValN").Value.ToString(),
                            cgstValue = invrecordset.Fields.Item("CGSTVal").Value.ToString(),
                            sgstValue = invrecordset.Fields.Item("SGSTVal").Value.ToString(),
                            igstValue = invrecordset.Fields.Item("IGSTVal").Value.ToString(),
                            transDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString(),
                            transDocDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString()),
                            totInvValue = invrecordset.Fields.Item("Doc Total").Value.ToString(),
                            transporterId = invrecordset.Fields.Item("TransID").Value.ToString(),
                            transporterName = invrecordset.Fields.Item("TransName").Value.ToString(),
                            transMode = invrecordset.Fields.Item("TransMode").Value.ToString(),
                            transDistance = invrecordset.Fields.Item("Distance").Value.ToString(),
                            vehicleNo = invrecordset.Fields.Item("VehicleNo").Value.ToString(),
                            vehicleType = invrecordset.Fields.Item("VehicleTyp").Value.ToString(),
                            shipToGSTIN = invrecordset.Fields.Item("ToGSTN").Value.ToString(),
                            dispatchFromGSTIN = invrecordset.Fields.Item("FrmGSTN").Value.ToString(),
                            dispatchFromTradeName = invrecordset.Fields.Item("FrmTraName").Value.ToString(),
                        });


                        for (int i = 0; i < invrecordset.RecordCount; i++)
                        {
                            GenerateIRNGetJson.billLists[0].itemList.Add(new Generate_EWay.Ewayitemlist
                            {
                                itemNo = Convert.ToInt32(invrecordset.Fields.Item("SINo").Value),
                                productName = invrecordset.Fields.Item("Dscription").Value.ToString(),
                                productDesc = invrecordset.Fields.Item("Dscription").Value.ToString(),
                                hsnCode = invrecordset.Fields.Item("HSN").Value.ToString(),//"9965" for Service Invoice,
                                quantity = invrecordset.Fields.Item("Quantity").Value.ToString(),
                                qtyUnit = AssignEwayunit,
                                taxableAmount = invrecordset.Fields.Item("AssAmt").Value.ToString(),
                                sgstRate = (Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString()) == 0) ? Convert.ToDecimal(invrecordset.Fields.Item("GSTRATE").Value.ToString()) / 2 : 0,
                                cgstRate = (Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString()) == 0) ? Convert.ToDecimal(invrecordset.Fields.Item("GSTRATE").Value.ToString()) / 2 : 0,
                                igstRate = (Convert.ToDecimal(invrecordset.Fields.Item("IGSTVal").Value.ToString()) == 0) ? 0 : Convert.ToDecimal(invrecordset.Fields.Item("GSTRATE").Value.ToString()),
                            });
                            invrecordset.MoveNext();
                        }

                        requestParams = JsonConvert.SerializeObject(GenerateIRNGetJson);
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());

                        E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                        string msg = datatable.Rows[0]["message"].ToString();
                        if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("No data found for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }


                    GenerateIRNGetJson = null;

                }
                else if (Create_Cancel == EinvoiceMethod.CancelIRN)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Cancelling E-Invoice. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    ClientCred_Cancel ClientCred = new ClientCred_Cancel();
                    {
                        strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' " +
                                  @"then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'" +
                                  @"and T1.""U_URLType"" ='Cancel IRN' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";

                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            SapMessage = "API is Missing for Cancel IRN. Please update in general settings... ";
                            clsModule.objaddon.objapplication.StatusBar.SetText(SapMessage, SAPbouiCOM.BoMessageTime.bmt_Short);
                            return false;
                        }

                        if (objRs.RecordCount > 0)
                        {
                            ClientCred.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                            ClientCred.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                            ClientCred.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                        }

                        switch (TransType)
                        {
                            case "INV":
                                strSQL = @"SELECT o.""U_IRNNo""  FROM OINV o  where o.""DocEntry"" ='" + DocEntry + "'";
                                break;
                            case "CRN":
                                strSQL = @"SELECT o.""U_IRNNo""  FROM ORIN o where o.""DocEntry"" ='" + DocEntry + "' ";
                                break;
                        }
                        string IRN = clsModule.objaddon.objglobalmethods.getSingleValue(strSQL);
                        ClientCred.cancelledeinvoicelist.Add(new Cancelledeinvoicelist
                        {
                            Irn = IRN,
                            CnlRem = "Cancelling against DocNum",//docnum include
                            CnlRsn = 4

                        });
                    }
                    requestParams = JsonConvert.SerializeObject(ClientCred);
                    datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                    E_Invoice_Logs(DocEntry, datatable, TransType, "Cancel", Type, requestParams);
                    string Emsg = datatable.Rows[0]["message"].ToString();
                    if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_IRN: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_IRN: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    ClientCred = null;

                }
                else if (Create_Cancel == EinvoiceMethod.CancelEway)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Cancelling E-Way Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    ClientCred_Cancel ClientCred = new ClientCred_Cancel();
                    {
                        strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' " +
                                     @"then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                        strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                        strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'" +
                                  @"and T1.""U_URLType"" ='Cancel IRN' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";

                        objRs.DoQuery(strSQL);
                        if (objRs.RecordCount == 0)
                        {
                            SapMessage = "API is Missing for Cancel IRN. Please update in general settings... ";
                            clsModule.objaddon.objapplication.StatusBar.SetText(SapMessage, SAPbouiCOM.BoMessageTime.bmt_Short);
                            return false;
                        }


                        if (objRs.RecordCount > 0)
                        {
                            ClientCred.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                            ClientCred.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                            ClientCred.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                        }

                        switch (TransType)
                        {
                            case "INV":
                                strSQL = @"SELECT i.""EWayBillNo""  FROM OINV o  inner JOIN INV26 i  ON o.""DocEntry"" =i.""DocEntry"" WHERE o.""DocEntry""='" + DocEntry + "'";
                                break;
                            case "CRN":

                                strSQL = @"SELECT i.""EWayBillNo"" FROM ORIN o JOIN RIN26 r ON o.""DocEntry"" =i.""DocEntry"" WHERE o.""DocEntry""='" + DocEntry + "'";
                                break;
                        }

                        string EwbNo = clsModule.objaddon.objglobalmethods.getSingleValue(strSQL);
                        ClientCred.cancelledeinvoiceewblist.Add(new cancelledeinvoiceewblist
                        {
                            ewbNo = EwbNo,
                            cancelRsnCode  = "2",
                            cancelRmrk = "OK"
                        });
                        //
                    }
                    requestParams = JsonConvert.SerializeObject(ClientCred);
                    datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                    E_Invoice_Logs(DocEntry, datatable, TransType, "Cancel", Type, requestParams);

                    string Emsg = datatable.Rows[0]["message"].ToString();
                    if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_Eay: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    }
                    else
                    {
                        clsModule.objaddon.objapplication.StatusBar.SetText("Cancel_Eway: " + Emsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    ClientCred = null;

                }
                else if (Create_Cancel == EinvoiceMethod.GetEwayByIRN)
                {

                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Way by IRN. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T0.""U_UATUrl"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Generate Eway by IRN' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";
                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Generate Eway by IRN\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }

                    GetEwayByIRN clienCred_GetIRN_DocNum = new GetEwayByIRN();
                    if (objRs.RecordCount > 0)
                    {
                        clienCred_GetIRN_DocNum.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                        clienCred_GetIRN_DocNum.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                        clienCred_GetIRN_DocNum.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string tb = "";
                        string eway = "";
                        switch (TransType)
                        {
                            case "INV":
                                tb = "OINV";
                                eway = "inv26";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                eway = "rin26";
                                break;
                        }

                        strSQL = @"SELECT t1.""U_IRNNo"" ,t2.""Distance"" ,t2.""TransMode"" ,t2.""TransID"" ,t2.""TransName"" ,t2.""TransDocNo"" ,t2.""TransDate"",t2.""FrmZipCode"",t2.""ToZipCode"",";
                        strSQL += @"t2.""VehicleNo"" ,t2.""VehicleTyp""  FROM " + tb + @" t1 LEFT JOIN " + eway + @" t2 ON t1.""DocEntry"" =t2.""DocEntry"" where t1.""DocEntry""='" + DocEntry + @"' AND t2.""Distance"" >0";

                        invrecordset.DoQuery(strSQL);
                        if (invrecordset.RecordCount > 0)
                        {
                            Generate_EWay distanceEway = new Generate_EWay();
                            distanceEway.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                            distanceEway.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                            distanceEway.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                            distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                            distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();

                            DataTable deway = Get_API_Response(JsonConvert.SerializeObject(distanceEway), objRs.Fields.Item("BaseURL").Value.ToString() + "/TransactionAPI/GetPincodeDistance");

                            if (deway.Rows.Count > 0)
                            {
                                if (Convert.ToDouble(deway.Rows[0]["Distance"].ToString()) < Convert.ToDouble(invrecordset.Fields.Item("Distance").Value))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Distance Must be Less than " + deway.Rows[0]["Distance"].ToString() + " ... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                            }

                            clienCred_GetIRN_DocNum.ewbeinvoicelist.Add(new Ewbeinvoicelist
                            {
                                Irn = invrecordset.Fields.Item("U_IRNNo").Value.ToString(),
                                Distance = invrecordset.Fields.Item("Distance").Value.ToString(),
                                TransDocNo = invrecordset.Fields.Item("TransDocNo").Value.ToString(),
                                TransId = invrecordset.Fields.Item("TransID").Value.ToString(),
                                TransMode = invrecordset.Fields.Item("TransMode").Value.ToString(),
                                TransName = invrecordset.Fields.Item("TransName").Value.ToString(),
                                VehNo = invrecordset.Fields.Item("VehicleNo").Value.ToString(),
                                VehType = invrecordset.Fields.Item("VehicleTyp").Value.ToString(),
                                TransDocDt = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString())
                            });
                            requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                            datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                            string mm = datatable.Rows[0]["message"].ToString();
                            E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                            string msg = datatable.Rows[0]["message"].ToString();
                            if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            }
                            else
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Check Eway Details for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }

                        clienCred_GetIRN_DocNum = null;

                    }
                }
                else if (Create_Cancel == EinvoiceMethod.UpdateEway)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Generating E-Way Update Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T0.""U_UATUrl"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Update Eway' and T1.""U_Type""='E-Way' Order by ""LineId"" Desc";
                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Update Eway\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }

                    UpdateEway clienCred_GetIRN_DocNum = new UpdateEway();
                    if (objRs.RecordCount > 0)
                    {
                        clienCred_GetIRN_DocNum.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                        clienCred_GetIRN_DocNum.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                        clienCred_GetIRN_DocNum.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string tb = "";
                        string eway = "";
                        switch (TransType)
                        {
                            case "INV":
                                tb = "OINV";
                                eway = "inv26";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                eway = "rin26";
                                break;
                        }

                        strSQL = @"SELECT t2.""VehicleTyp"",t1.""U_IRNNo"" ,t2.""Distance"" ,t2.""TransMode"" ,t2.""TransID"" ,t2.""TransName"" ,t2.""TransDocNo"" ,t2.""TransDate"",t2.""FrmZipCode"",t2.""ToZipCode"",";
                        strSQL += @"t2.""VehicleNo"" ,t2.""TransMode"",t2.""TransDocNo"" ,t2.""TransDate"" ,t2.""FrmState"" ,t2.""ToState"" ,t2.""VehicleTyp"" ,t2.""EWayBillNo"",t2.""FrmPlace"" ,t2.""ToPlace"" ,t2.""ActFrmStat"" ,t2.""ActToState"" FROM " + tb + @" t1 LEFT JOIN " + eway + @" t2 ON t1.""DocEntry"" =t2.""DocEntry"" where t1.""DocEntry""='" + DocEntry + @"' AND t2.""Distance"" >0";

                        invrecordset.DoQuery(strSQL);
                        if (invrecordset.RecordCount > 0)
                        {

                            Generate_EWay distanceEway = new Generate_EWay();
                            distanceEway.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();
                            distanceEway.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();
                            distanceEway.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();
                            distanceEway.frompincode = invrecordset.Fields.Item("FrmZipCode").Value.ToString();
                            distanceEway.topincode = invrecordset.Fields.Item("ToZipCode").Value.ToString();

                            DataTable deway = Get_API_Response(JsonConvert.SerializeObject(distanceEway), objRs.Fields.Item("BaseURL").Value.ToString() + "/TransactionAPI/GetPincodeDistance");

                            if (deway.Rows.Count > 0)
                            {
                                if (Convert.ToDouble(deway.Rows[0]["Distance"].ToString()) < Convert.ToDouble(invrecordset.Fields.Item("Distance").Value))
                                {
                                    clsModule.objaddon.objapplication.StatusBar.SetText("Distance Must be Less than " + deway.Rows[0]["Distance"].ToString() + " ... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                    return false;
                                }
                            }

                            clienCred_GetIRN_DocNum.Vehicleupdatelist.Add(new Vehicleupdatelist
                            {
                                ewbNo= invrecordset.Fields.Item("EWayBillNo").Value.ToString(),
                                fromPlace =invrecordset.Fields.Item("FrmPlace").Value.ToString(),
                                fromState =invrecordset.Fields.Item("FrmState").Value.ToString(),
                                reasonCode="1",
                                reasonRem="Change",
                                transDocDate= clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("TransDate").Value.ToString()),
                                transDocNo= invrecordset.Fields.Item("TransDocNo").Value.ToString(),
                                transMode= invrecordset.Fields.Item("TransMode").Value.ToString(),
                                vehicleNo= invrecordset.Fields.Item("VehicleNo").Value.ToString(),
                                vehicleType= invrecordset.Fields.Item("VehicleTyp").Value.ToString(),

                            });
                            requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                            datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                            string mm = datatable.Rows[0]["message"].ToString();
                            E_Invoice_Logs(DocEntry, datatable, TransType, "Create", Type, requestParams);

                            string msg = datatable.Rows[0]["message"].ToString();
                            if (datatable.Rows[0]["error_log_id"].ToString() == string.Empty)
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            }
                            else
                            {
                                clsModule.objaddon.objapplication.StatusBar.SetText("Generate: " + msg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
                        }
                        else
                        {
                            clsModule.objaddon.objapplication.StatusBar.SetText("Check Eway Details for this invoice...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }

                        clienCred_GetIRN_DocNum = null;

                    }
                }
                else if (Create_Cancel == EinvoiceMethod.GetIrnByDocnum)
                {
                    clsModule.objaddon.objapplication.StatusBar.SetText("Getting IRN. Please wait...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);

                    strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                    strSQL += @"from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Get IRN Details by Document number' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";

                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Get IRN Details by Document number\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }

                    ClienCred_GetIRN_DocNum clienCred_GetIRN_DocNum = new ClienCred_GetIRN_DocNum();
                    if (objRs.RecordCount > 0)
                    {
                        clienCred_GetIRN_DocNum.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();//"ptmuT";
                        clienCred_GetIRN_DocNum.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();// "Premier_DEMO";
                        clienCred_GetIRN_DocNum.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();//"Premier@123";
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        strSQL = "Select T0.GSTRegnNo,T2.DocEntry,T2.DocNum,Format(T2.DocDate,'dd/MM/yyyy') as DocDate,'INV' DocType";
                        strSQL += "\n from OLCT T0 left join INV1 T1 on T0.Code=T1.LocCode left join OINV T2 on T1.DocEntry=T2.DocEntry where T2.DocEntry=" + DocEntry + "";
                        invrecordset.DoQuery(strSQL);
                        clienCred_GetIRN_DocNum.RequestorGSTIN = invrecordset.Fields.Item("GSTRegnNo").Value.ToString();//"32ACRPV8768P1Z8";
                        clienCred_GetIRN_DocNum.docdetailslist.Add(new Docdetailslist
                        {
                            DocNum = invrecordset.Fields.Item("DocNum").Value.ToString(),
                            DocType = invrecordset.Fields.Item("DocType").Value.ToString(),
                            DocDate = clsModule.objaddon.objglobalmethods.Getdateformat(invrecordset.Fields.Item("DocDate").Value.ToString())
                        });
                        requestParams = JsonConvert.SerializeObject(clienCred_GetIRN_DocNum);
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                        string mm = datatable.Rows[0]["message"].ToString();//flag                        
                        if (datatable.Rows[0]["flag"].ToString().ToUpper() == "TRUE")
                        {
                            E_Invoice_Logs(DocEntry, datatable, "13", "Get IRN Details by Document number", Type, requestParams);
                            objRs.DoQuery("Update OINV Set U_IRNNo='" + datatable.Rows[0]["Irn"].ToString() + "',U_QRCode='" + datatable.Rows[0]["SignedQRCode"].ToString() + "' where DocEntry='" + DocEntry + "'");
                            clsModule.objaddon.objapplication.StatusBar.SetText("Get_IRN: " + mm, SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                        clienCred_GetIRN_DocNum = null;

                    }
                }
                else if (Create_Cancel == EinvoiceMethod.GETIRNDetails)
                {                   
                    strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",Case when T0.""U_Live""='N' then CONCAT(T0.""U_UATUrl"",T1.""U_URL"") Else CONCAT(T0.""U_LIVEUrl"",T1.""U_URL"") End as URL";
                    strSQL += @" ,Case when T0.""U_Live""='N' then T0.""U_UATUrl"" Else T0.""U_LIVEUrl"" End as BaseURL";
                    strSQL += @"from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'  and T1.""U_URLType"" ='Get IRN Details' and T1.""U_Type""='E-Invoice' Order by ""LineId"" Desc";


                    objRs.DoQuery(strSQL);
                    if (objRs.RecordCount == 0) { clsModule.objaddon.objapplication.StatusBar.SetText("API is Missing for \"Get IRN Details\". Please update in general settings... ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error); return false; }
                    GetIRN getIRN = new GetIRN();
                    if (objRs.RecordCount > 0)
                    {
                        getIRN.CLIENTCODE = objRs.Fields.Item("U_ClientCode").Value.ToString();//"ptmuT";
                        getIRN.USERCODE = objRs.Fields.Item("U_UserCode").Value.ToString();// "Premier_DEMO";
                        getIRN.PASSWORD = objRs.Fields.Item("U_Password").Value.ToString();//"Premier@123";
                        invrecordset = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        strSQL = @"Select T0.""GSTRegnNo"",T2.""DocEntry"",T2.""DocNum"",Format(T2.""DocDate"",'dd/MM/yyyy') as DocDate,'INV' DocType,T2.""U_IRNNo""";
                        strSQL += @" from ""OLCT"" T0 left join ""INV1"" T1 on T0.""Code""=T1.""LocCode"" left join ""OINV"" T2 on T1.""DocEntry""=T2.""DocEntry"" where T2.""DocEntry""=" + DocEntry + "";
                        invrecordset.DoQuery(strSQL);
                        getIRN.RequestorGSTIN = invrecordset.Fields.Item("GSTRegnNo").Value.ToString();//"32ACRPV8768P1Z8";
                        getIRN.irnlist.Add(new Irnlist
                        {
                            irn = invrecordset.Fields.Item("U_IRNNo").Value.ToString()

                        });
                        requestParams = JsonConvert.SerializeObject(getIRN);
                        datatable = Get_API_Response(requestParams, objRs.Fields.Item("URL").Value.ToString());
                        getIRN = null;
                    }
                }

               
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("Error_IRN: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            return true;
        }
        private bool E_Invoice_Logs(string InvDocEntry, DataTable einvDT, string ObjType, string Type, string TranType,string requrl)
        {
            try
            {
                bool Flag = false;
                string DocEntry;
                string obj = "";               
                switch (ObjType)
                {
                    case "INV":
                        obj = "13";
                        break;
                    case "CRN":
                        obj = "14";
                        break;
                }

                SAPbobsCOM.GeneralService oGeneralService;
                SAPbobsCOM.GeneralData oGeneralData;
                SAPbobsCOM.GeneralDataParams oGeneralParams;
                objRs = (SAPbobsCOM.Recordset)clsModule.objaddon.objcompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("ATEINV");
                oGeneralData = (SAPbobsCOM.GeneralData)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (SAPbobsCOM.GeneralDataParams)oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams);
                try
                {
                    DocEntry = clsModule.objaddon.objglobalmethods.getSingleValue(@"Select ""DocEntry"" from ""@ATPL_EINV"" where ""U_BaseEntry""='" + InvDocEntry + @"'And ""U_DocObjType""='" + obj + @"'  Order by ""DocEntry"" Desc");
                    oGeneralParams.SetProperty("DocEntry", DocEntry);
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    Flag = true;
                }
                catch (Exception ex)
                {
                    Flag = false;
                }
                if (Type == "Create")
                {

                    if (TranType == "E-Invoice")
                    {
                        oGeneralData.SetProperty("U_IRNNo", einvDT.Rows[0]["Irn"].ToString());
                        oGeneralData.SetProperty("U_QRCode", einvDT.Rows[0]["SignedQRCode"].ToString());
                        oGeneralData.SetProperty("U_SgnInv", einvDT.Rows[0]["SignedInvoice"].ToString());
                        oGeneralData.SetProperty("U_AckNo", einvDT.Rows[0]["AckNo"].ToString());
                        oGeneralData.SetProperty("U_IRNStat", einvDT.Rows[0]["Status"].ToString());
                        oGeneralData.SetProperty("U_DcrptInv", einvDT.Rows[0]["DcrySignedInvoice"].ToString());
                        oGeneralData.SetProperty("U_DcrptQRCode", einvDT.Rows[0]["DcrySignedQRCode"].ToString());
                        oGeneralData.SetProperty("U_ErrLogId", einvDT.Rows[0]["error_log_ids"].ToString());
                        oGeneralData.SetProperty("U_Einvreqjson", requrl);
                        oGeneralData.SetProperty("U_GenDate", einvDT.Rows[0]["DocDate"].ToString());
                        oGeneralData.SetProperty("U_BaseNo", einvDT.Rows[0]["DocNo"].ToString());
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNo", einvDT.Rows[0]["EwbNo"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["EwbDt"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["EwbValidTill"].ToString());
                    }
                    else if (TranType == "E-way")
                    {

                        oGeneralData.SetProperty("U_GenDate", einvDT.Rows[0]["docDate"].ToString());
                        oGeneralData.SetProperty("U_BaseNo", einvDT.Rows[0]["docNo"].ToString());
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);
                        oGeneralData.SetProperty("U_Ewayreqjson", requrl);
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNo", einvDT.Rows[0]["ewayBillNo"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["ewayBillDate"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["validUpto"].ToString());
                    }
                    else if (TranType == "E-way IRN")
                    {                       
                        oGeneralData.SetProperty("U_BaseEntry", InvDocEntry);
                        oGeneralData.SetProperty("U_DocObjType", obj);                        
                        oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                        oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                        oGeneralData.SetProperty("U_EwbNo", einvDT.Rows[0]["EwbNo"].ToString());
                        oGeneralData.SetProperty("U_EwbDate", einvDT.Rows[0]["EwbDt"].ToString());
                        oGeneralData.SetProperty("U_EwbValidTill", einvDT.Rows[0]["EwbValidTill"].ToString());
                    }
                    if (TranType == "E-Invoice")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "OINV";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                break;
                        }

                        string lstrquery = @"Update " + tb + @" Set ""U_IRNNo""='" + einvDT.Rows[0]["Irn"].ToString() + "',";
                        lstrquery += @"""U_QRCode""='" + einvDT.Rows[0]["SignedQRCode"].ToString() + "',";
                        lstrquery += @"""U_AckDate""='" + einvDT.Rows[0]["AckDt"].ToString() + "',";
                        lstrquery += @"""U_AckNo""='" + einvDT.Rows[0]["AckNo"].ToString() + "'";                       
                        lstrquery += @"where ""DocEntry""='" + InvDocEntry + "'";
                        objRs.DoQuery(lstrquery);
                        
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                        }
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                        {
                            lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["EwbNo"].ToString() + "',";
                            lstrquery += @"""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbDt"].ToString().Substring(0, 10), "yyyy-MM-dd") + "',";
                            lstrquery += @"""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbValidTill"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                            lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);
                        }
                       
                    }
                    else if (TranType == "E-way")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                        }
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                        {
                            string lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["ewayBillNo"].ToString() + "',";
                            lstrquery += @"""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["ewayBillDate"].ToString().Substring(0, 10), "dd/MM/yyyy") + "',";
                            lstrquery += @"""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["validUpto"].ToString().Substring(0, 10), "dd/MM/yyyy") + "'";
                            lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);
                        }
                    }

                    else if (TranType == "E-way IRN")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                        }
                        if (!string.IsNullOrEmpty(einvDT.Rows[0]["EwbNo"].ToString()))
                        {
                            string lstrquery = @"Update " + tb + @" set ""EWayBillNo""='" + einvDT.Rows[0]["EwbNo"].ToString() + "',";
                            lstrquery += @"""EwbDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbDt"].ToString().Substring(0, 10), "yyyy-MM-dd") + "',";
                            lstrquery += @"""ExpireDate""='" + clsModule.objaddon.objglobalmethods.Getdateformat(einvDT.Rows[0]["EwbValidTill"].ToString().Substring(0, 10), "yyyy-MM-dd") + "'";
                            lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                            objRs.DoQuery(lstrquery);
                        }
                    }
                }
                else if (Type == "Cancel")
                {
                    oGeneralData.SetProperty("U_Flag", einvDT.Rows[0]["flag"].ToString());
                    oGeneralData.SetProperty("U_Remarks", einvDT.Rows[0]["message"].ToString());
                    oGeneralData.SetProperty("U_CanDate", einvDT.Rows[0]["CancelDate"].ToString());
                    oGeneralData.SetProperty("U_ErrLogId", einvDT.Rows[0]["error_log_id"].ToString());

                    if (TranType == "E-Invoice")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "OINV";
                                break;
                            case "CRN":
                                tb = "ORIN";
                                break;
                        }

                        if (einvDT.Rows[0]["flag"].ToString() == "True")
                        {
                            objRs.DoQuery(@"Update " + tb + @" Set ""U_IRNNo""='',""U_QRCode""='',""U_AckDate""='',""U_AckNo""='' where ""DocEntry""='" + InvDocEntry + "'");
                        }
                    }
                    else if (TranType == "E-way")
                    {
                        string tb = "";
                        switch (ObjType)
                        {
                            case "INV":
                                tb = "INV26";
                                break;
                            case "CRN":
                                tb = "RIN26";
                                break;
                        }
                        string lstrquery = @"Update " + tb + @" set ""EWayBillNo""='',";
                        lstrquery += @"""EwbDate""='',";
                        lstrquery += @"""ExpireDate""=''";
                        lstrquery += @"Where ""DocEntry""='" + InvDocEntry + "'";
                        if (einvDT.Rows[0]["flag"].ToString() == "True")
                        {
                            objRs.DoQuery(lstrquery);
                        }
                    }
                }
                objRs = null;
                if (Flag == true)
                {
                    oGeneralService.Update(oGeneralData);
                    return true;
                }
                else
                {
                    oGeneralParams = oGeneralService.Add(oGeneralData);
                    return true;
                }

            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("E_Invoice_Logs: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

        private DataTable Get_API_Response(string JSON, string URL)
        {
            try
            {
                clsModule.objaddon.objglobalmethods.WriteErrorLog(URL);
                clsModule.objaddon.objglobalmethods.WriteErrorLog(JSON);
                
                DataTable datatable = new DataTable();
                HttpWebRequest webRequest;
                webRequest = (HttpWebRequest)WebRequest.Create(URL);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json";               
                byte[] byteArray = Encoding.UTF8.GetBytes(JSON);
                webRequest.ContentLength = byteArray.Length;
                using (Stream requestStream = webRequest.GetRequestStream())
                {
                    requestStream.Write(byteArray, 0, byteArray.Length);
                }
                // Get the response.
                using (WebResponse response = webRequest.GetResponse())
                {
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        StreamReader rdr = new StreamReader(responseStream, Encoding.UTF8);
                        string Json = rdr.ReadToEnd();
                        datatable= clsModule.objaddon.objglobalmethods.Jsontodt(Json);                    
                    }
                }
                return datatable;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}
