using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
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
    /// Interaction logic for PlayerDetail.xaml
    /// </summary>
    public partial class PlayerDetail : Window
    {
        Player _player;
        public PlayerDetail(Player player)
        {
            InitializeComponent();
            this._player = player;
            this.lbl_playerName.Content = "Loading Data, please wait...";
        }



        private async void Window_ContentRendered(object sender, RoutedEventArgs e)
        {
            if(this._player.PlayerAnalytics == null)
            {
                return;

            }
            this.grd_center.ItemsSource = this._player.PlayerAnalytics.CenterStats;
            this.grd_defense.ItemsSource = this._player.PlayerAnalytics.DefenseStats;
            this.grd_wing.ItemsSource = this._player.PlayerAnalytics.WingerStats;

            this.lbl_playerName.Content = this._player.Gamertag;


            this.grd_teamstats.ItemsSource = this._player.PlayerAnalytics.GetAllTeamsStats();
            this.txt_details.Text = GenerateReportString();

            List<PositionAverageViewModel> averages = new List<PositionAverageViewModel>();
            averages.Add(new PositionAverageViewModel()
            {
                Position = "Center",
                GamesPlayed = this._player.PlayerAnalytics.AverageGamesPlayedCenter,
                Goals = this._player.PlayerAnalytics.AverageGoalsPerGameCenter,
                Assists = this._player.PlayerAnalytics.AverageAssistsPerGameCenter,
                Points = this._player.PlayerAnalytics.AveragePointsPerGameCenter,
                PlusMinus = this._player.PlayerAnalytics.AveragePlusMinusPerGameCenter,
                Hits = this._player.PlayerAnalytics.AverageHitsPerGameCenter,
                PIMs = this._player.PlayerAnalytics.AveragePIMPerGameCenter,
                Shots = this._player.PlayerAnalytics.AverageShotsPerGameCenter,
                Faceoffs = this._player.PlayerAnalytics.AverageFaceoffPercentage,
                DefenseRating = 0
            });

            averages.Add(new PositionAverageViewModel()
            {
                Position = "Wing",
                GamesPlayed = this._player.PlayerAnalytics.AverageGamesPlayedWing,
                Goals = this._player.PlayerAnalytics.AverageGoalsPerGameWing,
                Assists = this._player.PlayerAnalytics.AverageAssistsPerGameWing,
                Points = this._player.PlayerAnalytics.AveragePointsPerGameWing,
                PlusMinus = this._player.PlayerAnalytics.AveragePlusMinusPerGameWing,
                Hits = this._player.PlayerAnalytics.AverageHitsPerGameWing,
                PIMs = this._player.PlayerAnalytics.AveragePIMPerGameWing,
                Shots = this._player.PlayerAnalytics.AverageShotsPerGameWing,
                Faceoffs = 0,
                DefenseRating = 0
            });

            averages.Add(new PositionAverageViewModel()
            {
                Position = "Defense",
                GamesPlayed = this._player.PlayerAnalytics.AverageGamesPlayedDefense,
                Goals = this._player.PlayerAnalytics.AverageGoalsPerGameDefense,
                Assists = this._player.PlayerAnalytics.AverageAssistsPerGameDefense,
                Points = this._player.PlayerAnalytics.AveragePointsPerGameDefense,
                PlusMinus = this._player.PlayerAnalytics.AveragePlusMinusPerGameDefense,
                Hits = this._player.PlayerAnalytics.AverageHitsPerGameDefense,
                PIMs = this._player.PlayerAnalytics.AveragePIMPerGameDefense,
                Shots = this._player.PlayerAnalytics.AverageShotsPerGameDefense,
                Faceoffs = 0,
                DefenseRating = this._player.PlayerAnalytics.AverageDefenseRating
            });

            this.grd_summary.ItemsSource = averages;

        }


        private string GenerateReportString()
        {
            // Things I want to know:

            // General:
            // What percentage of games did he play in?
            // On average how many games did he play per week?
            // Who are his teammates (nice to have)

            // Per position:
            // How does this player win relative to his team at each position?
            // Was this player on successful teams?  How do his stats compare between good and bad teams?
            // How much does he shoot?
            // How much does he hit? 
            // How penalty-prone is he?


            // Look up comparable players in the system (nice to have)
            // For D, how does his D rating compare to his teammates?
            // For C, how does this FO rating compare to his teammates?

            // What percentile is he in of:
            // Goal scorers
            // Playmakers
            // Hitters
            // D rating
            // penalties
            // Win percentage (how impactful is this player in wins)

            // Best team he was on
            // Worst team he was on
            // Comparison of stats in best team vs worst team

            StringBuilder sb = new StringBuilder();
            sb.Append(PlayerReportGenerator.GenerateCenterReport(this._player));
            sb.Append(PlayerReportGenerator.GenerateWingerReport(this._player));
            sb.Append(PlayerReportGenerator.GenerateDefenseReport(this._player));

            return sb.ToString();
        }


        private void Grd_teamstats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
