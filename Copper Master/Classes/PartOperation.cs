using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Copper_Master.Classes
{
    public static class PartOperation
    {
        public static double[,] CreateHoleByHoleWizard(ModelDoc2 swModel, double width, double length, bool holesOnBothSides)
        {
            double[,] holesCoordinatesMM;
            FeatureManager swFeatureManager;
            SketchManager swSketchManager;
            Feature swFeature;

            holesCoordinatesMM = GetHolesCoordinatesMM(width, length);
            swFeatureManager = (FeatureManager)swModel.FeatureManager;
            swSketchManager = (SketchManager)swModel.SketchManager;

            swModel.Extension.SelectByRay(holesCoordinatesMM[0, 0] / 1000d, holesCoordinatesMM[0, 1] / 1000d, -1d, 0d, 0d, 1d, 0.05d, 2, true, 0, 0);

            // Create hole wizard feature
            swFeature = (Feature)swFeatureManager.HoleWizard5(2, 8, 143, "Ø14.0", (int)swEndConditions_e.swEndCondUpToNext, 0.014d, 0.005d, -1, 1, 1.57079633, 0, 0, 0, 0, 0, -1, -1, -1, -1, -1, "", false, true, true, true, true, false);

            swFeature.GetFirstSubFeature().Select2(false, 0);
            swModel.EditSketch();

            if (holesOnBothSides)
            {
                swSketchManager.CreatePoint(holesCoordinatesMM[0, 0] / 1000d, holesCoordinatesMM[0, 1] / 1000d, 0d);

                for (int i = 1; i < holesCoordinatesMM.GetLength(0); i++)
                {
                    swSketchManager.CreatePoint(-holesCoordinatesMM[i, 0] / 1000d, holesCoordinatesMM[i, 1] / 1000d, 0d);
                    swSketchManager.CreatePoint(holesCoordinatesMM[i, 0] / 1000d, holesCoordinatesMM[i, 1] / 1000d, 0d);
                }
            }
            else
            {
                for (int i = 1; i < holesCoordinatesMM.GetLength(0); i++)
                {
                    swSketchManager.CreatePoint(-holesCoordinatesMM[i, 0] / 1000d, holesCoordinatesMM[i, 1] / 1000d, 0d);
                }
            }

            swSketchManager.InsertSketch(true);
            swModel.ForceRebuild3(true);
            swModel.Extension.SaveAs3("C:\\Users\\pirox\\Desktop\\Copper.sldprt", 0, 1, null, null, 0, 0);

            return holesCoordinatesMM;
        }

        static double[,] GetHolesCoordinatesMM(double widthM, double lengthM)
        {
            double width = widthM * 1000d;
            double length = lengthM * 1000d;
            double[,] points;
            int pointsCount = 0, e3Count = 0;
            double e1 = 0d, e2 = 0d, e3 = 0d;

            if (width <= 60d)
            {
                pointsCount = 2;
                e1 = 20d; e2 = 40d; e3 = 0d;
            }
            else if (width <= 80d)
            {
                pointsCount = 4;
                e1 = 20d; e2 = 40d; e3 = 40d;
            }
            else if (width <= 100d)
            {
                pointsCount = 4;
                e1 = 20d; e2 = 40d; e3 = 50d;

            }
            else if (width <= 120d)
            {
                pointsCount = 4;
                e1 = 20d; e2 = 40d; e3 = 60d;
            }
            else if (width <= 160d)
            {
                pointsCount = 8;
                e1 = 20d; e2 = 40d; e3 = 40d;
            }
            else if (width <= 200d)
            {
                pointsCount = 8;
                e1 = 20d; e2 = 40d; e3 = 50d;
            }

            points = new double[pointsCount, 2];
            e3Count = pointsCount / 2 - 1;

            points[0, 0] = length / 2d - e1;
            points[0, 1] = width / 2d - (width - e3 * e3Count) / 2d;
            points[1, 0] = points[0, 0] - e2;
            points[1, 1] = points[0, 1];
            for (int i = 1; i <= e3Count; i++)
            {
                points[2 * i, 0] = points[0, 0];
                points[2 * i, 1] = points[0, 1] - e3 * i;

                points[2 * i + 1, 0] = points[2 * i, 0] - e2;
                points[2 * i + 1, 1] = points[2 * i, 1];
            }

            return points;
        }

        public static List<Edge> GetHolesEdges(View swView)
        {
            Curve swCurve;
            Edge swEdge;
            List<Edge> holesEdges = new List<Edge>();

            for (int i = 0; i < swView.GetVisibleEntities2(swView.GetVisibleComponents()[0], 1).Length; i++)
            {
                swEdge = (Edge)swView.GetVisibleEntities2(swView.GetVisibleComponents()[0], 1)[i];
                swCurve = swEdge.GetCurve();

                if (swCurve.Identity() == 3002)
                {
                    holesEdges.Add(swEdge);
                }
            }

            return holesEdges;
        }

        public static void CreateSlottedCuts(ModelDoc2 swModel, double width, double length)
        {
            SketchManager swSketchManager;
            FeatureManager swFeatureManager;
            
            swSketchManager = (SketchManager)swModel.SketchManager;
            swFeatureManager = (FeatureManager)swModel.FeatureManager;

            swModel.Extension.SelectByID2("Front", "PLANE", 0, 0, 0, false, 0, null, 0);
            swSketchManager.InsertSketch(true);

            if(width.Equals(0.12))
            {
                swSketchManager.CreateCornerRectangle((length / 2d) - 0.08, 0.0015, 0, length / 2d, -0.0015, 0);
            }
            else
            {
                swSketchManager.CreateCornerRectangle((length / 2d) - 0.08, (width/4d)+0.0015, 0, length / 2d, (width / 4d) - 0.0015, 0);
                swSketchManager.CreateCornerRectangle((length / 2d) - 0.08, 0.0015, 0, length / 2d, -0.0015, 0);
                swSketchManager.CreateCornerRectangle((length / 2d) - 0.08, (-width / 4d) + 0.0015, 0, length / 2d, (-width / 4d) - 0.0015, 0);
            }

            swSketchManager.InsertSketch(true);

            swFeatureManager.FeatureCut4(true, false, true, 11, 11, 1, 1, false, false, false,
            false, 0, 0, false, false, false, false, true, true, true,
            true, true, false, 0, 0, false, true);
        }
    }
}
