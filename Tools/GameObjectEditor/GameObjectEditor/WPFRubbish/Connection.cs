using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using GameObjectEditor.GameComponent;

namespace GameObjectEditor.WPFRubbish
{
    public class Connection
    {
        public Button Start;
        public Button End;
        public Path ArcPath;
        public String Component1;
        public String Component2;
        public String Pin1;
        public String Pin2;

        Point startPoint;
        Point endPoint;

        Canvas Display;

        public Connection(Canvas canvas)
        {
            Display = canvas;
        }

        public void Setup()
        {
            startPoint = Start.TransformToAncestor(Display).Transform(new Point(0, 0));
            endPoint = End.TransformToAncestor(Display).Transform(new Point(0, 0));
            startPoint.X += 8;
            startPoint.Y += 8;
            endPoint.X += 8;
            endPoint.Y += 8;
            ArcPath = CreateBezierSegment(startPoint, endPoint);

            Display.Children.Add(ArcPath);

            Start.LayoutUpdated += Start_LayoutUpdated;
            End.LayoutUpdated += Start_LayoutUpdated;


            GameComponentLinkButton gcd1 = (GameComponentLinkButton)Start.Tag;
            Component1 = gcd1.Parent.Name;
            Pin1 = gcd1.Parent.Buttons[Start];

            GameComponentLinkButton gcd2 = (GameComponentLinkButton)End.Tag;
            Component2 = gcd2.Parent.Name;
            Pin2 = gcd2.Parent.Buttons[End];

        }

        private void Start_LayoutUpdated(object sender, EventArgs e)
        {

            Point sPoint = Start.TransformToAncestor(Display).Transform(new Point(0, 0));
            Point ePoint = End.TransformToAncestor(Display).Transform(new Point(0, 0));
            sPoint.X += 8;
            sPoint.Y += 8;
            ePoint.X += 8;
            ePoint.Y += 8;

            if ((sPoint == startPoint) && (ePoint == endPoint))
                return;

            Display.Children.Remove(ArcPath);
            startPoint = sPoint;
            endPoint = ePoint;

            ArcPath = CreateBezierSegment(sPoint, ePoint);
            Display.Children.Add(ArcPath);
        }

        private Path CreateBezierSegment(Point start, Point End)
        {
            Path arcPath;
            PathFigure pthFigure = new PathFigure();
            pthFigure.StartPoint = start;

            System.Windows.Point Point1 = new System.Windows.Point(start.X - 100, start.Y);
            System.Windows.Point Point2 = new System.Windows.Point(End.X + 100, End.Y);
            System.Windows.Point Point3 = new System.Windows.Point(End.X, End.Y);

            BezierSegment bzSeg = new BezierSegment();
            bzSeg.Point1 = Point1;
            bzSeg.Point2 = Point2;
            bzSeg.Point3 = Point3;


            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(bzSeg);

            pthFigure.Segments = myPathSegmentCollection;

            PathFigureCollection pthFigureCollection = new PathFigureCollection();
            pthFigureCollection.Add(pthFigure);

            PathGeometry pthGeometry = new PathGeometry();
            pthGeometry.Figures = pthFigureCollection;

            arcPath = new System.Windows.Shapes.Path();
            arcPath.Stroke = new SolidColorBrush(Colors.Black);
            arcPath.StrokeThickness = 3;
            arcPath.Data = pthGeometry;
            //arcPath.Fill = new SolidColorBrush(Colors.Yellow);
            Canvas.SetZIndex(arcPath, 1);

            return arcPath;
        }

    }
}
