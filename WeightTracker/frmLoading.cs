using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WeightTracker
{
    public partial class frmLoading : Form
    {
        public frmLoading(string message)
        {
            InitializeComponent();
            lblMessage.Text = message;
        }

        private void frmLoading_Load(object sender, EventArgs e)
        {

        }
    }
}
