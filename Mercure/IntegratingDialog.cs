using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SQLite;

namespace Mercure
{
    public partial class IntegratingDialog : Form
    {
        public IntegratingDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(openFileDialog1.FileName);
                textBox1.Text = openFileDialog1.FileName;
                sr.Close();
            } 
        }

        private void integrateButton_Click(object sender, EventArgs e)
        {
            if (newRButton.Checked) 
            {
                newIntegration();
            }
            else
            {
                Console.WriteLine("Update");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void newIntegration()
        {
            Console.WriteLine("Start new integration");

            // creat xml object
            string xmlPath = textBox1.Text;
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlPath);
            XmlNodeList nodeList = xmlDocument.SelectNodes("/materiels/article");
            
            // open sqlite connection
            SQLiteConnection sqlite = new SQLiteConnection("Data source=/Dev/c++/tpcsharp/Mercure/Mercure.SQLite");
            
            sqlite.Open();
            
            // loop over all article in the xml
            foreach (XmlNode node in nodeList)
            {
                // read article information
                string description = node.SelectSingleNode("description").InnerText;
                string refArticle = node.SelectSingleNode("refArticle").InnerText;
                string marque = node.SelectSingleNode("marque").InnerText;
                string famille = node.SelectSingleNode("famille").InnerText;
                string sousFamille = node.SelectSingleNode("sousFamille").InnerText;
                double prixHT = Math.Round(float.Parse(node.SelectSingleNode("prixHT").InnerText.Replace(",", ".")),2);

                // add article to database
                SQLiteCommand cmd;
                cmd = sqlite.CreateCommand();
                cmd.CommandText = "INSERT INTO Articles (RefArticle, Description, RefSousFamille, RefMarque, PrixHT, Quantite) VALUES (@RefArticle, @Description, @RefSousFamille, @RefMarque, @PrixHT, @Quantite );";
                
                int refMarque = getRefMarque(sqlite, marque);
                if (refMarque == -1)
                {
                    addMarque(sqlite, marque);
                    refMarque = getRefMarque(sqlite, marque);
                }

                int refFamille = getRefFamille(sqlite, famille);
                if (refFamille == -1)
                {
                    addFamille(sqlite, famille);
                }

                cmd.Parameters.Add(new SQLiteParameter("@RefArticle", refArticle));
                cmd.Parameters.Add(new SQLiteParameter("@Description", description));
                cmd.Parameters.Add(new SQLiteParameter("@RefSousFamille", 222));
                cmd.Parameters.Add(new SQLiteParameter("@RefMarque", refMarque));
                cmd.Parameters.Add(new SQLiteParameter("@PrixHT", prixHT));
                cmd.Parameters.Add(new SQLiteParameter("@Quantite", 1));
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }   

            }
            Console.WriteLine("Done");
            sqlite.Close();

        }

        private int getRefMarque(SQLiteConnection sqlite, string brand)
        {
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT RefMarque FROM Marques WHERE Nom = @brand";
            cmd.Parameters.Add(new SQLiteParameter("@brand", brand));
            try
            {
                Object result = cmd.ExecuteScalar();
                if (result == null)
                    return -1;
                else
                    return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }   
        }

        private int getRefFamille(SQLiteConnection sqlite, string famille)
        {
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT RefFamille FROM Familles WHERE Nom = @famille";
            cmd.Parameters.Add(new SQLiteParameter("@famille", famille));
            try
            {
                Object result = cmd.ExecuteScalar();
                if (result == null)
                    return -1;
                else
                    return Convert.ToInt32(result);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        private void addMarque(SQLiteConnection sqlite, string nom)
        {
            int newRef; 
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT MAX(RefMarque) FROM Marques";
            try
            {
                object obj = cmd.ExecuteScalar();
                if(obj.GetType() == typeof(DBNull))
                    newRef = 1;
                else
                    newRef =  Convert.ToInt32(cmd.ExecuteScalar())+1;
               
                cmd = sqlite.CreateCommand();
                cmd.CommandText = "INSERT INTO Marques (RefMarque, Nom) values (@RedMarque, @Nom)";
                cmd.Parameters.Add(new SQLiteParameter("@RedMarque", newRef));
                cmd.Parameters.Add(new SQLiteParameter("@Nom", nom));
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                } 

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            } 
            
        }

        private void addFamille(SQLiteConnection sqlite, string nom)
        {
            int newRef;
            SQLiteCommand cmd;
            cmd = sqlite.CreateCommand();
            cmd.CommandText = "SELECT MAX(RefFamille) FROM Familles";
            try
            {
                object obj = cmd.ExecuteScalar();
                if (obj.GetType() == typeof(DBNull))
                    newRef = 1;
                else
                    newRef = Convert.ToInt32(cmd.ExecuteScalar()) + 1;

                cmd = sqlite.CreateCommand();
                cmd.CommandText = "INSERT INTO Familles (RefFamille, Nom) values (@RefFamille, @Nom)";
                cmd.Parameters.Add(new SQLiteParameter("@RefFamille", newRef));
                cmd.Parameters.Add(new SQLiteParameter("@Nom", nom));
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }

        }
    }
}
