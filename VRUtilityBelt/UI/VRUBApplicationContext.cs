using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUB.Properties;
using VRUB.Utility;

namespace VRUB.UI
{
    class VRUBApplicationContext : ApplicationContext
    {
        private readonly ToolStripMenuItem _workshopSubmitterMenuItem;
        private readonly ToolStripMenuItem _quitMenuItem;
        private readonly ToolStripMenuItem _openLogFileItem;
        private readonly ToolStripMenuItem _settingsItem;
        private readonly NotifyIcon _trayIcon;

        private Workshop.WorkshopUploader _workshopUploader;

        ConfigForm _configForm = null;

        public VRUBApplicationContext() {
            _workshopSubmitterMenuItem = new ToolStripMenuItem("Submit Addon to Workshop", null, OnSubmitWorkshopClick);
            _quitMenuItem = new ToolStripMenuItem("Quit VR Utility Belt", null, OnQuitClick);
            _openLogFileItem = new ToolStripMenuItem("View Log File", null, OnLogFileClick);
            _settingsItem = new ToolStripMenuItem("Settings", null, OnSettingsClick);

            ContextMenuStrip contextMenuStrip = new ContextMenuStrip
            {
                Items =
                {
                    _settingsItem,
                    new ToolStripSeparator(),
                    _workshopSubmitterMenuItem,
                    _openLogFileItem,
                    new ToolStripSeparator(),
                    _quitMenuItem,
                },
                AutoClose = true
            };

            contextMenuStrip.Opening += ContextMenuStrip_Opening;

            _trayIcon = new NotifyIcon
            {
                Text = "VR Utility Belt (" + Application.ProductVersion.ToString() + ")",
                Icon = Resources.favicon,
                ContextMenuStrip = contextMenuStrip,
                Visible = true
            };

            Application.ApplicationExit += Application_ApplicationExit;
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            _trayIcon.Visible = false;
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ((ContextMenuStrip)sender).BringToFront();
        }

        void OnSubmitWorkshopClick(object sender, EventArgs args)
        {
            if (_workshopUploader == null)
                _workshopUploader = new Workshop.WorkshopUploader();

            _workshopUploader.Show();
        }

        void OnQuitClick(object sender, EventArgs args)
        {
            Program.Quit();
        }

        void OnLogFileClick(object sender, EventArgs args)
        {
            Process.Start("notepad.exe", Logger.LogFilePath);
        }

        void OnSettingsClick(object sender, EventArgs args)
        {
            if(_configForm == null)
            {
                _configForm = new ConfigForm();
            }

            _configForm.Show();
        }
    }
}
