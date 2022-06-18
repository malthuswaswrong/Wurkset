using NUlid;
using Wurkset;

namespace WeightTracker;

public partial class frmMain : Form
{
    WorksetRepository wsr;
    double bftarget = 15f;
    double caloriesIn1Lbs = 3500f;
    Ulid? selectedId = null;
    Workset<WeightEntry>? selectedWeightEntryWorkset = null;
    public Workset<WeightEntry>? SelectedWeightEntryWorkset
    {
        get
        {
            if (lvWeightData.SelectedItems.Count > 0)
            {
                Ulid wsId = (Ulid)lvWeightData.SelectedItems[0].Tag;
                if (selectedId != wsId)
                {
                    selectedId = wsId;
                    selectedWeightEntryWorkset = wsr.GetById<WeightEntry>(wsId);
                }
            }
            else if (selectedId is not null)
            {
                selectedId = null;
                selectedWeightEntryWorkset = null;
            }

            return selectedWeightEntryWorkset;
        }
    }
    public frmMain()
    {
        InitializeComponent();
    }
    private void frmMain_Load(object sender, EventArgs e)
    {
        using frmLoading loading = new frmLoading("Loading");
        loading.Show();
        wsr = new WorksetRepository(options =>
        {
            options.BasePath = Path.Combine(Directory.GetCurrentDirectory(), "WeightData");
        });
        bool todayFound = false;
        foreach (var weightEntry in wsr.GetAll<WeightEntry>().ToList().OrderBy(x => x.Value.Day))
        {
            if (AddLVI(weightEntry).Text == DateTime.Now.ToString("d"))
            {
                todayFound = true;
            }
        }
        if (!todayFound)
        {
            var todayWS = wsr.Create(
                new WeightEntry()
                {
                    Day = DateTime.Now,
                    Weight = 0,
                    BFPercent = 0
                }
            );
            AddLVI(todayWS);
        }

        lvWeightData.EnsureVisible(lvWeightData.Items.Count - 1);
        loading.Hide();
    }

    private ListViewItem AddLVI(Workset<WeightEntry> weightEntry, int? index = null)
    {

        ListViewItem lvi = UpdateLVI(new ListViewItem(), weightEntry);
        if (index is null)
        {
            lvWeightData.Items.Add(lvi);
        }
        else
        {
            lvWeightData.Items.Insert(index.Value, lvi);
        }
        lvWeightData.Refresh();
        return lvi;
    }
    private ListViewItem UpdateLVI(ListViewItem lvi, Workset<WeightEntry> weightEntry)
    {
        double extraPercent = weightEntry.Value.BFPercent - bftarget;
        double weightPlus = (weightEntry.Value.Weight * (extraPercent / 100));
        double calories = weightPlus * caloriesIn1Lbs;
        lvi.SubItems.Clear();
        lvi.Text = weightEntry.Value.Day.ToString("d");
        lvi.SubItems.Add(weightEntry.Value.Weight.ToString());
        lvi.SubItems.Add(weightEntry.Value.BFPercent.ToString());
        lvi.SubItems.Add(weightPlus.ToString("0"));
        lvi.SubItems.Add(calories.ToString("0"));
        lvi.Tag = weightEntry.WorksetId;
        lvWeightData.Refresh();
        return lvi;
    }

    private void lvWeightData_SelectedIndexChanged(object sender, EventArgs e)
    {
        toolStripStatusLabel1.Text = "";
        //If nothing selected clear the controlls
        if (SelectedWeightEntryWorkset is null)
        {
            lblDate.Text = "not selected";
            txtWeight.Text = "";
            txtBFPercent.Text = "";
            return;
        }
        //Update the controls
        lblDate.Text = SelectedWeightEntryWorkset.Value.Day.ToString("d");
        txtWeight.Text = SelectedWeightEntryWorkset.Value.Weight.ToString();
        txtBFPercent.Text = SelectedWeightEntryWorkset.Value.BFPercent.ToString();
    }

    private void cmdSave_Click(object sender, EventArgs e)
    {
        //Exit if nothing selected
        if (SelectedWeightEntryWorkset is null)
        {
            MessageBox.Show("No entry selected");
            return;
        }

        //Update workset
        toolStripStatusLabel1.Text = "Saving...";
        SelectedWeightEntryWorkset.Value.Weight = double.Parse(txtWeight.Text);
        SelectedWeightEntryWorkset.Value.BFPercent = double.Parse(txtBFPercent.Text);
        SelectedWeightEntryWorkset.Save();

        //Update UI
        UpdateLVI(lvWeightData.SelectedItems[0], SelectedWeightEntryWorkset);
        toolStripStatusLabel1.Text = "Saved";
    }

    private void cmdAddBefore_Click(object sender, EventArgs e)
    {
        //Exit if nothing selected
        if (SelectedWeightEntryWorkset is null)
        {
            MessageBox.Show("No entry selected");
            return;
        }

        AddNewDay(-1);
    }

    private void AddNewDay(int direction)
    {
        ArgumentNullException.ThrowIfNull(SelectedWeightEntryWorkset, nameof(SelectedWeightEntryWorkset));
        DateTime startingDay = SelectedWeightEntryWorkset.Value.Day;
        DateTime newDay = startingDay.AddDays(direction);
        var existing = wsr.GetAll<WeightEntry>().Where(x => x.Value.Day == newDay).FirstOrDefault();
        if (existing is not null)
        {
            MessageBox.Show($"Entry already exists for {newDay.ToString("d")}");
            return;
        }

        //Create workset
        var newWS = wsr.Create(
            new WeightEntry()
            {
                Day = newDay,
                Weight = SelectedWeightEntryWorkset.Value.Weight,
                BFPercent = SelectedWeightEntryWorkset.Value.BFPercent
            }
        );

        int newidx = (direction < 0) ? lvWeightData.SelectedItems[0].Index : lvWeightData.SelectedItems[0].Index + 1;
        AddLVI(newWS, newidx);
    }

    private void cmdAddAfter_Click(object sender, EventArgs e)
    {
        //Exit if nothing selected
        if (SelectedWeightEntryWorkset is null)
        {
            MessageBox.Show("No entry selected");
            return;
        }

        AddNewDay(1);
    }
}