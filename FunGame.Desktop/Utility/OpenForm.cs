using Milimoe.FunGame.Core.Api.Utility;
using Milimoe.FunGame.Core.Library.Constant;
using Milimoe.FunGame.Desktop.UI;

namespace Milimoe.FunGame.Desktop.Utility
{
    public class OpenForm
    {
        public static void SingleForm(FormType type, OpenFormType opentype = OpenFormType.General)
        {
            Form form = new();
            switch (type)
            {
                case FormType.Register:
                    form = new Register();
                    break;
                case FormType.Login:
                    form = new Login();
                    break;
                case FormType.Inventory:
                    form = new InventoryUI();
                    break;
                case FormType.RoomSetting:
                    form = new RoomSetting();
                    break;
                case FormType.Store:
                    form = new StoreUI();
                    break;
                case FormType.UserCenter:
                    form = new UserCenter();
                    break;
                case FormType.Main:
                    form = new Main();
                    break;
                default:
                    break;
            }
            if (!Singleton.IsExist(form))
            {
                NewSingleForm(form, opentype);
            }
            else
            {
                throw new Exception("目标窗口可能已处于打开状态。");
            }
        }

        private static void NewSingleForm(Form form, OpenFormType opentype)
        {
            if (Singleton.Add(form))
            {
                if (opentype == OpenFormType.Dialog) form.ShowDialog();
                else form.Show();
            }
            else throw new Exception("无法打开指定窗口。");
        }
    }
}
