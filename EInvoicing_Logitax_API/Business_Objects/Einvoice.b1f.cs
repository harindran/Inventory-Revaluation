using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EInvoicing_Logitax_API.Common;
using SAPbouiCOM.Framework;
using System.Data;

namespace EInvoicing_Logitax_API.Business_Objects
{
    [FormAttribute("EINV", "Business_Objects/Einvoice.b1f")]
    class Einvoice : UserFormBase
    {
        public Einvoice()
        {
        }
        public static SAPbouiCOM.Form objform;
        private clsGlobalMethods stf = new clsGlobalMethods();
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.ComboBox0 = ((SAPbouiCOM.ComboBox)(this.GetItem("EBType").Specific));
            this.ComboBox0.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.ComboBox0_ComboSelectAfter);
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("LbType").Specific));
            this.ComboBox1 = ((SAPbouiCOM.ComboBox)(this.GetItem("EBTrnType").Specific));
            this.ComboBox1.ComboSelectAfter += new SAPbouiCOM._IComboBoxEvents_ComboSelectAfterEventHandler(this.ComboBox1_ComboSelectAfter);
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("LBTrnType").Specific));
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("GRDet").Specific));
            this.Grid0.DoubleClickAfter += new SAPbouiCOM._IGridEvents_DoubleClickAfterEventHandler(this.Grid0_DoubleClickAfter);
            this.Grid0.LinkPressedAfter += new SAPbouiCOM._IGridEvents_LinkPressedAfterEventHandler(this.Grid0_LinkPressedAfter);
            this.Grid0.LinkPressedBefore += new SAPbouiCOM._IGridEvents_LinkPressedBeforeEventHandler(this.Grid0_LinkPressedBefore);
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("EDFrmDt").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("LBFrmDt").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("EBToDt").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("LBToDt").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("Item_11").Specific));
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.Button2 = ((SAPbouiCOM.Button)(this.GetItem("BGenarate").Specific));
            this.Button2.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button2_ClickAfter);
            this.Button2.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button2_ClickBefore);
            this.Button3 = ((SAPbouiCOM.Button)(this.GetItem("BDis").Specific));
            this.Button3.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button3_ClickBefore);
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }



        private void OnCustomInitialize()
        {
            objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
            ((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            ((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value = DateTime.Today.ToString("yyyyMMdd");
            ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value = DateTime.Today.ToString("yyyyMMdd");
            objform.Items.Item("EDFrmDt").Click();
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry").Visible = true;
        }

        private SAPbouiCOM.ComboBox ComboBox0;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.ComboBox ComboBox1;
        private SAPbouiCOM.StaticText StaticText1;

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                objform = clsModule.objaddon.objapplication.Forms.GetForm("EINV", pVal.FormTypeCount);

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private SAPbouiCOM.Grid Grid0;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.Button Button0;

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

        }

        private SAPbouiCOM.Button Button1;

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            DataTable dt = new DataTable();


            string lstrquery = @"SELECT t1.""DocEntry"",t1.""DocNum"",t1.""DocDate"",t2.""CardName"" ,t2.""CardCode"" ,t2.""Phone1"",t1.""DocTotal"" ,""ShipToCode"",t3.""GSTRegnNo"",";
            if (clsModule.objaddon.HANA)
            {
                lstrquery += @"  IFNULL(o.""U_Remarks"",'') U_Remarks FROM ";
            }
            else
            {
                lstrquery += @" isnull(o.""U_Remarks"",'') U_Remarks FROM ";
            }

            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
            {
                case "INV":
                    lstrquery += "oinv t1 ";
                    break;
                case "CRN":
                    lstrquery += "ORIN t1 ";
                    break;
            }


            lstrquery += @"inner Join OCRD t2 ON t2.""CardCode"" = t1.""CardCode""
                                inner JOIN crd1 t3 ON  t3.""Address"" = t1.""ShipToCode""  AND t3.""AdresType"" = 'S'
                                AND t3.""GSTRegnNo"" <> '' ";
            lstrquery += @" LEFT JOIN ""@ATPL_EINV"" o ON o.""U_BaseEntry"" =t1.""DocEntry"" ";

            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value)
            {

                case "E-way":
                    switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
                    {
                        case "INV":
                            lstrquery += @"JOIN inv26 i ON i.""DocEntry"" =t1.""DocEntry"" ";
                            break;
                        case "CRN":
                            lstrquery += @"JOIN RIN26 i ON i.""DocEntry"" =t1.""DocEntry"" ";
                            break;
                    }
                    break;

            }

            lstrquery += @" WHERE T1.""DocDate"">='" + ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value + "'";
            lstrquery += @" And t1.""DocDate"" <='" + ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value + "'";


            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value)
            {
                case "E-way":
                    lstrquery += @" AND i.""Distance"">'0'";
                    if (clsModule.objaddon.HANA)
                    {
                        lstrquery += @" AND IFNULL(i.""EWayBillNo"",'')=''";
                    }
                    else
                    {
                        lstrquery += @"And isnull(i.EWayBillNo,'')=''";
                    }
                    break;
                case "E-Invoice":
                    if (clsModule.objaddon.HANA)
                    {
                        lstrquery += @" AND IFNULL(t1.""U_IRNNo"",'')=''";
                    }
                    else
                    {
                        lstrquery += @"And isnull(t1.U_IRNNo,'')=''";
                    }
                    break;

            }
            lstrquery += @"Order by ""DocDate""";

            dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
            objform.Items.Item("GRDet").LinkTo = "DocEntry";
            if (dt.Rows.Count > 0)
            {
                objform.Items.Item("GRDet").Visible = false;

                int i = 0;
                foreach (DataRow Drow in dt.Rows)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Add();
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "N");
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Date", i, stf.Getdateformat(Drow["DocDate"].ToString()));
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Customer", i, Drow["CardName"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Mobile", i, Drow["phone1"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Total", i, Drow["DocTotal"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Remarks", i, Drow["U_Remarks"]);

                    i++;
                }
                objform.Items.Item("GRDet").Visible = true;
                objform.Items.Item("BGenarate").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Checkbox").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Remarks").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill No").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill Date").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("EWB Expiration Date").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("IRN NO").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK Date").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK No").Visible = false;
            }
        }

        private SAPbouiCOM.Button Button2;

        private void Button2_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            bool checkvalue = false; ;
            for (int i = 0; i < Grid0.Rows.Count; i++)
            {
                string ss2 = ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("Checkbox").Cells.Item(i).Value.ToString();
                if (ss2 == "Y")
                {
                    checkvalue = true;
                    break;
                }
            }
            if (!checkvalue)
            {
                Application.SBO_Application.SetStatusBarMessage("Please Select Checkbox !!!!", SAPbouiCOM.BoMessageTime.bmt_Short, true);
                BubbleEvent = false;
            }
        }

        private void Button2_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            //throw new System.NotImplementedException();

            if (Grid0.Rows.Count > 0)
            {

                for (int i = 0; i < Grid0.Rows.Count; i++)
                {
                    string lstrdocentry = ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("DocEntry").Cells.Item(i).Value.ToString();
                    string lstrcheckbox = ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("Checkbox").Cells.Item(i).Value.ToString();
                    string TransType = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value;
                    string Type = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value;
                    DataTable dt = new DataTable();
                    if (lstrcheckbox == "Y")
                    {
                        switch (Type)
                        {
                            case "E-Invoice":
                                clsModule.objaddon.objInvoice.Generate_Cancel_IRN(ClsARInvoice.EinvoiceMethod.CreateIRN, lstrdocentry, TransType, Type, ref dt);
                                break;
                            case "E-way":
                                clsModule.objaddon.objInvoice.Generate_Cancel_IRN(ClsARInvoice.EinvoiceMethod.CreateEway, lstrdocentry, TransType, Type, ref dt);
                                break;
                        }
                    }


                }
            }

        }

        private void ComboBox0_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();

        }

        private SAPbouiCOM.Button Button3;

        private void Button3_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            string ObjType = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value;
            string tb = "";
            DataTable dt = new DataTable();
            SAPbouiCOM.EditTextColumn oColumns;
            string lstrquery = @"SELECT t1.""DocEntry"",""DocNum"",""DocDate"",t2.""CardName"" ,t2.""CardCode"" ,t2.""Phone1"",t1.""DocTotal"" ,""ShipToCode"",t3.""GSTRegnNo"" ,t1.""U_IRNNo"" ,t1.""U_AckDate"" ,t1.""U_AckNo"" ,i.""EWayBillNo"" ,i.""EwbDate"" ,i.""ExpireDate""   FROM ";
            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBTrnType").Specific).Selected.Value)
            {
                case "INV":
                    lstrquery += "oinv t1 ";
                    oColumns = (SAPbouiCOM.EditTextColumn)((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "13";
                    break;
                case "CRN":

                    oColumns = (SAPbouiCOM.EditTextColumn)((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry");
                    oColumns.LinkedObjectType = "14";
                    lstrquery += "ORIN t1 ";
                    break;
            }


            lstrquery += @"inner Join OCRD t2 ON t2.""CardCode"" = t1.""CardCode""
                                inner JOIN crd1 t3 ON  t3.""Address"" = t1.""ShipToCode""  AND t3.""AdresType"" = 'S'
                                AND t3.""GSTRegnNo"" <> '' ";

            switch (ObjType)
            {
                case "INV":
                    tb = "INV26";
                    break;
                case "CRN":
                    tb = "RIN26";
                    break;
            }

            lstrquery += @"Left JOIN " + tb + @" i ON i.""DocEntry"" =t1.""DocEntry"" ";

            lstrquery += @" WHERE T1.""DocDate"">='" + ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value + "'";
            lstrquery += @" And t1.""DocDate"" <='" + ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value + "'";

            string TransType = ((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value;
            switch (TransType)
            {
                case "E-way":
                    lstrquery += @" AND i.""Distance"">'0'";
                    if (clsModule.objaddon.HANA)
                    {
                        lstrquery += @" AND IFNULL(i.""EWayBillNo"",'')<>''";
                    }
                    else
                    {
                        lstrquery += @"And isnull(i.EWayBillNo,'')<>''";
                    }
                    break;
                case "E-Invoice":
                    if (clsModule.objaddon.HANA)
                    {
                        lstrquery += @" AND IFNULL(t1.""U_IRNNo"",'')<>''";
                    }
                    else
                    {
                        lstrquery += @"And isnull(t1.U_IRNNo,'')<>''";
                    }
                    break;

            }
            lstrquery += @"Order by ""DocDate""";

            dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("IRN NO").Visible = TransType == "E-Invoice";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK Date").Visible = TransType == "E-Invoice";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("ACK No").Visible = TransType == "E-Invoice";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill No").Visible = TransType == "E-way";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("E-Way Bill Date").Visible = TransType == "E-way";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("EWB Expiration Date").Visible = TransType == "E-way";
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
            objform.Items.Item("GRDet").LinkTo = "DocEntry";
            if (dt.Rows.Count > 0)
            {
                objform.Items.Item("GRDet").Visible = false;
                int i = 0;
                foreach (DataRow Drow in dt.Rows)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Add();
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "N");
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Date", i, stf.Getdateformat(Drow["DocDate"].ToString()));
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Customer", i, Drow["CardName"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Mobile", i, Drow["phone1"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Total", i, Drow["DocTotal"]);

                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("E-Way Bill No", i, Drow["EWayBillNo"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("E-Way Bill Date", i, Drow["ExpireDate"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("EWB Expiration Date", i, Drow["DocTotal"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("IRN NO", i, Drow["U_IRNNo"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("ACK Date", i, Drow["U_AckDate"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("ACK No", i, Drow["U_AckNo"]);

                    i++;
                }
                objform.Items.Item("GRDet").Visible = true;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Checkbox").Visible = false;
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("Remarks").Visible = false;
                objform.Items.Item("BGenarate").Visible = false;
            }

        }

        private void ComboBox1_ComboSelectAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
        }

        private void Grid0_LinkPressedBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            // sboObject= ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Columns.Item("DocEntry").Cells.Item(pVal.Row).Value.ToString();

            //SAPbouiCOM.EditText sboEdit  = sboOrderForm.Items.Item("8").Specific

        }

        private void Grid0_LinkPressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {


        }

        private void Grid0_DoubleClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (pVal.Row == -1)
            {
                for (int i = 0; i < Grid0.Rows.Count; i++)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "Y");
                }
                   
            }
        }
    }
}
