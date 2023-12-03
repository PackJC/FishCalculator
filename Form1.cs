using static System.Windows.Forms.LinkLabel;
using System.Windows.Forms;
using System.Threading.Channels;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.ApplicationServices;
using FishCalculator.Properties;
using System.Security.Cryptography;

namespace FishCalculator
{

    public partial class GebsFishCalculator : Form
    {
        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursorFromFile(string fileName);

        private Cursor myCustomCursor;


        public GebsFishCalculator()
        {
            InitializeComponent();
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);
            myCustomCursor = new Cursor((Resources.fishcursor1.ToBitmap().GetHicon()));

            // Set the form's cursor to the custom cursor
            this.Cursor = myCustomCursor;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Gebsfish Config (.cfg)|*.cfg";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                List<double> chanceTotal = new List<double>();
                foreach (string line in File.ReadAllLines(openFileDialog.FileName))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int chance))
                    {
                        chanceTotal.Add(chance);
                    }

                }
                double total = 0;
                foreach (var x in chanceTotal)
                {
                    total += x;
                }

                foreach (string line in File.ReadAllLines(openFileDialog.FileName))
                {
                    var parts = line.Split('=');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int chance))
                    {
                        dataGridView1.Rows.Add(parts[0], chance, Math.Round((double)chance / total * 1000) + "%");
                    }
                }

            }

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 1) // Check for the correct column index
            {
                UpdatePercentColumn();
            }
        }

        private void UpdatePercentColumn()
        {
            int totalChance = dataGridView1.Rows.Cast<DataGridViewRow>()
                                    .Where(r => !r.IsNewRow)
                                    .Sum(r => Convert.ToInt32(r.Cells[1].Value ?? 0));

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    int chance = Convert.ToInt32(row.Cells[1].Value ?? 0);
                    double percent = totalChance != 0 ? (double)chance / totalChance * 1000 : 0;
                    row.Cells[2].Value = Math.Round(percent) + "%"; // Update the Percent column
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Saving Gebsfish Config";
            saveFileDialog.Filter = "Gebsfish Config (*.cfg)|*.cfg";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StringBuilder fileContent = new StringBuilder();

                // Iterate through the rows
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow) // Skip the new row at the end if present
                    {
                        // Iterate through each cell in the row
                        foreach (DataGridViewCell cell in row.Cells)
                        {

                            if (cell.OwningColumn.Name != "Percent")
                            {
                                if (cell.OwningColumn.Name == "Fish")
                                {
                                    fileContent.Append(cell.Value?.ToString().Trim() + "=");
                                }
                                else
                                {
                                    fileContent.Append(cell.Value?.ToString().Trim());
                                }

                            }
                            // Append the cell's text followed by a tab character
                        }

                        // Append a newline at the end of each row
                        fileContent.AppendLine();
                    }
                }

                // Write the text to a file

                File.WriteAllText(saveFileDialog.FileName, fileContent.ToString());
            }


        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}