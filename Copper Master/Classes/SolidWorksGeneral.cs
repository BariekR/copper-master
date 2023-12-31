using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Copper_Master.Classes
{
    public static class SolidWorksGeneral
    {
        public static void CloseAllDocuments(SldWorks swApp)
        {
            ModelDoc2 swModel;
            
            while (true)
            {
                swModel = (ModelDoc2)swApp.ActiveDoc;
                if (swModel != null)
                {
                    swApp.QuitDoc(swModel.GetTitle());
                }
                else
                {
                    break;
                }
            }
        }

        public static void CreateNewDocumentInSW(SldWorks swApp)
        {
            // Get default template
            string defaultPartTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplatePart);

            swApp.NewDocument(defaultPartTemplate, 0, 0, 0);
            swApp.Visible = true;
        }

        public static void CreateNewDrawingDocumentInSW(SldWorks swApp)
        {
            // Get default template
            string defaultDrawingTemplate = swApp.GetUserPreferenceStringValue((int)swUserPreferenceStringValue_e.swDefaultTemplateDrawing);

            //swApp.NewDocument("D:\\KnowledgeCenter\\SolidWorks\\Template\\A3_SY.SLDDRT", 8, 0, 0);
            swApp.NewDocument("D:\\SOLIDWORKS Corp\\SOLIDWORKS\\lang\\english\\Tutorial\\draw.drwdot", 8, 0, 0);
            swApp.Visible = true;
        }

        public static void SetDrawingSettings(Sheet swSheet, ModelDoc2 swDrawingDoc, double scaleNumerator, double scaleDenominator)
        {
            swSheet.SetScale(scaleNumerator, scaleDenominator, false, false);
            swSheet.SetSize((int)swDwgPaperSizes_e.swDwgPaperA3size, 0, 0);
            swDrawingDoc.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swDetailingDimTrailingZero, (int)swDetailingDimTrailingZero_e.swDimRemoveTrailingZeroes);
            swDrawingDoc.SetUserPreferenceIntegerValue((int)swUserPreferenceIntegerValue_e.swUnitSystem, (int)swUnitSystem_e.swUnitSystem_MMGS);
            swDrawingDoc.SetUserPreferenceToggle((int)swUserPreferenceToggle_e.swViewDisplayHideAllTypes, true);
        }
    }

    public enum XYZAxis
    {
        x = 0,
        y = 1,
        z = 2
    }
}
