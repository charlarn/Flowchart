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
    enum ShapeType
    {
        Process,
        Decision,
        Connector
    };

    class ShapeGrid : Grid
    {
        public Point LeftAnchor { get; private set; }
        public Point TopAnchor { get; private set; }
        public Point RightAnchor { get; private set; }
        public Point BottomAnchor { get; private set; }
        public Point Position { get; set; }
        public Point TargetAnchor { get; set; }

        private Shape Shape;
        public TextBox Text;
        public List<Line> lines;
        public ShapeType Type { get; set; }

        public ShapeGrid(ShapeType type, double x, double y)
        {
            Type = type;
            MakeTextBox();
            MakeShape();
            this.Children.Add(Shape);
            this.Children.Add(Text);
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
            Position = new Point(x + Shape.Width / 2, y + Shape.Height / 2);
            MakeAnchors();
            lines = new List<Line>();
        }

        public void RedrawLines()
        {
            foreach(Line line in lines)
            {

            }
        }

        public void MakeAnchors()
        {
            if (Type == ShapeType.Decision)
            {
                LeftAnchor = new Point(Canvas.GetLeft(this) - ((Math.Sqrt(2) * Shape.Height) - Shape.Height)/2, Canvas.GetTop(this) + (Shape.Height / 2));
                TopAnchor = new Point(Canvas.GetLeft(this) + (Shape.Height / 2), Canvas.GetTop(this) - ((Math.Sqrt(2) * Shape.Height) - Shape.Height) / 2);
                RightAnchor = new Point(Canvas.GetLeft(this) + ((Math.Sqrt(2) * Shape.Height) - Shape.Height) / 2, Canvas.GetTop(this) + (Shape.Height / 2));
                BottomAnchor = new Point(Canvas.GetLeft(this) + (Shape.Height / 2), Canvas.GetTop(this) + ((Math.Sqrt(2) * Shape.Height) + Shape.Height) / 2);
            }
            else
            {
                LeftAnchor = new Point(Canvas.GetLeft(this), Canvas.GetTop(this) + (Shape.Height / 2));
                TopAnchor = new Point(Canvas.GetLeft(this) + Shape.Width / 2, Canvas.GetTop(this));
                RightAnchor = new Point(Canvas.GetLeft(this) + Shape.Width, Canvas.GetTop(this) + (Shape.Height / 2));
                BottomAnchor = new Point(Canvas.GetLeft(this) + Shape.Width / 2, Canvas.GetTop(this) + (Shape.Height));
            }
        }

        private void MakeTextBox()
        {
            Text = new TextBox
            {
                Text = Type.ToString(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Background = new SolidColorBrush { Opacity = 0 },
                BorderThickness = new Thickness(0)
            };
        }

        private void MakeShape()
        {
            switch (Type)
            {
                case ShapeType.Process:
                    Shape = new Rectangle();
                    Shape.Width = 100;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.Crimson);
                    break;
                case ShapeType.Decision:
                    Shape = new Rectangle();
                    RotateTransform rotateTransform = new RotateTransform(45);
                    Shape.RenderTransformOrigin = new Point(0.5, 0.5);
                    Shape.RenderTransform = rotateTransform;
                    Shape.Width = 75;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.LimeGreen);
                    break;
                case ShapeType.Connector:
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
