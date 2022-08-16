using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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

namespace TheseusEditor.Editors
{
    /// <summary>
    /// Interaction logic for WindowEditorView.xaml
    /// </summary>
    public partial class WindowEditorView : UserControl
    {
        public WindowEditorView()
        {
            InitializeComponent();
            Loaded += OnWindowEditorViewLoaded;
        }

        private void OnWindowEditorViewLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnWindowEditorViewLoaded;
            Focus();
            ((INotifyCollectionChanged)GameProject.Project.UndoRedo.UndoList).CollectionChanged += (s, e) => Focus();
        }
    }
}
