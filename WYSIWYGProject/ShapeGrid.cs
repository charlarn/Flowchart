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
        private Point GridPosition;
        public Point Position { get { return GridPosition; } set { GridPosition = value; MakeAnchors(); } }
        public Point TargetAnchor { get; set; }

        public Shape Shape;
        public TextBox Text;
        public List<Arrow> Connections;

        public ShapeType Type { get; set; }

        public ShapeGrid(ShapeType type, double x, double y)
        {
            Type = type;
            MakeTextBox();
            MakeShape();
            Children.Add(Shape);
            Children.Add(Text);
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
            //Position sätts till att vara i mitten av Shape istället för uppe i vänstra hörnet så att rätt anchors kan hittas
            Position = new Point(x + Shape.Width / 2, y + Shape.Height / 2);
            Connections = new List<Arrow>();
        }

        public void RedrawArrows(Canvas flowchart)
        {
            ShapeGrid origin; 
            ShapeGrid target; 
            for (int i = 0; i < Connections.Count; i++)
            {
                Arrow oldArrow = Connections[i];
                origin = oldArrow.Origin;
                target = oldArrow.Target;
                oldArrow.Erase();
                Arrow newArrow = new Arrow(flowchart, oldArrow.Origin, oldArrow.Target);
                //Ersätter index för oldArrow med den nya i både origin och target
                origin.Connections[origin.Connections.IndexOf(oldArrow)] = newArrow;
                target.Connections[target.Connections.IndexOf(oldArrow)] = newArrow;
            }
        }

        private void MakeAnchors()
        {
            if (Type == ShapeType.Decision)
            {
                //För att nå ut till kanterna på "diamond" figuren så läggs halva differensen mellan diagonalen och höjden på (eller dras av)
                LeftAnchor = new Point(Canvas.GetLeft(this) - ((Math.Sqrt(2) * Shape.Height) - Shape.Height)/2, Canvas.GetTop(this) + (Shape.Height / 2));
                TopAnchor = new Point(Canvas.GetLeft(this) + (Shape.Height / 2), Canvas.GetTop(this) - ((Math.Sqrt(2) * Shape.Height) - Shape.Height) / 2);
                RightAnchor = new Point(Canvas.GetLeft(this) + Shape.Height + ((Math.Sqrt(2) * Shape.Height) - Shape.Height)/2, Canvas.GetTop(this) + (Shape.Height / 2));
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
                BorderThickness = new Thickness(0),
                IsEnabled = false
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
                    Shape.Fill = new SolidColorBrush(Colors.Cornsilk);
                    break;
                case ShapeType.Decision:
                    Shape = new Rectangle();
                    RotateTransform rotateTransform = new RotateTransform(45);
                    Shape.RenderTransformOrigin = new Point(0.5, 0.5);
                    Shape.RenderTransform = rotateTransform;
                    Shape.Width = 75;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.MistyRose);
                    break;
                case ShapeType.Connector:
                    Shape = new Ellipse();
                    Shape.Width = 75;
                    Shape.Height = 75;
                    Shape.Fill = new SolidColorBrush(Colors.Honeydew);
                    break;
            }
            Shape.Stroke = new SolidColorBrush(Colors.DarkMagenta);
        }
    }
}
