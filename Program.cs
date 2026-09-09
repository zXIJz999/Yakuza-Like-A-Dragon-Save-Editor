using DevExpress.LookAndFeel;
using DevExpress.Skins;

namespace YakuzaSaveEditor;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        SkinManager.EnableFormSkins();
        SkinManager.EnableMdiFormSkins();
        UserLookAndFeel.Default.SetSkinStyle("Office 2019 Black");
        Application.Run(new MainForm());
    }
}
