﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop_Manger
{
   public class PowerItem : Grid
    {
        private bool IsActive { get; set; }
        private PowerPlan Plan { get; set; }
        public PowerItem()
        {
            string color = AppTheme.GetAnotherColor(AppTheme.Background);
            Background  = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppTheme.GetAnotherColor(color)));
            MouseLeftButtonDown += PowerItem_MouseLeftButtonDown;
            MouseEnter += PowerItem_MouseEnter;
            MouseLeave += PowerItem_MouseLeave;
        }

        private void PowerItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsActive) return;

            SetColor(this);
          
        }
        private void SetColor(PowerItem item)
        {
            string color = AppTheme.GetAnotherColor(AppTheme.Background);
            item.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppTheme.GetAnotherColor(color)));
        }
        private void PowerItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsActive) return;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppTheme.Effects));

        }

        private void PowerItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            BackgroundWorker CmdWorkerThread = new BackgroundWorker();
            CmdWorkerThread.WorkerReportsProgress = true;
            CmdWorkerThread.ProgressChanged += CmdWorkerThread_ProgressChanged;
            CmdWorkerThread.DoWork += CmdWorkerThread_DoWork;
            CmdWorkerThread.RunWorkerAsync();
        }

        private void CmdWorkerThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            foreach (PowerItem item in power.PowerItems)
            {
                if (item.IsActive == true)
                {
                    item.IsActive = false;
                    SetColor(item);
                }
            }
            IsActive = true;
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppTheme.Effects));
        }

        private void CmdWorkerThread_DoWork(object sender, DoWorkEventArgs e)
        {
            (sender as BackgroundWorker).ReportProgress(100);
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = startInfo };
            process.Start();
            process.StandardInput.WriteLine(@"powercfg /S " + Plan.Id);
            process.StandardInput.WriteLine("exit");
            process.WaitForExit(); 
        }

        public PowerItem(PowerPlan MyPlan, bool IsActive = false) : this()
        {
            this.Plan = MyPlan;
            Children.Add(CreateTextBlock());
            if (IsActive == true)
            {
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppTheme.ActiveItems));
            }
            
                this.IsActive = IsActive;
            
        }
        private TextBlock CreateTextBlock()
        {
            TextBlock Tb = new TextBlock();
            Tb.FontSize = 22;
            Tb.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(AppTheme.Foreground));
            Tb.HorizontalAlignment = HorizontalAlignment.Center;
            Tb.VerticalAlignment = VerticalAlignment.Center;
            Tb.Text = Plan.Name;
            return Tb;
        }
    }
}
