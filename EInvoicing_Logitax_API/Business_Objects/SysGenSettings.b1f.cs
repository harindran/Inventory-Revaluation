using System;
using SAPbouiCOM.Framework;
using EInvoicing_Logitax_API.Common;
using SAPbobsCOM;
using System.Data;  

namespace EInvoicing_Logitax_API.Business_Objects
{
    
    [FormAttribute("138", "Business_Objects/SysGenSettings.b1f")]
    class SysGenSettings : SystemFormBase
    {
        private string FormName = "138";
        private string strSQL;              
        public static SAPbouiCOM.Form oForm;

        #region "DESIGN PART"
        public override void OnInitializeComponent()
        {
            this.Folder0 = ((SAPbouiCOM.Folder)(this.GetItem("feinvcfg").Specific));
            this.Folder0.PressedAfter += new SAPbouiCOM._IFolderEvents_PressedAfterEventHandler(this.Folder0_PressedAfter);
            this.Matrix0 = ((SAPbouiCOM.Matrix)(this.GetItem("mtxconfig").Specific));
            this.Matrix0.ValidateAfter += new SAPbouiCOM._IMatrixEvents_ValidateAfterEventHandler(this.Matrix0_ValidateAfter);
            this.StaticText0 = ((SAPbouiCOM.StaticText)(this.GetItem("lcntcod").Specific));
            this.EditText0 = ((SAPbouiCOM.EditText)(this.GetItem("tcntcod").Specific));
            this.StaticText1 = ((SAPbouiCOM.StaticText)(this.GetItem("lusrcod").Specific));
            this.EditText1 = ((SAPbouiCOM.EditText)(this.GetItem("tusrcod").Specific));
            this.StaticText2 = ((SAPbouiCOM.StaticText)(this.GetItem("Item_6").Specific));
            this.EditText2 = ((SAPbouiCOM.EditText)(this.GetItem("tpswd").Specific));
            this.Button0 = ((SAPbouiCOM.Button)(this.GetItem("1").Specific));
            this.Button0.ClickAfter += new SAPbouiCOM._IButtonEvents_ClickAfterEventHandler(this.Button0_ClickAfter);
            this.Folder1 = ((SAPbouiCOM.Folder)(this.GetItem("129").Specific));
            this.StaticText4 = ((SAPbouiCOM.StaticText)(this.GetItem("luburl").Specific));
            this.EditText4 = ((SAPbouiCOM.EditText)(this.GetItem("tuburl").Specific));
            this.StaticText5 = ((SAPbouiCOM.StaticText)(this.GetItem("llburl").Specific));
            this.EditText5 = ((SAPbouiCOM.EditText)(this.GetItem("tlburl").Specific));
            this.OptionBtn0 = ((SAPbouiCOM.OptionBtn)(this.GetItem("ouat").Specific));
            this.OptionBtn1 = ((SAPbouiCOM.OptionBtn)(this.GetItem("olive").Specific));
            this.StaticText3 = ((SAPbouiCOM.StaticText)(this.GetItem("LHSN").Specific));
            this.EditText3 = ((SAPbouiCOM.EditText)(this.GetItem("thsnL").Specific));
            this.OnCustomInitialize();

        }

        public override void OnInitializeFormEvents()
        {
            this.LoadAfter += new SAPbouiCOM.Framework.FormBase.LoadAfterHandler(this.Form_LoadAfter);

        }
        #region Fields        
        private SAPbouiCOM.Folder Folder0;
        private SAPbouiCOM.Matrix Matrix0;
        private SAPbouiCOM.StaticText StaticText0;
        private SAPbouiCOM.EditText EditText0;
        private SAPbouiCOM.StaticText StaticText1;
        private SAPbouiCOM.EditText EditText1;
        private SAPbouiCOM.StaticText StaticText2;
        private SAPbouiCOM.EditText EditText2;
        private SAPbouiCOM.Button Button0;
        private SAPbouiCOM.Folder Folder1;
        private SAPbouiCOM.StaticText StaticText4;
        private SAPbouiCOM.EditText EditText4;
        private SAPbouiCOM.StaticText StaticText5;
        private SAPbouiCOM.EditText EditText5;
        private SAPbouiCOM.OptionBtn OptionBtn0;
        private SAPbouiCOM.OptionBtn OptionBtn1;
        #endregion
        #endregion

        private void OnCustomInitialize()
        {
            Folder0.GroupWith("129");            
            clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "crurl", "#");
            OptionBtn0.GroupWith("olive");
            OptionBtn0.Item.Height = OptionBtn0.Item.Height + 2;
            OptionBtn0.Item.Width = OptionBtn0.Item.Width + 20;
            OptionBtn1.Item.Height = OptionBtn1.Item.Height + 2;
            OptionBtn1.Item.Width = OptionBtn1.Item.Width + 20;
            Folder1.Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
            Matrix0.Columns.Item("urltype").ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;
            Matrix0.Columns.Item("type").ExpandType = SAPbouiCOM.BoExpandType.et_DescriptionOnly;            
        }


        private void Form_LoadAfter(SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                oForm = clsModule.objaddon.objapplication.Forms.GetForm(FormName, pVal.FormTypeCount);              
              
                    strSQL = @"Select T0.""U_ClientCode"",T0.""U_UserCode"",T0.""U_Password"",T0.""U_Live"",T0.""U_UATUrl"",T0.""U_LIVEUrl"",
                              T1.""LineId"",T1.""U_URLType"",T1.""U_Type"",T1.""U_URL"",T0.""U_HSNL""";
                    strSQL += @" from ""@ATEICFG"" T0 join ""@ATEICFG1"" T1 on T0.""Code""=T1.""Code"" where T0.""Code""='01'";
                
                
               DataTable dt = new DataTable();
               dt= clsModule.objaddon.objglobalmethods.GetmultipleValue(strSQL);
                
                if (dt.Rows.Count>0)
                {
                    Matrix0.Clear();
                    foreach (DataRow Drow in dt.Rows)
                    {
                        oForm.DataSources.UserDataSources.Item("UD_ClnCod").Value = Drow["U_ClientCode"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_UsrCod").Value = Drow["U_UserCode"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_Pass").Value = Drow["U_Password"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_UbUrl").Value = Drow["U_UATUrl"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_LbUrl").Value = Drow["U_LIVEUrl"].ToString();
                        oForm.DataSources.UserDataSources.Item("UD_HSNL").Value = Drow["U_HSNL"].ToString();
                        Matrix0.AddRow();
                        ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("urltype").Cells.Item(Matrix0.VisualRowCount).Specific).Select(Drow["U_URLType"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                        ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("type").Cells.Item(Matrix0.VisualRowCount).Specific).Select(Drow["U_Type"].ToString(), SAPbouiCOM.BoSearchKey.psk_ByValue);
                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("#").Cells.Item(Matrix0.VisualRowCount).Specific).String = Drow["LineId"].ToString();
                        ((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(Matrix0.VisualRowCount).Specific).String = Drow["U_URL"].ToString();                       
                    }                    
                }
                Matrix0.AutoResizeColumns();
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
            }

            catch (Exception ex) 
            {

                //throw;
            }
        }

        private void Folder0_PressedAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                oForm = clsModule.objaddon.objapplication.Forms.ActiveForm;                
                oForm.PaneLevel = 26;

                if (!(OptionBtn0.Selected == true | OptionBtn1.Selected == true))
                {
                    strSQL = clsModule.objaddon.objglobalmethods.getSingleValue("Select \"U_Live\" from \"@ATEICFG\" where \"Code\"='01'");
                    if (strSQL == "Y")
                    {
                        OptionBtn1.Item.Click();
                    }
                    else
                    {
                        OptionBtn0.Selected = true;
                    }
                }               
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;               
                oForm.Select();
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        private void Button0_ClickAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_UPDATE_MODE) { return; }
            if (E_Invoice_Config())
            {
                clsModule.objaddon.objapplication.StatusBar.SetText("Data Saved Successfully...", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        private void Matrix0_ValidateAfter(object sboObject, SAPbouiCOM.SBOItemEventArg pVal)
        {
            try
            {
                switch (pVal.ColUID)
                {
                    case "url":
                        clsModule.objaddon.objglobalmethods.Matrix_Addrow(Matrix0, "url", "#");
                        break;
                }

            }
            catch (Exception)
            {
                throw;
            }

        }
        
        private bool E_Invoice_Config()
        {
            try
            {
                bool Flag = false;
                //string live;
                GeneralService oGeneralService;
                GeneralData oGeneralData;
                GeneralDataParams oGeneralParams;
                GeneralDataCollection oGeneralDataCollection;
                GeneralData oChild;

                oGeneralService = clsModule.objaddon.objcompany.GetCompanyService().GetGeneralService("ATCFG");
                oGeneralData = (GeneralData)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralData);
                oGeneralParams = (GeneralDataParams)oGeneralService.GetDataInterface(GeneralServiceDataInterfaces.gsGeneralDataParams);
                oGeneralDataCollection = oGeneralData.Child("ATEICFG1");
                try
                {
                    oGeneralParams.SetProperty("Code", "01");
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    Flag = true;
                }
                catch (Exception ex)
                {
                    Flag = false;
                }

                //string ss = GetEditBoxValue("tcntcod");

                oGeneralData.SetProperty("Code", "01");
                oGeneralData.SetProperty("Name", "01");
                oGeneralData.SetProperty("U_ClientCode", oForm.DataSources.UserDataSources.Item("UD_ClnCod").Value);                
                oGeneralData.SetProperty("U_UserCode", oForm.DataSources.UserDataSources.Item("UD_UsrCod").Value);
                oGeneralData.SetProperty("U_Password", oForm.DataSources.UserDataSources.Item("UD_Pass").Value);              
                oGeneralData.SetProperty("U_Live", Convert.ToString(((OptionBtn0.Selected == true) ? 'N' : 'Y')));
                oGeneralData.SetProperty("U_UATUrl", oForm.DataSources.UserDataSources.Item("UD_UbUrl").Value);
                oGeneralData.SetProperty("U_LIVEUrl", oForm.DataSources.UserDataSources.Item("UD_LbUrl").Value);
                if (oForm.DataSources.UserDataSources.Item("UD_HSNL").Value=="")
                {
                    oForm.DataSources.UserDataSources.Item("UD_HSNL").Value = "4";
                }
                oGeneralData.SetProperty("U_HSNL", oForm.DataSources.UserDataSources.Item("UD_HSNL").Value);

                oChild = oGeneralDataCollection.Add();

                for (int i = 1; i <= Matrix0.VisualRowCount; i++)
                {
                    if (((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(i).Specific).String != "")
                    {
                        if (i > oGeneralData.Child("ATEICFG1").Count)
                        {
                            oGeneralData.Child("ATEICFG1").Add();
                        }

                        oGeneralData.Child("ATEICFG1").Item(i - 1).SetProperty("U_URLType", ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("urltype").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("ATEICFG1").Item(i - 1).SetProperty("U_Type", ((SAPbouiCOM.ComboBox)Matrix0.Columns.Item("type").Cells.Item(i).Specific).Selected.Value);
                        oGeneralData.Child("ATEICFG1").Item(i - 1).SetProperty("U_URL", ((SAPbouiCOM.EditText)Matrix0.Columns.Item("url").Cells.Item(i).Specific).String);                  
                    }
                }
              

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
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }

        public string GetEditBoxValue(string uniqueid)
        {
            return  ((SAPbouiCOM.EditText)oForm.Items.Item(uniqueid).Specific).Value.ToString(); 
        }

        private SAPbouiCOM.StaticText StaticText3;
        private SAPbouiCOM.EditText EditText3;
    }
}
 