using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using XBS.Core;

namespace DraftUtility
{
    /// <summary>
    /// Interaction logic for DraftControl.xaml
    /// </summary>
    public partial class DraftControl : UserControl
    {
        Dictionary<int, Player> _players;
        //BindingList<Player> _displayablePlayers;
        Team _team;
        public DraftControl(int draftPosition, Team team)
        {
            InitializeComponent();
            lbl_teamname.Content = string.Format("{0}: {1}", draftPosition, team.Name);
            _players = new Dictionary<int, Player>();
            grd_players.ItemsSource = _players.Values.ToList();
            this._team = team;
            
        }
        public void AssignPlayer(int round, Player player)
        {
            _players.Add(round, player);
            grd_players.ItemsSource = null;
            grd_players.ItemsSource = _players.Values.ToList();
        }
        public void UnAssignPlayer(int round)
        {
            _players.Remove(round);
            grd_players.ItemsSource = null;
            grd_players.ItemsSource = _players.Values.ToList();
        }

        public void SetDraftingMode()
        {
            
        }
        public void UnsetDraftingMode()
        {

        }

        void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if((string)e.Column.Header == "Url")
            {
                e.Column = new DataGridHyperlinkColumn() { Binding = new Binding(e.PropertyName), Header = "Url" };
            }
            if (!e.Column.CanUserSort)
            {
                Type type = e.PropertyType;
                if (type.IsGenericType && type.IsValueType && typeof(IComparable).IsAssignableFrom(type.GetGenericArguments()[0]))
                {
                    // allow nullable primitives to be sorted
                    e.Column.CanUserSort = true;
                }
            }
        }

        private void Btn_details_Click(object sender, RoutedEventArgs e)
        {
            var detailsWindow = new TeamDetails(_team);
            detailsWindow.Show();
        }

        void DataGrid_DoubleClick(object sender, RoutedEventArgs e)
        {
            Player selectedPlayer = (Player)grd_players.SelectedItem;

            var window = new PlayerDetail(selectedPlayer);
            window.Show();
        }
    }
}
