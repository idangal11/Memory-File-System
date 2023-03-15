using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TinyMemFSApp
{
    public partial class Form1 : Form
    {
        TinyMemFS fs = new TinyMemFS();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //add(String fileName, String fileToAdd)
            string fileName = textBox4.Text;
            string fileToAdd = textBox1.Text;
            
            bool check = fs.add(fileName, fileToAdd);
            if (!check) { MessageBox.Show("The file allready saved/ wrong path form/ file not exist in the computer \nplease, add another file"); }              
            else { MessageBox.Show("The file has been saved"); }
            //byte[] bytes = fs.headers["a"].ToArray();          
            

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool check = fs.remove(textBox2.Text);
            if (!check) { MessageBox.Show("The file not exist in the system!"); }
            else {MessageBox.Show("The file has been removed"); }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //fs.save(textBox3.Text);
            string fileName = textBox3.Text;
            string fileToAdd = textBox5.Text;
            bool check = fs.save(fileName, fileToAdd);
            if (!check) { MessageBox.Show("The file not exist in the system!"); }
            else { MessageBox.Show("The file successfully saved in the system!"); }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

            MessageBox.Show("The file has been saved");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            fs.encrypt(textBox6.Text);
            MessageBox.Show("The files has been encrypted");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            fs.decrypt(textBox7.Text);
            MessageBox.Show("The files has been decrypted");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string message= "Files properties:\n";
            List<String> files = fs.listFiles();
            foreach (String file in files)
            {
                message += file;
            }
            MessageBox.Show(message);

        }
    }
}
