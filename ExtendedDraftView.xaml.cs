using System;
using System.Collections;
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
    public partial class ExtendedDraftView : Window
    {
        List<Player> _draftPool;
        Draft _draft;

        List<ExtendedSkaterViewModel> _extendedPlayers;

        const int MIN_GP_THRESHOLD = 0;
        public ExtendedDraftView(Draft draft)
        {
            
            InitializeComponent();
            this._draft = draft;
            this._draftPool = draft.DraftPool;

            List<ExtendedSkaterViewModel> extendedPlayers = new List<ExtendedSkaterViewModel>();
            extendedPlayers = this.GenerateViewModels(_draftPool);
            this._extendedPlayers = extendedPlayers;
            this.grd_players.ItemsSource = _extendedPlayers.Where(x => x.AverageGamesPlayed > MIN_GP_THRESHOLD);


        }
        private void Grid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            //// Have to do this in the unusual case where the border of the cell gets selected
            //// and causes a crash 'EditItem is not allowed'
            e.Cancel = true;
        }
        private void UpdateFilter(object sender, EventArgs e)
        {
            if(chk_hideC != null && chk_hideW != null && chk_hideD != null && txt_gpThreshold != null && chk_addowners != null)
            {
                decimal threshold;

                this.grd_players.ItemsSource = null;

                List<ExtendedSkaterViewModel> displayableItems = new List<ExtendedSkaterViewModel>();

                displayableItems.AddRange(_extendedPlayers);

                if(chk_addowners.IsChecked.HasValue && chk_addowners.IsChecked.Value)
                {
                    displayableItems.AddRange(this.GenerateViewModels(_draft.Owners));
                }
                bool isNumeric = decimal.TryParse(txt_gpThreshold.Text, out threshold);
                if (isNumeric)
                {
                    displayableItems.RemoveAll(x => x.AverageGamesPlayed < threshold);
                }
                if (chk_hideC.IsChecked.HasValue && chk_hideC.IsChecked.Value)
                {
                    displayableItems.RemoveAll(x => x.Position == "C");
                }
                if (chk_hideW.IsChecked.HasValue && chk_hideW.IsChecked.Value)
                {
                    displayableItems.RemoveAll(x => x.Position == "W");
                }
                if (chk_hideD.IsChecked.HasValue && chk_hideD.IsChecked.Value)
                {
                    displayableItems.RemoveAll(x => x.Position == "D");
                }

                this.grd_players.ItemsSource = null;
                this.grd_players.ItemsSource = displayableItems.Distinct(new ExtendedSkaterViewModelComparer());
            }
        }
        void DataGrid_DoubleClick(object sender, RoutedEventArgs e)
        {
            ExtendedSkaterViewModel selectedPlayer = (ExtendedSkaterViewModel)grd_players.SelectedItem;

            var player = _draftPool.Single(x => String.Equals(x.Gamertag, selectedPlayer.Gamertag, StringComparison.OrdinalIgnoreCase));

            var window = new PlayerDetail(player);
            window.Show();
        }
        private List<ExtendedSkaterViewModel> GenerateViewModels(List<Player> playerPool)
        {
            List<ExtendedSkaterViewModel> extendedPlayers = new List<ExtendedSkaterViewModel>();
            
            foreach (var player in playerPool)
            {
                if (player.ForumId == "0000")
                    continue;
                var playerAnalytics = player.PlayerAnalytics;
                ExtendedCenterViewModel centerViewModel = null;
                ExtendedWingerViewModel wingerViewModel = null;
                ExtendedDefenderViewModel defenderViewModel = null;
                if (playerAnalytics.AverageGamesPlayedCenter > MIN_GP_THRESHOLD)
                {
                    centerViewModel = new ExtendedCenterViewModel();
                    centerViewModel.AssistsPerGame = playerAnalytics.AverageAssistsPerGameCenter;
                    centerViewModel.AverageFaceoffPercentage = playerAnalytics.AverageFaceoffPercentage;
                    centerViewModel.AverageGamesPlayed = playerAnalytics.AverageGamesPlayedCenter;
                    centerViewModel.AveragePlusMinus = playerAnalytics.AveragePlusMinusPerGameCenter;
                    centerViewModel.Gamertag = player.Gamertag;
                    centerViewModel.GoalsPerGame = playerAnalytics.AverageGoalsPerGameCenter;
                    centerViewModel.HitsPerGame = playerAnalytics.AverageHitsPerGameCenter;
                    centerViewModel.PIMPerGame = playerAnalytics.AveragePIMPerGameCenter;
                    centerViewModel.PointsPerGame = playerAnalytics.AveragePointsPerGameCenter;
                    centerViewModel.ShotsPerGame = playerAnalytics.AverageShotsPerGameCenter;

                    centerViewModel.MedianAssistPercentage = playerAnalytics.MedianAssistPercentageCenter;
                    centerViewModel.MedianGoalScoringPercentage = playerAnalytics.MedianGoalScoringPercentageCenter;
                    centerViewModel.MedianOffensiveContribution = playerAnalytics.MedianOffensiveContributionCenter;
                    centerViewModel.MedianHitShare = playerAnalytics.MedianHitPercentageCenter;
                    centerViewModel.MedianPIMShare = playerAnalytics.MedianPIMPercentageCenter;
                    centerViewModel.MedianShootingPercentage = playerAnalytics.MedianShootingPercentageCenter;
                    centerViewModel.WinsRelativeToTeam = playerAnalytics.AverageWinnerRelativeToTeamCenter;
                    centerViewModel.AverageTeamWinPercentage = playerAnalytics.AverageTeamWinPercentage;
                    centerViewModel.AverageWinDifferential = playerAnalytics.AverageWinDifferentialCenter;

                    centerViewModel.CenterOverallRating = playerAnalytics.ComputedCenterRating;
                    centerViewModel.CenterSniperRating = playerAnalytics.ComputedSniperRatingCenter;
                    centerViewModel.CenterPlaymakerRating = playerAnalytics.ComputedPlaymakerRatingCenter;
                    centerViewModel.CenterSniperBias =
                        playerAnalytics.ComputedSniperRatingCenter > 0.0m && playerAnalytics.ComputedPlaymakerRatingCenter > 0.0m ?
                    playerAnalytics.ComputedSniperRatingCenter / playerAnalytics.ComputedPlaymakerRatingCenter : 0.0m;

                    extendedPlayers.Add(centerViewModel);
                }
                if (playerAnalytics.AverageGamesPlayedWing > MIN_GP_THRESHOLD)
                {

                    wingerViewModel = new ExtendedWingerViewModel();
                    wingerViewModel.AssistsPerGame = playerAnalytics.AverageAssistsPerGameWing;
                    //wingerViewModel.AverageFaceoffPercentage = playerAnalytics.AverageFaceoffPercentage;
                    wingerViewModel.AverageGamesPlayed = playerAnalytics.AverageGamesPlayedWing;
                    wingerViewModel.AveragePlusMinus = playerAnalytics.AveragePlusMinusPerGameWing;
                    wingerViewModel.Gamertag = player.Gamertag;
                    wingerViewModel.GoalsPerGame = playerAnalytics.AverageGoalsPerGameWing;
                    wingerViewModel.HitsPerGame = playerAnalytics.AverageHitsPerGameWing;
                    wingerViewModel.PIMPerGame = playerAnalytics.AveragePIMPerGameWing;
                    wingerViewModel.PointsPerGame = playerAnalytics.AveragePointsPerGameWing;
                    wingerViewModel.ShotsPerGame = playerAnalytics.AverageShotsPerGameWing;

                    wingerViewModel.MedianAssistPercentage = playerAnalytics.MedianAssistPercentageWing;
                    wingerViewModel.MedianGoalScoringPercentage = playerAnalytics.MedianGoalScoringPercentageWing;
                    wingerViewModel.MedianOffensiveContribution = playerAnalytics.MedianOffensiveContributionWing;
                    wingerViewModel.MedianHitShare = playerAnalytics.MedianHitPercentageWing;
                    wingerViewModel.MedianPIMShare = playerAnalytics.MedianPIMPercentageWing;
                    wingerViewModel.MedianShootingPercentage = playerAnalytics.MedianShootingPercentageWing;
                    wingerViewModel.WinsRelativeToTeam = playerAnalytics.AverageWinnerRelativeToTeamWing;
                    wingerViewModel.AverageTeamWinPercentage = playerAnalytics.AverageTeamWinPercentage;
                    wingerViewModel.AverageWinDifferential = playerAnalytics.AverageWinDifferentialWing;

                    wingerViewModel.WingerOverallRating = playerAnalytics.ComputedWingerRating;
                    wingerViewModel.WingerPlaymakerRating = playerAnalytics.ComputedPlaymakerRatingWing;
                    wingerViewModel.WingerSniperRating = playerAnalytics.ComputedSniperRatingWing;
                    wingerViewModel.WingerSniperBias =
                        playerAnalytics.ComputedSniperRatingWing > 0.0m && playerAnalytics.ComputedPlaymakerRatingWing > 0.0m ?
                        playerAnalytics.ComputedSniperRatingWing / playerAnalytics.ComputedPlaymakerRatingWing : 0.0m;

                    extendedPlayers.Add(wingerViewModel);
                }
                if (playerAnalytics.AverageGamesPlayedDefense > MIN_GP_THRESHOLD)
                {
                    defenderViewModel = new ExtendedDefenderViewModel();
                    defenderViewModel.AssistsPerGame = playerAnalytics.AverageAssistsPerGameDefense;
                    defenderViewModel.AverageDRating = playerAnalytics.AverageDefenseRating;
                    defenderViewModel.AverageGamesPlayed = playerAnalytics.AverageGamesPlayedDefense;
                    defenderViewModel.AveragePlusMinus = playerAnalytics.AveragePlusMinusPerGameDefense;
                    defenderViewModel.Gamertag = player.Gamertag;
                    defenderViewModel.GoalsPerGame = playerAnalytics.AverageGoalsPerGameDefense;
                    defenderViewModel.HitsPerGame = playerAnalytics.AverageHitsPerGameDefense;
                    defenderViewModel.PIMPerGame = playerAnalytics.AveragePIMPerGameDefense;
                    defenderViewModel.PointsPerGame = playerAnalytics.AveragePointsPerGameDefense;
                    defenderViewModel.ShotsPerGame = playerAnalytics.AverageShotsPerGameDefense;

                    defenderViewModel.MedianAssistPercentage = playerAnalytics.MedianAssistPercentageDefense;
                    defenderViewModel.MedianGoalScoringPercentage = playerAnalytics.MedianGoalScoringPercentageDefense;
                    defenderViewModel.MedianOffensiveContribution = playerAnalytics.MedianOffensiveContributionDefense;
                    defenderViewModel.MedianHitShare = playerAnalytics.MedianHitPercentageDefense;
                    defenderViewModel.MedianPIMShare = playerAnalytics.MedianPIMPercentageDefense;
                    defenderViewModel.MedianShootingPercentage = playerAnalytics.MedianShootingPercentageDefense;
                    defenderViewModel.WinsRelativeToTeam = playerAnalytics.AverageWinnerRelativeToTeamDefense;
                    defenderViewModel.DRatingPerPoint = playerAnalytics.AverageDefenseRatingPerPoint;
                    defenderViewModel.DRatingDifferential = playerAnalytics.OffensiveDefensiveBiasDefense;
                    defenderViewModel.AverageTeamWinPercentage = playerAnalytics.AverageTeamWinPercentage;

                    defenderViewModel.AverageWinDifferential = playerAnalytics.AverageWinDifferentialDefense;

                    extendedPlayers.Add(defenderViewModel);
                }

                var cumulativeGamesPlayed = (centerViewModel != null ? centerViewModel.AverageGamesPlayed : 0.0m)
                    + (wingerViewModel != null ? wingerViewModel.AverageGamesPlayed : 0.0m)
                    + (defenderViewModel != null ? defenderViewModel.AverageGamesPlayed : 0.0m);


                if (centerViewModel != null)
                    centerViewModel.CumulativeAverageGamesPlayed = cumulativeGamesPlayed;

                if (wingerViewModel != null)
                    wingerViewModel.CumulativeAverageGamesPlayed = cumulativeGamesPlayed;

                if (defenderViewModel != null)
                    defenderViewModel.CumulativeAverageGamesPlayed = cumulativeGamesPlayed;
            }

            return extendedPlayers;
        }

    }
}
