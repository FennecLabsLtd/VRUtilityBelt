using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUB.Addons;
using VRUB.Utility;
using static VRUB.Utility.ConfigUtility;

namespace VRUB.UI
{
    public partial class ConfigForm : Form
    {
        Dictionary<string, List<ConfigLayout>> _layouts;

        public ConfigForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            configList.Columns[0].Width = configList.ClientRectangle.Width;

            RefreshConfigs();

            configGrid.Item.Clear();
            configGrid.Refresh();
        }

        void RefreshConfigs() {
            configList.Groups.Clear();
            configList.Items.Clear();

/*            configList.Groups.AddRange(new ListViewGroup[] {
                new ListViewGroup("VRUB Settings"),
                new ListViewGroup("Addon Settings"),
                new ListViewGroup("Disabled Addons"),
            });*/

            //configList.Items.Add(new ListViewItem() { /*Group = configList.Groups[0],*/ Text = "Basic Settings", });

            _layouts = ConfigUtility.GetLayouts();

            foreach (KeyValuePair<string, List<ConfigLayout>> layout in _layouts)
            {
                configList.Items.Add(new ListViewItem()
                {
                    Text = layout.Value.Count > 0 && layout.Value[0].Addon != null ? "Addon: " + layout.Value[0].Addon.Name : layout.Key, // TODO: Titles for custom layouts.
                    Tag = layout.Key,
                    /*Group = layout.Key.Enabled ? configList.Groups[1] : configList.Groups[2],*/
                    ForeColor = layout.Value.Count > 0 && layout.Value[0].Addon != null && !layout.Value[0].Addon.Enabled ? SystemColors.ScrollBar : SystemColors.WindowText,
                });
            }

            configGrid.ExpandAllGridItems();
        }

        private void configList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedModule();
        }

        void ShowSelectedModule()
        {
            if (configList.SelectedItems.Count == 0)
                return;

            ListViewItem item = configList.SelectedItems[0];

            if (item.Tag is string)
            {
                UpdatePropertyGrid(item.Tag.ToString(), _layouts[item.Tag.ToString()]);
            }
        }

        void UpdatePropertyGrid(string layoutKey, List<ConfigLayout> layout)
        {
            configGrid.Item.Clear();

            foreach(ConfigLayout configOption in layout)
            {
                configGrid.Item.Add(configOption.Title, configOption.GetValue(), false, configOption.Category, configOption.Description, true);

                if(configOption.Options != null)
                    configGrid.Item[configGrid.Item.Count - 1].Choices = new PropertyGridEx.CustomChoices(configOption.Options);

                if(configOption.Type == "bool")
                    configGrid.Item[configGrid.Item.Count - 1].CustomTypeConverter = new ConfigTypeConverters.YesNoConverter();

                configGrid.Item[configGrid.Item.Count - 1].Tag = configOption;
            }

            configGrid.Refresh();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            foreach(PropertyGridEx.CustomProperty prop in configGrid.Item)
            {
                ConfigLayout layout = (ConfigLayout)prop.Tag;
                layout.SetValue(prop.Value);
            }

            ShowSelectedModule();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ShowSelectedModule();
        }

        private void configList_Resize(object sender, EventArgs e)
        {
            configList.Columns[0].Width = configList.ClientRectangle.Width;
        }
    }
}
