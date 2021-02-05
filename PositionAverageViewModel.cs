using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DraftUtility
{
    public class PositionAverageViewModel
    {
        public string Position { get; set; }
        public string Teams { get; set; }
        public decimal GamesPlayed { get; set; }
        public decimal Goals { get; set; }
        public decimal Assists { get; set; }
        public decimal Points { get; set; }
        public decimal PIMs { get; set; }
        public decimal Hits { get; set; }
        public decimal PlusMinus { get; set; }
        public decimal Shots { get; set; }
        public decimal Faceoffs { get; set; }
        public decimal DefenseRating { get; set; }
        public decimal OffensivePointShare { get; set; }


    }
    public class ExtendedCenterViewModel : ExtendedSkaterViewModel
    {
        public decimal AverageFaceoffPercentage { get; set; }
        public override string Position { get { return "C"; } }

        public decimal? CenterOverallRating { get; set; }

        public decimal? CenterSniperRating { get; set; }

        public decimal? CenterPlaymakerRating { get; set; }

        public decimal? CenterSniperBias { get; set; }
    }
    public class ExtendedWingerViewModel : ExtendedSkaterViewModel
    {
        public override string Position { get { return "W"; } }
        public decimal? WingerOverallRating { get; set; }

        public decimal? WingerSniperRating { get; set; }

        public decimal? WingerPlaymakerRating { get; set; }

        public decimal? WingerSniperBias { get; set; }
    }
    public class ExtendedDefenderViewModel : ExtendedSkaterViewModel
    {
        public decimal AverageDRating { get; set; }
        public override string Position { get { return "D"; } }
        public decimal? DRatingPerPoint { get; set; }

        public decimal? DRatingDifferential { get; set; }
    }
    public abstract class ExtendedSkaterViewModel
    {
        public abstract string Position { get; }
        public string Gamertag { get; set; }
        public decimal AverageGamesPlayed { get; set; }
        public decimal CumulativeAverageGamesPlayed { get; set; }
        public decimal GoalsPerGame { get; set; }
        public decimal AssistsPerGame { get; set; }
        public decimal PointsPerGame { get; set; }
        public decimal HitsPerGame { get; set; }
        public decimal PIMPerGame { get; set; }
        public decimal ShotsPerGame { get; set; }
        public decimal AveragePlusMinus { get; set; }
        public decimal WinPercentage { get; set; }

        // Shares and averages
        public decimal WinsRelativeToTeam { get; set; }
        public decimal AverageWinDifferential { get; set; }

        public decimal AverageTeamWinPercentage { get; set; }
        public decimal MedianGoalScoringPercentage { get; set; }
        public decimal MedianShootingPercentage { get; set; }
        public decimal MedianAssistPercentage { get; set; }
        public decimal MedianOffensiveContribution { get; set; }
        public decimal MedianHitShare { get; set; }
        public decimal MedianPIMShare { get; set; }
        public override string ToString()
        {
            return this.Gamertag + "-" + this.Position;
        }


    }
    public class ExtendedSkaterViewModelComparer : IEqualityComparer<ExtendedSkaterViewModel>
    {
        public bool Equals(ExtendedSkaterViewModel x, ExtendedSkaterViewModel y)
        {
            return x.Position == y.Position && x.Gamertag == y.Gamertag;
        }

        public int GetHashCode(ExtendedSkaterViewModel obj)
        {
            return obj.GetHashCode();
        }
    }

    public class BasicPlayerViewModel
    {
        public int? ForumId { get; set; }
        public string Gamertag { get; set; }
        public int? LW { get; set; }
        public int? C { get; set; }
        public int? RW { get; set; }
        public int? D { get; set; }
        public int? G { get; set; }

        public decimal? CRating { get; set; }
        public decimal? WRating { get; set; }
        public decimal? DRating { get; set; }
        public decimal? DRatingPerPoint { get; set; }
        public decimal? DRatingDifferential { get; set; }
        public decimal? FaceoffPercentage { get; set; }

        public Uri PlayerCardLink { get; set; } 

    }
}
