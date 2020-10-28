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
        [CommandMethod("AppUpdate")]
        public void AppUpdateScreen()
        {
            WriteMessage("APPUpdate");
            app.Update();
        }

        [CommandMethod("MainWindowClose")]
        public void MainWindowTestClose()
        {
            WriteMessage("MainWindowTestClose");
            Application.MainWindow.Close();
        }

    }
}
