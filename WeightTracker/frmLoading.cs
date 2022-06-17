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
