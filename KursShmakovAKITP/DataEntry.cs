using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KursShmakovAKITP
{
    public partial class DataEntry : Form
    {

 

        public DataEntry()
        {
            InitializeComponent();
        }

        private void DataEntry_Load(object sender, EventArgs e)
        {
  
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int value;
           
            if (radioButton1.Checked)
            {

                OpenFileDialog OPF = new OpenFileDialog();
                OPF.Filter = "Файлы txt|*.txt";
                OPF.Title = "Выбор карты";
                OPF.InitialDirectory = System.IO.Path.Combine(Application.StartupPath, @"data");
                if (OPF.ShowDialog() == DialogResult.OK)
                {
                    string path = OPF.FileName;
                    int x = 0;
                    int y = 0;

                    if (File.Exists(path))
                    {
                        StreamReader sr = File.OpenText(path);
                 
                        while (!sr.EndOfStream)
                        {
                            try
                            {
                                // считываем строку с файла
                                string line = sr.ReadLine();
                                // разделяем на массив из считанной строки до символа
                                string[] fields = line.Split('|');
                                x = Convert.ToInt32(fields[0]);
                                y = Convert.ToInt32(fields[1]);
                                Form1.map[x, y] = Convert.ToInt32(fields[2]);
                                Form1.readfile = true;

                                if (x == y) {
                                    Form1.sizeField = x + 1;
                                }

                            }
                            catch (Exception m)
                            {
                                MessageBox.Show(m.ToString());
                            }
                        }

                        Form1 example = new Form1();
                        example.Show();

                        sr.Close();
                    }
                }
            }
            else
            {
                try
                {
                    value = Convert.ToInt32(textBox1.Text);
                    Form1.sizeField = value;
                    Form1 example = new Form1();
                    example.Show();

                }
                catch (Exception)
                {
                    MessageBox.Show("Пожалуйста, введите корректное значение!");
                }
               
            }


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.ReadOnly = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            textBox1.ReadOnly = false;
        }
    }
}
