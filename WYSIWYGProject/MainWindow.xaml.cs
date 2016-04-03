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
            //IsInTriangle();
            //  startShape = null;
            //  endShape = null;
        }

        private void Draw(string type)
        {
            var shapeGrid = new ShapeGrid((ShapeType)Enum.Parse(typeof(ShapeType), type), mousePosition.X, mousePosition.Y);
            gridCollection.Add(shapeGrid);

            shapeGrid.MouseDown += new MouseButtonEventHandler(MoveGrid);
            shapeGrid.MouseMove += new MouseEventHandler(MouseMoveGrid);
            shapeGrid.MouseUp += new MouseButtonEventHandler(MouseUpGrid);
            shapeGrid.ContextMenu = ShapeContextMenu();

            FlowChart.Children.Add(shapeGrid);
        }

        private void Connect(ShapeGrid start, ShapeGrid end)
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
            MakeArrow(anchors[0], anchors[1]);
            MakeArrowhead(anchors[0], anchors[1]);
            FlowChart.Children.Add(line);
            startShape = null;
            endShape = null;
        }

        private void MakeArrowhead(Point a, Point b)
        {
            double deltaY = a.Y - b.Y;
            double deltaX = a.X - b.X;

            //  double angleInDegrees = Math.Atan(deltaY / deltaX) * 180 / Math.PI;
            double angleInRadians = Math.Atan(deltaY / deltaX);
            MessageBox.Show("ANGLE: " + angleInRadians);

            //Line lineX = new Line
            //{
            //    X1 = b.X,
            //    Y1 = b.Y,
            //    X2 = b.X + 15 * Math.Cos(angleInRadians + 10),
            //    Y2 = b.Y + 15 * Math.Sin(angleInRadians + 10),
            //    Stroke = Brushes.Black,
            //    StrokeThickness = 2
            //};
            //Line lineY = new Line
            //{
            //    X1 = b.X,
            //    Y1 = b.Y,
            //    X2 = b.X + 15 * Math.Cos(angleInRadians - 10),
            //    Y2 = b.Y + 15 * Math.Sin(angleInRadians - 10),
            //    Stroke = Brushes.Black,
            //    StrokeThickness = 2
            //};

            Line lineX = new Line
            {
                X1 = b.X,
                Y1 = b.Y,
                X2 = b.X + 15 * Math.Cos(angleInRadians + 10) *-1,
                Y2 = b.Y + 15 * Math.Sin(angleInRadians + 10) * -1,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Line lineY = new Line
            {
                X1 = b.X,
                Y1 = b.Y,
                X2 = b.X + 15 * Math.Cos(angleInRadians - 10) * -1,
                Y2 = b.Y + 15 * Math.Sin(angleInRadians - 10) * -1,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            // lineX.RenderTransformOrigin = new Point(0.5, 0.5);
            // lineY.RenderTransformOrigin = new Point(0.5, 0.5);
            //  lineX.RenderTransform = new RotateTransform(angleInDegrees + 20);
            // lineY.RenderTransform = new RotateTransform(angleInDegrees - 20);

            // lineX.RenderTransformOrigin()

            FlowChart.Children.Add(lineX);
            FlowChart.Children.Add(lineY);
        }

        private void MakeArrow(Point a, Point b)
        {
            double deltaY = a.Y - b.Y;
            double deltaX = a.X - b.X;

            double angleInDegrees = Math.Atan(deltaY / deltaX) * 180 / Math.PI;
            MessageBox.Show("ANGLE: " + angleInDegrees);
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

        bool IsInTriangle()
        {

            Point point = Mouse.GetPosition(FlowChart);
            Point a = new Point(FlowChart.Width/2, FlowChart.Height/2);
            Point b = new Point(FlowChart.Width/2, FlowChart.Height);
            Point c = new Point(0, FlowChart.Height);

            MessageBox.Show("X: " + (point.X) + " Y: " + (point.Y - FlowChart.Height/2));  
            if (point.Y > a.Y && point.X < b.X)
            {
                MessageBox.Show("Is in triangle: " + ((point.Y - FlowChart.Height / 2) - point.X ));
            }
            else
            {
                MessageBox.Show("Is outside of triangle" + (point.X - point.Y));
            }


            return false;
        }

        private Point[] GetBestAnchors(ShapeGrid origin, ShapeGrid target)
        {
            double originX = origin.Position.X, originY = origin.Position.Y;
            double targetX = target.Position.X, targetY = target.Position.Y;
            Point[] bestAnchors = new Point[2];

            // om target är på vänster sida av origin
            if (originX > targetX)
            {
                //om target är över origin = den övre vänstra kvadranten
                if (originY > targetY)
                {
                    //om target är i den vänstra triangeln i kvadranten
                    if (originX - targetX > originY - targetY)
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
                    if (targetY - originY < originX - targetX)
                    {
                        bestAnchors[0] = origin.LeftAnchor;
                        bestAnchors[1] = target.RightAnchor;
                    }
                    else
                    {
                        bestAnchors[0] = origin.BottomAnchor;
                        bestAnchors[1] = target.TopAnchor;
                    }
                }
            }
            else
            {
                if (originY > targetY)
                {
                    if (targetX - originX > originY - targetY)
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
                    if (targetY - originY < targetX - originX)
                    {
                        bestAnchors[0] = origin.RightAnchor;
                        bestAnchors[1] = target.LeftAnchor;
                    }
                    else
                    {
                        bestAnchors[0] = origin.BottomAnchor;
                        bestAnchors[1] = target.TopAnchor;
                    }
                }
            }
            return bestAnchors;
        }
    }
}
