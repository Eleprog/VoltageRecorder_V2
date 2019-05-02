using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRW.Model;
using VRW.Presenter;

namespace VRW
{
    static class Program
    {
        public static Settings settings;
        static MainForm mainForm;
        //TODO: private
        public static MainPresenter mainPresenter;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            settings = Settings.Load();
                mainForm = new MainForm();
            mainPresenter = new MainPresenter(mainForm);

            Application.Run(mainForm);
        }
    }
}
