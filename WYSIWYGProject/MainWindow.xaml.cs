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
        private TextBox typeText;
        Shape newShape = null;

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
            if (typeText != null)
            {
                Keyboard.ClearFocus();
            }
        }

        private void Draw(string type)
        {
            CheckCollision();
 
            var shapeGrid = new Grid();

            typeText = new TextBox
            {
                Text = type,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0 },
                BorderThickness = new Thickness(0)
            };

            switch (type)
            {
                case "Process":
                    newShape = new Rectangle();
                    newShape.Width = 100;
                    newShape.Height = 75;
                    newShape.Fill = new SolidColorBrush(Colors.Crimson);
                    break;
                case "Decision":
                    newShape = new Rectangle();
                    RotateTransform rotateTransform = new RotateTransform(-45);
                    newShape.RenderTransform = rotateTransform;
                    typeText.HorizontalAlignment = HorizontalAlignment.Right;
                    typeText.Margin = new Thickness(0, 0, 0, 75);
                    newShape.Width = 75;
                    newShape.Height = 75;
                    newShape.Fill = new SolidColorBrush(Colors.LimeGreen);
                    break;
                case "Connector":
                    newShape = new Ellipse();
                    newShape.Width = 75;
                    newShape.Height = 75;
                    newShape.Fill = new SolidColorBrush(Colors.DeepSkyBlue);
                    break;
            }
            newShape.Stroke = new SolidColorBrush(Colors.DarkMagenta);

            shapeGrid.Children.Add(newShape);
            shapeGrid.Children.Add(typeText);

            Canvas.SetLeft(shapeGrid, mousePosition.X);
            Canvas.SetTop(shapeGrid, mousePosition.Y);
            shapeGrid.MouseDown += new MouseButtonEventHandler(MoveGrid);
            shapeGrid.MouseMove += new MouseEventHandler(MouseMoveGrid);
            shapeGrid.MouseUp += new MouseButtonEventHandler(MouseUpGrid);
            shapeGrid.ContextMenu = ShapeContextMenu();

            FlowChart.Children.Add(shapeGrid);
        }

        private ContextMenu ShapeContextMenu()
        {
            ContextMenu shapeMenu = new ContextMenu();

            shapeMenu.Items.Add(new MenuItem { Header = "Connector" });
            ((MenuItem)shapeMenu.Items[0]).MouseLeftButtonDown += new MouseButtonEventHandler(DrawConnector);

            MenuItem colourMenu = new MenuItem
            {
                Header = "Colour",
                Items = {
                    new MenuItem { Header = "Red" },
                    new MenuItem { Header = "Blue" },
                    new MenuItem { Header = "Red again" }
                }
            };
            MenuItem my = new MenuItem { Header = "Thread" };
            my.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeColour);
            colourMenu.Items.Add(my);
            //foreach (MenuItem item in colourMenu.Items)
            //{
            //    item.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeColour);
            //}
            shapeMenu.Items.Add(colourMenu);

            return shapeMenu;
        }

        private void ShapeColour(object sender, MouseEventArgs e)
        {
            MessageBox.Show("Running" );
            switch (((MenuItem) sender).Header.ToString())
            {
                case "Red":
                    MessageBox.Show("Colouring");
                    newShape.Fill = Brushes.Cornsilk;
                    break;
                case "Blue":
                    newShape.Fill = Brushes.Honeydew;
                    break;
                case "Red again":
                    newShape.Fill = Brushes.Indigo;
                    break;
            } 
        }

        private void DrawConnector(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Connecting");
        }

        private void SaveMousePosition(object sender, RoutedEventArgs e)
        {
            mousePosition = Mouse.GetPosition(FlowChart);
        }

        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        UIElement source = null;

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
        }

        private bool CheckCollision()
        {
            //plainCanvas.Children ger alla element som ligger i den layouten och spara i en lista.
            UIElementCollection uiColl = FlowChart.Children;

            double xCoord, yCoord, xWidth, yHeight;
            Shape shape;

            //en loop igenom alla shapes för att se om musklicket gjordes på någon av dem.
            foreach (Grid shapeGrid in uiColl)
            {
                shape = (Shape)shapeGrid.Children[0];
                xCoord = Canvas.GetLeft(shape);
                yCoord = Canvas.GetTop(shape);

                xWidth = shape.Width;
                yHeight = shape.Height;

                //Detta är ett klassiskt sätt att kolla kollision på, ni kan lösa det med snyggare metoder som t.ex. nämns i boken. 
                if (xCoord < mousePosition.X && mousePosition.X < (xCoord + xWidth) && yCoord < mousePosition.Y && mousePosition.Y < (yCoord + yHeight))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
