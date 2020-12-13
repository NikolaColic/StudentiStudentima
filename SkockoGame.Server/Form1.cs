using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SkockoGame.Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPokreni_Click(object sender, EventArgs e)
        {
            if (Server.Instance.PokreniServer())
            {
                MessageBox.Show("Uspesno!");
            }
            else
            {
                MessageBox.Show("Neuspesno!");
            }
        }

        private void btnZaustavi_Click(object sender, EventArgs e)
        {
            if (Server.Instance.ZaustaviServer())
            {
                MessageBox.Show("Uspesno!");
            }
            else
            {
                MessageBox.Show("Neuspesno!");
            }
        }

        private void Form1_Leave(object sender, EventArgs e)
        {
            Server.Instance.ZaustaviServer();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Server.Instance.ZaustaviServer();

        }
    }
}
