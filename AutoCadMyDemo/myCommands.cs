// (C) Copyright 2020 by  
//
using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using AutoCAD;
using Autodesk.Windows;


// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(AutoCadMyDemo.MyCommands))]

namespace AutoCadMyDemo
{

    // This class is instantiated by AutoCAD for each document when
    // a command is called by the user the first time in the context
    // of a given document. In other words, non static data in this class
    // is implicitly per-document!
    public partial class MyCommands
    {
        // The CommandMethod attribute can be applied to any public  member 
        // function of any public class.
        // The function should take no arguments and return nothing.
        // If the method is an intance member then the enclosing class is 
        // intantiated for each document. If the member is a static member then
        // the enclosing class is NOT intantiated.
        //
        // NOTE: CommandMethod has overloads where you can provide helpid and
        // context menu.

        //自定义控件

        //自定义事件
        public delegate void OpenWindowEventHandler(object sender, string e);
        public event OpenWindowEventHandler OpenWindowRequest;


        //通用资源
        private AcadApplication app = (AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
        private Document doc = Application.DocumentManager.MdiActiveDocument;

        //自定义功能组件
        private void WriteMessage(String s)
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            doc.Editor.WriteMessage(s);
        }

        //命令
        // Modal Command with localized name
        [CommandMethod("MyGroup", "MyCommand", "MyCommandLocal", CommandFlags.Modal)]
        public void MyCommand() // This method can have any name
        {
            // Put your command code here
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed;
            if (doc != null)
            {
                ed = doc.Editor;
                ed.WriteMessage("Hello, this is your first command.");

            }
        }

        [CommandMethod("OpenWindow")]
        public void OpenWindow()
        {
            //OpenWindowRequest(this,null);
            Window1 w = new Window1();
            // 获取当前数据库，启动事务
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 以读模式打开块表
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
                // 以读模式打开块表记录模型空间
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId acObjId in acBlkTblRec)
                {
                    //acDoc.Editor.WriteMessage("\n" + acObjId.ObjectClass.DxfName);
                    //w.tb.Text += "\n" + acObjId.ObjectClass.DxfName;
                    w.sc.Content += "\n" + acObjId.ObjectClass.DxfName;
                }
            }

            w.ShowDialog();
            //w.Show();
            //if (w.Flag == 1)
            //{
            //    MyHook();
            //    w.ShowDialog();
            //}
            //if (w.Flag == 2)
            //{
            //    w.Area = Area;
            //    w.UpdateArea();
            //    w.ShowDialog();
            //}

        }

       

        [CommandMethod("Hello")]
        public void Method()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
            {
                Editor ed = doc.Editor;
                ed.WriteMessage("Hello");
            }
        }

        [CommandMethod("ListEntities")]
        public static void ListEntities()
        {
            // 获取当前数据库，启动事务
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 以读模式打开块表
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                OpenMode.ForRead) as BlockTable;
                // 以读模式打开块表记录模型空间
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                OpenMode.ForRead) as BlockTableRecord;
                int nCnt = 0;
                acDoc.Editor.WriteMessage("\nModel space objects: ");
                // 遍历模型空间里的每个对象，并显示找到的对象的类型
                foreach (ObjectId acObjId in acBlkTblRec)
                {
                    acDoc.Editor.WriteMessage("\n" + acObjId.ObjectClass.DxfName);
                    nCnt = nCnt + 1;
                }
                // 如果没发现对象则显示提示信息
                if (nCnt == 0)
                {
                    acDoc.Editor.WriteMessage("\n No objects found");
                }
                // 关闭事务
            }
        }



        // Modal Command with pickfirst selection
        //[CommandMethod("MyGroup", "MyPickFirst", "MyPickFirstLocal", CommandFlags.Modal | CommandFlags.UsePickSet)]
        //public void MyPickFirst() // This method can have any name
        //{
        //    PromptSelectionResult result = Application.DocumentManager.MdiActiveDocument.Editor.GetSelection();
        //    if (result.Status == PromptStatus.OK)
        //    {
        //        // There are selected entities
        //        // Put your command using pickfirst set code here
        //    }
        //    else
        //    {
        //        // There are no selected entities
        //        // Put your command code here
        //    }
        //}

        //// Application Session Command with localized name
        //[CommandMethod("MyGroup", "MySessionCmd", "MySessionCmdLocal", CommandFlags.Modal | CommandFlags.Session)]
        //public void MySessionCmd() // This method can have any name
        //{
        //    // Put your command code here
        //}

        // LispFunction is similar to CommandMethod but it creates a lisp 
        // callable function. Many return types are supported not just string
        // or integer.
        //[LispFunction("MyLispFunction", "MyLispFunctionLocal")]
        //public int MyLispFunction(ResultBuffer args) // This method can have any name
        //{
        //    // Put your command code here

        //    // Return a value to the AutoCAD Lisp Interpreter
        //    return 1;
        //}
    }

}
