using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Revisione2
{
    public partial class FormPrincipale : Form
    {

        public static string oldFolder = null;
        public static string newFolder = null;
        public static List<FileInfo> fileAggiornati;

        public FormPrincipale()
        {
            InitializeComponent();
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            button4.Enabled = false;
        }


        private static string choosePath()
        {
            string path = null;
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    path = fbd.SelectedPath;
                }
            }
            return path;
        }

        private static void aggiornoListView(ListView lw, IDictionary<string, bool> relazione, Button btn)
        {
            lw.Items.Clear();
            
            if (fileAggiornati.Count > 0)
            {
                foreach (FileInfo fi in fileAggiornati)
                {
                    string nuovo = "Nuovo";

                    if (relazione.ContainsKey(fi.Name)) 
                    {
                        if (!relazione[fi.Name])
                        {
                            nuovo = "Modificato";
                        }
                    }

                    string[] row = { fi.Name, fi.LastWriteTime.ToString(), nuovo, ""};
                    var listViewItem = new ListViewItem(row);
                    lw.Items.Add(listViewItem);
                }

                btn.Enabled = true;

                resizeColumns(lw);

                // Controller.generaFolderNuoviFile(fileAggiornati);
            }
            else
            {
                MessageBox.Show("Nessun file è stato aggiornato");

                btn.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oldFolder = choosePath();
            textBox1.Text = oldFolder;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            newFolder = choosePath();
            textBox2.Text = newFolder;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(newFolder) && !string.IsNullOrEmpty(oldFolder))
            {

                var analisi = Controller.trovoRevisioniRecenti(Controller.getFolderFile(@oldFolder), Controller.getFolderFile(@newFolder));

                fileAggiornati = analisi.Item1;

                IDictionary<string, bool> relazione = analisi.Item2;

                aggiornoListView(listView1, relazione, button4);
            }
            else
            {
                MessageBox.Show("Selezionare le cartelle");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (fileAggiornati.Count > 0)
            {
                IDictionary<string, bool>  results = Controller.replaceFileInOldFolder(fileAggiornati, @oldFolder);

                foreach (ListViewItem x in listView1.Items)
                {
                    if (results.ContainsKey(x.Text))
                    {
                        if (results[x.Text])
                        {
                            x.SubItems[3].Text = "Aggiornato";
                        }
                        else
                        {
                            x.SubItems[3].Text = "Non aggiornato";
                        }

                    }
                }

                resizeColumns(listView1);
            }
        }

        private static void resizeColumns(ListView lw)
        {
            lw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }
    }
}
