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
        private GridShape Origin, Target;

        public Arrow(GridShape origin, GridShape target)
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
