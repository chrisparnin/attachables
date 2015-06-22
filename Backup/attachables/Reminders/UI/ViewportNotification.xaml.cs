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

namespace ninlabs.attachables.UI
{
    /// <summary>
    /// Interaction logic for ViewportNotification.xaml
    /// </summary>
    public partial class ViewportNotification : UserControl
    {
        double prevOpacity;
        public ViewportNotification()
        {
            InitializeComponent();
            this.MouseEnter += ViewportNotification_MouseEnter;
            this.MouseLeave += ViewportNotification_MouseLeave;
        }

        void ViewportNotification_MouseLeave(object sender, MouseEventArgs e)
        {
            //this.Opacity = this.prevOpacity;
        }

        private void ViewportNotification_MouseEnter(object sender, MouseEventArgs e)
        {
            //this.prevOpacity = this.Opacity;
            //this.Opacity = 1.0;
            this.Opacity = 1.0;
            var model = this.DataContext as ViewportNotificationViewModel;
            model.ResetExposure();
        }
    }
}
