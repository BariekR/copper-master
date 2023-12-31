using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Copper_Master.Classes;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace Copper_Master
{
    internal class Cuboid
    {
        SldWorks swApp;
        DrawingDoc swDrawing;
        View swView;
        Sheet swSheet;
        bool holesOnBothSides;
        double width, thickness, length;
        double[,] holesCoordinatesMM;

        public Cuboid(double width, double thickness, double length, SldWorks swApp, bool holesOnBothSides)
        {
            this.width = width;
            this.thickness = thickness;
            this.length = length;
            this.swApp = swApp;
            this.holesOnBothSides = holesOnBothSides;
        }

        public void CreateCuboidIn3D()
        {
            ModelDoc2 swModel;
            Feature swFeature;

            // Turn off dimension dialog
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchInference, false);

            // Create new part document
            SolidWorksGeneral.CreateNewDocumentInSW(swApp);

            // Activate document
            swModel = (ModelDoc2)swApp.ActiveDoc;

            // Rename plane
            swModel.FeatureByPositionReverse(3).Name = "Front";
            swModel.FeatureByPositionReverse(2).Name = "Top";
            swModel.FeatureByPositionReverse(1).Name = "Right";

            // Select plane, insert sketch with one line, exit sketch
            swModel.Extension.SelectByID2("Top", "PLANE", 0, 0, 0, false, 0, null, 0);
            swModel.InsertSketch2(true);
            swModel.CreateLine2(-length / 2d, 0, 0, length / 2d, 0, 0);
            swModel.InsertSketch2(true);

            // Rename the sketch
            swFeature = swModel.FeatureByPositionReverse(0);
            swFeature.Name = "Sketch1";

            // Create base flange
            swModel.Extension.SelectByID2("Sketch1", "SKETCH", 0, 0, 0, false, 0, null, 0);
            swModel.FeatureManager.InsertSheetMetalBaseFlange2(thickness, false, 0, width, 0, true, (int)swEndConditions_e.swEndCondMidPlane, 0, 0, null, false, 0, 0, 0, 0, false, false, false, false);

            // Create Hole
            holesCoordinatesMM = PartOperation.CreateHoleByHoleWizard(swModel, width, length, holesOnBothSides);

            // Create Slotted Cuts
            if (width >= 0.12)
            {
                PartOperation.CreateSlottedCuts(swModel, width, length);
                PartOperation.CreateSlottedCuts(swModel, width, -length+0.16);
            }

            // Turn on dimension dialog
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, true);
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swSketchInference, true);
        }

        public void CreateCuboidDrawing()
        {
            ModelDoc2 swDrawingDoc;
            List<Edge> holesEdges;
            string modelPath = "C:\\Users\\pirox\\Desktop\\Copper.sldprt";
            double drawingStampHeight = 0.065d;
            double scaleNumerator = 1d;
            double scaleDenominator = 10d;

            // Turn off dimension dialog
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, false);

            // Create new drawing document
            SolidWorksGeneral.CreateNewDrawingDocumentInSW(swApp);

            // Activate document
            swDrawing = (DrawingDoc)swApp.ActiveDoc;
            swDrawingDoc = (ModelDoc2)swDrawing;

            // Settings
            swSheet = swDrawing.GetCurrentSheet();
            SolidWorksGeneral.SetDrawingSettings(swSheet, swDrawingDoc, scaleNumerator, scaleDenominator);
            double paperWidth = 0, paperHeight = 0;
            swSheet.GetSize(ref paperWidth, ref paperHeight);

            // Create Drawing views
            swDrawing.Create1stAngleViews2(modelPath);

            // Delete View
            swView = swDrawing.GetFirstView().GetNextView().GetNextView();
            swDrawingDoc.Extension.SelectByID2(swView.Name, "DRAWINGVIEW", 0, 0, 0, false, 0, null, 0);
            swDrawingDoc.Extension.DeleteSelection2(4);

            // Reposition
            swView = swDrawing.GetFirstView().GetNextView();
            swView.Position = new double[] { 0.3d * paperWidth, 0.75d * paperHeight };
            swView = swDrawing.GetFirstView().GetNextView().GetNextView();
            swView.Position = new double[] { 0.5d * paperWidth, 0.75d * paperHeight };

            // Insert Dimension 1st View
            swView = swDrawing.GetFirstView().GetNextView();

            // Insert edge dimension
            DrawingOperation.SelectLineEdge(swView, 1, width / 2d, 4, 0, 5, 0);
            double[] firstViewBoundingBox = swView.GetOutline();
            swDrawingDoc.AddDimension2((firstViewBoundingBox[2] + firstViewBoundingBox[0]) / 2d, firstViewBoundingBox[3], 0d);

            // Insert Holes Ordinate Dimension X
            DrawingOperation.SelectLineEdge(swView, 0, -length / 2d, 3, 0, 5, 0);
            holesEdges = PartOperation.GetHolesEdges(swView);
            DrawingOperation.SelectHolesEdges(XYZAxis.y, holesCoordinatesMM, holesEdges, swView);

            swDrawingDoc.Extension.AddOrdinateDimension(3, (-length / 2d), firstViewBoundingBox[1] - (firstViewBoundingBox[3] - firstViewBoundingBox[1]) / 4d, 0);
            swDrawingDoc.ClearSelection2(true);
            swDrawingDoc.SetPickMode();

            // Insert Holes Ordinate Dimension Y
            DrawingOperation.SelectLineEdge(swView, 1, -width / 2d, 4, 0, 5, 0);
            DrawingOperation.SelectHolesEdges(XYZAxis.x, holesCoordinatesMM, holesEdges, swView);

            swDrawingDoc.Extension.AddOrdinateDimension(2, firstViewBoundingBox[0] - (firstViewBoundingBox[2] - firstViewBoundingBox[0]) / 10d, (-width / 2d), 0);
            swDrawingDoc.ClearSelection2(true);
            swDrawingDoc.SetPickMode();

            // Insert Dimension 2nd View
            swView = swDrawing.GetFirstView().GetNextView().GetNextView();

            DrawingOperation.SelectLineEdge(swView, 1, width / 2d, 3, 0, 4, 0);
            double[] secondViewBoundingBox = swView.GetOutline();
            swDrawingDoc.AddDimension2(secondViewBoundingBox[2], secondViewBoundingBox[3], 0d);

            DrawingOperation.SelectLineEdge(swView, 2, 0, 3, 0, 5, 0);
            swDrawingDoc.AddDimension2(secondViewBoundingBox[2] + (secondViewBoundingBox[2] - secondViewBoundingBox[0]) / 2d,
                secondViewBoundingBox[1] - (secondViewBoundingBox[3] - secondViewBoundingBox[1]) / 6d, 0d).ArrowSide = 1;

            // Create Isometric View
            swView = swDrawing.CreateDrawViewFromModelView3(modelPath, "*Isometric", 0d, 0d, 0d);
            swView.ScaleRatio = new double[] { scaleNumerator, scaleDenominator };
            double[] isoViewBoundingBox = swView.GetOutline();
            swView.Position = new double[] { paperWidth - ((isoViewBoundingBox[2] - isoViewBoundingBox[0])/(2d)) - (paperWidth * 0.1d),
                drawingStampHeight + ((isoViewBoundingBox[3] - isoViewBoundingBox[1]) / (2d)) + (paperHeight * 0.025d) };

            // Turn on dimension dialog
            swApp.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swInputDimValOnCreate, true);
        }
    }
}
