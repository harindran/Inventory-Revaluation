using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using SAPbouiCOM.Framework;
using Common.Common;
namespace Inventory_Revalution
{
    [FormAttribute("Inventory_Revalution.MatCost", "MatCost.b1f")]
    public class MatCost : UserFormBase
    {
        public string Item;
        public string Whscode;
        public string frmdate;
        public string todate;
        public string trantype;
        public static SAPbouiCOM.Form objform;
        public MatCost()
        {
        }

        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Grid0 = ((SAPbouiCOM.Grid)(this.GetItem("GRDet").Specific));
            this.OnCustomInitialize();

        }

        /// <summary>
        /// Initialize form event. Called by framework before form creation.
        /// </summary>
        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new SAPbouiCOM.Framework.FormBase.LoadAfterHandler(this.Form_LoadAfter);
            this.ActivateAfter += new SAPbouiCOM.Framework.FormBase.ActivateAfterHandler(this.Form_ActivateAfter);
            this.VisibleAfter += new VisibleAfterHandler(this.Form_VisibleAfter);

        }

        private SAPbouiCOM.Grid Grid0;

        private void OnCustomInitialize()
        {
        
        }

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {


            //objform = clsModule.objaddon.objapplication.Forms.GetForm("Inventory_Revalution.MatCost", pVal.FormTypeCount - 1);
            objform = clsModule.objaddon.objapplication.Forms.GetForm("Inventory_Revalution.MatCost", pVal.FormTypeCount);


        }

        private void Form_ActivateAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {           



        }

        private void Form_VisibleAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                DataTable dt = new DataTable();
                string lstrquery = "";
                switch (trantype)
                {
                    case "BOM":
                        lstrquery = "SELECT 1 AS \"DocNum\",o.\"UpdateDate\" AS \"DocDate\" ,o.\"Code\" AS \"ItemCode\", o.\"Code\" AS \"ItemName\" ,1 AS \"Quantity\",";
                        lstrquery += " i.\"AvgPrice\" as \"Price\" ,i.\"AvgPrice\" AS \"Total\" FROM OITT o LEFT JOIN Itt1 L ON o.\"Code\" = L.\"Father\" ";
                        lstrquery += " LEFT JOIN oitm i ON i.\"ItemCode\" =o.\"Code\"";
                        lstrquery += " where  o.\"Code\" = '" + Item + "'";
                        break;
                    case "GRPO":
                        lstrquery = "select HD.\"DocNum\" ,HD.\"DocDate\",Line.\"ItemCode\",IT.\"ItemName\",LINE.\"Quantity\",";
                        lstrquery += " LINE.\"Price\",LINE.\"Quantity\" * LINE.\"Price\" as Total,LINE.\"Quantity\" * LINE.\"Price\"/ LINE.\"Quantity\"";
                        lstrquery += "   FROM OIGN HD";
                        lstrquery += " INNER JOIN IGN1 Line ON Line.\"DocEntry\" = HD.\"DocEntry\"";
                        lstrquery += " INNER JOIN OITM It  ON It .\"ItemCode\" = Line.\"ItemCode\"";
                        lstrquery += " where HD.\"DocDate\" >='" + frmdate + "' and  HD.\"DocDate\" <='" + todate + "' ";
                        lstrquery += " and  Line.\"ItemCode\" = '" + Item + "'";

                        if (Whscode != "")
                        {
                            lstrquery += "and Line.\"WhsCode\"='" + Whscode + "'";
                        }

                        lstrquery += "Order by HD.\"DocDate\"";
                        break;
                }


                dt = clsModule.objaddon.objglobalmethods.GetmultipleValue(lstrquery);

                if (dt.Rows.Count > 0)
                {
                    ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Clear();
                    objform.Items.Item("GRDet").Visible = false;
                    int i = 0;
                    foreach (DataRow Drow in dt.Rows)
                    {
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.Rows.Add();
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("GRN No", i, Drow["DocNum"]);
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("GRN Date", i, Drow["DocDate"]);
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Item Code", i, Drow["ItemCode"].ToString());
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Description", i, Drow["ItemName"]);
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Qty", i, Drow["Quantity"]);
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Unit Price", i, Drow["Price"]);
                        ((SAPbouiCOM.Grid)(objform.Items.Item("GRDet").Specific)).DataTable.SetValue("Total", i, Drow["Total"]);
                        i++;
                    }
                    objform.Items.Item("GRDet").Visible = true;
                }
            }
            catch (Exception ex)
            {

                //throw;
            }

        }
    }
}