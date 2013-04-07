using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EDSDKLib;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace HDRControl
{
    class camControl : IDisposable
    {
        #region "Private Variables"
        private IntPtr camObj;
        private bool TakingPhoto = false;
        private bool DownloadingPhoto = false;
        private EDSDK.EdsObjectEventHandler objEventHandlerDelegate;
        private EDSDK.EdsPropertyEventHandler propEventHandlerDelegate;
        private EDSDK.EdsStateEventHandler stateEventHandlerDelegate;
        #endregion

        #region "Events"
        public delegate void errorEventDelegate(string Message);
        public event errorEventDelegate errorEvent = null;

        public delegate void CameraModeChangedDelegate(CameraModes NewMode);
        public event CameraModeChangedDelegate CameraModeChanged = null;

        public delegate void ISOSpeedChangedDelegate(ISOSpeeds NewSpeed);
        public event ISOSpeedChangedDelegate ISOSpeedChanged = null;

        public delegate void ExposureCompensationChangedDelegate(ExposureCompensations NewExposureCompensation);
        public event ExposureCompensationChangedDelegate ExposureCompensationChanged = null;

        public delegate void BatteryLevelChangedDelegate(int NewBatteryLevel);
        public event BatteryLevelChangedDelegate BatteryLevelChanged = null;

        public delegate void PictureSavedToCameraDelegate(byte[] ImageData);
        public event PictureSavedToCameraDelegate PictureSaved = null;
        #endregion

        #region "Properties"
        private List<ExposureCompensations> exposureValues = new List<ExposureCompensations>();
        public ExposureCompensations[] ValidEsposureCompensations
        {
            get
            {
                //GET CAMERA BRACKETING PROPERTIES
                exposureValues.Clear();
                if (!getBrackets())
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set event handler");
                }
                return exposureValues.ToArray();
            }
        }
        private List<ISOSpeeds> isoValues = new List<ISOSpeeds>();
        public ISOSpeeds[] ValidISOSpeeds
        {
            get
            {
                //GET THE CAMERA ISO SPEEDS
                isoValues.Clear();
                if (!getISOS())
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set event handler");
                }

                return isoValues.ToArray();
            }
        }
        public string CameraName
        {
            get
            {
                uint err = 0;
                EDSDK.EdsDeviceInfo camInfo;

                err = EDSDK.EdsGetDeviceInfo(camObj, out camInfo);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to find a camera");
                    return null;
                }

                return camInfo.szDeviceDescription;
            }
        }
        public ISOSpeeds CurrentISOSpeed
        {
            get
            {
                uint ISO = 0;
                uint err = 0;

                err = EDSDK.EdsGetPropertyData(camObj, EDSDK.PropID_ISOSpeed, 0, out ISO);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to retrieve current ISO Speed");

                    return 0;
                }

                return (ISOSpeeds)(int)ISO;
            }
        }
        public ExposureCompensations CurrentExposureCompensation
        {
            get
            {
                uint EXP = 0;
                uint err = 0;

                err = EDSDK.EdsGetPropertyData(camObj, EDSDK.PropID_ExposureCompensation, 0, out EXP);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to retrieve current ISO Speed");

                    return 0;
                }

                return (ExposureCompensations)(int)EXP;
            }
        }
        public CameraModes CurrentCameraMode
        {
            get
            {
                uint Mode = 0;
                uint err = 0;

                err = EDSDK.EdsGetPropertyData(camObj, EDSDK.PropID_AEMode, 0, out Mode);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to retrieve current camera mode");

                    return 0;
                }

                return (CameraModes)Mode;
            }
        }
        public int CurrentBatteryLevel
        {
            get
            {
                uint Battery = 0;
                uint err = 0;

                err = EDSDK.EdsGetPropertyData(camObj, EDSDK.PropID_BatteryLevel, 0, out Battery);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to retrieve current camera mode");

                    return 0;
                }

                return (int)Battery;
            }
        }
        private List<ShutterSpeeds> shutterSpeeds = new List<ShutterSpeeds>();
        public ShutterSpeeds[] ValidShutterSpeeds
        {
            get
            {
                //GET THE CAMERA SHUTTER SPEEDS
                shutterSpeeds.Clear();
                if (!getShutterSpeeds())
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set event handler");
                }

                return shutterSpeeds.ToArray();
            }
        }
        public ShutterSpeeds CurrentShutterSpeed
        {
            get
            {
                uint ShutterSpeed = 0;
                uint err = 0;

                err = EDSDK.EdsGetPropertyData(camObj, EDSDK.PropID_Tv, 0, out ShutterSpeed);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to retrieve current camera mode");

                    return 0;
                }

                return (ShutterSpeeds)ShutterSpeed;
            }
        }

        private List<Apertures> apertureValues = new List<Apertures>();
        public Apertures[] ValidApertures
        {
            get
            {
                //GET THE CAMERA SHUTTER SPEEDS
                apertureValues.Clear();
                if (!getApertures())
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set event handler");
                }

                return apertureValues.ToArray();
            }
        }
        public Apertures CurrentAperture
        {
            get
            {
                uint Aperture = 0;
                uint err = 0;

                err = EDSDK.EdsGetPropertyData(camObj, EDSDK.PropID_Av, 0, out Aperture);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to retrieve current camera mode");

                    return 0;
                }

                return (Apertures)Aperture;
            }
        }
        #endregion

        #region "Constructors"
        public camControl()
        {
        }
        public camControl(bool init)
        {
            if (init)
                this.Init();
        }
        #endregion

        #region "Private Methods"
        private bool getBrackets()
        {
            uint err = 0;
            EDSDK.EdsPropertyDesc EXP;

            err = EDSDK.EdsGetPropertyDesc(camObj, EDSDK.PropID_ExposureCompensation, out EXP);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to get camera exposure compensation values");

                return false;
            }

            //ADD THE VALUES RETRIEVED TO THE PROPERTY LIST
            //exposureValues.AddRange((IEnumerable<ExposureCompensations>)EXP.PropDesc.Take(EXP.NumElements));
            foreach (int tmpExp in EXP.PropDesc.Take(EXP.NumElements))
            {
                exposureValues.Add((ExposureCompensations)tmpExp);
            }

            return true;
        }
        private bool getISOS()
        {
            uint err = 0;
            EDSDK.EdsPropertyDesc ISOS;

            err = EDSDK.EdsGetPropertyDesc(camObj, EDSDK.PropID_ISOSpeed, out ISOS);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to get camera iso speeds");

                return false;
            }

            //ADD THE ISO VALUES TO THE PROPERTY LIST
            //isoValues.AddRange((IEnumerable<ISOSpeeds>)ISOS.PropDesc.Take(ISOS.NumElements));
            foreach (int tmpISO in ISOS.PropDesc.Take(ISOS.NumElements))
            {
                isoValues.Add((ISOSpeeds)tmpISO);
            }

            return true;
        }
        private bool getShutterSpeeds()
        {
            uint err = 0;
            EDSDK.EdsPropertyDesc Shutters;

            err = EDSDK.EdsGetPropertyDesc(camObj, EDSDK.PropID_Tv, out Shutters);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to get camera iso speeds");

                return false;
            }

            //ADD THE ISO VALUES TO THE PROPERTY LIST
            //isoValues.AddRange((IEnumerable<ISOSpeeds>)ISOS.PropDesc.Take(ISOS.NumElements));
            foreach (int tmpISO in Shutters.PropDesc.Take(Shutters.NumElements))
            {
                shutterSpeeds.Add((ShutterSpeeds)tmpISO);
            }

            return true;
        }
        private bool getApertures()
        {
            uint err = 0;
            EDSDK.EdsPropertyDesc apertures;

            err = EDSDK.EdsGetPropertyDesc(camObj, EDSDK.PropID_Av, out apertures);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to get camera aperture values");

                return false;
            }

            //ADD THE ISO VALUES TO THE PROPERTY LIST
            //isoValues.AddRange((IEnumerable<ISOSpeeds>)ISOS.PropDesc.Take(ISOS.NumElements));
            foreach (int tmpAperture in apertures.PropDesc.Take(apertures.NumElements))
            {
                apertureValues.Add((Apertures)tmpAperture);
            }

            return true;
        }
        private IntPtr getVolume()
        {
            uint err;
            int count = 0;
            IntPtr volume;

            //GET VOLUME COUNT ON CAMERA
            err = EDSDK.EdsGetChildCount(camObj, out count);
            if (err != EDSDK.EDS_ERR_OK || count == 0)
            {
                if (errorEvent != null)
                    errorEvent("Volume Not Found!");

                return IntPtr.Zero;
            }

            //GET INITIAL VOLUME
            err = EDSDK.EdsGetChildAtIndex(camObj, 0, out volume);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Directory Not Found!");

                return IntPtr.Zero;
            }

            return volume;
        }
        private IntPtr? getDCIMFolder()
        {
            uint err;
            int count = 0;
            IntPtr volume = getVolume();
            IntPtr dirHandle;
            EDSDK.EdsDirectoryItemInfo dirInfo;

            //GET THE ROOT VOLUME FOR THE CAMERA
            if (volume == IntPtr.Zero)
            {
                if (errorEvent != null)
                    errorEvent("Failed To Get Root Volume!");

                return null;
            }

            //GET THE COUNT OF FOLDERS ON THE VOLUME
            err = EDSDK.EdsGetChildCount(volume, out count);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed To Get Folders on Volume!");

                return null;
            }

            for (int i = 0; i < count && err == EDSDK.EDS_ERR_OK; i++)
            {
                err = EDSDK.EdsGetChildAtIndex(volume, i, out dirHandle);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to get Folder at index " + i.ToString());

                    return null;
                }

                err = EDSDK.EdsGetDirectoryItemInfo(dirHandle, out dirInfo);
                if (err != EDSDK.EDS_ERR_OK)
                {
                    if (errorEvent != null)
                        errorEvent("Failed To Get Folder At Index " + i.ToString());

                    return null;
                }

                if (dirInfo.szFileName.ToUpper().Contains("DCIM") && dirInfo.isFolder == 1)
                {
                    EDSDK.EdsRelease(volume);
                    return dirHandle;
                }
                else
                {
                    EDSDK.EdsRelease(dirHandle);
                }
            }

            return null;
        }
        #endregion

        #region "Event Handlers"
        private uint objEventHandler(uint inEvent, IntPtr inRef, IntPtr inContext)
        {
            switch (inEvent)
            {
                case EDSDK.ObjectEvent_DirItemContentChanged:
                    break;
                case EDSDK.ObjectEvent_DirItemCreated:
                    TakingPhoto = false;

                    Application.DoEvents();
                    System.Threading.Thread tmpThread = new Thread(delegate()
                    {
                        if (!DownloadingPhoto)
                        {
                            DownloadingPhoto = true;

                            if (PictureSaved != null)
                            {
                                byte[] tmpPhoto = DownloadLastPhoto();
                                PictureSaved(tmpPhoto);
                            }
                        }
                    });
                    tmpThread.Start();

                    break;
                case EDSDK.ObjectEvent_DirItemInfoChanged:
                    break;
                case EDSDK.ObjectEvent_DirItemRemoved:
                    break;
                case EDSDK.ObjectEvent_DirItemRequestTransfer:
                    break;
                case EDSDK.ObjectEvent_DirItemRequestTransferDT:
                    break;
                case EDSDK.ObjectEvent_FolderUpdateItems:
                    break;
                case EDSDK.ObjectEvent_VolumeAdded:
                    break;
                case EDSDK.ObjectEvent_VolumeInfoChanged:
                    break;
                case EDSDK.ObjectEvent_VolumeRemoved:
                    break;
                case EDSDK.ObjectEvent_VolumeUpdateItems:
                    break;
            }

            if (inContext != IntPtr.Zero)
                EDSDK.EdsRelease(inContext);

            return 0;
        }
        private uint propEventHandler(uint inEvent, uint inPropertyID, uint inParam, IntPtr inContext)
        {
            switch (inEvent)
            {
                case EDSDK.PropertyEvent_PropertyChanged:
                    switch (inPropertyID)
                    {
                        case EDSDK.PropID_AEMode:
                            if (CameraModeChanged != null)
                                CameraModeChanged((CameraModes)CurrentCameraMode);
                            break;

                        case EDSDK.PropID_ISOSpeed:
                            if (ISOSpeedChanged != null)
                                ISOSpeedChanged((ISOSpeeds)CurrentISOSpeed);
                            break;

                        case EDSDK.PropID_ExposureCompensation:
                            if (ExposureCompensationChanged != null)
                                ExposureCompensationChanged((ExposureCompensations)CurrentExposureCompensation);
                            break;

                        case EDSDK.PropID_BatteryLevel:
                            if (BatteryLevelChanged != null)
                                BatteryLevelChanged(CurrentBatteryLevel);
                            break;
                    }
                    break;
                case EDSDK.PropertyEvent_PropertyDescChanged:
                    break;
            }

            if (inContext != IntPtr.Zero)
                EDSDK.EdsRelease(inContext);

            return 0;
        }
        private uint stateEventHandler(uint inEvent, uint inParameter, IntPtr inContext)
        {
            switch (inEvent)
            {
                case EDSDK.StateEvent_AfResult:
                    break;
                case EDSDK.StateEvent_BulbExposureTime:
                    break;
                case EDSDK.StateEvent_CaptureError:
                    break;
                case EDSDK.StateEvent_InternalError:
                    break;
                case EDSDK.StateEvent_JobStatusChanged:
                    break;
                case EDSDK.StateEvent_Shutdown:
                    break;
                case EDSDK.StateEvent_ShutDownTimerUpdate:
                    break;
                case EDSDK.StateEvent_WillSoonShutDown:
                    break;
            }

            if (inContext != IntPtr.Zero)
                EDSDK.EdsRelease(inContext);

            return 0;
        }
        #endregion

        #region "Public Methods"
        public bool Init()
        {
            IntPtr camList;
            int camCount = 0;
            uint err = 0;

            //SETUP DELEGATES FOR LATER USE
            objEventHandlerDelegate = new EDSDK.EdsObjectEventHandler(objEventHandler);
            propEventHandlerDelegate = new EDSDK.EdsPropertyEventHandler(propEventHandler);
            stateEventHandlerDelegate = new EDSDK.EdsStateEventHandler(stateEventHandler);

            //INIT THE SDK THIS MUST BE CALLED BEFORE ANYTHING ELSE CAN BE DONE
            err = EDSDK.EdsInitializeSDK();
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to init SDK");

                return false;
            }

            //GET ALL ATTACHED CAMERAS
            err = EDSDK.EdsGetCameraList(out camList);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to find a camera");

                return false;
            }

            //GET THE NUMBER OF ATTACHED CAMERAS
            err = EDSDK.EdsGetChildCount(camList, out camCount);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to get camera count");

                return false;
            }

            //IF THERE ARE 0 CAMERAS DETECETED THEN THROW AN ERROR
            if (camCount == 0)
            {
                if (errorEvent != null)
                    errorEvent("No cameras found");

                return false;
            }

            //WE ONLY CARE ABOUT THE FIRST CAMERA WE FIND
            err = EDSDK.EdsGetChildAtIndex(camList, 0, out camObj);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to retrieve camera object");

                return false;
            }

            //SET THE EVENT HANDLERS FOR LATER USE
            err = EDSDK.EdsSetObjectEventHandler(camObj, EDSDK.ObjectEvent_All, objEventHandlerDelegate, IntPtr.Zero);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to set event handler");

                return false;
            }
            err = EDSDK.EdsSetPropertyEventHandler(camObj, EDSDK.PropertyEvent_All, propEventHandlerDelegate, IntPtr.Zero);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to set event handler");

                return false;
            }
            err = EDSDK.EdsSetCameraStateEventHandler(camObj, EDSDK.StateEvent_All, stateEventHandlerDelegate, IntPtr.Zero);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to set event handler");

                return false;
            }

            //OPEN THE SESSION TO THE CAMERA
            err = EDSDK.EdsOpenSession(camObj);

            //LET THE CALLING FUNCTION KNOW THIS ALL WORKED
            return true;
        }
        public void Dispose()
        {
            uint err = 0;

            //CLOSE THE CAMERA SESSION (ASSUMING THERE IS ONE) RELEASE THE CAMERA AND TERMINATE THE SDK
            err = EDSDK.EdsCloseSession(camObj);
            err = EDSDK.EdsRelease(camObj);
            err = EDSDK.EdsTerminateSDK();
        }
        public void SetExposureCompensation(int ExposureCompenstaion)
        {
            uint err = 0;

            do
            {
                err = EDSDK.EdsSetPropertyData(camObj, EDSDK.PropID_ExposureCompensation, 0, sizeof(int), ExposureCompenstaion);
                if (err != EDSDK.EDS_ERR_OK && err != EDSDK.EDS_ERR_DEVICE_BUSY)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set exposure value");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
            while (err == EDSDK.EDS_ERR_DEVICE_BUSY);
        }
        public void SetISOSpeed(int ISO)
        {
            uint err = 0;
            do
            {
                err = EDSDK.EdsSetPropertyData(camObj, EDSDK.PropID_ISOSpeed, 0, sizeof(int), ISO);
                if (err != EDSDK.EDS_ERR_OK && err != EDSDK.EDS_ERR_DEVICE_BUSY)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set iso speed");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            } while (err == EDSDK.EDS_ERR_DEVICE_BUSY);
        }
        public void SetShutterSeed(int Shutter)
        {
            uint err = 0;
            do
            {
                err = EDSDK.EdsSetPropertyData(camObj, EDSDK.PropID_Tv, 0, sizeof(int), Shutter);
                if (err != EDSDK.EDS_ERR_OK && err != EDSDK.EDS_ERR_DEVICE_BUSY)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set iso speed");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            } while (err == EDSDK.EDS_ERR_DEVICE_BUSY);
        }
        public void SetAperture(int NewAperture)
        {
            uint err = 0;
            do
            {
                err = EDSDK.EdsSetPropertyData(camObj, EDSDK.PropID_Av, 0, sizeof(int), NewAperture);
                if (err != EDSDK.EDS_ERR_OK && err != EDSDK.EDS_ERR_DEVICE_BUSY)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to set aperture");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            } while (err == EDSDK.EDS_ERR_DEVICE_BUSY);
        }
        public void TakePhotoRapidFire()
        {
            uint err;
            do
            {
                TakingPhoto = true;
                err = EDSDK.EdsSendCommand(camObj, EDSDK.CameraCommand_TakePicture, 0);

                if (err != EDSDK.EDS_ERR_OK && err != EDSDK.EDS_ERR_DEVICE_BUSY)
                {
                    if (errorEvent != null)
                        errorEvent("Failed to take picture");
                    return;
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            } while (err == EDSDK.EDS_ERR_DEVICE_BUSY);
        }
        public void TakePhoto()
        {
            uint err;
            TakingPhoto = true;
            err = EDSDK.EdsSendCommand(camObj, EDSDK.CameraCommand_TakePicture, 0);

            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed to take picture");
                return;
            }

            while (TakingPhoto)
            {
                System.Threading.Thread.Sleep(1);
                System.Windows.Forms.Application.DoEvents();
            }
        }
        public byte[] DownloadLastPhoto()
        {
            int count;
            uint err;
            IntPtr? dirInfo = getDCIMFolder();
            IntPtr folderHandle;
            IntPtr imageHandle;
            EDSDK.EdsDirectoryItemInfo imageInfo;
            IntPtr memStream;
            IntPtr streamStart;

            //MAKE SURE WE HAVE A DCIM FOLDER
            if (dirInfo == null)
            {
                if (errorEvent != null)
                    errorEvent("Failed To Find DCIM Folder");

                return null;
            }

            err = EDSDK.EdsGetChildCount(dirInfo.Value, out count);
            if (err != EDSDK.EDS_ERR_OK)
            {
                if (errorEvent != null)
                    errorEvent("Failed To Get Child Folder!");

                return null;
            }


            err = EDSDK.EdsGetChildAtIndex(dirInfo.Value, count - 1, out folderHandle);
            err = EDSDK.EdsGetChildCount(folderHandle, out count);
            err = EDSDK.EdsGetChildAtIndex(folderHandle, count - 1, out imageHandle);
            err = EDSDK.EdsGetDirectoryItemInfo(imageHandle, out imageInfo);
            err = EDSDK.EdsCreateMemoryStream(0, out memStream);

            EDSDK.EdsDownload(imageHandle, imageInfo.Size, memStream);
            EDSDK.EdsDownloadComplete(imageHandle);

            byte[] imageData = new byte[imageInfo.Size];
            EDSDK.EdsGetPointer(memStream, out streamStart);
            Marshal.Copy(streamStart, imageData, 0, (int)imageInfo.Size);

            EDSDK.EdsRelease(streamStart);
            EDSDK.EdsRelease(memStream);
            EDSDK.EdsRelease(folderHandle);
            EDSDK.EdsRelease(imageHandle);
            EDSDK.EdsRelease(dirInfo.Value);

            DownloadingPhoto = false;

            return imageData;
        }
        #endregion

        #region "Public Enums"
        public enum CameraModes
        {
            [Extensions.StringValue("Program Auto Exposure")]
            ProgramAE = 0,
            [Extensions.StringValue("Shutter Priority")]
            TV = 1,
            [Extensions.StringValue("Aperture Priority")]
            AV = 2,
            [Extensions.StringValue("Full Manual")]
            Manual = 3,
            [Extensions.StringValue("Bulb")]
            Bulb = 4,
            [Extensions.StringValue("Auto Depth of Field")]
            ADEP = 5,
            [Extensions.StringValue("Depth of Field")]
            DEP = 6,
            [Extensions.StringValue("Full Auto")]
            Auto = 9
        }
        public enum ISOSpeeds
        {
            [Extensions.StringValue("ISO Auto")]
            ISOAuto = 0,
            [Extensions.StringValue("ISO 6")]
            ISO6 = 40,
            [Extensions.StringValue("ISO 12")]
            ISO12 = 48,
            [Extensions.StringValue("ISO 25")]
            ISO25 = 56,
            [Extensions.StringValue("ISO 50")]
            ISO50 = 64,
            [Extensions.StringValue("ISO 100")]
            ISO100 = 72,
            [Extensions.StringValue("ISO 125")]
            ISO125 = 75,
            [Extensions.StringValue("ISO 160")]
            ISO160 = 77,
            [Extensions.StringValue("ISO 200")]
            ISO200 = 80,
            [Extensions.StringValue("ISO 250")]
            ISO250 = 83,
            [Extensions.StringValue("ISO 320")]
            ISO320 = 85,
            [Extensions.StringValue("ISO 400")]
            ISO400 = 88,
            [Extensions.StringValue("ISO 500")]
            ISO500 = 91,
            [Extensions.StringValue("ISO 640")]
            ISO640 = 93,
            [Extensions.StringValue("ISO 800")]
            ISO800 = 96,
            [Extensions.StringValue("ISO 1000")]
            ISO1000 = 99,
            [Extensions.StringValue("ISO 1250")]
            ISO1250 = 101,
            [Extensions.StringValue("ISO 1600")]
            ISO1600 = 104,
            [Extensions.StringValue("ISO 3200")]
            ISO3200 = 112,
            [Extensions.StringValue("ISO 6400")]
            ISO6400 = 120
        }
        public enum ExposureCompensations
        {
            [Extensions.StringValue("+3")]
            Plus3 = 24,
            [Extensions.StringValue("+2 2/3")]
            Plus2_2_3 = 21,
            [Extensions.StringValue("+2 1/2")]
            Plus2_1_2 = 20,
            [Extensions.StringValue("+2 1/3")]
            Plus2_1_3 = 19,
            [Extensions.StringValue("+2")]
            Plus2 = 16,
            [Extensions.StringValue("+1 2/3")]
            Plus1_2_3 = 13,
            [Extensions.StringValue("+1 1/2")]
            Plus1_1_2 = 12,
            [Extensions.StringValue("+1 1/3")]
            Plus1_1_3 = 11,
            [Extensions.StringValue("+1")]
            Plus1 = 8,
            [Extensions.StringValue("+ 2/3")]
            Plus0_2_3 = 5,
            [Extensions.StringValue("+ 1/2")]
            Plus0_1_2 = 4,
            [Extensions.StringValue("+ 1/3")]
            Plus0_1_3 = 3,
            [Extensions.StringValue("0")]
            Zero = 0,
            [Extensions.StringValue("- 1/3")]
            Neg0_1_3 = 253,
            [Extensions.StringValue("- 1/2")]
            Neg0_1_2 = 252,
            [Extensions.StringValue("- 2/3")]
            Neg0_2_3 = 251,
            [Extensions.StringValue("-1")]
            Neg1 = 248,
            [Extensions.StringValue("-1 1/3")]
            Neg1_1_3 = 245,
            [Extensions.StringValue("-1 1/2")]
            Neg1_1_2 = 244,
            [Extensions.StringValue("-1 2/3")]
            Neg1_2_3 = 243,
            [Extensions.StringValue("-2")]
            Neg2 = 240,
            [Extensions.StringValue("-2 1/3")]
            Neg2_1_3 = 237,
            [Extensions.StringValue("-2 1/2")]
            Neg2_1_2 = 236,
            [Extensions.StringValue("-2 2/3")]
            Neg2_2_3 = 235,
            [Extensions.StringValue("-3")]
            Neg3 = 232
        }
        public enum ShutterSpeeds
        {
            [Extensions.StringValue("Bulb")]
            TVBulb = 0x0c,
            [Extensions.StringValue(@"30""")]
            TV30 = 0X10,
            [Extensions.StringValue(@"25""")]
            TV25 = 0X13,
            [Extensions.StringValue(@"20""")]
            TV20 = 0X14,
            [Extensions.StringValue(@"20""")]
            TV20_1_3 = 0X15,
            [Extensions.StringValue(@"15""")]
            TV15 = 0X18,
            [Extensions.StringValue(@"13""")]
            TV13 = 0X1B,
            [Extensions.StringValue(@"10""")]
            TV10 = 0X1C,
            [Extensions.StringValue(@"10""")]
            TV10_1_3 = 0X1D,
            [Extensions.StringValue(@"8""")]
            TV8 = 0X20,
            [Extensions.StringValue(@"6""")]
            TV6_1_3 = 0X23,
            [Extensions.StringValue(@"6""")]
            TV6 = 0X24,
            [Extensions.StringValue(@"5""")]
            TV5 = 0X25,
            [Extensions.StringValue(@"4""")]
            TV4 = 0X28,
            [Extensions.StringValue(@"3""2")]
            TV3_2 = 0X2B,
            [Extensions.StringValue(@"3""")]
            TV3 = 0X2C,
            [Extensions.StringValue(@"2""5")]
            TV2_5 = 0X2D,
            [Extensions.StringValue(@"2""")]
            TV2 = 0X30,
            [Extensions.StringValue(@"1""6")]
            TV1_6 = 0X33,
            [Extensions.StringValue(@"1""5")]
            TV1_5 = 0X34,
            [Extensions.StringValue(@"1""3")]
            TV1_3 = 0X35,
            [Extensions.StringValue(@"1""")]
            TV1 = 0X38,
            [Extensions.StringValue(@"0""8")]
            TV0_8 = 0X3b,
            [Extensions.StringValue(@"0""7")]
            TV0_7 = 0X3c,
            [Extensions.StringValue(@"0""6")]
            TV0_6 = 0X3d,
            [Extensions.StringValue(@"0""5")]
            TV0_5 = 0X40,
            [Extensions.StringValue(@"0""4")]
            TV0_4 = 0X43,
            [Extensions.StringValue(@"0""3")]
            TV0_3 = 0X44,
            [Extensions.StringValue(@"0""3")]
            TV0_3_1_3 = 0X45,
            [Extensions.StringValue(@"1/4")]
            TV0_1_4 = 0X48,
            [Extensions.StringValue(@"1/5")]
            TV0_1_5 = 0X4b,
            [Extensions.StringValue(@"1/6")]
            TV0_1_6 = 0X4c,
            [Extensions.StringValue(@"1/6")]
            TV0_1_6_1_3 = 0X4d,
            [Extensions.StringValue(@"1/8")]
            TV0_1_8 = 0X50,
            [Extensions.StringValue(@"1/10")]
            TV0_1_10_1_3 = 0X53,
            [Extensions.StringValue(@"1/10")]
            TV0_1_10 = 0X54,
            [Extensions.StringValue(@"1/13")]
            TV0_1_13 = 0X55,
            [Extensions.StringValue(@"1/15")]
            TV0_1_15 = 0X58,
            [Extensions.StringValue(@"1/20")]
            TV0_1_20 = 0X5b,
            [Extensions.StringValue(@"1/20")]
            TV0_1_20_1_3 = 0X5c,
            [Extensions.StringValue(@"1/25")]
            TV0_1_25 = 0X5d,
            [Extensions.StringValue(@"1/30")]
            TV0_1_30 = 0X60,
            [Extensions.StringValue(@"1/40")]
            TV0_1_40 = 0X63,
            [Extensions.StringValue(@"1/45")]
            TV0_1_45 = 0X64,
            [Extensions.StringValue(@"1/50")]
            TV0_1_50 = 0X65,
            [Extensions.StringValue(@"1/60")]
            TV0_1_60 = 0X68,
            [Extensions.StringValue(@"1/80")]
            TV0_1_80 = 0X6b,
            [Extensions.StringValue(@"1/90")]
            TV0_1_90 = 0X6c,
            [Extensions.StringValue(@"1/100")]
            TV0_1_100 = 0X6d,
            [Extensions.StringValue(@"1/125")]
            TV0_1_125 = 0X70,
            [Extensions.StringValue(@"1/160")]
            TV0_1_160 = 0X73,
            [Extensions.StringValue(@"1/180")]
            TV0_1_180 = 0X74,
            [Extensions.StringValue(@"1/200")]
            TV0_1_200 = 0X75,
            [Extensions.StringValue(@"1/250")]
            TV0_1_250 = 0X78,
            [Extensions.StringValue(@"1/320")]
            TV0_1_320 = 0X7b,
            [Extensions.StringValue(@"1/350")]
            TV0_1_350 = 0X7c,
            [Extensions.StringValue(@"1/400")]
            TV0_1_400 = 0X7d,
            [Extensions.StringValue(@"1/500")]
            TV0_1_500 = 0X80,
            [Extensions.StringValue(@"1/640")]
            TV0_1_640 = 0X83,
            [Extensions.StringValue(@"1/750")]
            TV0_1_750 = 0X84,
            [Extensions.StringValue(@"1/800")]
            TV0_1_800 = 0X85,
            [Extensions.StringValue(@"1/1000")]
            TV0_1_1000 = 0x88,
            [Extensions.StringValue(@"1/1250")]
            TV0_1_1250 = 0X8b,
            [Extensions.StringValue(@"1/1500")]
            TV0_1_1500 = 0X8c,
            [Extensions.StringValue(@"1/1600")]
            TV0_1_1600 = 0X8d,
            [Extensions.StringValue(@"1/2000")]
            TV0_1_2000 = 0X90,
            [Extensions.StringValue(@"1/2500")]
            TV0_1_2500 = 0X93,
            [Extensions.StringValue(@"1/3000")]
            TV0_1_3000 = 0X94,
            [Extensions.StringValue(@"1/3200")]
            TV0_1_3200 = 0X95,
            [Extensions.StringValue(@"1/4000")]
            TV0_1_4000 = 0X98,
            [Extensions.StringValue(@"1/5000")]
            TV0_1_5000 = 0X9b,
            [Extensions.StringValue(@"1/6000")]
            TV0_1_6000 = 0X9c,
            [Extensions.StringValue(@"1/6400")]
            TV0_1_6400 = 0X9d,
            [Extensions.StringValue(@"1/8000")]
            TV0_1_8000 = 0Xa0,
        }
        public enum Apertures
        {
            [Extensions.StringValue(@"1")]
            AV1 = 0X08,
            [Extensions.StringValue(@"1.1")]
            AV1_1 = 0X0b,
            [Extensions.StringValue(@"1.2")]
            AV1_2 = 0X0c,
            [Extensions.StringValue(@"1.2")]
            AV1_2_1_3 = 0X0d,
            [Extensions.StringValue(@"1.4")]
            AV1_4 = 0X10,
            [Extensions.StringValue(@"1.6")]
            AV1_6 = 0X13,
            [Extensions.StringValue(@"1.8")]
            AV1_8 = 0X14,
            [Extensions.StringValue(@"1.8")]
            AV1_8_1_3 = 0X15,
            [Extensions.StringValue(@"2")]
            AV2 = 0X18,
            [Extensions.StringValue(@"2.2")]
            AV2_2 = 0X1b,
            [Extensions.StringValue(@"2.5")]
            AV2_5 = 0X1c,
            [Extensions.StringValue(@"2.5")]
            AV2_5_1_3 = 0X1d,
            [Extensions.StringValue(@"2.8")]
            AV2_8 = 0X20,
            [Extensions.StringValue(@"3.2")]
            AV3_2 = 0X23,
            [Extensions.StringValue(@"3.5")]
            AV3_5 = 0X24,
            [Extensions.StringValue(@"3.5")]
            AV3_5_1_3 = 0X25,
            [Extensions.StringValue(@"4")]
            AV4 = 0X28,
            [Extensions.StringValue(@"4.5")]
            AV4_5 = 0X2b,
            [Extensions.StringValue(@"4.5")]
            AV4_5_1_3 = 0X2c,
            [Extensions.StringValue(@"5")]
            AV5 = 0X2d,
            [Extensions.StringValue(@"5.6")]
            AV5_6 = 0X30,
            [Extensions.StringValue(@"6.3")]
            AV6_3 = 0X33,
            [Extensions.StringValue(@"6.7")]
            AV6_7 = 0X34,
            [Extensions.StringValue(@"7.1")]
            AV7_1 = 0X35,
            [Extensions.StringValue(@"8")]
            AV8 = 0X38,
            [Extensions.StringValue(@"9")]
            AV9 = 0X3b,
            [Extensions.StringValue(@"9.5")]
            AV9_5 = 0X3c,
            [Extensions.StringValue(@"10")]
            AV10 = 0X3d,
            [Extensions.StringValue(@"11")]
            AV11 = 0X40,
            [Extensions.StringValue(@"13")]
            AV13 = 0X43,
            [Extensions.StringValue(@"13")]
            AV13_1_3 = 0X44,
            [Extensions.StringValue(@"14")]
            AV14 = 0X45,
            [Extensions.StringValue(@"16")]
            AV16 = 0X48,
            [Extensions.StringValue(@"18")]
            AV18 = 0X4b,
            [Extensions.StringValue(@"19")]
            AV19 = 0X4c,
            [Extensions.StringValue(@"20")]
            AV20 = 0X4d,
            [Extensions.StringValue(@"22")]
            AV22 = 0X50,
            [Extensions.StringValue(@"25")]
            AV25 = 0X53,
            [Extensions.StringValue(@"27")]
            AV27 = 0X54,
            [Extensions.StringValue(@"29")]
            AV29 = 0X55,
            [Extensions.StringValue(@"32")]
            AV32 = 0X58,
            [Extensions.StringValue(@"36")]
            AV36 = 0X5b,
            [Extensions.StringValue(@"38")]
            AV38 = 0X5c,
            [Extensions.StringValue(@"40")]
            AV40 = 0X5d,
            [Extensions.StringValue(@"45")]
            AV45 = 0X60,
            [Extensions.StringValue(@"51")]
            AV51 = 0X63,
            [Extensions.StringValue(@"54")]
            AV54 = 0X64,
            [Extensions.StringValue(@"57")]
            AV57 = 0X65,
            [Extensions.StringValue(@"64")]
            AV64 = 0X68,
            [Extensions.StringValue(@"72")]
            AV72 = 0X6b,
            [Extensions.StringValue(@"76")]
            AV76 = 0X6c,
            [Extensions.StringValue(@"80")]
            AV80 = 0X6d,
            [Extensions.StringValue(@"91")]
            AV91 = 0X70
        }
        public enum BatteryQualitys
        {
            [Extensions.StringValue("Full Battery")]
            Full = 3,
            [Extensions.StringValue("Hi Battery")]
            HI = 2,
            [Extensions.StringValue("Half Battery")]
            Half = 1,
            [Extensions.StringValue("Low Battery")]
            Low = 0,
        }
        #endregion

        #region IDisposable Members
        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        #endregion
    }
}
