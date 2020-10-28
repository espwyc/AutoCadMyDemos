using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using AutoCAD;
using Autodesk.Windows;

using AutoCadMyDemo;

namespace AutoCadMyDemo
{
    public partial class MyCommands
    {
        //[CommandMethod("DrawDeathlyHallows")]
        //public void DrawDeathlyHallows()
        //{

        //    Point3d Pt1 = new Point3d(0, 0, 0);
        //    double h = Math.Sqrt(1000 * 1000 - 500 * 500);

        //    Point3d Pt2 = new Point3d(500, h, 0);
        //    Point3d Pt3 = new Point3d(1000, 0, 0);
        //    Point3d Pt4 = new Point3d(500, 0, 0);

        //    Line l1 = new Line(Pt1, Pt2);
        //    Line l2 = new Line(Pt2, Pt3);
        //    Line l3 = new Line(Pt1, Pt3);
        //    Line l4 = new Line(Pt2, Pt4);

        //    Point3d Pc = new Point3d(500, h / 3, 0);
        //    Circle c = new Circle(Pc, Vector3d.ZAxis, h / 3);

        //    AddEntity(l1);
        //    AddEntity(l2);
        //    AddEntity(l3);
        //    AddEntity(l4);
        //    AddEntity(c);
        //}

        //private void AddEntity(Entity e)
        //{
        //    Database db = HostApplicationServices.WorkingDatabase;
        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        //(3-1)以读方式打开块表..
        //        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
        //        //(3-2)以写方式打开模型空间块表记录
        //        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
        //        //(3-3)将图形对象的信息添加到块表记录中,并返回ObjectId对象.
        //        btr.AppendEntity(e);
        //        //(3-4)把对象添加到事务处理中.
        //        trans.AddNewlyCreatedDBObject(e, true);
        //        //(3-5)提交事务处理
        //        trans.Commit();
        //    }
        //}



        //private double Angle(double a, double b, double c)
        //{
        //    double cosA = (b * b + c * c - a * a) / (2 * b * c);
        //    double rs = Math.Acos(cosA);
        //    return rs;
        //}

        [CommandMethod("Myintersect")]
        public void Myintersect()
        {
            Database db = HostApplicationServices.WorkingDatabase;
            Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

            Point3d p1 = new Point3d(100,0 , 0);
            Point3d p2 = new Point3d(0, 0, 0);
            Circle c1 = new Circle(p1,Vector3d.ZAxis,60);
            Circle c2 = new Circle(p2,Vector3d.ZAxis,60);

            //c2.EndPoint = new Point3d(60, 0, 0);
            //c2.StartPoint = new Point3d(-60, 0, 0);
            Entity[] es = { c1, c2 };
            AddEntities(es);

            Point3dCollection cc = new Point3dCollection();
            c1.IntersectWith(c2, Intersect.OnBothOperands, cc,new IntPtr(0), new IntPtr(0));
            foreach (var p in cc)
            {
                ed.WriteMessage("交点:{0}\n", p);
            }
        }

        [CommandMethod("IntersectionTest")]
        public void IntersectionTest()
        {
            Editor m_ed = Application.DocumentManager.MdiActiveDocument.Editor;
            Database m_db = HostApplicationServices.WorkingDatabase;
            PromptEntityOptions m_peo = new PromptEntityOptions("\n请选择第一条曲线:");
            PromptEntityResult m_per = m_ed.GetEntity(m_peo);
            if (m_per.Status != PromptStatus.OK) { return; }
            ObjectId m_objid1 = m_per.ObjectId;

            m_peo = new PromptEntityOptions("\n请选择第二条曲线:");
            m_per = m_ed.GetEntity(m_peo);
            if (m_per.Status != PromptStatus.OK) { return; }
            ObjectId m_objid2 = m_per.ObjectId;

            using (Transaction m_tr = m_db.TransactionManager.StartTransaction())
            {
                Curve m_cur1 = (Curve)m_tr.GetObject(m_objid1, OpenMode.ForRead);
                Curve m_cur2 = (Curve)m_tr.GetObject(m_objid2, OpenMode.ForRead);

                Point3dCollection m_ints = new Point3dCollection();
                m_cur1.IntersectWith(m_cur2, Intersect.OnBothOperands, new Plane(), m_ints, 0, 0); //得出的所有交点在c1曲线上
                foreach (Point3d m_pt in m_ints)
                {
                    m_ed.WriteMessage("\n第一条曲线与第二条曲线交点:{0}", m_pt);
                }

                m_ed.WriteMessage("\n===");
                m_ints.Clear();
                m_cur2.IntersectWith(m_cur1, Intersect.OnBothOperands, new Plane(), m_ints, 0, 0); //得出的所有交点在c2曲线上
                foreach (Point3d m_pt in m_ints)
                {
                    m_ed.WriteMessage("\n第二条曲线与第条曲线一交点:{0}", m_pt);
                }
                m_tr.Commit();
            }
        }

        [CommandMethod("CreateCompositeRegions")]
        public static void CreateCompositeRegions()
        {
            
            // 获取当前文档和数据库
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;
            // 启动事务
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // 以读模式打开 Block 表
                BlockTable acBlkTbl;
                acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId,
                OpenMode.ForRead) as BlockTable;
                // 以写模式打开 Block 表记录 Model 空间
                BlockTableRecord acBlkTblRec;
                acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace],
                OpenMode.ForWrite) as BlockTableRecord;
                // 在内存建两个圆
                Circle acCirc1 = new Circle();
                acCirc1.Center = new Point3d(4, 4, 0);
                acCirc1.Radius = 2;
                Circle acCirc2 = new Circle();
                acCirc2.Center = new Point3d(4, 4, 0);
                acCirc2.Radius = 1;
                // 将圆添加到对象数组
                DBObjectCollection acDBObjColl = new DBObjectCollection();
                acDBObjColl.Add(acCirc1);
                acDBObjColl.Add(acCirc2);

                // 基于每个闭环计算面域
                DBObjectCollection myRegionColl = new DBObjectCollection();
                myRegionColl = Region.CreateFromCurves(acDBObjColl);
                Region acRegion1 = myRegionColl[0] as Region;
                Region acRegion2 = myRegionColl[1] as Region;
                // 从面域 2 减去面域 1
                if (acRegion1.Area > acRegion2.Area)
                {
                    // 从较大面域中减去较小面域
                    acRegion1.BooleanOperation(BooleanOperationType.BoolSubtract,
                   acRegion2);
                    acRegion2.Dispose();
                    // 将最终的面域添加到数据库
                    acBlkTblRec.AppendEntity(acRegion1);
                    acTrans.AddNewlyCreatedDBObject(acRegion1, true);
                }
                else
                {
                    // 从较大面域中减去较小面域
                    acRegion2.BooleanOperation(BooleanOperationType.BoolSubtract,
                   acRegion1);
                    acRegion1.Dispose();
                    // 将最终的面域添加到数据库
                    acBlkTblRec.AppendEntity(acRegion2);
                    acTrans.AddNewlyCreatedDBObject(acRegion2, true);
                }
                // 销毁内存中的两个圆对象
                acCirc1.Dispose();
                acCirc2.Dispose();
                // 将新对象保存到数据库
                acTrans.Commit();
            }
        }
    }
}
