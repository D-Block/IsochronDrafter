﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace IsochronDrafter
{
    public partial class ServerWindow : Form
    {
        private DraftServer server;

        public ServerWindow()
        {
            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeComponent();
            MaximizeBox = false;
            textBox2.Text = isochron.Default.SetFile;
            textBox3.Text = isochron.Default.ImageDirectory;
            textBox8.Text = isochron.Default.Packs;
            textBox4.Text = isochron.Default.CommonsPerPack;
            textBox5.Text = isochron.Default.UncommonPerPack;
            textBox6.Text = isochron.Default.RaresPerPack;
            textBox7.Text = isochron.Default.MythicPercentage;
        }

        public void PrintLine(string text)
        {
            textBox1.Invoke(new MethodInvoker(delegate
            {
                if (textBox1.Text.Length != 0)
                    textBox1.Text += "\r\n";
                textBox1.Text += text;
                textBox1.SelectionStart = textBox1.Text.Length;
                textBox1.ScrollToCaret();
            }));
        }
        public void DraftButtonEnabled(bool enabled)
        {
            buttonDraft.Invoke(new MethodInvoker(delegate
            {
                buttonDraft.Enabled = enabled;
            }));
        }

        //Browse click event
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        //Launch click event
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length == 0)
            {
                MessageBox.Show("You must choose a set file.");
                return;
            }
            if (textBox3.Text.Length == 0)
            {
                MessageBox.Show("You must enter a remote image directory.");
                return;
            }
            int packs, commons, uncommons, rares;
            float mythicPercentage;
            if (!int.TryParse(textBox8.Text, out packs) || packs < 0)
            {
                MessageBox.Show("You must enter a positive integer number of packs.");
                return;
            }
            if (!int.TryParse(textBox4.Text, out commons) || commons < 0)
            {
                MessageBox.Show("You must enter a positive integer number of commons.");
                return;
            }
            if (!int.TryParse(textBox5.Text, out uncommons) || uncommons < 0)
            {
                MessageBox.Show("You must enter a positive integer number of uncommons.");
                return;
            }
            if (!int.TryParse(textBox6.Text, out rares) || rares < 0)
            {
                MessageBox.Show("You must enter a positive integer number of rares.");
                return;
            }
            if (!float.TryParse(textBox7.Text, out mythicPercentage) || mythicPercentage < 0 || mythicPercentage > 1)
            {
                MessageBox.Show("You must enter a mythic percentage between 0 and 1.");
                return;
            }
            Util.imageDirectory = textBox3.Text;
            if (!Util.imageDirectory.EndsWith("/"))
                Util.imageDirectory += "/";
            server = new DraftServer(this, textBox2.Text, packs, commons, uncommons, rares, mythicPercentage);
            if (server.IsValidSet())
            {
                isochron.Default.SetFile = textBox2.Text;
                isochron.Default.ImageDirectory = textBox3.Text;
                isochron.Default.Packs = textBox8.Text;
                isochron.Default.CommonsPerPack = textBox4.Text;
                isochron.Default.UncommonPerPack = textBox5.Text;
                isochron.Default.RaresPerPack = textBox6.Text;
                isochron.Default.MythicPercentage = textBox7.Text;
                isochron.Default.Save();
                buttonLaunch.Enabled = false;
                buttonBrowse.Enabled = false;
                textBox2.Enabled = false;
                textBox3.Enabled = false;
                textBox4.Enabled = false;
                textBox5.Enabled = false;
                textBox6.Enabled = false;
                textBox7.Enabled = false;
                textBox8.Enabled = false;
                server.PrintServerStartMessage();
            }
            else
                server.server.Close();
        }

        //Start draft click event
        private void button2_Click(object sender, EventArgs e)
        {
            PrintLine("Starting draft with " + server.aliases.Count + " players.");
            server.StartNextPack();
            buttonDraft.Enabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Environment.Exit(0);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void ServerWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
