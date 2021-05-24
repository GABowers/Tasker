using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace Tasker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<BindingClass> tasks = new ObservableCollection<BindingClass>();
        public ObservableCollection<BindingClass> Tasks
        {
            get
            {
                return tasks;
            }
            set
            {
                tasks = value;
            }
        }
        string database = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void taskButton_Click(object sender, RoutedEventArgs e)
        {
            if(Tasks == null)
            {
                Tasks = new ObservableCollection<BindingClass>();
            }
            Tasks.Add(new BindingClass());
            SaveDatabase();
        }

        public class BindingClass
        {
            public string Title { get; set; }
            public string Description { get; set; }
        }

        private void upButton_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem as BindingClass;
            var index = tasks.IndexOf(item);
            if(index > 0)
            {
                tasks.Move(index, index - 1);
                SaveDatabase();
            }
        }

        private void downButton_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem as BindingClass;
            var index = tasks.IndexOf(item);
            if (index < (tasks.Count - 1))
            {
                tasks.Move(index, index + 1);
                SaveDatabase();
            }
        }

        void SaveDatabase()
        {
            if(database != null && Directory.Exists(System.IO.Path.GetDirectoryName(database)))
            {
                var sds = JsonConvert.SerializeObject(tasks);
                File.WriteAllText(database, sds);
                updateLabel.Content = "Last saved " + System.DateTime.Now.ToString("F");
            }
        }

        private void newDatabase_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON|*.json";
            sfd.Title = "Select a database save location.";
            sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sfd.RestoreDirectory = true;
            var result = sfd.ShowDialog();
            if(sfd.FileName != "")
            {
                database = sfd.FileName;
                SaveDatabase();
            }
        }

        private void openDatabase_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "JSON|*.json";
            ofd.Title = "Load a previously saved database";
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //ofd.RestoreDirectory = true;
            var result = ofd.ShowDialog();
            if (ofd.FileName != "")
            {
                database = ofd.FileName;
                var sds = File.ReadAllText(database);
                var newT = JsonConvert.DeserializeObject<ObservableCollection<BindingClass>>(sds);
                Tasks.Clear();
                foreach (var t in newT)
                {
                    Tasks.Add(t);
                }
            }
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            SaveDatabase();
        }

        private void dropTask_Click(object sender, RoutedEventArgs e)
        {
            var item = dataGrid.SelectedItem as BindingClass;
            tasks.Remove(item);
            SaveDatabase();
        }
    }
}
