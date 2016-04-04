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

namespace WYSIWYGProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Point mousePosition;
        private bool dragging = false;
        private ShapeGrid startShape = null, endShape = null;
        private List<ShapeGrid> gridCollection = new List<ShapeGrid>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawEvent(object sender, RoutedEventArgs e)
        {
            Draw(((MenuItem)sender).Name);
        }

        private void CanvasClicked(object sender, RoutedEventArgs e)
        {
            Keyboard.ClearFocus();
            //  startShape = null;
            //  endShape = null;
        }

        private void Draw(string type)
        {
            var shapeGrid = new ShapeGrid((ShapeType)Enum.Parse(typeof(ShapeType), type), mousePosition.X, mousePosition.Y);
            gridCollection.Add(shapeGrid);

            shapeGrid.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeMousedown);
            shapeGrid.MouseMove += new MouseEventHandler(ShapeMousemove);
            shapeGrid.MouseLeftButtonUp += new MouseButtonEventHandler(ShapeMouseup);
            shapeGrid.ContextMenu = ShapeContextMenu();

            FlowChart.Children.Add(shapeGrid);
        }

        private void Connect(ShapeGrid origin, ShapeGrid target)
        {
            Arrow arrow = new Arrow(FlowChart, origin, target);
            origin.Connections.Add(arrow);
            target.Connections.Add(arrow);

            startShape = null;
            endShape = null;
        }

        private ContextMenu ShapeContextMenu()
        {
            ContextMenu shapeMenu = new ContextMenu();

            shapeMenu.Items.Add(new MenuItem { Header = "Connector" });
            ((MenuItem)shapeMenu.Items[0]).Click += new RoutedEventHandler(DrawConnector);

            MenuItem colourMenu = new MenuItem
            {
                Header = "Colour",
                Items = {
                    new MenuItem { Header = "Cornsilk" },
                    new MenuItem { Header = "Honeydew" },
                    new MenuItem { Header = "MistyRose" }
                }
            };
            foreach (MenuItem item in colourMenu.Items)
            {
                item.Click += new RoutedEventHandler(ShapeColour);
            }
            shapeMenu.Items.Add(colourMenu);

            return shapeMenu;
        }

        private void ShapeColour(object sender, RoutedEventArgs e)
        {
            //dålig rad - Hämtar contextmenus högsta parents targets första child (dvs vilken shape som menyn tillhör) 
            Shape shape = (Shape)((((ContextMenu)(((MenuItem)(((MenuItem)sender).Parent)).Parent)).PlacementTarget as Grid).Children[0]);

            switch (((MenuItem)sender).Header.ToString())
            {
                case "Cornsilk":
                    shape.Fill = Brushes.Cornsilk;
                    break;
                case "Honeydew":
                    shape.Fill = Brushes.Honeydew;
                    break;
                case "MistyRose":
                    shape.Fill = Brushes.MistyRose;
                    break;
            }
        }

        private void DrawConnector(object sender, RoutedEventArgs e)
        {
            if (startShape == null)
            {
                startShape = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as ShapeGrid;
            }
            else
            {
                endShape = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as ShapeGrid;
                Connect(startShape, endShape);
            }
        }

        private void SaveMousePosition(object sender, RoutedEventArgs e)
        {
            mousePosition = Mouse.GetPosition(FlowChart);
        }

        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIElement source = null;

        private void ButtonDrawLine_Click(object sender, RoutedEventArgs e)
        {
            FlowChart.Children.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UIElementCollection uie = FlowChart.Children;
            try
            {
                foreach (Grid grid in uie)

                {
                    RotateTransform rotateTransform = new RotateTransform(0);
                    ((Shape)grid.Children[0]).RenderTransformOrigin = new Point(0.5, 0.5);
                    ((Shape)grid.Children[0]).RenderTransform = rotateTransform;
                }
            }

            catch (Exception) { }
        }

        private void ShapeMousedown(object sender, RoutedEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = Canvas.GetLeft(source);
            x_canvas = Mouse.GetPosition(FlowChart).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = Mouse.GetPosition(FlowChart).Y;
        }

        private void ShapeMousemove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                ShapeGrid shape = ((ShapeGrid)source);
                double x = e.GetPosition(FlowChart).X;
                double y = e.GetPosition(FlowChart).Y;
                //Dessa två variabler används för att uppdatera "shapes" position från mitten
                double shapePosX = shape.Position.X;
                double shapePosY = shape.Position.Y;
                x_shape += x - x_canvas;
                shapePosX += x - x_canvas;
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                shapePosY += y - y_canvas;
                Canvas.SetTop(source, y_shape);
                y_canvas = y;
                shape.Position = new Point(shapePosX, shapePosY); 
                shape.RedrawArrows(FlowChart); 
            }
        }

        private void ShapeMouseup(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
            startShape = null;
            endShape = null;
        }

        private Grid CheckCollision()
        {
            UIElementCollection shapeGrids = FlowChart.Children;

            double xCoord, yCoord, xWidth, yHeight;
            Shape shape;

            //en loop igenom alla shapes för att se om musklicket gjordes på någon av dem.
            foreach (Grid shapeGrid in shapeGrids)
            {
                shape = (Shape)shapeGrid.Children[0];
                xCoord = Canvas.GetLeft(shape);
                yCoord = Canvas.GetTop(shape);

                xWidth = shape.Width;
                yHeight = shape.Height;

                //Detta är ett klassiskt sätt att kolla kollision på, ni kan lösa det med snyggare metoder som t.ex. nämns i boken. 
                if (xCoord < mousePosition.X && mousePosition.X < (xCoord + xWidth) && yCoord < mousePosition.Y && mousePosition.Y < (yCoord + yHeight))
                {
                    return shapeGrid;
                }
            }
            return null;
        }
    }
}
