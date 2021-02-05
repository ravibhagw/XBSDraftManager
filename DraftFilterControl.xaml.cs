using XBS.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace DraftUtility
{
    /// <summary>
    /// Interaction logic for FilterControl.xaml
    /// </summary>
    public partial class DraftFilterControl : UserControl
    {
        DataGrid _targetGrid;
        Draft _targetDraft;

        private const int AVAIL_MIN_THRESHOLD = 0;
        public DraftFilterControl(DataGrid targetGrid, Draft draft) : base()
        {
            this._targetGrid = targetGrid;
            this._targetDraft = draft;
            InitializeComponent();
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            int threshold;
            ComboBoxItem item = (ComboBoxItem)ddl_threshhold.SelectedItem;
            string val = item?.Content?.ToString();
            if(String.IsNullOrEmpty(val))
            {
                threshold = 1;
            }
            else
            {
                threshold = Int32.Parse(val);
            }
            FilterDraftPool(chk_wing.IsChecked, chk_center.IsChecked, chk_defense.IsChecked,
                chk_goalie.IsChecked, chk_scouted.IsChecked, chk_confident.IsChecked, txt_searchname.Text, threshold);
        }

        private void FilterDraftPool(bool? showWing, bool? showCenter, bool? showDefense, bool? showGoalie, bool? hideUnscounted, bool? hideLowConfidence, string searchParams, int minThreshold)
        {
            minThreshold--;
            int? minOverall = hideUnscounted == false ? 0 : 100;
            int? minConfidence = hideLowConfidence == false ? 0 : 100;



            _targetGrid.ItemsSource = null;
            var displayableItems = new List<Player>();
            if(showWing.HasValue && showWing.Value)
            {
                displayableItems.AddRange(_targetDraft.DraftPool.Where(x => (x.LW.HasValue || x.RW.HasValue) && ((x.LW.HasValue && x.LW.Value > minThreshold) || (x.RW.HasValue && x.RW.Value > minThreshold))));
            }
            if (showCenter.HasValue && showCenter.Value)
            {
                displayableItems.AddRange(_targetDraft.DraftPool.Where(x => x.C.HasValue && x.C.Value > minThreshold));
            }
            if (showDefense.HasValue && showDefense.Value)
            {
                displayableItems.AddRange(_targetDraft.DraftPool.Where(x => x.D.HasValue && x.D.Value > minThreshold));
            }
            if (showGoalie.HasValue && showGoalie.Value)
            {
                displayableItems.AddRange(_targetDraft.DraftPool.Where(x => x.G.HasValue && x.G.Value > minThreshold));
            }
            displayableItems = displayableItems.Distinct(new DistinctByGamertagEqualityComparer()).Where(x => FilterClause.GetBySearchString(x, searchParams)).ToList();

            UpdateSummaryLabel(displayableItems);
            _targetGrid.ItemsSource = displayableItems;


        }

        private void UpdateSummaryLabel(List<Player> players)
        {
            var lwPreferred = players.Count(x => x.LW == 3);
            var lwOccasional = players.Count(x => x.LW == 2);
            var lwRarely = players.Count(x => x.LW == 1);

            var cPreferred = players.Count(x => x.C == 3);
            var cOccasional = players.Count(x => x.C == 2);
            var cRarely = players.Count(x => x.C == 1);

            var rwPreferred = players.Count(x => x.RW == 3);
            var rwOccasional = players.Count(x => x.RW == 2);
            var rwRarely = players.Count(x => x.RW == 1);

            var dPreferred = players.Count(x => x.D == 3);
            var dOccasional = players.Count(x => x.D == 2);
            var dRarely = players.Count(x => x.D == 1);

            var gPreferred = players.Count(x => x.G == 3);
            var gOccasional = players.Count(x => x.G == 2);
            var gRarely = players.Count(x => x.G == 1);

            lbl_summary.Content = 
                $"LW: Preferred: {lwPreferred}\t | Occasional: {lwOccasional}\t | Rarely: {lwRarely}\n" +
                $" C: Preferred: {cPreferred}\t | Occasional: {cOccasional}\t | Rarely: {cRarely}\n" +
                $"RW: Preferred: {rwPreferred}\t | Occasional: {rwOccasional}\t | Rarely: {rwRarely}\n" +
                $" D: Preferred: {dPreferred}\t | Occasional: {dOccasional}\t | Rarely: {dRarely}\n" +
                $" G: Preferred: {gPreferred}\t | Occasional: {gOccasional}\t | Rarely: {gRarely}\n";

        }

        private void ForceFilterReset(object sender, RoutedEventArgs e)
        {
            _targetGrid.ItemsSource = _targetDraft.DraftPool;
        }
    }
    public class DistinctByGamertagEqualityComparer : IEqualityComparer<Player>
    {
        public bool Equals(Player x, Player y)
        {
            return x.Gamertag == y.Gamertag;
        }

        public int GetHashCode(Player obj)
        {
            return obj.GetHashCode();
        }
    }

}



