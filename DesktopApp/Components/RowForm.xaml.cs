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

namespace DesktopApp.Components
{
    public partial class RowForm : UserControl
    {
        public string Label { get; set; }
        public string DefaultValue
        {
            get { return (string)GetValue(defaultValprop); }
            set { SetValue(defaultValprop, value); }
        }

        public string Value
        {
            get
            {
                return (string)GetValue(valprop) ?? DefaultValue;
            }
            set
            {
                SetValue(valprop, value);
            }
        }

        private static DependencyProperty defaultValprop = DependencyProperty.Register(
            "DefaultValue", typeof(string), typeof(RowForm), new FrameworkPropertyMetadata(null));


        private static DependencyProperty valprop = DependencyProperty.Register(
            "Value", typeof(string), typeof(RowForm), new FrameworkPropertyMetadata() { BindsTwoWayByDefault = true });

        public RowForm()
        {
            InitializeComponent();
            //this.SetValue() prijima dependency property
        }
    }
}
