using Newtonsoft.Json;
using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VRUB.Steam;
using VRUB.Utility;

namespace VRUB.UI.Workshop
{
    public partial class WorkshopUploader : Form
    {
        List<WorkshoppableAddon> _addons;

        WorkshoppableAddon _selectedAddon;

        CallResult<CreateItemResult_t> OnCreatedItemResult;
        CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResult;

        public WorkshopUploader()
        {
            InitializeComponent();

            if(OnCreatedItemResult == null && SteamManager.Initialised)
                OnCreatedItemResult = CallResult<CreateItemResult_t>.Create(CreatedItemCallback);

            if (OnSubmitItemUpdateResult == null)
                OnSubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create(SubmittedItemCallback);

            FetchAddons();
        }

        private void cmbAddons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_addons == null)
                return;

            if(_addons.Count > cmbAddons.SelectedIndex)
            {
                SelectAddon(_addons[cmbAddons.SelectedIndex]);
            }
        }

        void SelectAddon(WorkshoppableAddon addon)
        {
            _selectedAddon = addon;

            lblAddonTitle.Text = addon.Title;
            lblAddonType.Text = String.Join(", ", addon.Type);
            lblAddonTags.Text = String.Join(", ", addon.Tags);
            lblAddonIgnore.Text = String.Join(", ", addon.Ignore);

            SetButtonText();

            if(addon.Title != null && addon.Type != null && addon.Type.Length > 0 && addon.Tags != null && addon.Tags.Length > 0)
            {
                btnSubmit.Enabled = true;
            } else
            {
                btnSubmit.Enabled = false;
            }
        }

        void SetButtonText()
        {
            if (_selectedAddon == null)
                return;

            if (_selectedAddon.FileId > 0)
            {
                btnSubmit.Text = "Update Workshop Item";
            }
            else
            {
                btnSubmit.Text = "Create Workshop Item";
            }
        }

        private void FetchAddons()
        {
            string Path = Environment.CurrentDirectory + "\\addons\\custom";

            cmbAddons.Items.Clear();

            if (Directory.Exists(Path))
            {
                _addons = new List<WorkshoppableAddon>();
                foreach (string folder in Directory.EnumerateDirectories(Path))
                {
                    if (File.Exists(folder + "\\workshop.json"))
                    {
                        WorkshoppableAddon addon = JsonConvert.DeserializeObject<WorkshoppableAddon>(File.ReadAllText(folder + "\\workshop.json"));
                        addon.Path = folder;

                        if (addon.Title != null)
                        {
                            _addons.Add(addon);
                            cmbAddons.Items.Add(addon.Title + (addon.FileId != 0 ? " (File ID: " + addon.FileId + ")" : ""));
                        }
                    }
                }

                if (_addons.Count == 0)
                {
                    cmbAddons.Text = "No Valid workshop.json files found";
                    cmbAddons.Enabled = false;
                }

            } else
            {
                cmbAddons.Text = "No Valid workshop.json files found";
                cmbAddons.Enabled = false;
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(btnSubmit.Enabled && _selectedAddon != null)
            {
                if (_selectedAddon.FileId > 0)
                    UpdateItem();
                else
                    SubmitWorkshopAddon();
            }
        }

        void SetAsSubmitting()
        {
            btnSubmit.Enabled = false;
            btnSubmit.Text = "Submitting...";
            cmbAddons.Enabled = false;
        }

        void SubmitWorkshopAddon()
        {
            SetAsSubmitting();

            Logger.Info("[WORKSHOP] Submitting Item");

            OnCreatedItemResult.Set(SteamUGC.CreateItem(SteamManager.AppID, EWorkshopFileType.k_EWorkshopFileTypeCommunity));
        }

        void CreatedItemCallback(CreateItemResult_t args, bool failure)
        {
            Logger.Trace("[WORKSHOP] Got callback for creating item");
            switch(args.m_eResult)
            {
                case EResult.k_EResultOK:
                    break;

                case EResult.k_EResultInsufficientPrivilege:
                    MessageBox.Show("You are not permitted to create workshop items.", "Item Creation Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultTimeout:
                    MessageBox.Show("This took longer than expected, Steam Servers may be acting up - please try again.", "Item Creation Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultNotLoggedOn:
                    MessageBox.Show("Please login to Steam before trying to submit an item.", "Item Creation Failed");
                    ResetUIState();
                    return;
            }

            _selectedAddon.FileId = args.m_nPublishedFileId.m_PublishedFileId;

            File.WriteAllText(_selectedAddon.Path + "\\workshop.json", JsonConvert.SerializeObject(_selectedAddon, Formatting.Indented));

            Logger.Info("[WORKSHOP] Created Item with File ID " + _selectedAddon.FileId);

            UpdateItem();
        }

        void UpdateItem()
        {
            SetAsSubmitting();
            Logger.Info("[WORKSHOP] Updating Data for " + _selectedAddon.FileId);
            UGCUpdateHandle_t handle = SteamUGC.StartItemUpdate(SteamManager.AppID, new PublishedFileId_t(_selectedAddon.FileId));
            SteamUGC.SetItemTitle(handle, _selectedAddon.Title);

            List<string> allTags = new List<string>();
            allTags.AddRange(_selectedAddon.Tags);
            allTags.AddRange(_selectedAddon.Type);

            SteamUGC.SetItemTags(handle, allTags);
            SteamUGC.SetItemContent(handle, _selectedAddon.Path.Replace("\\\\", "\\"));

            if (!Path.IsPathRooted(_selectedAddon.PreviewImage))
                SteamUGC.SetItemPreview(handle, _selectedAddon.Path + "\\" + _selectedAddon.PreviewImage.Replace("\\\\", "\\"));
            else
                SteamUGC.SetItemPreview(handle, _selectedAddon.PreviewImage.Replace("\\\\", "\\"));

            OnSubmitItemUpdateResult.Set(SteamUGC.SubmitItemUpdate(handle, txtUpdateNotes.Text));
        }

        void SubmittedItemCallback(SubmitItemUpdateResult_t args, bool failure)
        {
            Logger.Trace("[WORKSHOP] Got callback for updating item");
            switch(args.m_eResult)
            {
                case EResult.k_EResultOK:
                    break;

                case EResult.k_EResultFail:
                    MessageBox.Show("Something went wrong! We're not sure what, but something did. And it's probably Steam's fault :(", "Item Update Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultInvalidParam:
                    MessageBox.Show("This shouldn't happen... but something didn't match up with the App IDs", "Item Update Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultAccessDenied:
                    MessageBox.Show("You don't appear to have a license for VRUB... That's weird. Did you compile this from source per chance?", "Item Update Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultFileNotFound:
                    MessageBox.Show("We couldn't access the folder and/or preview image. OR the File ID is not valid.", "Item Update Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultLockingFailed:
                    MessageBox.Show("Failed to acquire UGC Lock", "Item Update Failed");
                    ResetUIState();
                    return;

                case EResult.k_EResultLimitExceeded:
                    MessageBox.Show("The preview image is too large, it must be less than 1 Megabyte. Or... your Steam Cloud doesn't have enough space left?", "Item Update Failed");
                    ResetUIState();
                    return;
            }

            if(args.m_bUserNeedsToAcceptWorkshopLegalAgreement)
            {
                System.Diagnostics.Process.Start("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
            }

            System.Diagnostics.Process.Start("https://steamcommunity.com/sharedfiles/filedetails/" + _selectedAddon.FileId);

            ResetUIState();

            MessageBox.Show("Done! The item has been updated.", "Item Updated!");
        }

        void ResetUIState()
        {
            Invoke((MethodInvoker)delegate {
                SetButtonText();
                btnSubmit.Enabled = true;
                cmbAddons.Enabled = true;
            });
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://steamcommunity.com/sharedfiles/workshoplegalagreement");
        }
    }
}
