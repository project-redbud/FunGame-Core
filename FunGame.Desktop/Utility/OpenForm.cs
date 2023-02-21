using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Core.Library.Exception;
using Milimoe.FunGame.Desktop.Library;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Utility
{
    public class OpenForm
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">窗体类型</param>
        /// <param name="opentype">打开类型</param>
        /// <param name="objs">构造参数</param>
        /// <exception cref="FormHasBeenOpenedException">目标窗口可能已处于打开状态</exception>
        public static void SingleForm(FormType type, OpenFormType opentype = OpenFormType.General, params object[]? objs)
        {
            try
            {
                Form form = new();
                bool IsExist = false;
                switch (type)
                {
                    case FormType.Register:
                        form = new Register();
                        IsExist = RunTime.Register != null;
                        RunTime.Register = (Register)form;
                        break;
                    case FormType.Login:
                        form = new Login();
                        IsExist = RunTime.Login != null;
                        RunTime.Login = (Login)form;
                        break;
                    case FormType.Inventory:
                        form = new InventoryUI();
                        IsExist = RunTime.Inventory != null;
                        RunTime.Inventory = (InventoryUI)form;
                        break;
                    case FormType.RoomSetting:
                        form = new RoomSetting();
                        IsExist = RunTime.RoomSetting != null;
                        RunTime.RoomSetting = (RoomSetting)form;
                        break;
                    case FormType.Store:
                        form = new StoreUI();
                        IsExist = RunTime.Store != null;
                        RunTime.Store = (StoreUI)form;
                        break;
                    case FormType.UserCenter:
                        form = new UserCenter();
                        IsExist = RunTime.UserCenter != null;
                        RunTime.UserCenter = (UserCenter)form;
                        break;
                    case FormType.Main:
                        form = new Main();
                        IsExist = RunTime.Main != null;
                        RunTime.Main = (Main)form;
                        break;
                    default:
                        break;
                }
                if (Singleton.IsExist(form) || IsExist)
                {
                    throw new FormHasBeenOpenedException();
                }
                else
                {
                    NewSingleForm(form, opentype);
                }
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }

        private static void NewSingleForm(Form form, OpenFormType opentype)
        {
            try
            {
                if (Singleton.Add(form))
                {
                    if (opentype == OpenFormType.Dialog) form.ShowDialog();
                    else form.Show();
                }
                else throw new FormCanNotOpenException();
            }
            catch (Exception e)
            {
                RunTime.WritelnSystemInfo(e.GetErrorInfo());
            }
        }
    }
}
