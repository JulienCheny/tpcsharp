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

        private SQLiteConnection sqlite;

        public Form1()
        {
            sqlite = new SQLiteConnection("Data source=Mercure.SQLite");
            sqlite.Open();
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

            refresh();
        }

        private void refresh()
        {
            listView1.Items.Clear();
            listView1.Update();

            //Add items in the listview
            ListViewItem itm;

            List<string[]> articles = getArticles();

            foreach (var article in articles)
            {
                itm = new ListViewItem(article);
                listView1.Items.Add(itm);
            }

            listView1.Refresh();
        }

        private List<string[]> getArticles()
        {
            List<string[]> articles = new List<string[]>();
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT RefArticle, Description, SousFamilles.Nom as NomSousFamille, Marques.Nom as NomMarque, PrixHT, Quantite FROM Articles INNER JOIN Marques ON Articles.RefMarque = Marques.RefMarque INNER JOIN SousFamilles ON SousFamilles.RefSousFamille = Articles.RefSousFamille";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                string[] articleArr = new string[6];
                articleArr[0] = (string)reader["RefArticle"];
                articleArr[1] = (string)reader["Description"];
                articleArr[2] = (string)reader["NomSousFamille"];
                articleArr[3] = (string)reader["NomMarque"];
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
                openUpdateArticleModal(refArticle);
            }
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    contextMenuStrip1.Show(Cursor.Position);
                }
            } 
        }

        protected override bool ProcessCmdKey (ref Message msg, Keys keyData)
        {
            bool bHandled = false;
            switch (keyData)
            {
                case Keys.F5:
                    refresh();
                    bHandled = true;
                    break;
            }
            return bHandled;
        }

        private void modifierToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            string refArticle = item.SubItems[0].Text;
            openUpdateArticleModal(refArticle);
        }

        private void openUpdateArticleModal(string refArticle)
        {
            ModifyArticleDialog modifyArticleDial = new ModifyArticleDialog(sqlite, refArticle);
            DialogResult dialogResult = modifyArticleDial.ShowDialog();
            if (dialogResult == DialogResult.OK)
                refresh();
        }

        private void supprimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem item = listView1.SelectedItems[0];
            string refArticle = item.SubItems[0].Text;

            var confirmDelete = MessageBox.Show("Voulez vous vraiment supprimer cet article ?", "Comfirmation", MessageBoxButtons.YesNo);
            if (confirmDelete == DialogResult.Yes)
            {
                SQLiteCommand cmd;
                cmd = sqlite.CreateCommand();
                cmd.CommandText = "DELETE FROM Articles WHERE RefArticle = @RefArticle";
                cmd.Parameters.Add(new SQLiteParameter("@RefArticle", refArticle));
                try
                {
                    cmd.ExecuteNonQuery();
                    refresh();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        private void ajouterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModifyArticleDialog modifyArticleDial = new ModifyArticleDialog(sqlite);
            DialogResult dialogResult = modifyArticleDial.ShowDialog();
            if (dialogResult == DialogResult.OK)
                refresh();
        }
    }
}
