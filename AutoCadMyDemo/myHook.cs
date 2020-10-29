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
       
        private double area;
        public double Area {
            get { return area; }
            set { area = value; }
        }

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
            Point3d[] points = { pt1, pt2, pt3, pt4};
            foreach (Point3d p in points)
            {
                Line tmpl1 = new Line(new Point3d(p.X - 20, p.Y, 0),new Point3d(p.X+20,p.Y,0));
                Line tmpl2 = new Line(new Point3d(p.X , p.Y-20, 0),new Point3d(p.X,p.Y+20,0));
                //tmpl1.Linetype = "CENTER2";
                //tmpl2.Linetype = "CENTER2";
                tmpl1.ColorIndex = 4;
                tmpl2.ColorIndex = 4;
                centerMarkList.Add(tmpl1);
                centerMarkList.Add(tmpl2);
            }

            //标注
            //c3直径
            DiametricDimension d1 = new DiametricDimension();
            Line tmpl = new Line(c3.Center, new Point3d(c3.Center.X - 1, c3.Center.Y + 1,0));
            pc.Clear();
            c3.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            d1.ChordPoint = pc[0];
            d1.FarChordPoint = pc[1];
            d1.LeaderLength = 20;
            d1.ColorIndex = 2;
            MarkList.Add(d1);

            //半径
            RadialDimension r1 = new RadialDimension();
            Arc tmparc = entityList[0] as Arc;
            tmpl=new Line(tmparc.Center, new Point3d(tmparc.Center.X + 1, tmparc.Center.Y - 0.5, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r1.Center = tmparc.Center;
            r1.ChordPoint = pc[1].X>pc[0].X?pc[1]:pc[0];
            r1.ColorIndex = 2;
            r1.LeaderLength = -5;
            MarkList.Add(r1);

            RadialDimension r2 = new RadialDimension();
            tmparc = entityList[1] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X - 1, tmparc.Center.Y - 0.5, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r2.Center = tmparc.Center;
            r2.ChordPoint = pc[1].X > pc[0].X ? pc[0] : pc[1];
            r2.ColorIndex = 2;
            r2.LeaderLength = -30;
            MarkList.Add(r2);

            RadialDimension r3 = new RadialDimension();
            tmparc = entityList[2] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X + 1, tmparc.Center.Y - 0.2, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r3.Center = tmparc.Center;
            r3.ChordPoint = pc[1].X > pc[0].X ? pc[0] : pc[1];
            r3.ColorIndex = 2;
            MarkList.Add(r3);

            RadialDimension r4 = new RadialDimension();
            tmparc = entityList[3] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X + 0.35, tmparc.Center.Y + 1, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r4.Center = tmparc.Center;
            r4.ChordPoint = pc[1].X < pc[0].X ? pc[0] : pc[1];
            r4.ColorIndex = 2;
            MarkList.Add(r4);

            RadialDimension r5 = new RadialDimension();
            tmparc = entityList[4] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X + 0.35, tmparc.Center.Y + 1, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r5.Center = tmparc.Center;
            r5.ChordPoint = pc[1].X > pc[0].X ? pc[0] : pc[1];
            r5.ColorIndex = 2;
            MarkList.Add(r5);

            RadialDimension r6 = new RadialDimension();
            tmparc = entityList[5] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X + 1, tmparc.Center.Y - 1, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r6.Center = tmparc.Center;
            r6.ChordPoint = pc[1].X < pc[0].X ? pc[0] : pc[1];
            r6.ColorIndex = 2;
            r6.LeaderLength = 20;
            MarkList.Add(r6);

            RadialDimension r7 = new RadialDimension();
            tmparc = entityList[6] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X + 1, tmparc.Center.Y + 1, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r7.Center = tmparc.Center;
            r7.ChordPoint = pc[1].X < pc[0].X ? pc[0] : pc[1];
            r7.ColorIndex = 2;
            r7.LeaderLength = 20;
            MarkList.Add(r7);

            RadialDimension r8 = new RadialDimension();
            tmparc = entityList[7] as Arc;
            tmpl = new Line(tmparc.Center, new Point3d(tmparc.Center.X - 1, tmparc.Center.Y - 1, 0));
            pc.Clear();
            tmparc.IntersectWith(tmpl, Intersect.ExtendBoth, pc, new IntPtr(0), new IntPtr(0));
            r8.Center = tmparc.Center;
            r8.ChordPoint = pc[1].X < pc[0].X ? pc[0] : pc[1];
            r8.ColorIndex = 2;
            r8.LeaderLength = 20;
            MarkList.Add(r8);

            //对齐标注1
            AlignedDimension am1 = new AlignedDimension();
            am1.XLine1Point = pt1;
            am1.XLine2Point = new Point3d(5, 0, 0);
            am1.DimLinePoint = new Point3d(0, -40, 0);
            am1.ColorIndex = 2;
            MarkList.Add(am1);
            
            //对齐标注2
            AlignedDimension am2 = new AlignedDimension();
            am2.XLine1Point = pt1;
            am2.XLine2Point = new Point3d(0, -3, 0);
            am2.DimLinePoint = new Point3d(40,0 , 0);
            am2.ColorIndex = 2;
            MarkList.Add(am2);

            //对齐标注3
            AlignedDimension am3 = new AlignedDimension();
            am3.XLine1Point = pt1;
            am3.XLine2Point = new Point3d(0, 70, 0);
            am3.DimLinePoint = new Point3d(70, 30, 0);
            am3.ColorIndex = 2;
            MarkList.Add(am3);

            //对齐标注4
            AlignedDimension am4 = new AlignedDimension();
            am4.XLine1Point = l1.EndPoint;
            am4.XLine2Point = l2.EndPoint;
            am4.DimLinePoint = new Point3d(-60, 0, 0);
            am4.ColorIndex = 2;
            MarkList.Add(am4);

            //对齐标注5
            AlignedDimension am5 = new AlignedDimension();
            am5.XLine1Point = new Point3d(0,34,0);
            am5.XLine2Point = new Point3d(-18,34,0);
            am5.DimLinePoint = new Point3d(0, 8, 0);
            am5.ColorIndex = 2;
            MarkList.Add(am5);

            //对齐标注6
            AlignedDimension am6 = new AlignedDimension();
            am6.XLine1Point = new Point3d(-25, 70, 0);
            am6.XLine2Point = new Point3d(-25, 46, 0);
            am6.DimLinePoint = new Point3d(-50, 0, 0);
            am6.ColorIndex = 2;
            MarkList.Add(am6);

            AddEntities(MarkList.ToArray());

            Entity[] es = { c3,  l3 };
            ////AddEntities(es);
            entityList.AddRange(es);
            //组成面域
            DBObjectCollection dbc1 = new DBObjectCollection();
            foreach (Entity e in entityList)
            {
                dbc1.Add(e);
            }
            dbc1.Add(c3);

            DBObjectCollection regions = new DBObjectCollection();
            regions = Region.CreateFromCurves(dbc1);
            //doc.Editor.WriteMessage("r1{0}", dbc1regions.Count);
            Region re1 = regions[0] as Region;
            Region re2 = regions[1] as Region;

            doc.Editor.WriteMessage("r1:{0},r2:{1}", re1.Area,re2.Area);

            double area = re1.Area > re2.Area ?re1.Area-re2.Area:re2.Area-re1.Area;
            Area = area;
            doc.Editor.WriteMessage("面积{0}", area);

            //entityList.Clear();
            //entityList.Add(r1);
            //entityList.Add(r2);


            foreach (Entity e in entityList)
            {
                e.ColorIndex = 1;
            }
            entityList.Add(l1);
            entityList.Add(l2);
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
