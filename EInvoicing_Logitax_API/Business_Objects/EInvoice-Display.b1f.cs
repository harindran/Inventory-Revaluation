using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using EInvoicing_Logitax_API.Common;
using SAPbouiCOM.Framework;

namespace EInvoicing_Logitax_API.Business_Objects
{
    [FormAttribute("EINVDIS", "Business_Objects/EInvoice-Display.b1f")]
    class EInvoice_Display : UserFormBase
    {
        public EInvoice_Display()
        {
        }
        public static SAPbouiCOM.Form objform;
        private clsGlobalMethods stf = new clsGlobalMethods();
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("EDFrmDt").Specific));
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("frmdt").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("lbToDt").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("EDToDt").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("BtnFetch").Specific));
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.Button0.ClickBefore += new SAPbouiCOM._IButtonEvents_ClickBeforeEventHandler(this.Button0_ClickBefore);
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("GRDet").Specific));
            this.Button1 = ((SAPbouiCOM.Button)(this.GetItem("2").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new LoadAfterHandler(this.Form_LoadAfter);

        }

        private SAPbouiCOM.EditText EditText0;

        private void OnCustomInitialize()
        {
            objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
         
            ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value = DateTime.Today.ToString("yyyyMMdd");
            ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value = DateTime.Today.ToString("yyyyMMdd");
            objform.Items.Item("EDFrmDt").Click();
            ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).Columns.Item("DocEntry").Visible = false;
        }
        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            //throw new System.NotImplementedException();
            try
            {
                objform = clsModule.objaddon.objapplication.Forms.GetForm("EINVDIS", pVal.FormTypeCount - 1);

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Grid Grid0;
        private SAPbouiCOM.Button Button1;

        private void Button0_ClickBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
           // throw new System.NotImplementedException();

        }

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            //throw new System.NotImplementedException();
            DataTable dt = new DataTable();
            string lstrquery = @"SELECT t1.""DocEntry"",""DocNum"",""DocDate"",t2.""CardName"" ,t2.""CardCode"" ,t2.""Phone1"",t1.""DocTotal"" ,""ShipToCode"",t3.""GSTRegnNo""   FROM oinv t1
                               inner Join OCRD t2 ON t2.""CardCode"" = t1.""CardCode""
                                inner JOIN crd1 t3 ON  t3.""Address"" = t1.""ShipToCode""  AND t3.""AdresType"" = 'S'
                                AND t3.""GSTRegnNo"" <> '' ";

            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value)
            {
                case "E-way":
                    lstrquery += @"JOIN inv26 i ON i.""DocEntry"" =t1.""DocEntry"" ";
                    break;
            }

            lstrquery += @" WHERE T1.""DocDate"">='" + ((SAPbouiCOM.EditText)objform.Items.Item("EDFrmDt").Specific).Value + "'";
            lstrquery += @" And t1.""DocDate"" <='" + ((SAPbouiCOM.EditText)objform.Items.Item("EBToDt").Specific).Value + "'";


            switch (((SAPbouiCOM.ComboBox)objform.Items.Item("EBType").Specific).Selected.Value)
            {
                case "E-way":
                    lstrquery += @" WHERE i.""Distance"">='0'";
                    break;
            }
            lstrquery += @"Order by ""DocDate""";

            dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);

            if (dt.Rows.Count > 0)
            {
                ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
                objform.Items.Item("GRDet").Visible = false;
                int i = 0;
                foreach (DataRow Drow in dt.Rows)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Add();
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Checkbox", i, "N");
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Number", i, Drow["DocNum"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("DocEntry", i, Drow["DocEntry"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Doc Date", i, Drow["DocDate"].ToString());
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Customer", i, Drow["CardName"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Mobile", i, Drow["phone1"]);
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Total", i, Drow["DocTotal"]);
                    i++;
                }
                objform.Items.Item("GRDet").Visible = true;
            }
        }

      
    }
}
