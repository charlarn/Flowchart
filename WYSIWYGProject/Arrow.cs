using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WYSIWYGProject
{
    class Arrow 
    {
        private Line line;
        private ShapeGrid Origin, Target;

        public Arrow(ShapeGrid origin, ShapeGrid target)
        {
            Origin = origin;
            Target = target;
            Draw();
        }

        public Line Draw()
        {
            Line line = new Line
            {
                X1 = Origin.TopAnchor.X,
                Y1 = Origin.TopAnchor.Y,
                X2 = Target.BottomAnchor.X,
                Y2 = Target.BottomAnchor.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            return line;
        }

        private void GetBestAnchor()
        {

        }
    }
}
