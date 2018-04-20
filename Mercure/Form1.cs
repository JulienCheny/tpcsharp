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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IntegratingDialog dial = new IntegratingDialog();
            dial.ShowDialog();
        }

        private void Form1_Load_1(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.Sort();

            //Add column header
            listView1.Columns.Add("RefArticle", 100);
            listView1.Columns.Add("Description", 70);
            listView1.Columns.Add("RefSousFamille", 70);
            listView1.Columns.Add("RefMArque", 70);
            listView1.Columns.Add("PrixHT", 70);
            listView1.Columns.Add("Quantite", 70);

            //Add items in the listview
            ListViewItem itm;

            List<string[]> articles = getArticles();

            foreach (var article in articles)
            {
                itm = new ListViewItem(article);
                listView1.Items.Add(itm);
            }
        }

        private List<string[]> getArticles()
        {
            List<string[]> articles = new List<string[]>();
            SQLiteConnection sqlite = new SQLiteConnection("Data source=C:/Users/JULIEN/Desktop/Divers/Polytech/tpcsharp/Mercure/Mercure.SQLite");
            sqlite.Open();
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT * FROM Articles";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string[] articleArr = new string[6];
                articleArr[0] = (string)reader["RefArticle"];
                articleArr[1] = (string)reader["Description"];
                articleArr[2] = ((int)reader["RefSousFamille"]).ToString();
                articleArr[3] = ((int)reader["RefMArque"]).ToString();
                articleArr[4] = ((double)reader["PrixHT"]).ToString();
                articleArr[5] = ((int)reader["Quantite"]).ToString();
                articles.Add(articleArr);
            }
            return articles;
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem item = listView1.SelectedItems[0];
                string refArticle = item.SubItems[0].Text;
                Console.WriteLine(refArticle);
                ModifyArticleDialog modifyArticleDial = new ModifyArticleDialog(refArticle);
                modifyArticleDial.Show();
            }
        }
    }
}
