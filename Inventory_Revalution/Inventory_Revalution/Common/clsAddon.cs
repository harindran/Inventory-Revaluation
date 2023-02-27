using System;
using System.Linq;
using Inventory_Revalution;
using SAPbouiCOM.Framework;
namespace Common.Common
{
    class clsAddon
    {
        public SAPbouiCOM.Application objapplication;
        public SAPbobsCOM.Company objcompany;
        public clsMenuEvent objmenuevent;
        public clsRightClickEvent objrightclickevent;        
        public clsGlobalMethods objglobalmethods;    
        public bool HANA = true;
        public string[] HWKEY = { "L1653539483", "X1211807750" };
        #region Constructor
        public clsAddon()
        {

        }
        #endregion

        public void Intialize(string[] args)
        {
            try
            {
                Application oapplication;
                if ((args.Length < 1))
                    oapplication = new Application();
                else
                    oapplication = new Application(args[0]);
                objapplication = Application.SBO_Application;
                if (isValidLicense())
                {
                    objapplication.StatusBar.SetText("Establishing Company Connection Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    objcompany = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();

                    Create_DatabaseFields(); // UDF & UDO Creation Part    
                    //Menu(); // Menu Creation Part
                    Create_Objects(); // Object Creation Part

                    objapplication.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(objapplication_AppEvent);
                    objapplication.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(objapplication_MenuEvent);
                    objapplication.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(objapplication_ItemEvent);
                    objapplication.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(ref FormDataEvent);
                    objapplication.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(objapplication_RightClickEvent);

                    objapplication.StatusBar.SetText("Addon Connected Successfully..!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    oapplication.Run();
                }
                else
                {
                    objapplication.StatusBar.SetText("Addon Disconnected due to license mismatch..!!!", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return;
                }
            }          
            catch (Exception ex)
            {
                objapplication.SetStatusBarMessage(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, true);
            }
        }

        public bool isValidLicense()
        {
            try
            {
                if (HANA)
                {
                    try
                    {
                        if (objapplication.Forms.ActiveForm.TypeCount > 0)
                        {
                            for (int i = 0; i <= objapplication.Forms.ActiveForm.TypeCount - 1; i++)
                                objapplication.Forms.ActiveForm.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }

                objapplication.Menus.Item("257").Activate();
                SAPbouiCOM.EditText objedit = (SAPbouiCOM.EditText)objapplication.Forms.ActiveForm.Items.Item("79").Specific;

                string CrrHWKEY = objedit.Value.ToString();
                objapplication.Forms.ActiveForm.Close();

                for (int i = 0; i <= HWKEY.Length - 1; i++)
                {
                    if (HWKEY[i] == CrrHWKEY)
                    {
                        return true;
                    }

                }

                System.Windows.Forms.MessageBox.Show("Installing Add-On failed due to License mismatch");
                return false;
            }
            catch (Exception ex)
            {
                objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            return true;
        }

        public void Create_Objects()
        {
           objmenuevent = new clsMenuEvent();
            objrightclickevent = new clsRightClickEvent();
            objglobalmethods = new clsGlobalMethods();
    
        }

        private void Create_DatabaseFields()
        {
            objapplication.StatusBar.SetText("Creating Database Fields.Please Wait...", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            var objtable = new clsTable();
            objtable.FieldCreation();

        }

        #region Menu Creation Details

        private void Menu()
        {
            int Menucount = 0;
            if (objapplication.Menus.Item("43545").SubMenus.Exists("E-Invoice"))
                return;
            //Menucount = objapplication.Menus.Item("43545").SubMenus.Count;
            //Menucount += 1;
            //CreateMenu("", Menucount, "E-Invoice", SAPbouiCOM.BoMenuType.mt_STRING, "EINV", "43545");
            //Menucount += 1;
            //CreateMenu("", Menucount, "E-Invoice Display", SAPbouiCOM.BoMenuType.mt_STRING, "EINVDIS", "43545");
        }

        private void CreateMenu(string ImagePath, int Position, string DisplayName, SAPbouiCOM.BoMenuType MenuType, string UniqueID, string ParentMenuID)
        {
            try
            {
                SAPbouiCOM.MenuCreationParams oMenuPackage;
                SAPbouiCOM.MenuItem parentmenu;
                parentmenu = objapplication.Menus.Item(ParentMenuID);
                if (parentmenu.SubMenus.Exists(UniqueID.ToString()))
                    return;
                oMenuPackage = (SAPbouiCOM.MenuCreationParams)objapplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);
                oMenuPackage.Image = ImagePath;
                oMenuPackage.Position = Position;
                oMenuPackage.Type = MenuType;
                oMenuPackage.UniqueID = UniqueID;
                oMenuPackage.String = DisplayName;
                parentmenu.SubMenus.AddEx(oMenuPackage);
            }
            catch (Exception ex)
            {
                objapplication.StatusBar.SetText("Menu Already Exists", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_None);
            }
        }

        #endregion

        public bool FormExist(string FormID)
        {
            bool FormExistRet = false;
            try
            {
                FormExistRet = false;
                foreach (SAPbouiCOM.Form uid in clsModule.objaddon.objapplication.Forms)
                {
                    if (uid.TypeEx == FormID)
                    {
                        FormExistRet = true;
                        break;
                    }
                }
                if (FormExistRet)
                {
                    clsModule.objaddon.objapplication.Forms.Item(FormID).Visible = true;
                    clsModule.objaddon.objapplication.Forms.Item(FormID).Select();
                }
            }
            catch (Exception ex)
            {
                clsModule.objaddon.objapplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }

            return FormExistRet;

        }

        #region VIRTUAL FUNCTIONS
        public virtual void Menu_Event(ref SAPbouiCOM.MenuEvent pVal, ref bool BubbleEvent)
        { }

        public virtual void Item_Event(string oFormUID, ref SAPbouiCOM.ItemEvent pVal, ref bool BubbleEvent)
        { }

        public virtual void RightClick_Event(ref SAPbouiCOM.ContextMenuInfo oEventInfo, ref bool BubbleEvent)
        { }

        public virtual void FormData_Event(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, ref bool BubbleEvent)
        { }


        #endregion

        #region ItemEvent

        private void objapplication_ItemEvent(string FormUID, ref SAPbouiCOM.ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                switch (pVal.FormTypeEx)
                {
                 
                    
                }

                if (pVal.BeforeAction)
                {
                    {
                        switch (pVal.EventType)
                        {
                            case SAPbouiCOM.BoEventTypes.et_FORM_CLOSE:
                                {
                                    SAPbouiCOM.BoEventTypes EventEnum;
                                    EventEnum = pVal.EventType;
                                    break;
                                }
                            case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                                {
                                    break;
                                }
                            case SAPbouiCOM.BoEventTypes.et_COMBO_SELECT:
                                {
                                    break;
                                }
                        }
                    }

                }
                else
                {
                    switch (pVal.EventType)
                    {
                        case SAPbouiCOM.BoEventTypes.et_FORM_LOAD:
                            {
                                break;
                            }
                        case SAPbouiCOM.BoEventTypes.et_LOST_FOCUS:
                            {                               
                                break;
                            }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region FormDataEvent

        private void FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
               
            }
            catch (Exception ex)
            {
               throw ex;
            }
        }

        #endregion

        #region MenuEvent
        private void objapplication_MenuEvent(ref SAPbouiCOM.MenuEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;
            if (pVal.BeforeAction == false)
            {
                switch (pVal.MenuUID)
                {
                    //case "EINV":
                    //    Einvoice activeform = new Einvoice();
                    //    activeform.Show();
                    //    break;

                    case "MCV":                       
                        MatCost activeform = new MatCost();
                        SAPbouiCOM.Form objform = clsModule.objaddon.objapplication.Forms.ActiveForm;
                        SAPbouiCOM.Matrix Matrix3 = (SAPbouiCOM.Matrix)objform.Items.Item("41").Specific;
                        int selectrow=Matrix3.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder);
                        activeform.Item = ((SAPbouiCOM.EditText)Matrix3.Columns.Item("6").Cells.Item(selectrow).Specific).Value.ToString();
                        activeform.Whscode = ((SAPbouiCOM.EditText)Matrix3.Columns.Item("4").Cells.Item(selectrow).Specific).Value.ToString();
                        activeform.frmdate = ((SAPbouiCOM.EditText)objform.Items.Item("TFrmDt").Specific).Value;
                        activeform.todate = ((SAPbouiCOM.EditText)objform.Items.Item("TtoDt").Specific).Value;
                        activeform.trantype = "GRPO";
                        activeform.Show();
                        break;
                    case "BCV":
                        MatCost activeform1= new MatCost();
                        SAPbouiCOM.Form objform1 = clsModule.objaddon.objapplication.Forms.ActiveForm;
                        SAPbouiCOM.Matrix Matrix31 = (SAPbouiCOM.Matrix)objform1.Items.Item("41").Specific;
                        int selectrow1 = Matrix31.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder);
                        activeform1.Item = ((SAPbouiCOM.EditText)Matrix31.Columns.Item("6").Cells.Item(selectrow1).Specific).Value.ToString();
                        activeform1.Whscode = ((SAPbouiCOM.EditText)Matrix31.Columns.Item("4").Cells.Item(selectrow1).Specific).Value.ToString();
                        activeform1.frmdate = ((SAPbouiCOM.EditText)objform1.Items.Item("TFrmDt").Specific).Value;
                        activeform1.todate = ((SAPbouiCOM.EditText)objform1.Items.Item("TtoDt").Specific).Value;
                        activeform1.trantype = "BOM";
                        activeform1.Show();
                        break;
                    case "PCV":
                        break;

                }
            }
            else
            {
                switch (pVal.MenuUID)
                {
                    case "MCV":                        
                        break;
                    case "PCV":
                        break;
                }
            }

        }

        #endregion
        
        #region RightClickEvent

        private void objapplication_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;
            try
            {
                objrightclickevent.RightClickEvent(ref eventInfo, ref BubbleEvent);
            }
            catch (Exception ex) { }

        }

        #endregion

        #region AppEvent

        private void objapplication_AppEvent(SAPbouiCOM.BoAppEventTypes EventType)
        {
            switch (EventType)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                    try
                    {
                        System.Windows.Forms.Application.Exit();
                        if (objapplication != null)
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(objapplication);
                        if (objcompany != null)
                        {
                            if (objcompany.Connected)
                                objcompany.Disconnect();
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(objcompany);
                        }
                        GC.Collect();

                    }
                    catch (Exception ex)
                    {
                    }
                    break;

            }
        }

        #endregion

    }


}
