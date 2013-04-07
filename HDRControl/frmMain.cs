using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace HDRControl
{
    public partial class frmMain : Form
    {
        camControl CameraControl = new camControl();
        List<Control> UnlockedCTLS = new List<Control>();
        bool adjustingUI = false;
        frmPreview picPreview = new frmPreview();

        public frmMain()
        {
            InitializeComponent();
        }

        private void SetupUI()
        {
            //IF WE ARE CURRENTLY ADJUSTING THE UI DONT REFRESH ANYTHING AGAIN AND GET STUCK IN A LOOP
            if (adjustingUI)
                return;

            //FLAG THAT WE ARE ADJUSTING THE UI SO WE DONT TRY AGAIN
            adjustingUI = true;
            lockUI();

            //CLEAR ALL OF OUR LIST BOXES SO WE CAN REBUILD THEM
            lstExposureCompensation.Items.Clear();
            lstISOSpeeds.Items.Clear();
            lstBrackets.Items.Clear();

            //LOOP THROUGH EACH OF THE PROPERTIES AND REBUILD THE LIST BOXES
            foreach (camControl.ExposureCompensations tmpExp in CameraControl.ValidEsposureCompensations)
            {
                lstExposureCompensation.Items.Add(new ListItem<string, int>(tmpExp.GetStringValue(), (int)tmpExp));
            }
            foreach (camControl.Apertures tmpAV in CameraControl.ValidApertures)
            {
                lstAperture.Items.Add(new ListItem<string, int>(tmpAV.GetStringValue(), (int)tmpAV));
            }
            foreach (camControl.ISOSpeeds tmpISO in CameraControl.ValidISOSpeeds)
            {
                lstISOSpeeds.Items.Add(new ListItem<string, int>(tmpISO.GetStringValue(), (int)tmpISO));
            }
            foreach (camControl.ShutterSpeeds tmpShutter in CameraControl.ValidShutterSpeeds)
            {
                lstShutterSpeeds.Items.Add(new ListItem<string, int>(tmpShutter.GetStringValue(), (int)tmpShutter));
            }
            lstBrackets.Items.Add("N/A");
            for (int i = 3; i <= CameraControl.ValidEsposureCompensations.Count(); i += 2)
            {
                lstBrackets.Items.Add(i);
            }

            //UPDATE THE BATTERY PROGRESS AND THE CAMERA NAME
            prgBattery.Value = CameraControl.CurrentBatteryLevel;
            lblCameraName.Text = CameraControl.CameraName;

            //MAKE SURE WE NOW SELECT THE CORRECT ITEMS FROM THE DROP DOWN LIST
            lstISOSpeeds.SelectedIndex = lstISOSpeeds.FindStringExact(CameraControl.CurrentISOSpeed.GetStringValue());
            lstExposureCompensation.SelectedIndex = lstExposureCompensation.FindStringExact(CameraControl.CurrentExposureCompensation.GetStringValue());
            lstAperture.SelectedIndex = lstAperture.FindStringExact(CameraControl.CurrentAperture.GetStringValue());
            lstShutterSpeeds.SelectedIndex = lstShutterSpeeds.FindStringExact(CameraControl.CurrentShutterSpeed.GetStringValue());
            lstBrackets.SelectedIndex = 0;

            CameraControl_CameraModeChanged((camControl.CameraModes)CameraControl.CurrentCameraMode);

            //UN-FLAG THE THE UI IS NO LONGER BEING UPDATED
            adjustingUI = false;
            unlockUI();

            btnRelease.Focus();
        }
        private void lockUI()
        {
            //DISABLE EVERY CONTROL ON THE FORM SO NOTHING CAN BE TOUCHED
            foreach (Control tmpCTL in this.Controls)
                if (tmpCTL.GetType() != typeof(Label))
                    tmpCTL.Enabled = false;
        }
        private void unlockUI()
        {
            //UNLOCK ALL CONTROLS SO THEY CAN BE USED AGAIN
            foreach (Control tmpCTL in UnlockedCTLS)
                tmpCTL.Enabled = true;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            //SETUP EVENT HANDLER FOR THE CAMERA CONTROL
            CameraControl.errorEvent += new camControl.errorEventDelegate(CameraControl_errorEvent);

            if (CameraControl.Init())
            {
                //SETUP ALL ADDITIONAL EVENTS FOR THIS CAMERA NOW
                CameraControl.CameraModeChanged += new camControl.CameraModeChangedDelegate(CameraControl_CameraModeChanged);
                CameraControl.ISOSpeedChanged += new camControl.ISOSpeedChangedDelegate(CameraControl_ISOSpeedChanged);
                CameraControl.ExposureCompensationChanged += new camControl.ExposureCompensationChangedDelegate(CameraControl_ExposireCompensationChanged);
                CameraControl.BatteryLevelChanged += new camControl.BatteryLevelChangedDelegate(CameraControl_BatteryLevelChanged);
                CameraControl.PictureSaved += new camControl.PictureSavedToCameraDelegate(CameraControl_PictureSaved);

                SetupUI();
            }
            else
            {
                lockUI();
            }


        }

        private void ExposureCompensation_SelectedIndexChanged(object sender, EventArgs e)
        {
            //IF THE EXPOSURE SETTING HAS CHANGED THEN WE NEED TO SEND THE COMMAND TO THE CAMERA
            if (!adjustingUI)
                CameraControl.SetExposureCompensation(((ListItem<string, int>)((ComboBox)sender).SelectedItem).Value);
        }
        private void ISOSpeeds_SelectedIndexChanged(object sender, EventArgs e)
        {
            //IF THE ISO SPEED HAS CHANGED THEN WEE NEED TO SEND THE COMMAND TO THE CAMERA
            if (!adjustingUI)
                CameraControl.SetISOSpeed(((ListItem<string, int>)((ComboBox)sender).SelectedItem).Value);
        }
        private void ApertureValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!adjustingUI)
                CameraControl.SetAperture(((ListItem<string, int>)((ComboBox)sender).SelectedItem).Value);
        }
        private void ShutterSpeed_SlectedIndexChanged(object sender, EventArgs e)
        {
            if (!adjustingUI)
                CameraControl.SetShutterSeed(((ListItem<string, int>)((ComboBox)sender).SelectedItem).Value);
        }
        private void btnRelease_Click(object sender, EventArgs e)
        {
            //CHECK FOR INVALID SETTINGS
            if (numMulti.Value > 1 && numDelay.Value == 0)
            {
                MessageBox.Show("Please Select At Least A 1 Second Delay Between Photos!");
                return;
            }
            else if (numMulti.Value == 0)
            {
                MessageBox.Show("You Must At Least Have A Single Photo To Press Release!");
                return;
            }

            //LOCK THE UI SO THEY CAN NOT CHANGE ANYTHING DURING THE EXPOSURE
            lockUI();

            //LOOP THROUGH AND TAKE A PICTURE FOR AS MANY AS THE USER HAS ASKED
            while (((int)numMulti.Value) != 0)
            {
                //IF THE USER HAS SPECIFIED A DELAY THEN BEGIN THE DELAY COUNT DOWN
                if (numDelay.Value != 0)
                    if (new frmCountDown((int)numDelay.Value).ShowDialog(this) != DialogResult.OK)
                    {
                        //UNLOCK THE UI THEN STOP TAKING THIS PHOTO
                        unlockUI();
                        return;
                    }

                if (lstBrackets.SelectedIndex != 0)
                {
                    //IF WE ARE TAKING A BRACKET SHOT THEN WE NEED TO LOOP THROUGH ALL OF THE BRACKETS OTHERWISE JUST TAKE A SINGLE PHOTO
                    //CALCULATE THE START POINT AND THE END POINT OF THE BRACKET SET
                    int zeroPoint = lstExposureCompensation.Items.Count / 2;
                    int PlusMinus = (((int)lstBrackets.SelectedItem - 1) / 2);

                    //LOOP THROUGH THE BRACKET ADJUST THE EXPOSURE SETTING AND TAKE THE PICTURE
                    for (int i = zeroPoint - PlusMinus; i <= zeroPoint + PlusMinus; i++)
                    {
                        lstExposureCompensation.SelectedIndex = i;

                        CameraControl.TakePhotoRapidFire();
                    }

                    //RESET THE CAMERA EXPOSURE BACK TO 0
                    lstExposureCompensation.SelectedIndex = zeroPoint;
                }
                else
                {
                    CameraControl.TakePhoto();
                }

                //DECREMENT THE MULTI SHOT COUNT
                numMulti.Value--;
                Application.DoEvents();
            }

            //UNLOCK THE UI AGAIN
            numMulti.Value = 1;
            unlockUI();
        }
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //UPDATE THE UI MANUALLY
            SetupUI();
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CameraControl.Dispose();
        }

        void CameraControl_errorEvent(string Message)
        {
            MessageBox.Show(Message);
        }
        void CameraControl_BatteryLevelChanged(int NewBatteryLevel)
        {
            prgBattery.Value = NewBatteryLevel;
        }
        void CameraControl_ExposireCompensationChanged(camControl.ExposureCompensations NewExposureCompensation)
        {
        }
        void CameraControl_ISOSpeedChanged(camControl.ISOSpeeds NewSpeed)
        {
        }
        void CameraControl_CameraModeChanged(camControl.CameraModes NewMode)
        {
            UnlockedCTLS.Clear();

            //CHANGE THE TEXT OF THE CAMERA MODE DROP DOWN TO THE NEW MODE
            lstMode.Text = NewMode.GetStringValue();

            switch (NewMode)
            {
                case camControl.CameraModes.Manual:
                    UnlockedCTLS.Add(lstShutterSpeeds);
                    UnlockedCTLS.Add(lstAperture);
                    UnlockedCTLS.Add(lstISOSpeeds);
                    UnlockedCTLS.Add(lstExposureCompensation);
                    UnlockedCTLS.Add(lstBrackets);
                    break;
                case camControl.CameraModes.ProgramAE:
                    UnlockedCTLS.Add(lstBrackets);
                    UnlockedCTLS.Add(lstISOSpeeds);
                    UnlockedCTLS.Add(lstExposureCompensation);
                    break;
                case camControl.CameraModes.AV:
                    UnlockedCTLS.Add(lstExposureCompensation);
                    UnlockedCTLS.Add(lstISOSpeeds);
                    UnlockedCTLS.Add(lstBrackets);
                    UnlockedCTLS.Add(lstAperture);
                    break;
                case camControl.CameraModes.TV:
                    UnlockedCTLS.Add(lstExposureCompensation);
                    UnlockedCTLS.Add(lstBrackets);
                    UnlockedCTLS.Add(lstShutterSpeeds);
                    UnlockedCTLS.Add(lstISOSpeeds);
                    break;
            }

            //ADD THESE CONTROLS NO MATTER WHAT MODE THE CAMERA IS IN
            UnlockedCTLS.Add(lblCameraName);
            UnlockedCTLS.Add(prgBattery);
            UnlockedCTLS.Add(btnRefresh);
            UnlockedCTLS.Add(btnRelease);
            UnlockedCTLS.Add(numMulti);
            UnlockedCTLS.Add(numDelay);

            //FILL ALL OF THE DROP DOWNS WITH THE APPROPRIATE INFORMATION
            SetupUI();
        }
        void CameraControl_PictureSaved(byte[] ImageData)
        {
            this.Invoke((MethodInvoker)delegate
            {
                if (picPreview == null || picPreview.IsDisposed)
                    picPreview = new frmPreview();

                using (MemoryStream tmpStream = new MemoryStream(ImageData))
                {
                    picPreview.picPreview.Image = Image.FromStream(tmpStream);
                    picPreview.Show();
                }
            });
        }
    }
}
