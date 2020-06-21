using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GameObjectEditor.GameComponent
{
    public class GameComponentDescriptor
    {
        public String Name;
        public String Type;
        public String Group = "Generic";
        public List<String> Comments = new List<string>();
        public List<String> Presets = new List<string>();
        public List<GameComponentConnection> Connections = new List<GameComponentConnection>();
        public List<GameComponentConnection> ListConnections = new List<GameComponentConnection>();
        public Dictionary<String, List<GameComponentConnection>> ActualListConnections = new Dictionary<string, List<GameComponentConnection>>();
        public List<GameComponentParameter> Parameters = new List<GameComponentParameter>();
        public List<GameComponentLinkButton> Links = new List<GameComponentLinkButton>();

        public Dictionary<Button, String> Buttons = new Dictionary<Button, string>();
        public Dictionary<String, Button> Pins = new Dictionary<string,Button>();

        Frame holder;

        #region Creators
        public GameComponentDescriptor()
        {

        }

        public GameComponentDescriptor(GameComponentDescriptor other, int n)
        {
            Name = String.Format("{0}_{1}", other.Name, n);
            Type = other.Type;
            foreach (GameComponentConnection gcc in other.Connections)
            {
                GameComponentConnection ngcc = new GameComponentConnection();
                ngcc.Type = gcc.Type;
                ngcc.Name = gcc.Name;
                ngcc.Parent = Name;
                Connections.Add(ngcc);
            }
            foreach (GameComponentConnection gcc in other.ListConnections)
            {
                GameComponentConnection ngcc = new GameComponentConnection();
                ngcc.Type = gcc.Type;
                ngcc.Name = gcc.Name;
                ngcc.Parent = Name;
                ListConnections.Add(ngcc);
            }
            foreach (GameComponentParameter gcp in other.Parameters)
            {
                Parameters.Add(new GameComponentParameter(gcp));
            }
            foreach (String s in other.Comments)
            {
                Comments.Add(s);
            }
            foreach (String s in other.Presets)
            {
                Presets.Add(s);
            }

        }
        #endregion

        public Frame BuildFrame()
        {
            Frame res = new Frame();
            res.Background = new SolidColorBrush(Colors.Aqua);
            res.BorderThickness = new System.Windows.Thickness(3);
            res.BorderBrush = new SolidColorBrush(Colors.Black);

            StackPanel sp = new StackPanel();

            TextBox t = new TextBox();
            t.Text = Name;
            t.BorderBrush = new SolidColorBrush(Colors.Black);
            t.BorderThickness = new System.Windows.Thickness(1);
            sp.Children.Add(t);

            Label l = new Label();
            l.Content = Type;
            sp.Children.Add(l);

            if (Connections.Count > 0)
            {
                l = new Label();
                l.Content = "Connections";
                l.Background = new SolidColorBrush(Colors.BlanchedAlmond);
                sp.Children.Add(l);

                StackPanel sp2 = new StackPanel();
                sp.Children.Add(sp2);
                sp2.Background = new SolidColorBrush(Colors.AliceBlue);
                foreach (GameComponentConnection s in Connections)
                {
                    Grid sp3 = new Grid();
                    sp3.Width = 300;
                    ColumnDefinition col1 = new ColumnDefinition();
                    col1.Width = new GridLength(30);
                    ColumnDefinition col2 = new ColumnDefinition();
                    col2.Width = new GridLength(120);
                    ColumnDefinition col3 = new ColumnDefinition();
                    col3.Width = new GridLength(1, System.Windows.GridUnitType.Star);

                    sp3.ColumnDefinitions.Add(col1);
                    sp3.ColumnDefinitions.Add(col2);
                    sp3.ColumnDefinitions.Add(col3);

                    l = new Label();
                    l.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    l.Content = s.Name;
                    l.Background = new SolidColorBrush(Colors.AliceBlue);
                    Grid.SetColumn(l, 1);
                    sp3.Children.Add(l);

                    Button b = new Button();
                    b.Width = 18;
                    b.Height = 18;
                    b.Tag = this;
                    Image im = new Image();
                    im.Source = new BitmapImage(new Uri("pack://application:,,/images/tag.png"));
                    im.Width = 16;
                    im.Height = 16;
                    b.Content = im;
                    Grid.SetColumn(b, 0);
                    sp3.Children.Add(b);
                    Buttons.Add(b, s.Name);
                    Pins.Add(s.Name, b);

                    GameComponentLinkButton lb = new GameComponentLinkButton();
                    lb.Parent = this;
                    lb.Connection = s;
                    lb.Button = b;
                    Links.Add(lb);

                    sp2.Children.Add(sp3);
                }
            }
            if (ListConnections.Count > 0)
            {
                l = new Label();
                l.Content = "List Connections";
                l.Background = new SolidColorBrush(Colors.BlanchedAlmond);
                sp.Children.Add(l);

                StackPanel sp2 = new StackPanel();
                sp.Children.Add(sp2);
                sp2.Background = new SolidColorBrush(Colors.AliceBlue);
                foreach (GameComponentConnection s in ListConnections)
                {
                    Grid sp3 = new Grid();
                    sp3.Width = 300;
                    ColumnDefinition col1 = new ColumnDefinition();
                    col1.Width = new GridLength(30);
                    ColumnDefinition col2 = new ColumnDefinition();
                    col2.Width = new GridLength(120);
                    ColumnDefinition col3 = new ColumnDefinition();
                    col3.Width = new GridLength(1, System.Windows.GridUnitType.Star);

                    sp3.ColumnDefinitions.Add(col1);
                    sp3.ColumnDefinitions.Add(col2);
                    sp3.ColumnDefinitions.Add(col3);

                    l = new Label();
                    l.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    l.Content = s.Name;
                    l.Background = new SolidColorBrush(Colors.AliceBlue);
                    Grid.SetColumn(l, 1);
                    sp3.Children.Add(l);

                    Button b = new Button();
                    b.Width = 18;
                    b.Height = 18;
                    Image im = new Image();
                    im.Source = new BitmapImage(new Uri("pack://application:,,/images/tag.png"));
                    im.Width = 16;
                    im.Height = 16;
                    b.Content = im;
                    Grid.SetColumn(b, 2);
                    sp3.Children.Add(b);
                    Buttons.Add(b, s.Name);
                    Pins.Add(s.Name, b);

                    GameComponentLinkButton lb = new GameComponentLinkButton();
                    lb.Parent = this;
                    lb.Connection = s;
                    lb.Button = b;
                    Links.Add(lb);

                    sp2.Children.Add(sp3);
                }
            }

            if (Presets.Count > 0)
            {
                l = new Label();
                l.Content = "Presets";
                l.Background = new SolidColorBrush(Colors.AliceBlue);
                sp.Children.Add(l);

                StackPanel sp2 = new StackPanel();
                sp.Children.Add(sp2);
                sp2.Background = new SolidColorBrush(Colors.AliceBlue);
                ComboBox c1 = new ComboBox();

                foreach (String s in Presets)
                {
                    String[] parts = s.Split(' ');
                    c1.Items.Add(parts[2]);
                }
                sp2.Children.Add(c1);
                c1.SelectionChanged += SelectionChanged;
            }



            if (Parameters.Count > 0)
            {
                l = new Label();
                l.Content = "Parameters";
                l.Background = new SolidColorBrush(Colors.BlanchedAlmond);
                sp.Children.Add(l);

                StackPanel sp2 = new StackPanel();
                sp.Children.Add(sp2);
                sp2.Background = new SolidColorBrush(Colors.AliceBlue);
                foreach (GameComponentParameter s in Parameters)
                {
                    Grid sp3 = new Grid();
                    sp3.Width = 300;
                    ColumnDefinition col1 = new ColumnDefinition();
                    col1.Width = new GridLength(150);
                    ColumnDefinition col2 = new ColumnDefinition();
                    col2.Width = new GridLength(1, System.Windows.GridUnitType.Star);

                    sp3.ColumnDefinitions.Add(col1);
                    sp3.ColumnDefinitions.Add(col2);

                    l = new Label();
                    l.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    l.Content = s.Name;
                    l.Background = new SolidColorBrush(Colors.AliceBlue);
                    Grid.SetColumn(l, 0);
                    sp3.Children.Add(l);

                    TextBox t1 = new TextBox();
                    t1.MinWidth = 50;
                    s.textbox = t1;

                    t1.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                    t1.Text = s.GetValue();
                    t1.Name = Name + s.Name;
                    Grid.SetColumn(t1, 1);
                    sp3.Children.Add(t1);


                    sp2.Children.Add(sp3);
                }
            }
            
            res.Content = sp;
            res.MouseLeftButtonDown += Res_MouseDown;
            res.MouseLeftButtonUp += Res_MouseLeftButtonUp;
            res.MouseMove += Res_MouseMove;
            holder = res;
            return res;
        }

        private void Res_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (dragging )
            {
                var mouseWithinParent = e.GetPosition(holder.Parent as UIElement);

                Canvas.SetLeft(holder, mouseWithinParent.X - sp.X);
                Canvas.SetTop(holder, mouseWithinParent.Y - sp.Y);
            }

            
        }

        private void Res_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            dragging = false;
            holder.ReleaseMouseCapture();
        }

        bool dragging = false;
        Point sp;

        private void Res_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!dragging)
            {

                dragging = true;
                sp = e.GetPosition(holder as UIElement); 
                holder.CaptureMouse();

            }
        }

        public void SelectionChanged(object sender, EventArgs e)
        {
            ComboBox SelectBox = (ComboBox)sender;
            if (SelectBox.SelectedIndex >= 0)
            {
                //( Preset Browning303 0.01066849 4.442131E-005F 835 0 0.0018 0 2.5 false false 20mmRed SmokeBlack_BulletTrail2 0 0xd90000ff )

                String s = Presets[SelectBox.SelectedIndex];
                String[] parts = s.Split(' ');
                int start = 3;
                foreach(GameComponentParameter gcp in Parameters)
                {
                    gcp.textbox.Text = parts[start];
                    start++;
                }
            }
        }

        public void Load(String name, TextReader reader)
        {
            Name = name;
            Type = reader.ReadLine();
            int nc = int.Parse(reader.ReadLine());
            for (int i = 0; i < nc; i++)
            {
                String line = reader.ReadLine();
                string[] parts = line.Split('#');
                GameComponentConnection gcc = new GameComponentConnection();
                gcc.Parent = name;
                gcc.Type = parts[0];
                gcc.Name = parts[1];
                gcc.ConnectedTo = parts[2];
                Connections.Add(gcc);

            }
            nc = int.Parse(reader.ReadLine());
            for (int i = 0; i < nc; i++)
            {
                GameComponentConnection gcc = new GameComponentConnection();

                String line = reader.ReadLine();
                string[] parts = line.Split('#');
                gcc.Name = parts[0];

                gcc.Parent = name;
                string[] cons = parts[1].Split(',');
                for (int j = 0; j < cons.Length - 1; j++)
                {
                    gcc.AllConnections.Add(cons[j]);
                }
                ListConnections.Add(gcc);
            }
            nc = int.Parse(reader.ReadLine());

            for (int i = 0; i < nc; i++)
            {
                String line = reader.ReadLine();
                string[] parts = line.Split(' ');
                GameComponentParameter gcp = new GameComponentParameter();
                gcp.Name = parts[0];
                gcp.Type = parts[1];
                gcp.SetValue(parts[2]);
                Parameters.Add(gcp);
            }
            

        }

        public Size GetSize()
        {
            Size s = new Size();

            s.Width = holder.ActualWidth;
            s.Height = holder.ActualHeight;

            return s;
        }

        public Point GetPosition()
        {
            Point p = new Point(0,0);
            return holder.PointToScreen(p);
        }

        public void SetPosition(float x, float y)
        {
            Canvas.SetLeft(holder, x);
            Canvas.SetTop(holder, y);
        }
    }
}
