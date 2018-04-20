using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace Mercure
{
    public partial class ModifyArticleDialog : Form
    {
        private string lastRefArticle;
        private SQLiteConnection sqlite = new SQLiteConnection("Data source=Mercure.SQLite");
        public class ComboboxItem
        {
            public string Text { get; set; }
            public int Value { get; set; }

            public ComboboxItem(string text, int value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString()
            {
                return Text;
            }
        }

        public ModifyArticleDialog(string refArticle)
        {
            this.lastRefArticle = refArticle;
            setupWindow();
            /////////   get ARTICLE    ///////////
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT * FROM Articles WHERE RefArticle = @RefArticle";
            cmd.Parameters.Add(new SQLiteParameter("@RefArticle", refArticle));
            SQLiteDataReader reader;
            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            reader.Read();
            textBox1.Text = (string)reader["RefArticle"];
            textBox2.Text = (string)reader["Description"];
            textBox3.Text = ((double) reader["PrixHT"]).ToString();
            textBox4.Text = ((int)reader["Quantite"]).ToString();
            comboBox1.SelectedIndex = findComboboxIndexBy((int)reader["RefSousFamille"], comboBox1);
            comboBox2.SelectedIndex = findComboboxIndexBy((int)reader["RefMarque"], comboBox2);
            sqlite.Close();

            /*articleArr[2] = ((int)reader["RefSousFamille"]).ToString();
            articleArr[3] = ((int)reader["RefMArque"]).ToString();*/

        }

        public ModifyArticleDialog()
        {
            setupWindow();
        }

        private void setupWindow() 
        {
            InitializeComponent();
            sqlite.Open();

            /////////   get all "sous_famille"     ///////////

            SQLiteCommand cmdFamily;
            cmdFamily = sqlite.CreateCommand();
            cmdFamily.CommandText = "SELECT * FROM SousFamilles";
            SQLiteDataReader readerFamily;
            try
            {
                readerFamily = cmdFamily.ExecuteReader();
                while (readerFamily.Read())
                {
                    comboBox1.Items.Add(new ComboboxItem((string)readerFamily["Nom"], (int)readerFamily["RefSousFamille"]));
                }
            }
            catch (Exception ex)
            {
            }

            /////////   get all "marque"     ///////////

            SQLiteCommand cmdBrand;
            cmdBrand = sqlite.CreateCommand();
            cmdBrand.CommandText = "SELECT * FROM Marques";
            SQLiteDataReader readerBrand;
            try
            {
                readerBrand = cmdBrand.ExecuteReader();
                while (readerBrand.Read())
                {
                    comboBox2.Items.Add(new ComboboxItem((string)readerBrand["Nom"], (int)readerBrand["RefMarque"]));
                }
            }
            catch (Exception ex)
            {
            }
        }

        private int findComboboxIndexBy(int id, ComboBox cBox)
        {
            int indexCount = 0;
            foreach (ComboboxItem currentItem in cBox.Items)
            {
                if (currentItem.Value == id)
                    return indexCount;
                indexCount++;
            }
            return -1;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            if (lastRefArticle == "")
            {
                cmd.CommandText = "INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@RefArticle, @Description, @RefSousFamille, @RefMarque, @PrixHT, @Quantite );";
            }
            else
            {
                cmd.CommandText = "UPDATE Articles SET RefArticle =  @RefArticle, Description = @Description, RefSousFamille = @RefSousFamille, RefMarque = @RefMarque, PrixHT = @PrixHT, Quantite = @Quantite FROM Articles WHERE RefArticle = @LastRefArticle";
                cmd.Parameters.Add(new SQLiteParameter("@LastRefArticle", lastRefArticle));
            }
            cmd.Parameters.Add(new SQLiteParameter("@RefArticle", textBox1.Text));
            cmd.Parameters.Add(new SQLiteParameter("@Description", textBox2.Text));
            ComboboxItem itemFamily = (ComboboxItem)comboBox1.SelectedItem;
            ComboboxItem itemBrand = (ComboboxItem)comboBox2.SelectedItem;

            cmd.Parameters.Add(new SQLiteParameter("@RefSousFamille", itemFamily.Value));
            cmd.Parameters.Add(new SQLiteParameter("@RefMarque", itemBrand.Value));
            cmd.Parameters.Add(new SQLiteParameter("@PrixHT", Convert.ToDouble(textBox3.Text)));
            cmd.Parameters.Add(new SQLiteParameter("@Quantite", Convert.ToInt32(textBox4.Text)));
            cmd.ExecuteNonQuery();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
