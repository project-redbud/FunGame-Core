using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Net.NetworkInformation;
using Milimoe.FunGame.Core.Entity;
using Milimoe.FunGame.Desktop.Others;
using Milimoe.FunGame.Desktop.UI;
using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Api.Factory;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.Library.Component;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Milimoe.FunGame.Desktop.Utility
{
    public class OpenForm
    {
        public static void SingleForm(FormType type, OpenFormType opentype = OpenFormType.General)
        {
            string formtype = "";
            switch(type)
            {
                case FormType.Register:
                    formtype = FormSet.Register;
                    break;
                case FormType.Login:
                    formtype = FormSet.Login;
                    break;
                case FormType.Inventory:
                    formtype = FormSet.Inventory;
                    break;
                case FormType.RoomSetting:
                    formtype = FormSet.RoomSetting;
                    break;
                case FormType.Store:
                    formtype = FormSet.Store;
                    break;
                case FormType.UserCenter:
                    formtype = FormSet.UserCenter;
                    break;
                case FormType.Main:
                    formtype = FormSet.Main;
                    break;
                default:
                    break;
            }
            if (!Singleton.IsExist(formtype))
            {
                NewSingleForm(type, opentype);
            }
            else
            {
                OpenSingleForm(formtype, opentype);
            }
        }

        private static void NewSingleForm(FormType type, OpenFormType opentype)
        {
            System.Windows.Forms.Form form = new();
            switch (type)
            {
                case FormType.Register:
                    form = new Register(); 
                    break;
                case FormType.Login:
                    form = new Login();
                    break;
                case FormType.Inventory:
                    break;
                case FormType.RoomSetting:
                    break;
                case FormType.Store:
                    break;
                case FormType.UserCenter:
                    break;
                case FormType.Main:
                    form = new Main();
                    break;
                default:
                    break;
            }
            if (Singleton.Add(form))
            {
                if (opentype == OpenFormType.Dialog) form.ShowDialog();
                else form.Show();
            }
            else throw new Exception("无法打开指定窗口。");
        }
    
        private static void OpenSingleForm(string key, OpenFormType opentype)
        {
            System.Windows.Forms.Form form = new();
            object? obj = Singleton.Get(key);
            if (obj != null)
            {
                switch (key)
                {
                    case FormSet.Register:
                        form = (Register)obj;
                        break;
                    case FormSet.Login:
                        form = (Login)obj;
                        break;
                    case FormSet.Inventory:
                        break;
                    case FormSet.RoomSetting:
                        break;
                    case FormSet.Store:
                        break;
                    case FormSet.UserCenter:
                        break;
                    case FormSet.Main:
                        form = (Main)obj;
                        break;
                    default:
                        break;
                }
                if (opentype == OpenFormType.Dialog) form.ShowDialog();
                else form.Show();
            }
            else throw new Exception("无法打开指定窗口。");
        }
    }
}
