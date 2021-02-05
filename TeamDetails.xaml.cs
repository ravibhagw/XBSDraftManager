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
using XBS.Core;

namespace DraftUtility
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class TeamDetails : Window
    {
        Team _team;
        TeamAnalysis _anal;
        public TeamDetails(Team team) : base()
        {
            this._team = team;
            this._anal = team.GetAnalysis();

            InitializeComponent();

            grd_players.ItemsSource = team.Roster.Values;
            lbl_analysis.Content = _anal;
        }
    }
}
