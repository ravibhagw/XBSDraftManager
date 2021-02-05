using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DraftMetadata config;
        Draft draft;
        Dictionary<string, DraftControl> draftControls;
        SolidColorBrush draftingColor = new SolidColorBrush();
        SolidColorBrush notDraftingColor = new SolidColorBrush();
        IConfiguration configuration;
        public MainWindow()
        {
            #region stuff to implement
            // Analytics features
            // Evaluate and rank defensemen
            // Evaluate and rank wingers
            // Evaluate and rank centers


            // Next set of features:
            // 3. Get player's availability text
            // 5. Get player's stats from non-league (nice to have)
            // 7. Compare players (nice to have)

            // DONE
            // 6. Make who is drafting way more obvious
            // 1. Undo button
            // 2. Support reset rounds
            // 4. Get player's stats from past season(s)
            // Add "eager load and persist" mode
            // Should retrieve player analytics 
            // 8. Show team player played for per season
            #endregion
            draftingColor.Color = Colors.LightGreen;
            notDraftingColor.Color = Colors.White;

            var builder = new ConfigurationBuilder()
             .SetBasePath(@"C:\XBSDraftManager\XBSDraftManager\")
             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            this.configuration = builder.Build();

            InitializeComponent();
          
        }



        public void InitializeDraft()
        {

            config = new DraftMetadata();
            draftControls = new Dictionary<string, DraftControl>();
            // Load data
            List<Team> teams = new List<Team>();

            using (FileStream file = new FileStream(configuration.GetSection("TeamFilePath").Value, FileMode.Open, FileAccess.Read))
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<Team>));
                teams = (List<Team>)ser.Deserialize(file);
            }
            config.Teams = this.GenerateTeamsAndOrder(teams, chk_randomize.IsChecked ?? false);

            List<Player> players = new List<Player>();
            using (FileStream file = new FileStream(configuration.GetSection("PlayerFilePath").Value, FileMode.Open, FileAccess.Read))
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<Player>));
                players = (List<Player>)ser.Deserialize(file);
            }

            List<StatsTeam> statsTeams = new List<StatsTeam>();
            using (FileStream file = new FileStream(configuration.GetSection("TeamStatsFilePath").Value, FileMode.Open, FileAccess.Read))
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<StatsTeam>));
                statsTeams = (List<StatsTeam>)ser.Deserialize(file);
            }

            foreach(var player in players)
            {
                player?.PlayerAnalytics?.BackfillTeamStats(statsTeams);
            }

            config.Players = players.OrderBy(x => x.Gamertag).ToList();
            stackpanela.Children.Clear();
            stackpanelb.Children.Clear();

            // Every team is "pre-loaded" with its owner
            var ownerGamertags = new List<string>();
            bool stackA = true;
            foreach (var team in config.Teams)
            {
                ownerGamertags.Add(team.Value.Owner.Gamertag);
                var ownerPlayer = players.Single(x => String.Equals(x.Gamertag, team.Value.Owner.Gamertag, StringComparison.OrdinalIgnoreCase));
                team.Value.Roster.Add(0, ownerPlayer);
                team.Value.Owner = ownerPlayer;

                draftControls.Add(team.Value.Name, new DraftControl(team.Key+1, team.Value));
                if(stackA)
                {
                    stackpanela.Children.Add(draftControls[team.Value.Name]);
                }
                else
                {
                    stackpanelb.Children.Add(draftControls[team.Value.Name]);
                }
                
                draftControls[team.Value.Name].AssignPlayer(0, team.Value.Roster[0]);


                stackA = !stackA;
            }

            // Remove owners from draft pool
            foreach (var owner in ownerGamertags)
            {
                config.Players.RemoveAll(x => String.Equals(x.Gamertag,owner,StringComparison.OrdinalIgnoreCase));
            }

            draft = new Draft(config.Teams, config.Players);

            
            grd_players.ItemsSource = null;
            grd_players.ItemsSource = draft.DraftPool;

            UpdateHeader();
            var draftFilterControl = new DraftFilterControl(grd_players, draft);

            draftFilterControl.chk_center.IsChecked = true;
            draftFilterControl.chk_defense.IsChecked = true;
            draftFilterControl.chk_wing.IsChecked = true;
            draftFilterControl.chk_goalie.IsChecked = true;

            filterPanel.Children.Add(draftFilterControl);
        }

        private void BeginDraft_Click(object sender, RoutedEventArgs e)
        {
            InitializeDraft();
            draftControls[draft.DraftingTeam.Name].Background = draftingColor;
        }

        private void Btn_draftplayer_Click(object sender, RoutedEventArgs e)
        {
            Player selectedPlayer = (Player)grd_players.SelectedItem;
            if (selectedPlayer == null)
                return;

            draftControls[draft.DraftingTeam.Name].AssignPlayer(draft.Round, selectedPlayer);
            draftControls[draft.DraftingTeam.Name].Background = notDraftingColor;
            draft.DraftPlayer(selectedPlayer.Gamertag);
            draftControls[draft.DraftingTeam.Name].Background = draftingColor;


            grd_players.ItemsSource = null;
            grd_players.ItemsSource = draft.DraftPool;

            UpdateHeader();

        }

        private void UpdateHeader()
        {
            lbl_draftdetails.Content = String.Format("Currently Drafting: {0} | Round: {1} Pick {2} ({3} overall)", draft.DraftingTeam.Name, draft.Round, draft.RelativePick, draft.AbsolutePick);
        }

        private Dictionary<int, Team> GenerateTeamsAndOrder(List<Team> teams, bool randomize = false)
        {


            if(!randomize)
            {

                var dict = new SortedDictionary<int, Team>();

                dict.Add(0, teams.Single(x => x.Owner.Gamertag == "DesiredCustoms"));
                dict.Add(13, teams.Single(x => x.Owner.Gamertag == "baultista"));
                dict.Add(3, teams.Single(x => x.Owner.Gamertag == "Dr McCringle"));
                dict.Add(9, teams.Single(x => x.Owner.Gamertag == "Stup1dbasterd"));
                dict.Add(11, teams.Single(x => x.Owner.Gamertag == "GetLowBanks"));
                dict.Add(12, teams.Single(x => x.Owner.Gamertag == "Habfan5693"));
                dict.Add(8, teams.Single(x => x.Owner.Gamertag == "SirRocko"));
                dict.Add(6, teams.Single(x => x.Owner.Gamertag == "allthway"));
                dict.Add(7, teams.Single(x => x.Owner.Gamertag == "Shuck_Gnorris"));
                dict.Add(1, teams.Single(x => x.Owner.Gamertag == "Nutty Grandpa"));
                dict.Add(4, teams.Single(x => x.Owner.Gamertag == "DankinIT"));
                dict.Add(2, teams.Single(x => x.Owner.Gamertag == "Whiplash444"));
                dict.Add(10, teams.Single(x => x.Owner.Gamertag == "long live brad"));
                dict.Add(5, teams.Single(x => x.Owner.Gamertag == "Edrosenbruins"));

                return dict.ToDictionary(x => x.Key, y => y.Value);
            }
            else
            {
                var result = new Dictionary<int, Team>();
                var random = new Random();
                int teamId = 0;

                for(int i = teams.Max(x => x.CompensatedRound); i > 0; i--)
                {
                    var targetTeams = teams.Where(x => x.CompensatedRound == i).ToList();

                    while(targetTeams.Any())
                    {
                        int selectedIndex = random.Next(targetTeams.Count);
                        result.Add(teamId, targetTeams[selectedIndex]);
                        targetTeams.Remove(targetTeams[selectedIndex]);
                        teamId++;
                    }
                }
                return result;
            }
        }




        private void Btn_savechanges_Click(object sender, RoutedEventArgs e)
        {
            using (FileStream file = new FileStream("c:\\testoutputdraft\\players_modified.xml", FileMode.Create, FileAccess.Write))
            {
                var allPlayers = new List<Player>();
                allPlayers.AddRange(draft.DraftPool.ToList());
                allPlayers.AddRange(draft.Owners.ToList());

                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(List<Player>));
                ser.Serialize(file, allPlayers);
            }
            MessageBox.Show("Changes saved!", "DraftUtility");
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex()+1).ToString();
            Player player = e.Row.DataContext as Player;

            if(player != null)
            {
                var eliteBrush = new SolidColorBrush();
                eliteBrush.Color = Colors.DarkBlue;

                var starBrush = new SolidColorBrush();
                starBrush.Color = Colors.DarkGreen;

                var coreBrush = new SolidColorBrush();
                coreBrush.Color = Colors.DarkOrange;

                var depthBrush = new SolidColorBrush();
                depthBrush.Color = Colors.DarkGoldenrod;

                var avoidBrush = new SolidColorBrush();
                avoidBrush.Color = Colors.DarkRed;

                var baseBrush = new SolidColorBrush();
                baseBrush.Color = Colors.Black;

                if (player.Rating > 86)
                    e.Row.Foreground = eliteBrush;
                else if (player.Rating > 83)
                    e.Row.Foreground = starBrush;
                else if (player.Rating > 79)
                    e.Row.Foreground = coreBrush;
                else if (player.Rating > 70)
                    e.Row.Foreground = depthBrush;
                else if (player.Rating > 1)
                    e.Row.Foreground = avoidBrush;
                else if (player.Rating == null || player.Rating == 0)
                    e.Row.Foreground = baseBrush;
            }

        }

        private void Btn_undo_Click(object sender, RoutedEventArgs e)
        {
            draftControls[draft.DraftingTeam.Name].Background = notDraftingColor;
            draft.RevertLastPick();
            draftControls[draft.DraftingTeam.Name].UnAssignPlayer(draft.Round);
            draftControls[draft.DraftingTeam.Name].Background = draftingColor;

            grd_players.ItemsSource = null;
            grd_players.ItemsSource = draft.DraftPool;

            UpdateHeader();
        }

        void DataGrid_DoubleClick(object sender, RoutedEventArgs e)
        {
            Player selectedPlayer = (Player)grd_players.SelectedItem;

            var window = new PlayerDetail(selectedPlayer);
            window.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var x = new ExtendedDraftView(draft);
            x.Show();
        }
    }
}
