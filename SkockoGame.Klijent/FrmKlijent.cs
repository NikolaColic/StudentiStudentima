using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SkockoGame.Klijent
{
    public partial class FrmKlijent : Form
    {
        public FrmKlijent()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void FrmKlijent_Load(object sender, EventArgs e)
        {
            if (!Komunikacija.Instance.PoveziSe())
            {
                 MessageBox.Show("Ne moze da se poveze");
                return;
            }
            Komunikacija.Instance.forma = this;
            new Thread(Komunikacija.Instance.PrijemPoruke).Start();
        }
        public void PrikaziPoruku(string poruka)
        {
            MessageBox.Show(poruka);
        }
        private void btnIme_Click(object sender, EventArgs e)
        {
            Komunikacija.Instance.PosaljiPoruku(txtIme.Text);
        }

        internal void DodajPorukuRezultat(string poruka)
        {
            lblRez.Text = poruka;
        }

        internal void DodajPorukuProtivnik(string poruka)
        {
            lblProtivnik.Text = poruka;
        }

        internal void DodajPorukuJa(string poruka)
        {
            lblJa.Text = poruka;
        }

        private void btnZvezda_Click(object sender, EventArgs e)
        {
            TextBox txt = ProveraTxt("Zvezda ");
            if (txt is null) return;
            Komunikacija.Instance.PosaljiPoruku(txt.Text);
        }

        private void btnSkocko_Click(object sender, EventArgs e)
        {
            TextBox txt = ProveraTxt("Skocko ");
            if (txt is null) return;
            Komunikacija.Instance.PosaljiPoruku(txt.Text);
        }
        private TextBox ProveraTxt(string tekst)
        {
            if (ProveraBrojReci(txtPrva) <= 3)
            {
                txtPrva.Text += tekst;
                if (ProveraBrojReci(txtPrva) == 4) return txtPrva;
                return null;
            }
            if (ProveraBrojReci(txtDruga) <= 3)
            {
                txtDruga.Text += tekst;
                if (ProveraBrojReci(txtDruga) == 4) return txtDruga;
                return null;
            }
            if (ProveraBrojReci(txtTreca) <= 3)
            {
                txtTreca.Text += tekst;
                if (ProveraBrojReci(txtTreca) == 4) return txtTreca;
                return null;
            }
            if (ProveraBrojReci(txtCetvrta) <= 3)
            {
                txtCetvrta.Text += tekst;
                if (ProveraBrojReci(txtCetvrta) == 4) return txtCetvrta;
                return null;
            }
            if (ProveraBrojReci(txtPeta) <= 3)
            {
                txtPeta.Text += tekst;
                if (ProveraBrojReci(txtPeta) == 4) return txtPeta;
                return null;
            }
            return null;

        }

        private int ProveraBrojReci(TextBox txt)
        {
            //return txt.Text.Split(' ').Length -1;
            return txt.Text.Split(' ').Count(rec => rec != "");

        }

        private void btnTref_Click(object sender, EventArgs e)
        {
            TextBox txt = ProveraTxt("Tref ");
            if (txt is null) return;
            Komunikacija.Instance.PosaljiPoruku(txt.Text);
        }

        private void btnKaro_Click(object sender, EventArgs e)
        {
            TextBox txt = ProveraTxt("Karo ");
            if (txt is null) return;
            Komunikacija.Instance.PosaljiPoruku(txt.Text);
        }

        private void btnPik_Click(object sender, EventArgs e)
        {
            TextBox txt = ProveraTxt("Pik ");
            if (txt is null) return;
            Komunikacija.Instance.PosaljiPoruku(txt.Text);
        }

        private void btnHerc_Click(object sender, EventArgs e)
        {
            TextBox txt = ProveraTxt("Herc ");
            if (txt is null) return;
            Komunikacija.Instance.PosaljiPoruku(txt.Text);
        }
    }
}
