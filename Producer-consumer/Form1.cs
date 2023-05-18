using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sunny.UI;

namespace Producer_consumer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            IList<int> list1 = new List<int>(), list2 = new List<int>();
            //在两个ComboBox中添加可选项
            for (int i = 3; i <= 10; i++)
            {
                list1.Add(i);
                list2.Add(i);
            }
            uiComboBox1.DataSource = list1;
            uiComboBox2.DataSource = list2;
            uiComboBox1.SelectedItem = 3;
            uiComboBox2.SelectedItem = 3;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            //确定按钮被点击以后，隐藏当前窗口，打开展示窗口
            Form2 fn = new Form2((int)uiComboBox1.SelectedItem, (int)uiComboBox2.SelectedItem);   //将用户选择的生产者和消费者数量传入Form2
            this.Hide();
            fn.StartPosition = FormStartPosition.CenterParent;
            fn.ShowDialog();
            this.Close();
        }

        private void uiComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
