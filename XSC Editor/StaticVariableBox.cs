using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XSCEditor
{
    public partial class StaticVariableBox : Form
    {
        public StaticVariableBox()
        {
            InitializeComponent();
        }

        public void SetWindowHeaderText(string val)
        {
            this.Text = "Static Variable #" + val;
        }

        public void SetEditTextBoxValue(string val)
        {
            valueTextBox.Text = val;
        }

        public int GetEditTextBoxValue()
        {
            int staticValue = 0;
            try
            {
                staticValue = Convert.ToInt32(valueTextBox.Text);
            }
            catch(Exception)
            {
                staticValue = 0;
            }
            return staticValue;
        }

        private void StaticVariableBox_Load(object sender, EventArgs e)
        {

        }

        private void valueTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void valueTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
        private void valueTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control == true)
            {
               
            }
        }

        private void enterButton_Click(object sender, EventArgs e)
        {

        }

    }
}
