using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.Json;
using System;
using System.IO;

namespace DevelopApp
{
    public partial class Form1 : Form
    {
        private readonly List<Rank> ranks;
        public Form1()
        {
            InitializeComponent();
            ranks = new List<Rank>();
            Rank bronze = new Rank("bronze", 0, 999);
            Rank silver = new Rank("silver", 1000, 1999);
            Rank gold = new Rank("gold", 2000, -1);
            ranks.Add(bronze);
            ranks.Add(silver);
            ranks.Add(gold);
        }

        private void LogOutButton_Click(object sender, EventArgs e)
        {
            Program.client = new Client(Program.serverAddress);
            Communication.LogOut(Program.userLogin);
            Program.client.Disconnect();
            this.Hide();
            var form2 = new LogonForm();
            form2.Closed += (s, args) => this.Close();
            form2.Show();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            //ranks
            List<int> ranklimits = new List<int>();
            foreach (var rank in ranks)
            {
                if (rank.MaxRating > 0) ranklimits.Add(rank.MaxRating);
            }

            GameConfig new_game = new GameConfig
            {
                //general settings
                Name = nameBox.Text,
                Description = descBox.Text,
                NumberOfPlayers = Int32.Parse(mPlayersBox.Text),
                Server = serverBox.Text,
                TieGames = tieCheck.Checked,
                AvgTime = Int32.Parse(avgTime.Text),

                //advanced settings
                KValue = KvalueBar.Value,
                StartRating = SratingBar.Value,
                PktsRatio = pRatioCheck.Checked,
                MatchmakingLimit = matchmakBar.Value,
                NumberOfRanks = ranks.Count,

                //ranks
                RanksLimit = JsonSerializer.Serialize(ranklimits)
            };

            using (var dbContext = new ConfigContext())
            {
                dbContext.games.Add(new_game);
                dbContext.SaveChanges();
            }

            MessageBox.Show("Gra została poprawnie utworzona!");

            nameBox.Text = String.Empty;
            descBox.Text = String.Empty;
            mPlayersBox.Text = String.Empty;
            serverBox.Text = String.Empty;
            tieCheck.Checked = false;
            avgTime.Text = String.Empty;

            KvalueBar.Value = 21;
            pRatioCheck.Checked = false;
            matchmakBar.Value = 160;
            SratingBar.Value = 375;

            if (EliteCheck.Checked)
            {
                EliteCheck.Checked = false;
                eliteMinBox.Text = String.Empty;
                eliteMaxBox.Text = String.Empty;
                DiamondCheck.Enabled = true;
                elitePanel.Enabled = false;
                RemoveRank();
            }

            if (DiamondCheck.Checked)
            {
                DiamondCheck.Checked = false;
                diamondMinBox.Text = String.Empty;
                diamondMaxBox.Text = String.Empty;
                EliteCheck.Enabled = false;
                diamondPanel.Enabled = false;
                PlatinCheck.Enabled = true;
                RemoveRank();
            }

            if (PlatinCheck.Checked)
            {
                PlatinCheck.Checked = false;
                DiamondCheck.Enabled = false;
                platinMinBox.Text = String.Empty;
                PlatinMaxBox.Text = String.Empty;
                platinPanel.Enabled = false;
                RemoveRank();
            }

            tabControl1.Enabled = true;
            button1.Enabled = false;
            button5.Enabled = false;
            button2.Enabled = true;
        }

        private void AddRank(string name)
        {
            Rank rank = new Rank(name, ranks[ranks.Count - 1].MinRating + 1000, -1);
            ranks[ranks.Count - 1].MaxRating = ranks[ranks.Count - 1].MinRating + 999;
            ranks.Add(rank);
            UpdateRanks();
        }

        private void RemoveRank()
        {
            ranks.RemoveAt(ranks.Count-1);
            ranks[ranks.Count - 1].MaxRating = -1;
            UpdateRanks();
        }

        private void UpdateRanks()
        {
            foreach(var rank in ranks)
            {
                if (rank.Name == "gold")
                {
                    goldMinBox.Text = rank.MinRating.ToString();
                    if (rank.MaxRating < 0) goldMaxBox.Text = "MAX";
                    else goldMaxBox.Text = rank.MaxRating.ToString();
                }
                else if (rank.Name == "platin")
                {
                    platinMinBox.Text = rank.MinRating.ToString();
                    if (rank.MaxRating < 0) PlatinMaxBox.Text = "MAX";
                    else PlatinMaxBox.Text = rank.MaxRating.ToString();
                }
                else if (rank.Name == "diamond")
                {
                    diamondMinBox.Text = rank.MinRating.ToString();
                    if (rank.MaxRating < 0) diamondMaxBox.Text = "MAX";
                    else diamondMaxBox.Text = rank.MaxRating.ToString();
                }
                else if (rank.Name=="elite")
                {
                    eliteMinBox.Text = rank.MinRating.ToString();
                    if (rank.MaxRating < 0) eliteMaxBox.Text = "MAX";
                    else eliteMaxBox.Text = rank.MaxRating.ToString();
                }
            }
        }
        private void PlatinCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (PlatinCheck.Checked)
            {
                DiamondCheck.Enabled = true;
                platinPanel.Enabled = true;
                AddRank("platin");
            }
            else
            {
                DiamondCheck.Enabled = false;
                platinMinBox.Text = String.Empty;
                PlatinMaxBox.Text = String.Empty;
                platinPanel.Enabled = false;
                RemoveRank();
            }
        }

        private void DiamondCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (DiamondCheck.Checked)
            {
                EliteCheck.Enabled = true;
                diamondPanel.Enabled = true;
                PlatinCheck.Enabled = false;
                AddRank("diamond");
            }
            else
            {
                diamondMinBox.Text = String.Empty;
                diamondMaxBox.Text = String.Empty;
                EliteCheck.Enabled = false;
                diamondPanel.Enabled = false;
                PlatinCheck.Enabled = true;
                RemoveRank();
            }
        }

        private void EliteCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (EliteCheck.Checked)
            {
                elitePanel.Enabled = true;
                DiamondCheck.Enabled = false;
                AddRank("elite");
            }
            else
            {
                eliteMinBox.Text = String.Empty;
                eliteMaxBox.Text = String.Empty;
                DiamondCheck.Enabled = true;
                elitePanel.Enabled = false;
                RemoveRank();
            }
        }

        private void KvalueBar_Scroll(object sender, EventArgs e)
        {
            kvalueLabel.Text = KvalueBar.Value.ToString();
        }

        private void SratingBar_Scroll(object sender, EventArgs e)
        {
            startRLabel.Text = SratingBar.Value.ToString();
        }

        private void MatchmakBar_Scroll(object sender, EventArgs e)
        {
            MMlabel.Text = matchmakBar.Value.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (File.Exists("Games"))
            {
                File.Delete("Games");
            }

            using (var dbContext = new ConfigContext())
            {
                dbContext.Database.EnsureCreated();
                dbContext.SaveChanges();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            bool correct = true;
            string error_message = "";

            if(nameBox.Text == String.Empty)
            {
                correct = false;
                nameBox.BackColor = System.Drawing.Color.Red;
                error_message += "Podaj nazwę gry!\n";
            }
            else
            {
                nameBox.BackColor = System.Drawing.Color.White;
            }

            string server = serverBox.Text;

            try
            {
                string[] s = server.Split(':');
                string ip = s[0];
                string port = s[1];
                System.Net.IPAddress IP = System.Net.IPAddress.Parse(ip);
                int PortNum = Int32.Parse(port);
                serverBox.BackColor = System.Drawing.Color.White;
            }
            catch
            {
                correct = false;
                serverBox.BackColor = System.Drawing.Color.Red;
                error_message += "Błędny adres serwera gry!\n";
            }

            if (correct)
            {
                button2.Enabled = false;
                button1.Enabled = true;
                button5.Enabled = true;
                MessageBox.Show("Dane poprawne. Możesz utworzyć grę.");
                tabControl1.Enabled = false;
            }
            else
            {
                MessageBox.Show(error_message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Dane opisowe do gry:\n" +
                "+ Name (wymagana) - nazwa gry,\n" +
                "+ Description (opcjonalny) - zasady gry,\n" +
                "+ Server (wymagany) - adres serwera gry oraz port,\n" +
                "+ Max players (opcjonalny) - maksymalna liczba graczy,\n" +
                "+ Tie results (wymagany) - czy możliwe remisy,\n" +
                "+ Avg Match Time (opcjonalny) - średni czas rozgrywki w minutach.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Opcje dotyczące rankingu:\n" +
                "+ Ranks - jakie rangi mają być użyte w systemie,\n" +
                "+ K value - stała wyznaczająca dynamikę rankingu (im wyższa tym większa zdobycz/strata po rozgrywce),\n" +
                "+ Points ratio - czy brać pod uwagę stosunek zdobytych punktów po rozgrywce,\n" +
                "+ Start rating - początkowy skill rating nowego gracza,\n" +
                "+ Opponent limit - rama wyszukiwania przeciwnika (im wyższa tym możliwe wyszukanie przeciwnika z większą różnicą punktową).");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.Enabled = true;
            button1.Enabled = false;
            button5.Enabled = false;
            button2.Enabled = true;
        }
    }
}
