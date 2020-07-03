using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using GameObjectEditor.GameComponent;
using GameObjectEditor.WPFRubbish;
using GameObjectEditor.Layouts;
using Microsoft.Win32;



namespace GameObjectEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        String gamepath = @"C:\GitHub\Fox1\Game\GuruEngineTest\GuruEngineTest\GuruEngine\ECS";
        String InitialDirectory = @"C:\Data\Fox1";
        Dictionary<String, GameComponentDescriptor> componentTypes = new Dictionary<string, GameComponentDescriptor>();
        private ScaleTransform _scaleTransform;
        private TransformGroup _transformGroup;
        private TranslateTransform _translateTransform;
        private Point _pointOnClick; // Click Position for panning

        Dictionary<String, int> Counters = new Dictionary<string, int>();

        List<GameComponentDescriptor> activeComponents = new List<GameComponentDescriptor>();

        Button start_button;

        ConnectionManager manager;

        public MainWindow()
        {
            InitializeComponent();

            #region Parse the source tree for components
            DirectoryInfo di = new DirectoryInfo(gamepath);
            TreeViewItem root = new TreeViewItem();
            root.Header = "Library";
            ComponentList.Items.Add(root);

            FullDirList(di, "*.cs", root);
            #endregion

            _translateTransform = new TranslateTransform();
            _scaleTransform = new ScaleTransform();
            _transformGroup = new TransformGroup();
            _transformGroup.Children.Add(_scaleTransform);
            _transformGroup.Children.Add(_translateTransform);

            Display.LayoutTransform = _transformGroup;

            this.MouseWheel += MainWindow_MouseWheel;
            Display.MouseRightButtonDown += Display_MouseRightButtonDown;
            Display.MouseRightButtonUp += Display_MouseRightButtonUp;
            Display.MouseMove += Display_MouseMove;

            manager = new ConnectionManager();
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region Scan for valid components
        List<FileInfo> files = new List<FileInfo>();                // List that will hold the files and subfiles in path
        List<DirectoryInfo> folders = new List<DirectoryInfo>();    // List that hold directories that cannot be accessed

        void FullDirList(DirectoryInfo dir, string searchPattern, TreeViewItem root)
        {
            // list the files
            try
            {
                foreach (FileInfo f in dir.GetFiles(searchPattern))
                {
                    TreeViewItem tn = new TreeViewItem();
                    StackPanel sp = new StackPanel();
                    sp.Orientation = Orientation.Horizontal;
                    Image image = new Image();
                    image.Source = new BitmapImage(new Uri("pack://application:,,/images/component.png"));
                    image.Width = 16;
                    image.Height = 16;
                    // Label
                    Label lbl = new Label();
                    lbl.Content = f.Name;


                    // Add into stack
                    sp.Children.Add(image);
                    sp.Children.Add(lbl);

                    tn.Header = sp;
                    if (ValidateFile(f, tn))
                    {
                        files.Add(f);
                        root.Items.Add(tn);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Directory {0}  \n could not be accessed!!!!", dir.FullName);
                return;  // We alredy got an error trying to access dir so dont try to access it again
            }

            // process each directory
            // If I have been able to see the files in the directory I should also be able 
            // to look at its directories so I dont think I should place this in a try catch block
            foreach (DirectoryInfo d in dir.GetDirectories())
            {
                folders.Add(d);
                TreeViewItem next = new TreeViewItem();
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                Image image = new Image();
                image.Source = new BitmapImage(new Uri("pack://application:,,/images/directory.png"));
                image.Width = 16;
                image.Height = 16;
                // Label
                Label lbl = new Label();
                lbl.Content = d.Name;


                // Add into stack
                sp.Children.Add(image);
                sp.Children.Add(lbl);

                next.Header = sp;
                root.Items.Add(next);
                FullDirList(d, searchPattern, next);
            }

        }

        bool ValidateFile(FileInfo file, TreeViewItem tn)
        {
            bool ret = false;
            GameComponentDescriptor desc = null;
            try
            {
                string line = null;
                System.IO.TextReader readFile = new StreamReader(file.FullName);
                bool parsing = true;
                while (parsing)
                {

                    line = readFile.ReadLine();
                    if (line != null)
                    {
                        if (line.StartsWith("//("))
                        {
                            string command = line.Substring(4);
                            int fs = command.IndexOf(' ');
                            command = command.Substring(0, fs);

                            if (line.Contains("Class"))
                            {
                                // a new game component class
                                desc = new GameComponentDescriptor();
                                string[] parts = line.Split(' ');
                                desc.Name = parts[2];
                                componentTypes.Add(desc.Name, desc);
                                tn.ToolTip = line;
                                tn.ToolTip += "\n";
                                tn.Tag = desc;
                                GameComponentConnection gc = new GameComponentConnection();
                                gc.Name = "Root";
                                gc.Type = "Root";
                                desc.Connections.Add(gc);
                            }
                            if (command == "Group")
                            {
                                string[] parts = line.Split(' ');
                                desc.Group = parts[2];
                                tn.ToolTip += line;
                                tn.ToolTip += "\n";
                            }
                            if (command == "Comment")
                            {
                                string[] parts = line.Split('"');
                                desc.Comments.Add(parts[1]);
                            }
                            if (line.Contains("Preset"))
                            {
                                desc.Presets.Add(line);
                            }
                            if (command == "Type")
                            {
                                string[] parts = line.Split(' ');
                                desc.Type = parts[2];
                                tn.ToolTip += line;
                                tn.ToolTip += "\n";
                            }
                            if (line.Contains("ConnectionList"))
                            {
                                string[] parts = line.Split(' ');
                                GameComponentConnection gc = new GameComponentConnection();
                                gc.Name = parts[3];
                                gc.Type = parts[2];
                                desc.ListConnections.Add(gc);
                                desc.ActualListConnections.Add(gc.Name, new List<GameComponentConnection>());
                                tn.ToolTip += line;
                                tn.ToolTip += "\n";
                            }
                            else if (line.Contains("Connection"))
                            {
                                string[] parts = line.Split(' ');
                                GameComponentConnection gc = new GameComponentConnection();
                                gc.Name = parts[3];
                                gc.Type = parts[2];
                                desc.Connections.Add(gc);
                                tn.ToolTip += line;
                                tn.ToolTip += "\n";
                            }
                            else if (line.Contains("Parameter"))
                            {
                                string[] parts = line.Split(' ');
                                GameComponentParameter gacp = new GameComponentParameter();
                                gacp.Name = parts[3];
                                gacp.Type = parts[2];
                                desc.Parameters.Add(gacp);
                                tn.ToolTip += line;
                                tn.ToolTip += "\n";
                            }

                            ret = true;
                        }

                    }
                    else
                    {
                        parsing = false;
                    }
                }
                readFile.Close();
                readFile = null;

            }
            catch (IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return ret;
        }
        #endregion

        #region Drag and drop
        Point _startPoint;
        bool _IsDragging = false;

        private void List_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Store the mouse position
            _startPoint = e.GetPosition(null);
        }

        private void List_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_IsDragging)
            {
                Point position = e.GetPosition(null);
                if (Math.Abs(position.X - _startPoint.X) >
                        SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - _startPoint.Y) >
                        SystemParameters.MinimumVerticalDragDistance)
                {
                    StartDrag(e);
                }
            }
        }

        private void StartDrag(MouseEventArgs e)
        {
            _IsDragging = true;
            object temp = this.ComponentList.SelectedItem;
            if (temp == null)
                return;

            DataObject data = null;

            data = new DataObject("inadt", temp);

            if (data != null)
            {
                DragDropEffects dde = DragDropEffects.Copy;
                DragDropEffects de = DragDrop.DoDragDrop(this.ComponentList, data, dde);
            }
            _IsDragging = false;
        }

        private void droponcanvas(object sender, DragEventArgs e)
        {
            TreeViewItem t = ComponentList.SelectedItem as TreeViewItem;
            if (t.Tag != null)
            {
                GameComponentDescriptor desc = t.Tag as GameComponentDescriptor;
                int n = 1;
                if (Counters.Keys.Contains(desc.Name))
                {
                    Counters[desc.Name]++;
                    n = Counters[desc.Name];
                }
                else
                {
                    Counters.Add(desc.Name, n);
                }
                GameComponentDescriptor ndesc = new GameComponentDescriptor(desc, n);
                activeComponents.Add(ndesc);
                Frame f = ndesc.BuildFrame();

                foreach (GameComponentLinkButton glb in ndesc.Links)
                {
                    glb.Button.Click += Button_Click;
                    glb.Button.Tag = glb;
                }

                Point p = e.GetPosition(Display);
                Canvas.SetTop(f, p.Y);
                Canvas.SetLeft(f, p.X);
                Display.Children.Add(f);
            }
        }

        #endregion

        #region Link handling
        bool AddingLine = false;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (AddingLine)
            {
                AddingLine = false;
                Button end = (Button)(sender);

                manager.AddConnection(Display, start_button, end);
                Mouse.OverrideCursor = null;
            }
            else
            {
                AddingLine = true;
                start_button = (Button)(sender);
                Mouse.OverrideCursor = Cursors.Hand;
            }
        }
        #endregion

        #region Display handling

        double zoom = 1;

        private void MainWindow_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point mousePosition = e.GetPosition(Display);
            double zoomScale = 0.1;
            double valZoom = e.Delta > 0 ? zoomScale : -zoomScale;
            Point pointOnMove = e.GetPosition((FrameworkElement)Display.Parent);

            Display.RenderTransformOrigin = new Point(mousePosition.X / Display.ActualWidth, mousePosition.Y / Display.ActualHeight);

            zoom += valZoom;
            zoom = Math.Max(zoom, 0.1);

            Zoom(new Point(mousePosition.X, mousePosition.Y),zoom);
          

        }

        /// Zoom function
        private void Zoom(Point point, double scale)
        {
            double centerX = (point.X - _translateTransform.X) / _scaleTransform.ScaleX;
            double centerY = (point.Y - _translateTransform.Y) / _scaleTransform.ScaleY;
            _scaleTransform.ScaleX = scale;
            _scaleTransform.ScaleY = scale;
            _translateTransform.X = point.X - centerX * _scaleTransform.ScaleX;
            _translateTransform.Y = point.Y - centerY * _scaleTransform.ScaleY;


        }
        bool panning = false;


        private void Display_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!panning)
            {
                panning = true;
                _pointOnClick = e.GetPosition((FrameworkElement)Display.Parent);

 
            }
        }

        private void Display_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            panning = false;
        }
        private void Display_MouseMove(object sender, MouseEventArgs e)
        {
            if (panning)
            {
                //Point on move from Parent
                Point pointOnMove = e.GetPosition((FrameworkElement)Display.Parent);

                double dx = (_pointOnClick.X - pointOnMove.X) / _scaleTransform.ScaleX;
                double dy = (_pointOnClick.Y - pointOnMove.Y) / _scaleTransform.ScaleY;

                foreach (UIElement ue in Display.Children)
                {
                    Canvas.SetTop(ue, Canvas.GetTop(ue) - dy);
                    Canvas.SetLeft(ue, Canvas.GetLeft(ue) - dx);
                }

                _pointOnClick = e.GetPosition((FrameworkElement)Display.Parent);

                e.Handled = true;

            }
            if (AddingLine)
            {
                //if (arcPath != null)
                //{
                //    Display.Children.Remove(arcPath);
                //}
                //Point startPoint = start_button.TransformToAncestor(Display).Transform(new Point(0, 0));
                //Point endPoint =  e.GetPosition((FrameworkElement)Display);
                //startPoint.X += 8;
                //startPoint.Y += 8;
                //CreateBezierSegment(startPoint, endPoint);
            }
            
        }





        #endregion

        #region Layout

        private void FSAAOverlap(object sender, RoutedEventArgs e)
        {
            if (activeComponents.Count > 0)
            {
                
            }
        }

        private void BFLayout(object sender, RoutedEventArgs e)
        {
            if (activeComponents.Count > 0)
            {
                BruteForceLayout bfl = new BruteForceLayout();
                foreach(GameComponentDescriptor g in activeComponents)
                {
                    bfl.AddVertex(g);
                }
                bfl.Run();
            }
        }

        private void KKLayout(object sender, RoutedEventArgs e)
        {
            if (activeComponents.Count > 0)
            {
               

            }
        }
        #endregion

        #region File IO
        public void OpenGameObject(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Game Objects(*.gameobject)|*.gameobject";
            ofd.DefaultExt = ".gameobject";
            ofd.InitialDirectory = InitialDirectory;
            int x = 100;
            int y = 100;
            if (ofd.ShowDialog()== true)
            {
                activeComponents.Clear();
                String Name = System.IO.Path.GetFileNameWithoutExtension(ofd.FileName);

                using (TextReader reader = File.OpenText(ofd.FileName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        GameComponentDescriptor ndesc = new GameComponentDescriptor();

                        ndesc.Load(line, reader);

                        activeComponents.Add(ndesc);
                        Frame f = ndesc.BuildFrame();

                        foreach (GameComponentLinkButton glb in ndesc.Links)
                        {
                            glb.Button.Click += Button_Click;
                            glb.Button.Tag = glb;
                        }

                        Point p = new Point(x, y);
                        Canvas.SetTop(f, p.Y);
                        Canvas.SetLeft(f, p.X);
                        Display.Children.Add(f);
                        x += 10;
                        y += 5;
                    }
                }
            }

           
        }
        #endregion

        #region Helpers
        GameComponentDescriptor FindComponentByName(String name)
        {
            foreach(GameComponentDescriptor gcd in activeComponents)
            {
                if (gcd.Name == name)
                    return gcd;
            }
            return null;
        }

        public void Reconnect(object sender, RoutedEventArgs args)
        {
            foreach (GameComponentDescriptor gcd in activeComponents)
            {
                String con1 = gcd.Name;

                foreach (GameComponentConnection gcc in gcd.Connections)
                {
                    String pin1 = gcc.Name;
                    if (pin1 != "Root")
                    {
                        String temp = gcc.ConnectedTo;
                        if (temp != "")
                        {
                            string comp, name;

                            if (pin1 == "Collision")
                            {
                                comp = temp;
                                name = "Root";
                            }
                            else
                            {
                                String[] parts = temp.Split(':');
                                comp = parts[0];
                                name = parts[1];
                            }
                            
                            if (!manager.DoesConnectionExist(con1, pin1, comp, name))
                            {
                                Button b1 = gcd.Pins[pin1];
                                GameComponentDescriptor gcd2 = FindComponentByName(comp);
                                Button b2 = gcd2.Pins[name];
                                manager.AddConnection(Display, b1, b2);
                            }
                        }
                    }

                }
            }
        }
        #endregion
    }
}
