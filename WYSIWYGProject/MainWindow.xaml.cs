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
        List<bool> list = new List<bool>();
        private GridShape startShape = null, endShape = null;
        private List<GridShape> gridCollection = new List<GridShape>();

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
            var shapeGrid = new GridShape((ShapeType)Enum.Parse(typeof(ShapeType), type), mousePosition.X, mousePosition.Y);
            gridCollection.Add(shapeGrid);

            shapeGrid.MouseDown += new MouseButtonEventHandler(MoveGrid);
            shapeGrid.MouseMove += new MouseEventHandler(MouseMoveGrid);
            shapeGrid.MouseUp += new MouseButtonEventHandler(MouseUpGrid);
            shapeGrid.ContextMenu = ShapeContextMenu();

            FlowChart.Children.Add(shapeGrid);
        }

        private void Connect(GridShape start, GridShape end)
        {
            Point[] anchors = GetBestAnchors(start, end);

            Line line = new Line
            {
                X1 = anchors[0].X,
                Y1 = anchors[0].Y,
                X2 = anchors[1].X,
                Y2 = anchors[1].Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            FlowChart.Children.Add(new Arrow(start, end).Draw());
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
            //dålig rad
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
                startShape = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as GridShape;
            }
            else
            {
                endShape = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as GridShape;
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

            catch (Exception) {}
        }

        private void MoveGrid(object sender, RoutedEventArgs e)
        {
            source = (UIElement)sender;
            Mouse.Capture(source);
            captured = true;
            x_shape = Canvas.GetLeft(source);
            x_canvas = Mouse.GetPosition(FlowChart).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = Mouse.GetPosition(FlowChart).Y;
        }

        private void MouseMoveGrid(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                double x = e.GetPosition(FlowChart).X;
                double y = e.GetPosition(FlowChart).Y;
                x_shape += x - x_canvas;
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                Canvas.SetTop(source, y_shape);
                y_canvas = y;
            }
        }

        private void MouseUpGrid(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            captured = false;
           // ((GridShape)source).MakeAnchors();
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

        private Point[] GetBestAnchors(GridShape origin, GridShape target)
        {
            double startX = origin.Position.X, startY = origin.Position.Y;
            double endX = target.Position.X, endY = target.Position.X;
            Point[] bestAnchors = new Point[2];

            if(startX < endX)
            {
                MessageBox.Show("Origin is left of target");
                if (startY > endY)
                {
                    if (endX > endY)
                    {
                        bestAnchors[0] = origin.RightAnchor;
                        bestAnchors[1] = target.LeftAnchor;
                    }
                    else
                    {
                        bestAnchors[0] = origin.TopAnchor;
                        bestAnchors[1] = target.BottomAnchor;
                    }
                }
                else
                {
                    //this is right
                    if (endX < endY)
                    {
                        bestAnchors[0] = origin.RightAnchor;
                        bestAnchors[1] = target.LeftAnchor;
                    }
                    else
                    {
                        bestAnchors[0] = origin.TopAnchor;
                        bestAnchors[1] = target.BottomAnchor;
                    }
                }                
            }
            else
            {
                MessageBox.Show("Origin is right of target");
                //this is right
                if (startY < endY)
                {
                    if (endX > endY)
                    {
                        bestAnchors[0] = origin.LeftAnchor;
                        bestAnchors[1] = target.RightAnchor;
                    }
                    else
                    {
                        bestAnchors[0] = origin.TopAnchor;
                        bestAnchors[1] = target.BottomAnchor;
                    }
                }
                else
                {
                    if (endX < endY)
                    {
                        bestAnchors[0] = origin.LeftAnchor;
                        bestAnchors[1] = target.RightAnchor;
                    }
                    else
                    {
                        bestAnchors[0] = origin.TopAnchor;
                        bestAnchors[1] = target.BottomAnchor;
                    }
                }
            }
            
            return bestAnchors;
        }
    }
}
