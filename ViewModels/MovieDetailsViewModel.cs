using System.Collections.Generic;
using Fall2025_Project3_snkyemba.Models;
using VaderSharp2;

namespace Fall2025_Project3_snkyemba.ViewModels
{
    public class MovieDetailsViewModel
    {
        public Movie Movie { get; set; }
        public List<string> Reviews { get; set; }
        public List<SentimentAnalysisResults> ReviewSentiments { get; set; }
        public double AverageSentiment { get; set; }
    }
}