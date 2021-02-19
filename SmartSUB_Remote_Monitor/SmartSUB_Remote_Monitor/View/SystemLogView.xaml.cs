using Dms.Cms.SystemModel;
using SmartSUB_Remote_Monitor.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SmartSUB_Remote_Monitor.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SystemLogView : ContentPage
    {
        public SystemLogView(SystemInterface systemInterface)
        {
            InitializeComponent();

            SystemLogViewModel vm = new SystemLogViewModel(systemInterface);

            BindingContext = vm;
        }
    }
}