using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Copper_Master.Classes
{
    public static class DrawingOperation
    {
        public static void SelectLineEdge(View swView, int firstParamIndex, double firstParamValue,
            int secondParamIndex, double secondParamValue, int thirdParamIndex, double thirdParamValue)
        {
            Curve swCurve;
            Edge swEdge;
            double[] curveParam;
            int counter = -1;
            
            while (true)
            {
                counter++;
                swEdge = (Edge)swView.GetVisibleEntities2(swView.GetVisibleComponents()[0], 1)[counter];
                swCurve = swEdge.GetCurve();
                try
                {
                    curveParam = swCurve.LineParams;

                    if (curveParam[firstParamIndex] == firstParamValue
                        && curveParam[secondParamIndex] == secondParamValue
                        && curveParam[thirdParamIndex] == thirdParamValue)
                        break;
                }
                catch { }
            }

            swView.SelectEntity(swEdge, false);
        }

        public static void SelectHolesEdges(XYZAxis xyzAxis, double[,] holesCoordinatesMM, List<Edge> holesEdges, View swView)
        {
            double[] curveParam;
            double holesMax = double.MinValue;
            int axisIndex = (int)xyzAxis;
            
            for (int i = 0; i < holesCoordinatesMM.GetLength(0); i++)
            {
                if (holesCoordinatesMM[i, axisIndex] > holesMax)
                {
                    holesMax = holesCoordinatesMM[i, axisIndex];
                }
            }

            for (int i = 0; i < holesEdges.Count; i++)
            {
                curveParam = holesEdges[i].GetCurve().CircleParams;
                if (curveParam[axisIndex] == holesMax / 1000d)
                {
                    swView.SelectEntity(holesEdges[i], true);
                }
            }
        }

    }
}
