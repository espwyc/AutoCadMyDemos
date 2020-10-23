using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using AutoCAD;
using Autodesk.Windows;

namespace AutoCadMyDemo
{
    public partial class MyCommands
    {
        private static RibbonTab myRibbonTab = null;

        [CommandMethod("MyRibbonTab")]
        public void MyRibbonTab()
        {
            // 创建Ribbon Tab页
            if (myRibbonTab == null)
            {
                myRibbonTab = new RibbonTab();
                myRibbonTab.Title = "测试Ribbon页";
                myRibbonTab.Id = "MyRibbonTab";

                //
                RibbonPanel panel1 = new RibbonPanel();
                RibbonPanelSource panel1Src = new RibbonPanelSource();
                panel1Src.Title = "Panel 1";
                panel1.Source = panel1Src;
                myRibbonTab.Panels.Add(panel1);

                RibbonButton rbnBtnLine = NewRibbonBtn("直线");
                rbnBtnLine.CommandParameter = "LINE\n";
                rbnBtnLine.CommandHandler = new RibbonbtnCmdHandle();
                panel1Src.Items.Add(rbnBtnLine);
                RibbonButton rbnBtnCircle = NewRibbonBtn("圆");
                rbnBtnCircle.CommandParameter = "CIRCLE\n";
                rbnBtnCircle.CommandHandler = new RibbonbtnCmdHandle();
                panel1Src.Items.Add(rbnBtnCircle);

                //
                RibbonPanel panel2 = new RibbonPanel();
                RibbonPanelSource panel2Src = new RibbonPanelSource();
                panel2Src.Title = "Panel 2";
                panel2.Source = panel2Src;
                myRibbonTab.Panels.Add(panel2);


                RibbonButton rbnBtnTest = NewRibbonBtn("Open Window");
                rbnBtnTest.CommandParameter = "OPENWINDOW\n";
                rbnBtnTest.CommandHandler = new RibbonbtnCmdHandle();

                panel2Src.Items.Add(rbnBtnTest);
            }

            RibbonControl rc = ComponentManager.Ribbon;
            rc.Tabs.Add(myRibbonTab);
            // 在AutoCAD的Ribbon窗口中显示
            //RibbonControl ribCntrl = RibbonServices.RibbonPaletteSet.RibbonControl;
            //ribCntrl.Tabs.Add(myRibbonTab);
        }

        class RibbonbtnCmdHandle : System.Windows.Input.ICommand
        {
            public event EventHandler CanExecuteChanged;
            public bool CanExecute(object parameter)
            {
                return true;
            }
            public void Execute(object parameter)
            {
                if (parameter is RibbonButton)
                {
                    RibbonButton rb = parameter as RibbonButton;
                    Application.DocumentManager.MdiActiveDocument.SendStringToExecute(
                        (string)rb.CommandParameter, true, false, false);
                }
            }
        }

        private static RibbonButton NewRibbonBtn(string text)
        {
            RibbonButton button = new RibbonButton();
            button.Text = text;
            button.ShowText = true;
            return button;
        }


    }
}
