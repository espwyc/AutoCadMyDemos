using System;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;

using AutoCAD;
using Autodesk.Windows;
using System.Collections.Generic;

namespace AutoCadMyDemo
{
    public partial class MyCommands
    {
        [CommandMethod("MyHook")]
        public void MyHook()
        {
            Database db = HostApplicationServices.WorkingDatabase;

            List<Entity> entityList = new List<Entity>();
            List<Entity> centerMarkList = new List<Entity>();
            List<Entity> MarkList = new List<Entity>();



            Point3d pt1 = new Point3d(0, 0, 0);
            Point3d pt2 = new Point3d(5, -3, 0);
            Point3d pt3 = new Point3d(0, 70, 0);
            Point3d pt4 = new Point3d(-18, 46, 0);

            //已知圆心的圆
            Circle c1 = new Circle(pt1, Vector3d.ZAxis, 20);
            Circle c2 = new Circle(pt2, Vector3d.ZAxis, 45);
            Circle c3 = new Circle(pt3, Vector3d.ZAxis, 10);
            Circle c4 = new Circle(pt3, Vector3d.ZAxis, 20);
            Circle c5 = new Circle(pt4, Vector3d.ZAxis, 8);

            

            //已知两条辅助线

            Line l1 = new Line(new Point3d(5, -3 - 45, 0), new Point3d(-60, -3 - 45, 0));
            l1.ColorIndex = 2;
            Line l2 = new Line(new Point3d(5, -3 - 45 + 64, 0), new Point3d(-60, -3 - 45 + 64, 0));
            l2.ColorIndex = 2;

            //圆心线
            Line lc1x = new Line(new Point3d(-30, 0, 0), new Point3d(30, 0, 0));
            Line lc1y = new Line(new Point3d(0, -30, 0), new Point3d(0, 30, 0));
            Line lc2x = new Line(new Point3d(-30+5, 0-3, 0), new Point3d(30+5, 0-3, 0));
            Line lc2y = new Line(new Point3d(-30+5, 0-3, 0), new Point3d(30+5, 0-3, 0));
            Line lc3x = new Line(new Point3d(-30, 0+70, 0), new Point3d(30, 0+70, 0));
            Line lc3y = new Line(new Point3d(-30, 0+70, 0), new Point3d(30, 0+70, 0));


            //需要求解的圆-->弧

            //第一个切圆-->裁剪弧
            Circle fc1 = new Circle(pt4, Vector3d.ZAxis, 8 + 18);
            Circle fc2 = new Circle(pt3, Vector3d.ZAxis, 20 + 18);
            Point3dCollection pc = new Point3dCollection();
            fc1.IntersectWith(fc2, Intersect.OnBothOperands, pc, new IntPtr(0), new IntPtr(0));
            fc1.Dispose();
            fc2.Dispose();
            foreach (Point3d p in pc)
            {
                if (p.X < c4.Center.X)
                {
                    double sa = new Line(p, pt4).Angle;
                    double sb = new Line(p, pt3).Angle;
                    Arc a = new Arc(p, 18, sa, sb);
                    entityList.Add(a);
                    //第一条弧
                    break;
                }

            }

            //第二个切圆-->裁剪弧
            Circle fc3 = new Circle(pt2, Vector3d.ZAxis, 45 + 86);
            Circle fc4 = new Circle(pt3, Vector3d.ZAxis, 20 + 86);
            pc.Clear();
            fc3.IntersectWith(fc4, Intersect.OnBothOperands, pc, new IntPtr(0), new IntPtr(0));
            fc3.Dispose();
            fc4.Dispose();
            foreach (Point3d p in pc)
            {
                if (p.X > c2.Center.X)
                {
                    double sa = new Line(p, pt3).Angle;
                    double sb = new Line(p, pt2).Angle;
                    Arc a = new Arc(p, 86, sa, sb);
                    entityList.Add(a);
                    //第二条弧
                    break;
                }
            }

            //第三个切圆-->裁剪弧
            Circle fc5 = new Circle(pt1, Vector3d.ZAxis, 20 + 9.7144);
            Circle fc6 = new Circle(pt2, Vector3d.ZAxis, 45 - 9.7144);
            pc.Clear();
            fc5.IntersectWith(fc6, Intersect.OnBothOperands, pc, new IntPtr(0), new IntPtr(0));
            fc5.Dispose();
            fc6.Dispose();
            foreach (Point3d p in pc)
            {
                if (p.X < c1.Center.X && p.Y < (-3 - 45 + 64))
                {
                    double sa = new Line(p, pt1).Angle;
                    double sb = new Line(pt2, p).Angle;
                    Arc a = new Arc(p, 9.7144, sa, sb);
                    entityList.Add(a);
                    //第三条弧
                    break;
                }
            }

            //线
            Line fl3 = new Line(new Point3d(0, -30, 0), new Point3d(0, 100, 0));
            fl3.TransformBy(Matrix3d.Rotation((Math.PI * 15 / 180), doc.Editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis, fl3.EndPoint));
            Point3d fp1 = fl3.GetClosestPointTo(c1.Center, true);
            Line l3 = new Line(c1.Center, fp1);
            pc.Clear();
            l3.IntersectWith(c1, Intersect.ExtendThis, pc, new IntPtr(0), new IntPtr(0));
            Point3d fp2 = l3.StartPoint;
            foreach (Point3d p in pc)
            {
                if (p.X > c1.Center.X)
                {
                    fp2 = p;
                    break;
                }
            }

            l3.Extend(50);
            l3.TransformBy(Matrix3d.Rotation((Math.PI * 90 / 180), doc.Editor.CurrentUserCoordinateSystem.CoordinateSystem3d.Zaxis, fp2));

            //l3.GetOffsetCurvesGivenPlaneNormal(Vector3d.XAxis, 18);
            Vector3d ve = new Vector3d(fp2.X, fp2.Y, fp2.Z);
            ve = ve.GetNormal();
            //DBObjectCollection lc= l3.GetOffsetCurvesGivenPlaneNormal(ve, 14);
            DBObjectCollection lc = l3.GetOffsetCurves(14);


            //第四个切圆-->弧
            Circle fc7 = new Circle(pt4, Vector3d.ZAxis, 22);
            Point3d sc1 = new Point3d();
            foreach (Entity l in lc)
            {
                Point3dCollection fc = new Point3dCollection();
                fc7.IntersectWith(l, Intersect.ExtendBoth, fc, new IntPtr(0), new IntPtr(0));
                foreach (Point3d p in fc)
                {
                    if(p.Y<pt4.Y)
                    {
                        //Circle tmpc = new Circle(p, Vector3d.ZAxis, 14);
                        Arc a = new Arc(p, 14, new Line(pt1, fp1).Angle, new Line(p, pt4).Angle);
                        entityList.Add(a);
                        //第四条弧
                        break;
                    }
                }
            }

            //公切线修剪
            l3.StartPoint = fp2;
            l3.EndPoint = ((Arc)(entityList[entityList.Count - 1])).StartPoint;

            //修剪c5圆-->弧
            Arc a1 = new Arc(c5.Center, c5.Radius, new Line(c5.Center, ((Arc)(entityList[0])).StartPoint).Angle, new Line(c5.Center, ((Arc)(entityList[3])).EndPoint).Angle);
            entityList.Add(a1);//第五条弧

            //修剪C2号圆-->弧
            Arc a2 = new Arc(c2.Center, c2.Radius, new Line(c2.Center, ((Arc)(entityList[2])).EndPoint).Angle,new Line(c2.Center, ((Arc)(entityList[1])).EndPoint).Angle);
            entityList.Add(a2);//第6条弧

            //修剪C4号圆-->弧
            Arc a3 = new Arc(c4.Center, c4.Radius, new Line(c4.Center, ((Arc)(entityList[1])).StartPoint).Angle, new Line(c4.Center, ((Arc)(entityList[0])).EndPoint).Angle);
            entityList.Add(a3);//第7条弧

            //修剪c1号圆-->弧
            Arc a4 = new Arc(c1.Center, c1.Radius, new Line(c1.Center, ((Arc)(entityList[2])).StartPoint).Angle, new Line(c1.Center, l3.StartPoint).Angle);
            entityList.Add(a4);//第8条弧

            //画十字线
            Point3d[] points = { pt1, pt2, pt3 };
            foreach (Point3d p in points)
            {
                Line tmpl1 = new Line(new Point3d(p.X - 30, p.Y, 0),new Point3d(p.X+30,p.Y,0));
                Line tmpl2 = new Line(new Point3d(p.X , p.Y-30, 0),new Point3d(p.X,p.Y+30,0));
                //tmpl1.Linetype = "CENTER2";
                //tmpl2.Linetype = "CENTER2";
                tmpl1.ColorIndex = 4;
                tmpl2.ColorIndex = 4;
                centerMarkList.Add(tmpl1);
                centerMarkList.Add(tmpl2);
            }


            Entity[] es = { c3,l1,l2,l3};
            //AddEntities(es);
            entityList.AddRange(es);
            AddEntities(entityList.ToArray());

            AddEntities(centerMarkList.ToArray());
        }


        //private LinetypeTable GetLinetypeTable()
        //{
        //    Database db = HostApplicationServices.WorkingDatabase;
        //    using (Transaction trans = db.TransactionManager.StartTransaction())
        //    {
        //        LinetypeTable ltb = trans.GetObject(db.LinetypeTableId, OpenMode.ForRead) as LinetypeTable;
        //        //if(ltb.Has)
        //    }
        //}

        private void AddEntities(Entity[] es)
        {
            Database db = HostApplicationServices.WorkingDatabase;
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                //(3-1)以读方式打开块表..
                BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                //(3-2)以写方式打开模型空间块表记录
                BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
                //(3-3)将图形对象的信息添加到块表记录中,并返回ObjectId对象.
                //btr.AppendEntity(e);
                ////(3-4)把对象添加到事务处理中.
                //trans.AddNewlyCreatedDBObject(e, true);
                foreach (var e in es)
                {
                    btr.AppendEntity(e);
                    trans.AddNewlyCreatedDBObject(e, true);
                }
                //(3-5)提交事务处理
                trans.Commit();
            }
        }

        [CommandMethod("DrawDeathlyHallows")]
        public void DrawDeathlyHallows()
        {

            Point3d Pt1 = new Point3d(0, 0, 0);
            double h = Math.Sqrt(1000 * 1000 - 500 * 500);

            Point3d Pt2 = new Point3d(500, h, 0);
            Point3d Pt3 = new Point3d(1000, 0, 0);
            Point3d Pt4 = new Point3d(500, 0, 0);

            Line l1 = new Line(Pt1, Pt2);
            Line l2 = new Line(Pt2, Pt3);
            Line l3 = new Line(Pt1, Pt3);
            Line l4 = new Line(Pt2, Pt4);


            
            Point3d Pc = new Point3d(500, h / 3, 0);
            Circle c = new Circle(Pc, Vector3d.ZAxis, h / 3);

            //AddEntity(l1);
            //AddEntity(l2);
            //AddEntity(l3);
            //AddEntity(l4);
            //AddEntity(c);
        }
    }
}
