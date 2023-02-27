using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SAPbouiCOM.Framework;
using Common.Common;
using System.Data;

namespace Inventory_Revalution
{
    [FormAttribute("70001", "Inv_Revaluation.b1f")]
    class Inv_Revaluation : SystemFormBase
    {
        public Inv_Revaluation()
        {
        }
        public static SAPbouiCOM.Form objform;
        private clsGlobalMethods stf = new clsGlobalMethods();
        /// <summary>
        /// Initialize components. Called by framework after form created.
        /// </summary>
        public override void OnInitializeComponent()
        {
            this.Matrix3 = ((SAPbouiCOM.Matrix)(this.GetItem("41").Specific));
            this.Matrix3.LostFocusAfter += new SAPbouiCOM._IMatrixEvents_LostFocusAfterEventHandler(this.Matrix3_LostFocusAfter);
            this.Matrix3.ChooseFromListAfter += new SAPbouiCOM._IMatrixEvents_ChooseFromListAfterEventHandler(this.Matrix3_ChooseFromListAfter);
            this.Matrix3.ChooseFromListBefore += new SAPbouiCOM._IMatrixEvents_ChooseFromListBeforeEventHandler(this.Matrix3_ChooseFromListBefore);
            this.ComboBox0 = ((SAPbouiCOM.ComboBox)(this.GetItem("CcalType").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("L_CalTyp").Specific));
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


        }
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText3;

        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                objform = clsModule.objaddon.objapplication.Forms.GetForm("70001", pVal.FormTypeCount);                
                ((SAPbouiCOM.ComboBox)objform.Items.Item("CcalType").Specific).Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private SAPbouiCOM.Matrix Matrix3;

        private void ChooseFromList_Condition(string CFLID, string Alias, string CondVal, string Query)
        {
            try
            {
                SAPbouiCOM.ChooseFromList oCFL = objform.ChooseFromLists.Item(CFLID);
                SAPbouiCOM.Conditions oConds;
                SAPbouiCOM.Condition oCond;
                DataTable dataTabe = new DataTable();
                var oEmptyConds = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(oEmptyConds);
                oConds = oCFL.GetConditions();

                oCond = oConds.Add();
                oCond.Alias = Alias;
                oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCond.CondVal = CondVal;// "Y";
                if (Query != "")
                {
                    dataTabe= stf.GetmultipleValue(Query);
                    if (dataTabe.Rows.Count > 0)
                    {
                        for (int i = 0; i < dataTabe.Rows.Count; i++)
                        {
                            oCond.Relationship =  SAPbouiCOM.BoConditionRelationship.cr_OR;
                            oCond = oConds.Add();
                            oCond.Alias = Alias;
                            oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                            oCond.CondVal = dataTabe.Rows[i][0].ToString();                                                     
                        }
                    }                 
                }


                oCFL.SetConditions(oConds);
            }
            catch (Exception ex)
            {
                //clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }

        private void Matrix3_ChooseFromListBefore(object sboObject, SAPbouiCOM.SBOItemEventArg pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            string frmdate = ((SAPbouiCOM.EditText)objform.Items.Item("TFrmDt").Specific).Value;
            string todate = ((SAPbouiCOM.EditText)objform.Items.Item("TtoDt").Specific).Value;
            string type = ((SAPbouiCOM.ComboBox)objform.Items.Item("CcalType").Specific).Selected.Value;

            try
            {
                if ((frmdate == "" || todate == "")&& type!="BOM")
                {
                    clsModule.objaddon.objapplication.SetStatusBarMessage("Please Select From Date and  To Date", SAPbouiCOM.BoMessageTime.bmt_Medium, true);
                    BubbleEvent = false;
                }
                string lstrquery= "SELECT \"Father\" FROM \"ITT1\" i  GROUP BY \"Father\"";
                ChooseFromList_Condition("4", "ItemCode", "Y", lstrquery);
            }
            catch (Exception ex)
            {

                clsModule.objaddon.objapplication.SetStatusBarMessage(ex.Message + ex.StackTrace, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }


        }

        private void Matrix3_ChooseFromListAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            string Item = "";
            string Whscode = "";
            try
            {
                SAPbouiCOM.ISBOChooseFromListEventArg pCFL;
                pCFL = (SAPbouiCOM.ISBOChooseFromListEventArg)pVal;
                if (pCFL.SelectedObjects == null) return;
                if (!(pVal.ItemUID == "41" && pVal.InnerEvent == true))
                {
                    return;
                }
                switch (pVal.ColUID)
                {
                    case "6":
                        Item = Convert.ToString(pCFL.SelectedObjects.Columns.Item("ItemCode").Cells.Item(0).Value);
                        break;
                    case "4":
                        Item = ((SAPbouiCOM.EditText)Matrix3.Columns.Item("6").Cells.Item(pVal.Row).Specific).Value.ToString();
                        Whscode = Convert.ToString(pCFL.SelectedObjects.Columns.Item("WhsCode").Cells.Item(0).Value);
                        break;
                    default:
                        return;
                }
                
                LoadData(Item, Whscode, pVal);
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.SetStatusBarMessage(ex.Message + ex.StackTrace, SAPbouiCOM.BoMessageTime.bmt_Medium, true);
            }

        }

        public void LoadData(string Item, string whscode, SAPbouiCOM.SBOItemEventArg pVal)
        {
            string frmdate = ((SAPbouiCOM.EditText)objform.Items.Item("TFrmDt").Specific).Value;
            string todate = ((SAPbouiCOM.EditText)objform.Items.Item("TtoDt").Specific).Value;
            string lstrmatsql;
            string lstrprodsql;
            string lstrpBOMsql;

            lstrmatsql = " SELECT  sum(amt)/sum(Quantity) from( SELECT p.\"Quantity\" AS Quantity,";
            lstrmatsql += " p.\"Price\",p.\"Quantity\" * P.\"Price\",p.\"Quantity\" * P.\"Price\" AS amt  FROM OIGN";
            lstrmatsql += " o2 LEFT JOIN IGN1 p ON p.\"DocEntry\" = o2.\"DocEntry\"";
            lstrmatsql += " WHERE p.\"ItemCode\" = '" + Item + "'  and p.\"DocDate\" >= '" + frmdate + "' AND p.\"DocDate\" <= '" + todate + "'";

            if (whscode != "")
            {
                lstrmatsql += "and p.\"WhsCode\"='" + whscode + "'";
            }
            lstrmatsql += "  ) AS T1";

            string matcost = stf.getSingleValue(lstrmatsql);            
            lstrprodsql = " SELECT TOP 1 \"U_AllCost\"  FROM \"@COST_LINE\" cl";
            lstrprodsql += " WHERE \"Code\" =  '" + Item + "' and \"U_EffDate\" >= '" + frmdate + "' AND \"U_EffDate\" <= '" + todate + "' ORDER BY \"U_EffDate\"";
            string prodcost = stf.getSingleValue(lstrprodsql);

            lstrpBOMsql = " SELECT  sum(\"AvgPrice\")  FROM \"ITT1\" o  LEFT JOIN oitm i ON i.\"ItemCode\" =o.\"Code\"  ";
            lstrpBOMsql += " WHERE o.\"Father\" =  '" + Item + "'";
            string BOMcost = stf.getSingleValue(lstrpBOMsql);


            Matrix3.SetCellWithoutValidation(pVal.Row, "U_IRMatCost", matcost);
            Matrix3.SetCellWithoutValidation(pVal.Row, "U_IRProCost", prodcost);
            Matrix3.SetCellWithoutValidation(pVal.Row, "U_IRBOMCost", BOMcost);
        }

        private void Matrix3_LostFocusAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            string Item = "";
            string Whscode = "";
            decimal total = 0;
            if (!(pVal.ItemUID == "41" && pVal.InnerEvent == true))
            {
                return;
            }

            switch (pVal.ColUID)
            {
                case "6":
                case "4":
                    break;
                default:
                    return;
            }
            Item = ((SAPbouiCOM.EditText)Matrix3.Columns.Item("6").Cells.Item(pVal.Row).Specific).Value.ToString();
            if (Item == "") return;
            Whscode = ((SAPbouiCOM.EditText)Matrix3.Columns.Item("4").Cells.Item(pVal.Row).Specific).Value.ToString();
            
            LoadData(Item, Whscode, pVal);

            decimal prodcost = Convert.ToDecimal(((SAPbouiCOM.EditText)Matrix3.Columns.Item("U_IRMatCost").Cells.Item(pVal.Row).Specific).Value.ToString());
            decimal matcost = Convert.ToDecimal(((SAPbouiCOM.EditText)Matrix3.Columns.Item("U_IRProCost").Cells.Item(pVal.Row).Specific).Value.ToString());
            decimal BOmcost = Convert.ToDecimal(((SAPbouiCOM.EditText)Matrix3.Columns.Item("U_IRBOMCost").Cells.Item(pVal.Row).Specific).Value.ToString());
         switch (((SAPbouiCOM.ComboBox)objform.Items.Item("CcalType").Specific).Selected.Value)
            {
                case "BOM":
                    total = prodcost + BOmcost;
                    break;
                case "GRPO":
                    total = prodcost + matcost;
                    break;
            }

            SAPbouiCOM.Column oColumn = Matrix3.Columns.Item("2");
            bool isEditable = oColumn.Editable;
            if(isEditable==true)
            {                             
                SAPbouiCOM.EditText oCell = (SAPbouiCOM.EditText)Matrix3.Columns.Item("2").Cells.Item(pVal.Row).Specific;
                try
                {
                    if (oCell.Active == true)
                    {
                        oCell.Value = Convert.ToString(total);
                    }
                }
                catch (Exception)
                {

                   
                }                                              
            }           
        }

        private SAPbouiCOM.ComboBox ComboBox0;
        private SAPbouiCOM.StaticText StaticText2;
    }
}
