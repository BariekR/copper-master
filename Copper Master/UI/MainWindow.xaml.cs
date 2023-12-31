using Copper_Master.Classes;
using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Copper_Master
{
    public partial class MainWindow : Window
    {
        SldWorks swApp = new SldWorks();

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void CreateBar_Click(object sender, RoutedEventArgs e)
        {
            double width, thickness, length;
                        
            if (CreateBarWidth.Text == "" || CreateBarThickness.Text == "" || CreateBarLength.Text == "")
            {
                width = 100d / 1000d;
                thickness = 10d / 1000d;
                length = 750d / 1000d;
                CreateBarWidth.Text = (width * 1000d).ToString();
                CreateBarThickness.Text = (thickness * 1000d).ToString();
                CreateBarLength.Text = (length * 1000d).ToString();
            }
            else
            {
                width = (Double.Parse(CreateBarWidth.Text)) / 1000d;
                thickness = (Double.Parse(CreateBarThickness.Text)) / 1000d;
                length = (Double.Parse(CreateBarLength.Text)) / 1000d;
            }

            Cuboid cuboid = new Cuboid(width, thickness, length, swApp, (bool)holesOnBothSidesCHB.IsChecked);
            cuboid.CreateCuboidIn3D();
            cuboid.CreateCuboidDrawing();
        }

        private void CloseDocuments_Click(object sender, RoutedEventArgs e)
        {
            SolidWorksGeneral.CloseAllDocuments(swApp);
        }
    }
}
