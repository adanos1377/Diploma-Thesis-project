using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace API.Models
{
    public class MatchTeam
    {
        [Key]
        public int GameId { get; set; }
        [Required]
        public string Teams { get; set; }
        [Required]
        public bool RankedGame { get; set; }
        [Required]
        public DateTime GameDate { get; set; }
        [Required]
        public string Scores { get; set; }
        [Required]
        public bool Finished { get; set; }

        public void Add_Result(List<int> scores)
        {
            Scores = JsonSerializer.Serialize(scores);
            Finished = true;
        }

        private double Count_Probablity(double rival_skill, double skill)
        {
            double result = 1.0 / (1.0 + Math.Pow(10, (rival_skill - skill) / 400));
            return result;
        }

        public double Count_Skills(int K, double result, double skillrival, double skill, int pkt_ratio)
        {
            double Kval;
            if (pkt_ratio == 1 || pkt_ratio == 0) Kval = K;
            else if (pkt_ratio == 2) Kval = 1.5;
            else
            {
                Kval = (11 + pkt_ratio) / 16;
            }

            return Kval * (result - Count_Probablity(skillrival, skill));
        }

        public List<double> CountRanking(List<int> scores, List<double> rankings, int K, bool pkt_ratio)
        {
            List<double> new_rankings = new List<double>();
            for (int i = 0; i < scores.Count; i++)
            {
                double new_skill = 0;
                for (int j = 0; j < scores.Count; j++)
                {
                    if (j != i)
                    {
                        if (pkt_ratio)
                        {
                            if (scores[i] > scores[j]) new_skill += Count_Skills(K, 1, rankings[j], rankings[i], scores[i] - scores[j]);
                            else if (scores[i] < scores[j]) new_skill += Count_Skills(K, 0, rankings[j], rankings[i], scores[j] - scores[i]);
                            else new_skill += Count_Skills(K, 0.5, rankings[j], rankings[i], 0);
                        }
                        else
                        {
                            if (scores[i] > scores[j]) new_skill += Count_Skills(K, 1, rankings[j], rankings[i], 1);
                            else if (scores[i] < scores[j]) new_skill += Count_Skills(K, 0, rankings[j], rankings[i], 1);
                            else new_skill += Count_Skills(K, 0.5, rankings[j], rankings[i], 0);
                        }
                    }
                }
                new_rankings.Add(new_skill / scores.Count);
            }
            return new_rankings;
        }
    }

    public class MatchSolo
    {
        [Key]
        public int GameId { get; set; }
        [Required]
        public string Players { get; set; }
        [Required]
        public bool RankedGame { get; set; }
        [Required]
        public DateTime GameDate { get; set; }
        [Required]
        public string Scores { get; set; }
        [Required]
        public bool Finished { get; set; }

        public void Add_Result(List<int> scores)
        {
            Scores = JsonSerializer.Serialize(scores);
            Finished = true;
        }

        private double Count_Probablity(double rival_skill, double skill)
        {
            double result = 1.0 / (1.0 + Math.Pow(10, (rival_skill-skill) / 400));
            return result;
        }

        public double Count_Skills(int K, double result, double skillrival, double skill, int pkt_ratio)
        {
            double Kval;
            if (pkt_ratio == 1 || pkt_ratio == 0) Kval = 1;
            else if (pkt_ratio == 2) Kval = 1.5;
            else
            {
                Kval = (double)(11 + pkt_ratio) / 16;
            }

            return K * Kval * (result - Count_Probablity(skillrival, skill));
        }

        public List<double> CountRanking(List<int> scores, List<double> rankings, int K, bool pkt_ratio)
        {
            List<double> new_rankings = new List<double>();
            for (int i = 0; i < scores.Count; i++)
            {
                double new_skill = 0;
                for (int j = 0; j < scores.Count; j++)
                {
                    if (j != i)
                    {
                        if (pkt_ratio)
                        {
                            if (scores[i] > scores[j]) new_skill += Count_Skills(K, 1, rankings[j], rankings[i], scores[i] - scores[j]);
                            else if (scores[i] < scores[j]) new_skill += Count_Skills(K, 0, rankings[j], rankings[i], scores[j] - scores[i]);
                            else new_skill += Count_Skills(K, 0.5, rankings[j], rankings[i], 0);
                        }
                        else
                        {
                            if (scores[i] > scores[j]) new_skill += Count_Skills(K, 1, rankings[j], rankings[i], 1);
                            else if (scores[i] < scores[j]) new_skill += Count_Skills(K, 0, rankings[j], rankings[i], 1);
                            else new_skill += Count_Skills(K, 0.5, rankings[j], rankings[i], 0);
                        }
                    }
                }
                new_rankings.Add(new_skill / scores.Count);
            }
            return new_rankings;
        }
    }
}
