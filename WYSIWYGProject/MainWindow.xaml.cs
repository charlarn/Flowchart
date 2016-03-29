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
        Point m_start;
        Vector m_startOffset;

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
                Console.WriteLine("DESELECTING");
                Keyboard.ClearFocus();
            }
        }

        private void Draw(string type)
        {
            Shape shape = null;
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
                    shape = new Rectangle();
                    shape.Width = 100;
                    shape.Height = 75;
                    shape.Fill = new SolidColorBrush(Colors.Crimson);
                    break;
                case "Decision":
                    shape = new Rectangle();
                    RotateTransform rotateTransform = new RotateTransform(-45);
                    shape.RenderTransform = rotateTransform;
                    typeText.HorizontalAlignment = HorizontalAlignment.Right;
                    typeText.Margin = new Thickness(0, 0, 0, 75);
                    shape.Width = 75;
                    shape.Height = 75;
                    shape.Fill = new SolidColorBrush(Colors.LimeGreen);
                    break;
                case "Connector":
                    shape = new Ellipse();
                    shape.Width = 75;
                    shape.Height = 75;
                    shape.Fill = new SolidColorBrush(Colors.DeepSkyBlue);
                    break;
            }
            shape.Stroke = new SolidColorBrush(Colors.DarkMagenta);

            shapeGrid.Children.Add(shape);
            shapeGrid.Children.Add(typeText);

            Canvas.SetLeft(shapeGrid, mousePosition.X);
            Canvas.SetTop(shapeGrid, mousePosition.Y);
            shapeGrid.MouseDown += new MouseButtonEventHandler(MoveGrid);

            FlowChart.Children.Add(shapeGrid);
        }

        private void SaveMousePosition(object sender, RoutedEventArgs e)
        {
            mousePosition = Mouse.GetPosition(FlowChart);
        }

        private void MoveGrid(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as Grid;
            TranslateTransform translate = element.RenderTransform as TranslateTransform;
            Console.WriteLine("Dragging");
           // m_start = e.GetPosition(Main);
            m_startOffset = new Vector(translate.X, translate.Y);
            element.CaptureMouse();
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            FrameworkElement element = sender as Grid;
            TranslateTransform translate = element.RenderTransform as TranslateTransform;

            if (element.IsMouseCaptured)
            {
                Vector offset = Point.Subtract(e.GetPosition(FlowChart), m_start);

                translate.X = m_startOffset.X + offset.X;
                translate.Y = m_startOffset.Y + offset.Y;
            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = sender as Grid;
            element.ReleaseMouseCapture();
        }
    }
}
