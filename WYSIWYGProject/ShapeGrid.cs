using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WYSIWYGProject
{
    class ShapeGrid
    {
        public Point LeftAnchor { get; private set; }
        public Point TopAnchor { get; private set; }
        public Point RightAnchor { get; private set; }
        public Point BottomAnchor { get; private set; }
        public Grid Grid;
        public Shape Shape;
        public TextBox Text;

        public ShapeGrid(string type, double x, double y)
        {
            Grid = new Grid();
            MakeTextBox(type);
            MakeShape(type);
            Grid.Children.Add(Shape);
            Grid.Children.Add(Text);
            Canvas.SetLeft(Grid, x);
            Canvas.SetTop(Grid, y);
            MakeAnchors(type);
        }

        private void MakeAnchors(string type)
        {
            if (type.Equals("Decision"))
            {
                LeftAnchor = new Point(Canvas.GetLeft(Grid), Canvas.GetTop(Grid) + (Shape.Height / 2));
                TopAnchor = new Point(Canvas.GetLeft(Grid) + (Math.Sqrt(2) * Shape.Height), Canvas.GetTop(Grid));
                RightAnchor = new Point(Canvas.GetLeft(Grid) + Shape.Height, Canvas.GetTop(Grid) + (Shape.Height / 2));
                BottomAnchor = new Point(Canvas.GetLeft(Grid) + Shape.Width / 2, Canvas.GetTop(Grid) + (Math.Sqrt(2) * Shape.Height));
            }
            else
            {
                LeftAnchor = new Point(Canvas.GetLeft(Grid), Canvas.GetTop(Grid) + (Shape.Height / 2));
                TopAnchor = new Point(Canvas.GetLeft(Grid) + Shape.Width / 2, Canvas.GetTop(Grid));
                RightAnchor = new Point(Canvas.GetLeft(Grid) + Shape.Width, Canvas.GetTop(Grid) + (Shape.Height / 2));
                BottomAnchor = new Point(Canvas.GetLeft(Grid) + Shape.Width / 2, Canvas.GetTop(Grid) + (Shape.Height));
            }
        }

        private void MakeTextBox(string text)
        {
            Text = new TextBox
            {
                Text = text,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0 },
                BorderThickness = new Thickness(0)
            };
        }

        private void MakeShape(string type)
        {
            switch (type)
            {
                case "Process":
                    Shape = new Rectangle();
                    Shape.Width = 100;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.Crimson);
                    break;
                case "Decision":
                    Shape = new Rectangle();
                    RotateTransform rotateTransform = new RotateTransform(45);
                    Shape.RenderTransformOrigin = new Point(0.5, 0.5);
                    Shape.RenderTransform = rotateTransform;
                    Shape.Width = 75;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.LimeGreen);
                    break;
                case "Connector":
                    Shape = new Ellipse();
                    Shape.Width = 75;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.DeepSkyBlue);
                    break;
            }
            Shape.Stroke = new SolidColorBrush(Colors.DarkMagenta);
        }
    }
}
