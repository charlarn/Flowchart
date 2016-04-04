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
        private ShapeGrid startShape = null, endShape = null;
        private List<ShapeGrid> gridCollection = new List<ShapeGrid>();
        private bool editingActive = false;

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
            //Keyboard.ClearFocus();
            //  startShape = null;
            //  endShape = null;
        }

        private void Draw(string type)
        {
            var shapeGrid = new ShapeGrid((ShapeType)Enum.Parse(typeof(ShapeType), type), mousePosition.X, mousePosition.Y);
            gridCollection.Add(shapeGrid);

            shapeGrid.MouseLeftButtonDown += new MouseButtonEventHandler(ShapeMousedown);
            shapeGrid.MouseMove += new MouseEventHandler(ShapeMousemove);
            shapeGrid.MouseLeftButtonUp += new MouseButtonEventHandler(ShapeMouseup);
            shapeGrid.ContextMenu = ShapeContextMenu();

            FlowChart.Children.Add(shapeGrid);
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
            Arrow arrow;
            for(int i = 0; i < shapeGrid.Connections.Count; i++)
            {

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
                Connect(startShape, endShape);
            }
        }

        private void SaveMousePosition(object sender, RoutedEventArgs e)
        {
            mousePosition = Mouse.GetPosition(FlowChart);
        }

        bool captured = false;
        double x_shape, x_canvas, y_shape, y_canvas;
        Point originalPosition;
        UIElement source = null;

        private void ClearBoard_Click(object sender, RoutedEventArgs e)
        {
            FlowChart.Children.Clear();
        }

        private void EditText_Click(object sender, RoutedEventArgs e)
        {
            List<ShapeGrid> shapeGrids = new List<ShapeGrid>();
            shapeGrids.AddRange(FlowChart.Children.OfType<ShapeGrid>());
            if (!editingActive)
            {
                ButtonEditText.BorderBrush = Brushes.Red;
                foreach (ShapeGrid shape in shapeGrids)
                {
                    shape.Text.IsEnabled = true;
                }
                editingActive = true;
            }
            else
            {
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
            x_shape = Canvas.GetLeft(source);
            x_canvas = Mouse.GetPosition(FlowChart).X;
            y_shape = Canvas.GetTop(source);
            y_canvas = Mouse.GetPosition(FlowChart).Y;
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
                x_shape += x - x_canvas;
                shapePosX += x - x_canvas;
                Canvas.SetLeft(source, x_shape);
                x_canvas = x;
                y_shape += y - y_canvas;
                shapePosY += y - y_canvas;
                Canvas.SetTop(source, y_shape);
                y_canvas = y;
                shape.Position = new Point(shapePosX, shapePosY);
                shape.RedrawArrows(FlowChart);
            }
        }

        private void ShapeMouseup(object sender, MouseButtonEventArgs e)
        {
            ShapeGrid shape = (ShapeGrid)sender;
            if (CheckCollision())
            {
                //Flytta tillbaks shape till orginalpositionen om den kolliderar
                MessageBox.Show("Collison!");
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

        //Ändrad så att kollisionsdetektion sker på hela rektanglar istället för punkter
        private bool CheckCollision()
        {
            ShapeGrid movingShape = (ShapeGrid)source;
            List<ShapeGrid> shapeGrids = new List<ShapeGrid>();
            shapeGrids.AddRange(FlowChart.Children.OfType<ShapeGrid>());
            shapeGrids.Remove(movingShape);

            Shape shape = (Shape)movingShape.Children[0];

            double xCoord, yCoord, xWidth, yHeight;

            //padding gör kollisionsarean större
            double padding = 13;

            double shapeX1 = Canvas.GetLeft(movingShape), shapeX2 = Canvas.GetLeft(movingShape) + shape.Width,
                shapeY1 = Canvas.GetTop(movingShape), shapeY2 = Canvas.GetTop(movingShape) + shape.Height;

            foreach (ShapeGrid shapeGrid in shapeGrids)
            {
                if (shapeGrid.Type == ShapeType.Decision)
                {
                    padding += ((Math.Sqrt(2) * shapeGrid.Shape.Width) - shapeGrid.Shape.Height) / 2 - 8;
                }
                if (movingShape.Type == ShapeType.Decision)
                {
                    padding += ((Math.Sqrt(2) * movingShape.Shape.Width) - movingShape.Shape.Height) / 2 - 8;
                }

                xCoord = Canvas.GetLeft(shapeGrid);
                yCoord = Canvas.GetTop(shapeGrid);

                xWidth = ((Shape)shapeGrid.Children[0]).Width;
                yHeight = ((Shape)shapeGrid.Children[0]).Height;

                if (xCoord - padding < shapeX1 && shapeX1 < (xCoord + xWidth) + padding && yCoord - padding < shapeY1 && shapeY1 < (yCoord + yHeight) + padding)
                {
                    return true;
                }
                else if (xCoord - padding < shapeX2 && shapeX2 < (xCoord + xWidth) + padding && yCoord - padding < shapeY1 && shapeY1 < (yCoord + yHeight) + padding)
                {
                    return true;
                }
                else if (xCoord - padding < shapeX1 && shapeX1 < (xCoord + xWidth) + padding && yCoord - padding < shapeY2 && shapeY2 < (yCoord + yHeight) + padding)
                {
                    return true;
                }
                else if (xCoord - padding < shapeX2 && shapeX2 < (xCoord + xWidth) + padding && yCoord - padding < shapeY2 && shapeY2 < (yCoord + yHeight) + padding)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
