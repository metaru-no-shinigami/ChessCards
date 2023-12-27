﻿using System;
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
        private int[] Ranks = { 0, 0, 0, 0, 0, 0 };
        private bool Finished = false;

        public ChessCards()
        {
            InitializeComponent();
            this.KeyPreview = true;
            GenerateNewCard();
        }

        public void CheckStatistics()
        {
            string json = File.ReadAllText(path);
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
            string json = File.ReadAllText(path);
            var CardData = JsonConvert.DeserializeObject<Flashcard>(json);
            List<Position> SetData = CardData.Position;
            if (CompletedCards.Count == SetData.Count)
            {
                int Percent = (int)Math.Round((decimal) 100 * Correct / Total);
                MessageBox.Show($"All cards complete! Your score is {Percent}% ({Correct}/{Total})", "Message");
                Finished = true;
            }
            else
            {
                Random random = new Random();
                int TempCatagory = random.Next(1, 6);
                CheckStatistics();
                int Check = 0;
                for (int i = 0; i <= TempCatagory; i++)
                {
                    Check = Check + Ranks[i];
                }
                if (Check == 0)
                {
                    TempCatagory = 5;
                }
                List<Position> CatagorySet = new List<Position>();
                List<Position> CompletedList = new List<Position>();
                foreach (int id in CompletedCards)
                {
                    CompletedList.Add(SetData[id]);
                }
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
                NewCard.Catagory = (int)Math.Ceiling((decimal) SuccessRate / 20);
                string json = File.ReadAllText(path);
                var TempData = JsonConvert.DeserializeObject<Flashcard>(json);
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
            CheckStatistics();
            MessageBox.Show($"Total: {CatagorySearch.Count} \nRank 1: {Ranks[5]} ({(int)Math.Round((decimal)100 * Ranks[5] / CatagorySearch.Count)}%) \nRank 2: {Ranks[4]} ({(int)Math.Round((decimal)100 * Ranks[4] / CatagorySearch.Count)}%) \nRank 3: {Ranks[3]} ({(int)Math.Round((decimal)100 * Ranks[3] / CatagorySearch.Count)}%) \nRank 4: {Ranks[2]} ({(int)Math.Round((decimal)100 * Ranks[2] / CatagorySearch.Count)}%) \nRank 5: {Ranks[1]} ({(int)Math.Round((decimal)100 * Ranks[1] / CatagorySearch.Count)}%) \nUnranked: {Ranks[0]} ({(int)Math.Round((decimal)100 * Ranks[0] / CatagorySearch.Count)}%) \n\n*NOTE: Each rank is 20%", "Stats");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
