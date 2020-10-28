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
        [CommandMethod("MyMenu")]
        public void MyMenu()
        {
            //获取CAD应用程序
            
            AcadPopupMenus menus = app.MenuGroups.Item(0).Menus;
            foreach (AcadPopupMenu menu in menus)
            {
                if (menu.Name == "我的Com菜单")
                {
                    menu.InsertInMenuBar(app.MenuBar.Count + 1);
                    return;
                }
            }

            AcadPopupMenu pmParnet = menus.Add("我的Com菜单");  //添加根菜单


            //多级
            AcadPopupMenu pm = pmParnet.AddSubMenu(pmParnet.Count + 1, "一级菜单");
            AcadPopupMenuItem pmi0 = pm.AddMenuItem(pm.Count + 1, "打开窗口  ", "OpenWindow\n");  //第一个参数是在菜单项中的位置（第几项），第二个参数是显示的名称，第三个参数是点击之后执行的命令
            AcadPopupMenuItem pmi1 = pm.AddMenuItem(pm.Count + 1, "输出实体 ", "ListEntities\n");

            //单级

            AcadPopupMenuItem pmi2 = pmParnet.AddMenuItem(pmParnet.Count + 1, "退出", "EXIT\n");
            AcadPopupMenuItem pmi3 = pmParnet.AddMenuItem(pmParnet.Count + 1, "删除我的菜单", "DeleteMenu\n");

            //将创建的菜单加入到CAD的菜单中
            pmParnet.InsertInMenuBar(app.MenuBar.Count + 1);
        }

        [CommandMethod("DeleteMenu")]
        public void DeleteMenu()
        {
            //AcadApplication app = (AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
            foreach (AcadPopupMenu menu in app.MenuGroups.Item(0).Menus)
            {
                if (menu.Name == "我的Com菜单")
                    menu.RemoveFromMenuBar();

            }

        }
    }
}
