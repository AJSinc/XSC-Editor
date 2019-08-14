using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace XSCEditor
{
    public partial class MainWindow : Form
    {
        private String XSCLocation;
        private XSC xscToOpen;
        private XSC xscToSave;
        private List<int> staticVarList;
        public MainWindow()
        {
            InitializeComponent();
            XSCLocation = @"c:\";
            staticVarList = new List<int>();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Title = "Open";
            fd.InitialDirectory = XSCLocation;
            fd.Filter = "Xenon Script Container|*.xsc";
            fd.FilterIndex = 1;
            fd.RestoreDirectory = true;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                //FileLocation.Text = fd.FileName;
                XSCLocation = fd.FileName;
                //string convertedAssembly;
                xscToOpen = new XSC(XSCLocation);
                paramCountTextBox.Text = xscToOpen.GetParameterCount().ToString();
                //string table
                stringTable.Enabled = true;
                stringTable.DataSource = xscToOpen.GetStringTable();
                //natives table
                nativesTable.Enabled = true;
                nativesTable.DataSource = xscToOpen.GetNativesTable();
                //assembly
                assemblyTextEditor.Text = xscToOpen.GetASM();
                //XSC name
                XSCNameTextBox.Text = xscToOpen.GetXSCName();
                staticVariables.Items.Clear();
                List<int> statics = xscToOpen.GetStaticVariablesTable();
                for (int i = 0; i < statics.Count; i++)
                {
                    staticVariables.Items.Add(statics[i]);
                }
                    //staticVariables.DataSource = xscToOpen.GetStaticVariablesTable();
                varCounter.Text = "(" + staticVariables.Items.Count + ")";
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveXSCFileDialog = new SaveFileDialog();
            saveXSCFileDialog.Title = "Save";
            saveXSCFileDialog.InitialDirectory = XSCLocation;
            saveXSCFileDialog.Filter = "Xenon Script Container|*.xsc";
            saveXSCFileDialog.FilterIndex = 1;
            saveXSCFileDialog.RestoreDirectory = true;
            if (saveXSCFileDialog.ShowDialog() == DialogResult.OK)
            {
                string newXSCFileLocation = saveXSCFileDialog.FileName;
                xscToSave = new XSC();
                List<byte> newXSCFileBytes = new List<byte>();
                //XSC Name
                try
                {
                    xscToSave.SetNewXSCName(XSCNameTextBox.Text);
                }
                catch (InvalidDataException)
                {
                    MessageBox.Show("XSC name can only contain letters, numbers and underscores, and cannot end in an underscore");
                    return;
                }
                xscToSave.SetNewParamCount(paramCountTextBox.Text);
                //XSC ASM
                string convertStatus = xscToSave.ConvertASMtoBytes(assemblyTextEditor.Text);
                if (convertStatus != "ASM converted successfully")
                {
                    MessageBox.Show(convertStatus);
                    return;
                }
                //Static Variables
                xscToSave.ConvertNewStaticVariablesTable(staticVariables.Items.Cast<int>().ToList());
                //Natives
                convertStatus = xscToSave.ConvertNewNativeTableBytes();
                if (convertStatus != "Natives Table converted successfully")
                {
                    MessageBox.Show(convertStatus);
                    return;
                }
                //String
                xscToSave.ConvertNewStringTable();
                //Combine all the tables
                xscToSave.MergeXSCTables();
                //outputTextBox.Text = BitConverter.ToString(xscToSave.GetNewXSCBytes()).Replace("-", " ");
                try
                {
                    File.WriteAllBytes(newXSCFileLocation, xscToSave.GetNewXSCBytes());
                    XSCLocation = newXSCFileLocation;
                }
                catch(IOException)
                {
                    MessageBox.Show("I/O Exception while saving XSC to specified location");
                    return;
                }
                catch(UnauthorizedAccessException)
                {
                    MessageBox.Show("Error saving XSC to specified location");
                    return;
                }

                //re-open certain parts
                xscToOpen = new XSC(newXSCFileLocation);
                //string table
                stringTable.Enabled = true;
                stringTable.DataSource = xscToOpen.GetStringTable();
                //natives table
                nativesTable.Enabled = true;
                nativesTable.DataSource = xscToOpen.GetNativesTable();

            }
        }

        private void closeFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //string convertedAssembly;
            xscToOpen = null;
            //string table
            stringTable.Enabled = false;
            stringTable.DataSource = null;
            //natives table
            nativesTable.Enabled = false;
            nativesTable.DataSource = null;
            //assembly
            assemblyTextEditor.Text = "";
            //XSC name
            XSCNameTextBox.Text = "";
            varCounter.Text = "(0)";
            staticVariables.Items.Clear();
            paramCountTextBox.Text = "0";
        }

        private void assemblyTextEditor_TextChanged(object sender, EventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void addVarButton_Click(object sender, EventArgs e)
        {
            staticVariables.Items.Add(0);
            varCounter.Text = "(" + staticVariables.Items.Count + ")";
        }

        private void deleteVarButton_Click(object sender, EventArgs e)
        {
            try
            {
                int index = staticVariables.SelectedIndex;
                staticVariables.Items.Remove(staticVariables.SelectedItems[0]);
                varCounter.Text = "(" + staticVariables.Items.Count + ")";
                if(--index < 0)
                {
                    index = 0;
                }
                staticVariables.SelectedIndex = index;
            }
            catch(Exception)
            {

            }
        }

        private void editVarButton_Click(object sender, EventArgs e)
        {
            try
            {
                int staticIndex = staticVariables.SelectedIndex;
                string staticValue = staticVariables.SelectedItem.ToString();
                StaticVariableBox varEditWindow = new StaticVariableBox();
                varEditWindow.SetWindowHeaderText(staticIndex.ToString());
                varEditWindow.SetEditTextBoxValue(staticValue);
                DialogResult result = varEditWindow.ShowDialog();
                if(result == DialogResult.OK)
                {
                    staticVariables.Items[staticIndex] = varEditWindow.GetEditTextBoxValue();
                }
                varEditWindow.Dispose();
            }
            catch (Exception)
            {

            }  
        }


    }
}
