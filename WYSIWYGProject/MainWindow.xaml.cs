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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void DrawEvent(object sender, RoutedEventArgs e)
        {
            Draw(((MenuItem)sender).Name);
        }

        private void Draw(string type)
        {
            Console.WriteLine("Name: " + type);
            Shape shape = null;
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

            var grid = new Grid();
            grid.Children.Add(shape);
            grid.Children.Add(new TextBlock { Text = "SHAPE", HorizontalAlignment=HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center  });

            Canvas.SetLeft(grid, mousePosition.X);
            Canvas.SetTop(grid, mousePosition.Y);

            FlowChart.Children.Add(grid);
        }

        private void SaveMousePosition(object sender, RoutedEventArgs e)
        {
            mousePosition = Mouse.GetPosition(FlowChart);
        }
    }
}
