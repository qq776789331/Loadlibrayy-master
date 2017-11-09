using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DriverExploits;
using Loadlibrayy.Injection;
using Loadlibrayy.Logger;

namespace Loadlibrayy.Forms
{
    public partial class NewInjectionForm : Form
    {

        public NewInjectionForm()
        {
            InitializeComponent();
        }

        private void InjectButton_Click(object sender, EventArgs e)
        {
            Process[] pros = Process.GetProcessesByName("TslGame");
            Console.WriteLine(pros.Length);
            // SANITY CHECKS
            if (pros.Length == 0)
            {
                Log.ShowError("请先运行游戏,并且匹配成功进入游戏等待倒计时大厅", "错误");
                return;
            }
            // LOAD EXPLOITABLE DRIVER 
            bool driverLoaded = false;

            if (!(driverLoaded = ElevateHandle.Driver.Load()))
            {
                Log.ShowError("CPUZ141.sys failed to load", "lol fuck");
                return;
            }

            ElevateHandle.UpdateDynamicData(); // UPDATE KERNEL OFFSETS
            ElevateHandle.Attach();            // ATTACH TO CURRENT PROCESS
            ElevateHandle.Elevate((ulong)pros[0].Handle, 0x1fffff);


            InjectionOptions options = new InjectionOptions()
            {
                ElevateHandle = true,
                EraseHeaders = true,
                CreateLoaderReference = false,
                LoaderImagePath = Environment.CurrentDirectory + "\\ts.dll"
            };

            ExecutionType executionType = ExecutionType.HijackThread;

            IInjectionMethod injectionMethod = new ManualMapInjection(pros[0], executionType, options);
            //IInjectionMethod injectionMethod = new LoadLibraryInjection(g_SelectedProcess, executionType, options);
            injectionMethod.InjectImage(Environment.CurrentDirectory + "\\ts.dll");

            if (driverLoaded)
                ElevateHandle.Driver.Unload();
        }

        private void NewInjectionForm_Load(object sender, EventArgs e)
        {

        }
    }
}
