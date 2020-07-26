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
using System.Windows.Shapes;

using GameObjectEditor.GameComponent;

namespace GameObjectEditor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public bool Valid = false;
        Dictionary<String, bool> flags = new Dictionary<string, bool>();
        List<CheckBox> boxes = new List<CheckBox>();

        public Window1(Dictionary<String, GameComponentDescriptor> componentTypes)
        {
            InitializeComponent();

            foreach (String s in componentTypes.Keys)
            {
                GameComponentDescriptor g = componentTypes[s];
                if (!flags.ContainsKey(g.Group))
                {
                    flags.Add(g.Group, true);
                    CheckBox c = new CheckBox();
                    c.IsChecked = true;
                    c.Content = g.Group;
                    c.FontSize = 24;
                    c.Margin = new Thickness(10, 10, 10, 10);
                    c.Click += Checkbox_Click;
                    boxes.Add(c);
                    Groups.Children.Add(c);
                }
            }
        }

        private void AllOn(object sender, RoutedEventArgs e)
        {
            Groups.Children.Clear();
            foreach (CheckBox c in boxes)
            {
                c.IsChecked = true;
                Groups.Children.Add(c);
            }

        }

        private void AllOff(object sender, RoutedEventArgs e)
        {
            Groups.Children.Clear();
            foreach (CheckBox c in boxes)
            {
                c.IsChecked = false;
                Groups.Children.Add(c);
            }

        }

        private void Checkbox_Click(object sender, RoutedEventArgs e)
        {
            CheckBox c = (CheckBox)sender;
            flags[(String)c.Content] = (bool)c.IsChecked;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Valid = true;
            foreach(CheckBox c in boxes)
            {
                flags[(String)c.Content] = (bool)c.IsChecked;
            }
            this.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Valid = false;
            this.Close();
        }
    }
}
