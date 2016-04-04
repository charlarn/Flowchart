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
    class Arrow 
    {
        public Line line, head1, head2;
        public ShapeGrid Origin, Target;
        private Point AnchorOrigin, AnchorTarget;
        private Canvas Flowchart;

        public Arrow(Canvas flowchart, ShapeGrid origin, ShapeGrid target)
        {
            Flowchart = flowchart;
            Origin = origin;
            Target = target;
            SetAnchors();
            Set();
        }
        
        public void Erase()
        {
            Flowchart.Children.Remove(head1);
            Flowchart.Children.Remove(head2);
            Flowchart.Children.Remove(line);
        }

        private void MakeArrowhead(int side)
        {
            Point a = AnchorOrigin, b = AnchorTarget;

            double deltaY = a.Y - b.Y;
            double deltaX = a.X - b.X;

            //Linjens vinkel
            double angleInRadians = Math.Atan(deltaY / deltaX);

            //15 är längden på pilhuvudet och 10 är vinkeln från linjen
            head1 = new Line
            {
                X1 = b.X,
                Y1 = b.Y,
                X2 = b.X + 15 * Math.Cos(angleInRadians + 10) * side,
                Y2 = b.Y + 15 * Math.Sin(angleInRadians + 10) * side,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            head2 = new Line
            {
                X1 = b.X,
                Y1 = b.Y,
                X2 = b.X + 15 * Math.Cos(angleInRadians - 10) * side,
                Y2 = b.Y + 15 * Math.Sin(angleInRadians - 10) * side,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
        }

        private void Set()
        {
            line = new Line
            {
                X1 = AnchorOrigin.X,
                Y1 = AnchorOrigin.Y,
                X2 = AnchorTarget.X,
                Y2 = AnchorTarget.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            line.ContextMenu = LineContextMenu();
            Flowchart.Children.Add(line);
            Flowchart.Children.Add(head1);
            Flowchart.Children.Add(head2);
        }

        private ContextMenu LineContextMenu()
        {
            ContextMenu cm = new ContextMenu();
            MenuItem mi = new MenuItem { Header = "Delete" };
            mi.Click += new RoutedEventHandler(DeleteArrow);
            cm.Items.Add(mi);
            return cm;
        }

        private void DeleteArrow(object sender, RoutedEventArgs e)
        {
            Erase();
            Origin.Connections.Remove(this);
            Target.Connections.Remove(this);
        }

        private void SetAnchors()
        {
            double originX = Origin.Position.X, originY = Origin.Position.Y;
            double targetX = Target.Position.X, targetY = Target.Position.Y;

            //Om target är på vänster sida av origin
            if (originX > targetX)
            {
                //Om target är över origin = den övre vänstra kvadranten
                if (originY > targetY)
                {
                    //Om target är i den vänstra triangeln i kvadranten
                    if (originX - targetX > originY - targetY)
                    {
                        AnchorOrigin = Origin.LeftAnchor;
                        AnchorTarget = Target.RightAnchor;
                    }
                    else
                    {
                        AnchorOrigin = Origin.TopAnchor;
                        AnchorTarget = Target.BottomAnchor;
                    }
                }
                else
                {
                    if (targetY - originY < originX - targetX)
                    {
                        AnchorOrigin = Origin.LeftAnchor;
                        AnchorTarget = Target.RightAnchor;
                    }
                    else
                    {
                        AnchorOrigin = Origin.BottomAnchor;
                        AnchorTarget = Target.TopAnchor;
                    }
                }
                //Körs med -1 om target är på vänster sida av origin, annars 1
                MakeArrowhead(-1);
            }
            else
            {
                if (originY > targetY)
                {
                    if (targetX - originX > originY - targetY)
                    {
                        AnchorOrigin = Origin.RightAnchor;
                        AnchorTarget = Target.LeftAnchor;
                    }
                    else
                    {
                        AnchorOrigin = Origin.TopAnchor;
                        AnchorTarget = Target.BottomAnchor;
                    }

                }
                else
                {
                    if (targetY - originY < targetX - originX)
                    {
                        AnchorOrigin = Origin.RightAnchor;
                        AnchorTarget = Target.LeftAnchor;
                    }
                    else
                    {
                        AnchorOrigin = Origin.BottomAnchor;
                        AnchorTarget = Target.TopAnchor;
                    }
                }
                MakeArrowhead(1);
            }
        }
    }
}
