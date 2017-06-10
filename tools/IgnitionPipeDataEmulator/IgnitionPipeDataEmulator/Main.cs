//-----------------------------------------------------------------------
// <copyright file="Main.cs" company="Ace Poker Solutions">
// Copyright © 2017 Ace Poker Solutions. All Rights Reserved.
// Unless otherwise noted, all materials contained in this Site are copyrights, 
// trademarks, trade dress and/or other intellectual properties, owned, 
// controlled or licensed by Ace Poker Solutions and may not be used without 
// written consent except as provided in these terms and conditions or in the 
// copyright notice (documents and software) or other proprietary notices 
// provided with the relevant materials.
// </copyright>
//----------------------------------------------------------------------

using IgnitionPipeDataEmulator.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IgnitionPipeDataEmulator
{
    public partial class Main : Form
    {
        private bool isRunning;
        private bool stopRequested = false;

        public Main()
        {
            InitializeComponent();

            Logger.Configure(tbLog);

            Settings.Default.Upgrade();
            tbLogFile.Text = Settings.Default.LogFile;

            bwMain.DoWork += bwMain_DoWork;
            bwMain.ProgressChanged += bwMain_ProgressChanged;
            bwMain.RunWorkerCompleted += bwMain_RunWorkerCompleted;
        }

        private void bwMain_DoWork(object sender, DoWorkEventArgs e)
        {
            if (isRunning)
            {
                return;
            }

            isRunning = true;
            stopRequested = false;
            btEmulate.Enabled = false;
            btStop.Enabled = true;

            var logFile = tbLogFile.Text;

            Logger.Log("Analyzing file");
            Logger.Log("Initialize crypto provider...");

            try
            {
                using (var pipeService = new PipeService())
                {
                    using (var decryptor = new Decryptor())
                    {
                        var fileLineCounts = File.ReadAllLines(logFile).Count();

                        var lineCounter = 0;

                        using (var sr = new StreamReader(logFile, Encoding.UTF8))
                        {
                            string line;

                            bwMain.ReportProgress(0);

                            while ((line = sr.ReadLine()) != null)
                            {
                                try
                                {
                                    if (stopRequested)
                                    {
                                        Logger.Log("Stopping emulation");
                                        break;
                                    }

                                    var pipeData = decryptor.Decrypt(line);
                                    pipeService.Send(pipeData);

                                    Logger.Log($"Line {lineCounter + 1} proccessed.{(pipeData != null ? $" [{pipeData.TimeString}]" : string.Empty)}");

                                }
                                catch (Exception exc)
                                {
                                    Logger.Log($"Line {lineCounter} couldn't be proccessed, and it will be skipped:{Environment.NewLine}{exc}");
                                }

                                lineCounter++;

                                bwMain.ReportProgress((int)((double)(lineCounter) / fileLineCounts * 100d));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"Emulation failed:{Environment.NewLine}{ex}");
            }

            isRunning = false;
        }

        public void bwMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Logger.Log("Emulation completed");

            isRunning = false;
            btStop.Enabled = false;
            btEmulate.Enabled = true;
        }

        private void bwMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbMain.Value = e.ProgressPercentage;
        }

        private void btBrowse_Click(object sender, EventArgs e)
        {
            if (opfLogFile.ShowDialog() == DialogResult.OK)
            {
                tbLogFile.Text = opfLogFile.FileName;
            }
        }

        private void tbLogFile_TextChanged(object sender, EventArgs e)
        {
            if (File.Exists(tbLogFile.Text))
            {
                Settings.Default.LogFile = tbLogFile.Text;
                Settings.Default.Save();

                btEmulate.Enabled = true;

                return;
            }

            btEmulate.Enabled = false;
            btStop.Enabled = false;
        }

        private void btEmulate_Click(object sender, EventArgs e)
        {
            btStop.Enabled = true;
            btEmulate.Enabled = false;

            bwMain.RunWorkerAsync();
        }

        private void btStop_Click(object sender, EventArgs e)
        {
            stopRequested = true;
        }
    }
}