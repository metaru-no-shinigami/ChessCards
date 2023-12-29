using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using ChessDotNet;

namespace ChessCards
{
    public partial class ChessCards : Form
    {

        public class Position
        {
            public int Catagory { get; set; }
            public int Successes { get; set; }
            public int TimesPlayed { get; set; }
            public string PositionURL { get; set; }
            public string Move { get; set; }
        }

        public class Flashcard
        {
            public List<Position> Position { get; set; }
        }

        public Position NewCard { get; set; }
        List<int> CompletedCards = new List<int>();
        private int Correct = 0;
        private int Total = 0;
        private int CardNum = 0;
        private readonly string Path = @"C:\Users\SirGr\source\repos\ChessCards\ChessCards\ChessCardsData.txt";
        private readonly string PrepPath = @"C:\Users\SirGr\source\repos\ChessCards\ChessCards\Prep.txt";
        private int[] Ranks = { 0, 0, 0, 0, 0, 0 };
        private int[] RanksCompleted = { 0, 0, 0, 0, 0, 0 };
        private bool Finished = false;

        public ChessCards()
        {
            InitializeComponent();
            this.KeyPreview = true;
            GenerateNewCard();
        }

        public void CheckStatistics()
        {
            string json = System.IO.File.ReadAllText(Path);
            var StatData = JsonConvert.DeserializeObject<Flashcard>(json);
            List<Position> CatagorySearch = StatData.Position;
            Array.Clear(Ranks, 0, 6);
            foreach (Position Search in CatagorySearch)
            {
                Ranks[Search.Catagory]++;
            }
        }

        public void GenerateNewCard()
        {
            string json = System.IO.File.ReadAllText(Path);
            var CardData = JsonConvert.DeserializeObject<Flashcard>(json);
            List<Position> SetData = CardData.Position;
            if (CompletedCards.Count == SetData.Count)
            {
                int Percent = (int)Math.Round((decimal)100 * Correct / Total);
                MessageBox.Show($"All cards complete! Your score is {Percent}% ({Correct}/{Total})", "Message");
                Finished = true;
            }
            else
            {
                Random random = new Random();
                List<Position> CompletedList = new List<Position>();
                int TempCatagory = random.Next(1, 6);
                CheckStatistics();
                Array.Clear(RanksCompleted, 0, 6);
                foreach (int id in CompletedCards)
                {
                    CompletedList.Add(SetData[id]);
                    RanksCompleted[SetData[id].Catagory]++;
                }
                bool Check = false;
                for (int i = 0; i <= TempCatagory; i++)
                {
                    if (Ranks[i] - RanksCompleted[i] != 0)
                    {
                        Check = true;
                        break;
                    }
                }
                if (!Check)
                {
                    for (int v = TempCatagory + 1; v <= 5; v++)
                    {
                        if (Ranks[v] - RanksCompleted[v] != 0)
                        {
                            TempCatagory = v;
                            break;
                        }
                    }
                }
                List<Position> CatagorySet = new List<Position>();
                foreach (Position Card in SetData)
                {
                    if (Card.Catagory <= TempCatagory && !CompletedList.Contains(Card))
                    {
                        CatagorySet.Add(Card);
                    }
                }
                int size = CatagorySet.Count;
                int CardTempNum = random.Next(size);
                NewCard = CatagorySet[CardTempNum]; 
                CardNum = SetData.IndexOf(NewCard);
                CompletedCards.Add(CardNum);
                pictureBox1.Load(NewCard.PositionURL);
            }

        }

        private void ChessCards_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && !Finished && textBox1.Text != "")
            {
                Total++;
                NewCard.TimesPlayed++;
                if (textBox1.Text == NewCard.Move)
                {
                    Correct++;
                    NewCard.Successes++;
                }
                else
                {
                    MessageBox.Show($"Incorrect! The answer was {NewCard.Move}", "Notice");
                }
                int SuccessRate = (int)Math.Round((decimal)100 * NewCard.Successes / NewCard.TimesPlayed);
                NewCard.Catagory = (int)Math.Ceiling((decimal)SuccessRate / 20);
                if (NewCard.Catagory == 0)
                {
                    NewCard.Catagory = 1;
                }
                string json = System.IO.File.ReadAllText(Path);
                var TempData = JsonConvert.DeserializeObject<Flashcard>(json);
                TempData.Position[CardNum] = NewCard;
                string WriteTempData = JsonConvert.SerializeObject(TempData, Formatting.Indented);
                System.IO.File.WriteAllText(Path, WriteTempData);
                textBox1.Clear();
                int Percent = (int)Math.Round((decimal)100 * Correct / Total);
                label1.Text = $"Score: {Percent}% ({Correct}/{Total})";
                GenerateNewCard();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        public void button1_Click(object sender, EventArgs e)
        {
            CheckStatistics();
            int T = Ranks[0] + Ranks[1] + Ranks[2] + Ranks[3] + Ranks[4] + Ranks[5];
            MessageBox.Show($"Total: {T} \nRank 1: {Ranks[5]} ({(int)Math.Round((decimal)100 * Ranks[5] / T)}%) \nRank 2: {Ranks[4]} ({(int)Math.Round((decimal)100 * Ranks[4] / T)}%) \nRank 3: {Ranks[3]} ({(int)Math.Round((decimal)100 * Ranks[3] / T)}%) \nRank 4: {Ranks[2]} ({(int)Math.Round((decimal)100 * Ranks[2] / T)}%) \nRank 5: {Ranks[1]} ({(int)Math.Round((decimal)100 * Ranks[1] / T)}%) \nUnranked: {Ranks[0]} ({(int)Math.Round((decimal)100 * Ranks[0] / T)}%) \n\n*NOTE: Each rank is 20%", "Stats");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] opening = System.IO.File.ReadAllLines(PrepPath);
            bool IsWhite = true;
            List<string> URLs = new List<string>();
            int Added = 0;
            int Removed = 0;
            foreach (string line in opening)
            {
                if (line == "White")
                {
                    IsWhite = true;
                }
                else if (line == "Black")
                {
                    IsWhite = false;
                }
                else if (line[0] == '[' && line[line.Length - 1] == ']')
                {

                }
                else
                {
                    List<string> Moves = new List<string>();
                    string[] Splice = line.Split(' ');
                    string json = System.IO.File.ReadAllText(Path);
                    var TempData = JsonConvert.DeserializeObject<Flashcard>(json);
                    List<Position> CardList = TempData.Position;
                    for (int S = 3; S <= Splice.Length; S += 3)
                    {
                        string SANMove = Splice[S - 3] + " " + Splice[S - 2] + " " + Splice[S - 1] + " ";
                        Moves.Add(SANMove);
                    }
                    string Rolling = "";
                    for (int IndexNum = 0; IndexNum < Moves.Count - 1; IndexNum++)
                    {

                        string URL = "";
                        string CardMove = "";
                        PgnReader<ChessGame> reader = new PgnReader<ChessGame>();
                        if (IsWhite)
                        {
                            
                            CardMove = Moves[IndexNum + 1].Split(' ')[1];
                            Rolling += Moves[IndexNum];
                            reader.ReadPgnFromString(Rolling);
                            string FEN = reader.Game.GetFen().Replace(" ", "_");
                            URL = "https://fen2image.chessvision.ai/" + FEN + "?turn=white";
                        }
                        else
                        {
                            
                            CardMove = Moves[IndexNum + 1].Split(' ')[2];
                            Rolling += Moves[IndexNum];
                            reader.ReadPgnFromString(Rolling + Moves[IndexNum + 1].Split(' ')[0] + Moves[IndexNum + 1].Split(' ')[1]);
                            string FEN = reader.Game.GetFen().Replace(" ", "_");
                            URL = "https://fen2image.chessvision.ai/" + FEN + "?turn=black&pov=black";
                        }
                        if (!URLs.Contains(URL))
                        {
                            URLs.Add(URL);
                        }
                        bool Unique = true;
                        foreach (Position Flash in CardList)
                        {
                            if (Flash.PositionURL == URL)
                            {
                                Unique = false;
                            }
                        }
                        if (Unique)
                        {
                            Position GeneratedCard = new Position();
                            GeneratedCard.Catagory = 0;
                            GeneratedCard.Successes = 0;
                            GeneratedCard.TimesPlayed = 0;
                            GeneratedCard.PositionURL = URL;
                            GeneratedCard.Move = CardMove;
                            CardList.Add(GeneratedCard);
                            string WriteTempData = JsonConvert.SerializeObject(TempData, Formatting.Indented);
                            System.IO.File.WriteAllText(Path, WriteTempData);
                            Added++;
                        }
                    }
                }
            }
            string jsons = System.IO.File.ReadAllText(Path);
            var CheckData = JsonConvert.DeserializeObject<Flashcard>(jsons);
            List<Position> Checklist = CheckData.Position;
            List<Position> MarkForRemoval = new List<Position>();
            foreach (Position Check in Checklist)
            {
                if (!URLs.Contains(Check.PositionURL))
                { 
                    MarkForRemoval.Add(Check);
                    Removed++;
                }
            }
            foreach (Position Fish in MarkForRemoval)
            {
                Checklist.Remove(Fish);
            }
            string WriteTempDatas = JsonConvert.SerializeObject(CheckData, Formatting.Indented);
            System.IO.File.WriteAllText(Path, WriteTempDatas);
            MessageBox.Show($"Done! {Added} added and {Removed} removed", "Notice");
        }
    }
}

