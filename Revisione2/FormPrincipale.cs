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

        private static void aggiornoListView(ListView lw, Button btn)
        {
            lw.Items.Clear();
            
            if (fileAggiornati.Count > 0)
            {
                foreach (FileInfo fi in fileAggiornati)
                {
                    string[] row = { fi.Name, fi.LastWriteTime.ToString()};
                    var listViewItem = new ListViewItem(row);
                    lw.Items.Add(listViewItem);
                }

                btn.Enabled = true;

                lw.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

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

                fileAggiornati = Controller.trovoRevisioniRecenti(Controller.getFolderFile(@oldFolder), Controller.getFolderFile(newFolder));
                aggiornoListView(listView1, button4);
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
                Controller.replaceFileInOldFolder(fileAggiornati, oldFolder);

                MessageBox.Show("Operazione Conclusa");
            }
        }
    }
}
