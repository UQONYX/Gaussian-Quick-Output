﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Gaussian_Quick_Output
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static string dataset = "";

        private void button1_Click(object sender, EventArgs e)
        {
            ChooseFolder();
        }

        //Function to preview data of log files
        //Some data validation would be nice, but not completely necessary
        public void dataLookup()
        {
            //Everything below is pretty much "hard-coded" such as the numbers for the substring references. I did this by trial and error\
            //Not sure if all log files follow this exact format, but this can be expanded upon in the future if necessary
            //TODO: add an error message if value is null
            //Right now, it'll just return a substring from -1 onwards which can be fixed
            //TODO: add more functions such as entropies and items

            if (comboBox1.Text == "File Name")
            {
                textBox1.Text = listBox1.Text.Substring(listBox1.Text.LastIndexOf(@"\") + 1);
            }
            else if (comboBox1.Text == "Enthalpies")
            {
                string file = System.IO.File.ReadAllText(listBox1.Text);
                textBox1.Text = (file.Substring(file.IndexOf("Enthalpies") + 23, 12));
            }
            else if (comboBox1.Text == "Entropies")
            {
                string file = System.IO.File.ReadAllText(listBox1.Text);
                //  textBox1.Text = (file.Substring(file.IndexOf("Entropies") + 22, 12));
                textBox1.Text = "in progress";
            }
            else if (comboBox1.Text == "Image")
            {
                string file = System.IO.File.ReadAllText(listBox1.Text);
                textBox1.Text = (file.Substring(file.IndexOf("Imag=")+5, 1));
            }

        }
        /*Iterates through all files and builds the csv string file
            Returns a string with following format
                 File Name, Enthalpies, Imag=
                 filename1.log, 0000.00000, 0
                 filename2.log, 0000.00000, 0
                 filename3.log, 0000.00000, 0
         */
         //TODO: need to add and remove columns based on selected data parameters from combobox1
         //TODO URGENT (potential app-breaking bug):
         //Not a fan of how csv string is a temporary, limited scope string. Needs to be saved as an application property
         //that can be properly getted and setted
        public string processData ()
        {
            if (!String.IsNullOrEmpty( folderBrowserDialog1.SelectedPath))
            {
                string csv = "File Name, Enthalpies, Imag= \n";
                int i = 0;
                foreach (string file in Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                {
                    if (file.EndsWith(".log"))
                    {
                        i++;
                        string filetext = System.IO.File.ReadAllText(file);
                        string filename = file.Substring(file.LastIndexOf(@"\") + 1);
                        string enthalpies = filetext.Substring(filetext.IndexOf("Enthalpies") + 23, 11);
                        string imag = filetext.Substring(filetext.IndexOf("Imag=") + 5, 1);
                        csv += string.Format("{0} , {1} , {2} \n ", filename , enthalpies, imag);
                    }
                }
                MessageBox.Show(string.Format("Quick Output found {0} log files to analyze. Saving now will create an Excel data sheet with {0} entries.", i.ToString()), "Data Analysis Status");
                dataset = csv;
                return csv;
            }
            else
            {
                return "";
            }
        }
        public void ChooseFolder()
        {
            //A little bit of QoL code to set the default file to be the most recent file selected. 
            //Reduces the headache of navigating through hella directories just to find your folder
            //Just a little code snippet from Stackoverflow
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(Properties.Settings.Default.lastDirectory)){ 
                    Properties.Settings.Default.lastDirectory = folderBrowserDialog1.SelectedPath;
                }
                    Properties.Settings.Default.lastDirectory = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.Save();

                //Encapsulated in a try/catch block so program doesnt die because of any exception
                //Simply populates the list with all log files
                listBox1.Items.Clear();
                try
                {
                    folderBrowserDialog1.SelectedPath = Properties.Settings.Default.lastDirectory;
                   foreach (string file in Directory.GetFiles(folderBrowserDialog1.SelectedPath))
                    {
                        //Not sure if there's a smarter way to do this but this seems pretty fool-proof
                        if (file.EndsWith(".log"))
                        {
                            listBox1.Items.Add(file);
                        }
                    }
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.ToString());
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataLookup();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
        }

        //Method saves everything by processing the data and opening up file system dialog. 
        //Can be expanded upon in the future in case further customization is needed, but right now this is perfectly fine
        private void button2_Click(object sender, EventArgs e)
        {
           string x = processData();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, x);
                
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(Properties.Settings.Default.lastDirectory))
            {
                folderBrowserDialog1.SelectedPath = Properties.Settings.Default.lastDirectory;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataLookup();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string s = processData();
            Form2 form = new Form2();
            form.Show();
        }
    }
}
