using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InkDesktop
{
    public partial class SettingChangeDialog : Form
    {
        public SettingChangeDialog()
        {
            InitializeComponent();
        }

        public static string ShowDialog(IWin32Window owner, string settingName, object oldValue)
        {
            SettingChangeDialog win = new SettingChangeDialog();
            win.Text = settingName;
            win.txtCurrent.Text = oldValue.ToString();
            win.txtNew.Text = "";
            if(win.ShowDialog(owner) == DialogResult.Yes)
            {
                return win.txtNew.Text;
            }
            else
            {
                return null;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            
            DialogResult = DialogResult.Yes;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
        }

        private void txtNew_KeyDown(object sender, KeyEventArgs e)
        {
            
        }
    }
}
