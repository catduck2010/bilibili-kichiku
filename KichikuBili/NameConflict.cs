using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KichikuBili {
    public partial class NameConflict : Form {
        String invalidfile = ResourceCulture.GetString("Invastr");
        String text1 = ResourceCulture.GetString("Vidstr");
        String text2 = ResourceCulture.GetString("Epistr");
        public string alias = "";
        public bool cancel = false;
        public NameConflict() {
            InitializeComponent();
            label1.Text = text1;
        }
        public NameConflict(String oriname) {
            InitializeComponent();
            textBox1.Text = oriname;
            label1.Text = text1;
        }
        public NameConflict(String oriname, int mode) {
            InitializeComponent();
            label1.Text = text2;
            textBox1.Text = oriname;
        }
        private void button1_Click(object sender, EventArgs e) {
            if (!AVManage.Tools.NameCheck(textBox1.Text)) {
                MessageBox.Show(invalidfile);
            } else {
                this.alias = textBox1.Text;
                this.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            cancel = true;
            this.Dispose();
        }
    }
}
