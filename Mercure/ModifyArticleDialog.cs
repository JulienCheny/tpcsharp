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
            InitializeComponent();
            SQLiteConnection sqlite = new SQLiteConnection("Data source=C:/Users/JULIEN/Desktop/Divers/Polytech/tpcsharp/Mercure/Mercure.SQLite");
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
    }
}
