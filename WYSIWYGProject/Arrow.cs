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
        private List<Line> Lines;
        public Line line, head1, head2;
        public ShapeGrid Origin, Target;
        private Point AnchorOrigin, AnchorTarget, PathAnchor1, PathAnchor2;
        private Canvas Flowchart;

        public Arrow(Canvas flowchart, ShapeGrid origin, ShapeGrid target)
        {
            Flowchart = flowchart;
            Origin = origin;
            Target = target;
            Lines = new List<Line>();
            SetAnchors();
            Set();
        }

        public void Erase()
        {
            foreach (Line line in Lines)
            {
                Flowchart.Children.Remove(line);
            }
        }

        private void MakeArrowhead(int angleInDegrees)
        {
            Point a = AnchorOrigin, b = AnchorTarget;

            double deltaY = a.Y - b.Y;
            double deltaX = a.X - b.X;
            double angleInRadians = angleInDegrees * Math.PI / 180;

            //Linjens vinkel

            //15 är längden på pilhuvudet och 10 är vinkeln från linjen
            head1 = new Line
            {
                X1 = b.X,
                Y1 = b.Y,
                X2 = b.X + 10 * Math.Cos(angleInRadians + 10),
                Y2 = b.Y + 10 * Math.Sin(angleInRadians + 10),
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            head2 = new Line
            {
                X1 = b.X,
                Y1 = b.Y,
                X2 = b.X + 10 * Math.Cos(angleInRadians - 10),
                Y2 = b.Y + 10 * Math.Sin(angleInRadians - 10),
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
                X2 = PathAnchor1.X,
                Y2 = PathAnchor1.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Line line2 = new Line
            {
                X1 = PathAnchor1.X,
                Y1 = PathAnchor1.Y,
                X2 = PathAnchor2.X,
                Y2 = PathAnchor2.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Line line3 = new Line
            {
                X1 = PathAnchor2.X,
                Y1 = PathAnchor2.Y,
                X2 = AnchorTarget.X,
                Y2 = AnchorTarget.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Lines.Add(line);
            Lines.Add(line2);
            Lines.Add(line3);
            Lines.Add(head1);
            Lines.Add(head2);
            foreach (Line line in Lines)
            {
                line.ContextMenu = LineContextMenu();
                Flowchart.Children.Add(line);
            }
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

        //Denna metod kan innehålla onödigt många rader
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
                        PathAnchor1 = new Point((AnchorOrigin.X) - ((AnchorOrigin.X - AnchorTarget.X) / 2), AnchorOrigin.Y);
                        PathAnchor2 = new Point((AnchorOrigin.X) - ((AnchorOrigin.X - AnchorTarget.X) / 2), AnchorTarget.Y);
                        MakeArrowhead(180);
                    }
                    else
                    {
                        AnchorOrigin = Origin.TopAnchor;
                        AnchorTarget = Target.BottomAnchor;
                        PathAnchor1 = new Point(AnchorOrigin.X, (AnchorOrigin.Y) - ((AnchorOrigin.Y - AnchorTarget.Y) / 2));
                        PathAnchor2 = new Point(AnchorTarget.X, (AnchorOrigin.Y) - ((AnchorOrigin.Y - AnchorTarget.Y) / 2));
                        MakeArrowhead(270);
                    }
                }
                else
                {
                    if (targetY - originY < originX - targetX)
                    {
                        AnchorOrigin = Origin.LeftAnchor;
                        AnchorTarget = Target.RightAnchor;
                        PathAnchor1 = new Point((AnchorOrigin.X) - ((AnchorOrigin.X - AnchorTarget.X) / 2), AnchorOrigin.Y);
                        PathAnchor2 = new Point((AnchorOrigin.X) - ((AnchorOrigin.X - AnchorTarget.X) / 2), AnchorTarget.Y);
                        MakeArrowhead(180);
                    }
                    else
                    {
                        AnchorOrigin = Origin.BottomAnchor;
                        AnchorTarget = Target.TopAnchor;
                        PathAnchor1 = new Point(AnchorOrigin.X, (AnchorOrigin.Y) - ((AnchorOrigin.Y - AnchorTarget.Y) / 2));
                        PathAnchor2 = new Point(AnchorTarget.X, (AnchorOrigin.Y) - ((AnchorOrigin.Y - AnchorTarget.Y) / 2));
                        MakeArrowhead(90);
                    }
                }
            }
            else
            {
                if (originY > targetY)
                {
                    if (targetX - originX > originY - targetY)
                    {
                        AnchorOrigin = Origin.RightAnchor;
                        AnchorTarget = Target.LeftAnchor;
                        PathAnchor1 = new Point((AnchorOrigin.X) + ((AnchorTarget.X - AnchorOrigin.X) / 2), AnchorOrigin.Y);
                        PathAnchor2 = new Point((AnchorOrigin.X) + ((AnchorTarget.X - AnchorOrigin.X) / 2), AnchorTarget.Y);
                        MakeArrowhead(0);
                    }
                    else
                    {
                        AnchorOrigin = Origin.TopAnchor;
                        AnchorTarget = Target.BottomAnchor;
                        PathAnchor1 = new Point(AnchorOrigin.X, (AnchorOrigin.Y) + ((AnchorTarget.Y - AnchorOrigin.Y) / 2));
                        PathAnchor2 = new Point(AnchorTarget.X, (AnchorOrigin.Y) + ((AnchorTarget.Y - AnchorOrigin.Y) / 2));
                        MakeArrowhead(270);
                    }

                }
                else
                {
                    if (targetY - originY < targetX - originX)
                    {
                        AnchorOrigin = Origin.RightAnchor;
                        AnchorTarget = Target.LeftAnchor;
                        PathAnchor1 = new Point((AnchorOrigin.X) + ((AnchorTarget.X - AnchorOrigin.X) / 2), AnchorOrigin.Y);
                        PathAnchor2 = new Point((AnchorOrigin.X) + ((AnchorTarget.X - AnchorOrigin.X) / 2), AnchorTarget.Y);
                        MakeArrowhead(0);
                    }
                    else
                    {
                        AnchorOrigin = Origin.BottomAnchor;
                        AnchorTarget = Target.TopAnchor;
                        PathAnchor1 = new Point(AnchorOrigin.X, (AnchorOrigin.Y) + ((AnchorTarget.Y - AnchorOrigin.Y) / 2));
                        PathAnchor2 = new Point(AnchorTarget.X, (AnchorOrigin.Y) + ((AnchorTarget.Y - AnchorOrigin.Y) / 2));
                        MakeArrowhead(90);
                    }
                }
            }
        }
    }
}
