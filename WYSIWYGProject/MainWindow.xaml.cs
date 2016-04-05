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
        private Point mousePosition, originalPosition;
        private ShapeGrid startShape = null, endShape = null;
        private UIElement source = null;
        private bool editingActive = false, captured = false;
        private double x_Shape, x_Canvas, y_Shape, y_Canvas;

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
            startShape = null;
            endShape = null;
        }

        private void Draw(string type)
        {
            var shapeGrid = new ShapeGrid((ShapeType)Enum.Parse(typeof(ShapeType), type), mousePosition.X, mousePosition.Y);

            if (!CheckCollision(shapeGrid))
            {
                shapeGrid.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeMousedown);
                shapeGrid.MouseMove += new MouseEventHandler(ShapeMousemove);
                shapeGrid.MouseLeftButtonUp += new MouseButtonEventHandler(ShapeMouseup);
                shapeGrid.ContextMenu = ShapeContextMenu();

                FlowChart.Children.Add(shapeGrid);
            }
            else
            {
                MessageBox.Show("Collision!");
            }
        }

        private void Connect(ShapeGrid origin, ShapeGrid target)
        {
            Arrow arrow = new Arrow(FlowChart, origin, target);
            origin.Connections.Add(arrow);
            target.Connections.Add(arrow);

            startShape = null;
            endShape = null;
        }

        private ContextMenu ShapeContextMenu()
        {
            ContextMenu shapeMenu = new ContextMenu();

            shapeMenu.Items.Add(new MenuItem { Header = "Connector" });
            ((MenuItem)shapeMenu.Items[0]).Click += new RoutedEventHandler(DrawConnector);

            MenuItem colourMenu = new MenuItem
            {
                Header = "Colour",
                Items = {
                    new MenuItem { Header = "Cornsilk" },
                    new MenuItem { Header = "Honeydew" },
                    new MenuItem { Header = "MistyRose" }
                }
            };
            foreach (MenuItem item in colourMenu.Items)
            {
                item.Click += new RoutedEventHandler(ShapeColour);
            }
            shapeMenu.Items.Add(colourMenu);

            shapeMenu.Items.Add(new MenuItem { Header = "Delete" });
            ((MenuItem)shapeMenu.Items[2]).Click += new RoutedEventHandler(DeleteShapegrid);

            return shapeMenu;
        }

        private void DeleteShapegrid(object sender, RoutedEventArgs e)
        {
            ShapeGrid shapeGrid = (((ContextMenu)((((MenuItem)sender).Parent))).PlacementTarget as ShapeGrid);
            List<Arrow> ToRemove = new List<Arrow>();
            foreach (Arrow arrow in shapeGrid.Connections)
            {
                ToRemove.Add(arrow);
            }
            foreach (Arrow arrow in ToRemove)
            {
                arrow.Erase();
                arrow.Origin.Connections.Remove(arrow);
                arrow.Target.Connections.Remove(arrow);
            }


            FlowChart.Children.Remove(shapeGrid);
        }

        private void ShapeColour(object sender, RoutedEventArgs e)
        {
            //dålig rad - Hämtar contextmenus högsta parents targets första child (dvs vilken shape som menyn tillhör) 
            Shape shape = (Shape)((((ContextMenu)(((MenuItem)(((MenuItem)sender).Parent)).Parent)).PlacementTarget as Grid).Children[0]);

            switch (((MenuItem)sender).Header.ToString())
            {
                case "Cornsilk":
                    shape.Fill = Brushes.Cornsilk;
                    break;
                case "Honeydew":
                    shape.Fill = Brushes.Honeydew;
                    break;
                case "MistyRose":
                    shape.Fill = Brushes.MistyRose;
                    break;
            }
        }

        private void DrawConnector(object sender, RoutedEventArgs e)
        {
            if (startShape == null)
            {
                startShape = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as ShapeGrid;
            }
            else
            {
                endShape = ((ContextMenu)((MenuItem)sender).Parent).PlacementTarget as ShapeGrid;
                if (startShape != endShape)
                    Connect(startShape, endShape);
                else
                    startShape = null; endShape = null;
            }
        }

        private void SaveMousePosition(object sender, RoutedEventArgs e)
        {
            mousePosition = Mouse.GetPosition(FlowChart);
            if (LabelPlaceComponent.IsVisible)
                LabelPlaceComponent.Visibility = Visibility.Hidden;
        }

        private void ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            FlowChart.Children.Clear();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //Med hjälp från stackoverflow.com
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(FlowChart);
            double dpi = 96d;

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, dpi, dpi, System.Windows.Media.PixelFormats.Default);

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(FlowChart);
                dc.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                pngEncoder.Save(ms);
                ms.Close();

                System.IO.File.WriteAllBytes("Flowchart.png", ms.ToArray());
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Flowchart saved as 'Flowchart.png'!");
        }

        private void EditText_Click(object sender, RoutedEventArgs e)
        {
            List<ShapeGrid> shapeGrids = new List<ShapeGrid>();
            shapeGrids.AddRange(FlowChart.Children.OfType<ShapeGrid>());
            if (!editingActive)
            {
                Resources["EditBorder"] = Brushes.Red;
                foreach (ShapeGrid shape in shapeGrids)
                {
                    shape.Text.IsEnabled = true;
                }
                editingActive = true;
            }
            else
            {
                Resources["EditBorder"] = Brushes.Transparent;
                ButtonEditText.BorderBrush = Brushes.White;
                foreach (ShapeGrid shape in shapeGrids)
                {
                    shape.Text.IsEnabled = false;
                }
                editingActive = false;
            }
        }

        private void ShapeMousedown(object sender, RoutedEventArgs e)
        {
            source = (UIElement)sender;
            originalPosition = new Point(Canvas.GetLeft(source), Canvas.GetTop(source));
            Mouse.Capture(source);
            captured = true;
            x_Shape = Canvas.GetLeft(source);
            x_Canvas = Mouse.GetPosition(FlowChart).X;
            y_Shape = Canvas.GetTop(source);
            y_Canvas = Mouse.GetPosition(FlowChart).Y;
        }

        private void ShapeMousemove(object sender, MouseEventArgs e)
        {
            if (captured)
            {
                ShapeGrid shape = ((ShapeGrid)source);
                double x = e.GetPosition(FlowChart).X;
                double y = e.GetPosition(FlowChart).Y;
                //Dessa två variabler används för att uppdatera shapes position från mitten
                double shapePosX = shape.Position.X;
                double shapePosY = shape.Position.Y;
                x_Shape += x - x_Canvas;
                shapePosX += x - x_Canvas;
                Canvas.SetLeft(source, x_Shape);
                x_Canvas = x;
                y_Shape += y - y_Canvas;
                shapePosY += y - y_Canvas;
                Canvas.SetTop(source, y_Shape);
                y_Canvas = y;
                shape.Position = new Point(shapePosX, shapePosY);
                shape.RedrawArrows(FlowChart);
            }
        }

        private void ShapeMouseup(object sender, MouseButtonEventArgs e)
        {
            ShapeGrid shape = (ShapeGrid)sender;
            if (CheckCollision((ShapeGrid)source))
            {
                //Flytta tillbaks shape till orginalpositionen om den kolliderar
                Canvas.SetLeft(shape, originalPosition.X);
                Canvas.SetTop(shape, originalPosition.Y);
                shape.Position = new Point(originalPosition.X + shape.Shape.Width / 2, originalPosition.Y + shape.Shape.Height / 2);
                shape.RedrawArrows(FlowChart);
            }
            Mouse.Capture(null);
            captured = false;
            startShape = null;
            endShape = null;
        }

        //Ändrad så att kollisionsdetektion sker på hela rektanglar istället för en punkt
        private bool CheckCollision(ShapeGrid movingShape)
        {
            Shape shape = (Shape)movingShape.Children[0];

            double shapeX1 = Canvas.GetLeft(movingShape), shapeX2 = Canvas.GetLeft(movingShape) + shape.Width, 
                shapeY1 = Canvas.GetTop(movingShape), shapeY2 = Canvas.GetTop(movingShape) + shape.Height;

            //Om shape dras utanför canvas
            if (shapeX1 < 0 || shapeY1 < 0 || shapeX2 > FlowChart.Width || shapeY2 > FlowChart.Height)
            {
                MessageBox.Show("Out of bounds!");
                return true;
            }

            List<ShapeGrid> shapeGrids = new List<ShapeGrid>();
            shapeGrids.AddRange(FlowChart.Children.OfType<ShapeGrid>());
            shapeGrids.Remove(movingShape);

            double xCoord, yCoord, xWidth, yHeight;

            //margin gör kollisionsarean större
            double margin = 13;

            foreach (ShapeGrid shapeGrid in shapeGrids)
            {
                if (shapeGrid.Type == ShapeType.Decision)
                {
                    margin += ((Math.Sqrt(2) * shapeGrid.Shape.Width) - shapeGrid.Shape.Height) / 2 - 10;
                }
                if (movingShape.Type == ShapeType.Decision)
                {
                    margin += ((Math.Sqrt(2) * movingShape.Shape.Width) - movingShape.Shape.Height) / 2 - 10;
                }

                xCoord = Canvas.GetLeft(shapeGrid);
                yCoord = Canvas.GetTop(shapeGrid);

                xWidth = ((Shape)shapeGrid.Children[0]).Width;
                yHeight = ((Shape)shapeGrid.Children[0]).Height;

                if (xCoord - margin < shapeX1 && shapeX1 < (xCoord + xWidth) + margin && yCoord - margin < shapeY1 && shapeY1 < (yCoord + yHeight) + margin)
                {
                    MessageBox.Show("Collison!");
                    return true;
                }
                else if (xCoord - margin < shapeX2 && shapeX2 < (xCoord + xWidth) + margin && yCoord - margin < shapeY1 && shapeY1 < (yCoord + yHeight) + margin)
                {
                    MessageBox.Show("Collison!");
                    return true;
                }
                else if (xCoord - margin < shapeX1 && shapeX1 < (xCoord + xWidth) + margin && yCoord - margin < shapeY2 && shapeY2 < (yCoord + yHeight) + margin)
                {
                    MessageBox.Show("Collison!");
                    return true;
                }
                else if (xCoord - margin < shapeX2 && shapeX2 < (xCoord + xWidth) + margin && yCoord - margin < shapeY2 && shapeY2 < (yCoord + yHeight) + margin)
                {
                    MessageBox.Show("Collison!");
                    return true;
                }
            }
            return false;
        }
    }
}
