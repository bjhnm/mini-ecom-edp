using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using mvvm_edp.ViewModels;

namespace mvvm_edp.Views
{
    public partial class ReportTab : UserControl
    {
        public ReportTab()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}