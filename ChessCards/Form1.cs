using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

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
        private readonly string path = @"C:\Users\SirGr\source\repos\ChessCards\ChessCards\ChessCardsData.txt";

        public ChessCards()
        {
            InitializeComponent();
            this.KeyPreview = true;
            GenerateNewCard();
        }

        public void GenerateNewCard()
        {
            string json = File.ReadAllText(path);
            var CardData = JsonConvert.DeserializeObject<Flashcard>(json);
            List<Position> SetData = CardData.Position;
            if (CompletedCards.Count == SetData.Count)
            {
                int Percent = (int)Math.Round((decimal) 100 * Correct / Total);
                MessageBox.Show($"All cards complete! Your score is {Percent}% ({Correct}/{Total})", "Message");
            }
            else
            {
                Random random = new Random();
                int TempCatagory = random.Next(1, 6);
                List<Position> CatagorySet = new List<Position>();
                foreach (Position Card in SetData)
                {
                    if (Card.Catagory <= TempCatagory)
                    {
                        CatagorySet.Add(Card);
                    }
                }
                int size = CatagorySet.Count;
                if (size == 0)
                {
                    CatagorySet = SetData;
                    size = CatagorySet.Count;
                }
                int CardTempNum = random.Next(size);
                NewCard = CatagorySet[CardTempNum];
                CardNum = SetData.IndexOf(NewCard);
                while (CompletedCards.Count < SetData.Count && CompletedCards.Contains(CardNum))
                {
                    CardTempNum = random.Next(size);
                    NewCard = CatagorySet[CardTempNum];
                    CardNum = SetData.IndexOf(NewCard);
                }
                pictureBox1.Load(NewCard.PositionURL);
            }
            if (CompletedCards.Contains(CardNum) == false)
            {
                CompletedCards.Add(CardNum);
            }

        }

        private void ChessCards_KeyDown(object sender, KeyEventArgs e)
        {
            string json = File.ReadAllText(path);
            var TempData = JsonConvert.DeserializeObject<Flashcard>(json);
            if (e.KeyCode == Keys.Enter && CompletedCards.Count <= TempData.Position.Count && textBox1.Text != null)
            {
                Total++;
                NewCard.TimesPlayed++;
                if (textBox1.Text == NewCard.Move)
                {
                    Correct++;
                    NewCard.Successes++;
                }
                int SuccessRate = (int)Math.Round((decimal)100 * NewCard.Successes / NewCard.TimesPlayed);
                NewCard.Catagory = 5 - (int)Math.Floor((decimal)(100 - SuccessRate) / 20);
                TempData.Position[CardNum] = NewCard;
                string WriteTempData = JsonConvert.SerializeObject(TempData, Formatting.Indented);
                File.WriteAllText(path, WriteTempData);
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
            string json = File.ReadAllText(path);
            var StatData = JsonConvert.DeserializeObject<Flashcard>(json);
            List<Position> CatagorySearch = StatData.Position;
            int[] Ranks = {0, 0, 0, 0, 0, 0};
            foreach (Position Search in CatagorySearch)
            {
                Ranks[Search.Catagory]++;
            }
            MessageBox.Show($"Total: {CatagorySearch.Count} \nRank 1: {Ranks[5]} ({(int)Math.Round((decimal)100 * Ranks[5] / CatagorySearch.Count)}%) \nRank 2: {Ranks[4]} ({(int)Math.Round((decimal)100 * Ranks[4] / CatagorySearch.Count)}%) \nRank 3: {Ranks[3]} ({(int)Math.Round((decimal)100 * Ranks[3] / CatagorySearch.Count)}%) \nRank 4: {Ranks[2]} ({(int)Math.Round((decimal)100 * Ranks[2] / CatagorySearch.Count)}%) \nRank 5: {Ranks[1]} ({(int)Math.Round((decimal)100 * Ranks[1] / CatagorySearch.Count)}%) \nUnranked: {Ranks[0]} ({(int)Math.Round((decimal)100 * Ranks[0] / CatagorySearch.Count)}%) \n\n*NOTE: Each rank is 20%", "Stats");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
