using System.Windows;
using System.Windows.Media;

namespace EveOMock
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			this.InitializeComponent();

			Random random = new Random();

			// The first command line argument is the window title to pretend to be, so that
			// several of these can stand in for a fleet. Without one it gets a random name.
			string[] arguments = Environment.GetCommandLineArgs();
			this.Title = arguments.Length > 1 ? arguments[1] : $"Mock {random.Next():X}";

			this.grid.Background = new SolidColorBrush(Color.FromArgb(255, (byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255)));
		}
	}
}
