using System.Collections.Generic;
using Fall2025_Project3_snkyemba.Models;
using VaderSharp2;

namespace Fall2025_Project3_snkyemba.ViewModels
{
    public class ActorDetailsViewModel
    {
        public Actor Actor { get; set; }
        public List<string> Tweets { get; set; }
        public List<SentimentAnalysisResults> TweetSentiments { get; set; }
        public double OverallSentiment { get; set; }
    }
}