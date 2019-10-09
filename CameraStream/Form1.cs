using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SampleDX;
using RS = Intel.RealSense;

namespace CameraStream
{
    public partial class Form1 : Form
    {
        public RS.Session session;

        private Dictionary<object, RS.DeviceInfo> devices = new Dictionary<object, RS.DeviceInfo>();
        private Dictionary<object, int> devices_iuid = new Dictionary<object, int>();
        private Dictionary<object, RS.StreamProfile> profiles = new Dictionary<object, RS.StreamProfile>();

        private volatile bool closing = false;
        private D2D1Render render = new D2D1Render();
        private RenderStreams streams = new RenderStreams();
        public Form1(RS.Session session)
        {
            InitializeComponent();
            pb_Main.Paint += Pb_Main_Paint;

            this.session = session;
            streams.UpdateStatus += Streams_UpdateStatus;
            streams.RenderFrame += Streams_RenderFrame;
            ResetStreamTypes();
            PopulateDevice();

            render.SetHWND(pb_Main);


        }

        private void PopulateDevice()
        {
            devices.Clear();
            devices_iuid.Clear();

            RS.ImplDesc desc = new RS.ImplDesc();
            desc.group = RS.ImplGroup.IMPL_GROUP_SENSOR;
            desc.subgroup = RS.ImplSubgroup.IMPL_SUBGROUP_VIDEO_CAPTURE;
            cb_Devices.Items.Clear();

            for (int i = 0; ; i++)
            {
                RS.ImplDesc desc1 = session.QueryImpl(desc, i);
                if (desc1 == null)
                    break;
                RS.Capture capture;
                if (session.CreateImpl<RS.Capture>(desc1, out capture) < RS.Status.STATUS_NO_ERROR) continue;
                for (int j = 0; ; j++)
                {
                    RS.DeviceInfo dinfo;
                    if (capture.QueryDeviceInfo(j, out dinfo) < RS.Status.STATUS_NO_ERROR) break;

                    string deviceName = dinfo.name;
                    devices[deviceName] = dinfo;
                    devices_iuid[deviceName] = desc1.iuid;
                    cb_Devices.Items.Add(deviceName);
                }
                capture.Dispose();
            }

        }

        private void PopulateProfiles(RS.DeviceInfo dinfo)
        {
            RS.SenseManager pp = RS.SenseManager.CreateInstance();
            RS.Device device = pp.CaptureManager.Device;
            if (device == null)
            {
                pp.Dispose();
                
            }
            RS.StreamProfileSet profile = new RS.StreamProfileSet();

            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                RS.StreamType st = RS.Capture.StreamTypeFromIndex(s);
                if (((int)dinfo.streams & (int)st) != 0)
                {
                    
                    int num = device.QueryStreamProfileSetNum(st);
                    for (int p = 0; p < num; p++)
                    {
                        if (device.QueryStreamProfileSet(st, p, out profile) < RS.Status.STATUS_NO_ERROR) break;
                        RS.StreamProfile sprofile = profile[st];
                        string profNime = ProfileToString(sprofile);
                        profiles[profile] = sprofile;
                        cb_Profile.Items.Add(profNime);
                    }
                }
                else if (((int)dinfo.streams & (int)st) == 0)
                {
                    
                }
            }

            
        }

        private void ResetStreamTypes()
        {
            streams.MainPanel = RS.StreamType.STREAM_TYPE_ANY;
        }

        private void Pb_Main_Paint(object sender, PaintEventArgs e)
        {
            render.UpdatePanel();
        }

        private void Streams_RenderFrame(object sender, RenderFrameEventArgs e)
        {
            if (e.image == null) return;
            render.UpdatePanel(e.image);
        }

        private void Streams_UpdateStatus(object sender, UpdateStatusEventArgs e)
        {
            
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            btn_Start.Enabled = false;
            btn_Stop.Enabled = true;
            //streams.StreamProfileSet = ;
            streams.StreamProfileSet = new RS.StreamProfileSet();
            streams.DeviceInfo = GetCheckedDevice();
            streams.Stop = false;
            System.Threading.Thread thread = new System.Threading.Thread(DoStreaming);
            thread.Start();
            System.Threading.Thread.Sleep(5);
        }

        delegate void DoStreamingEnd();
        private void DoStreaming()
        {
            streams.StreamColorDepth();
            Invoke(new DoStreamingEnd(
                delegate
                {
                    btn_Start.Enabled = true;
                    btn_Stop.Enabled = false;
                    
                    if (closing) Close();
                }
            ));
        }

        private RS.DeviceInfo GetCheckedDevice()
        {
            return devices[cb_Devices.SelectedItem];
        }

        private void cb_Devices_Click(object sender, EventArgs e)
        {
            PopulateDevice();
        }

        
        private RS.StreamProfileSet GetStreamSetConfiguration()
        {
            RS.StreamProfileSet profiles = new RS.StreamProfileSet();
            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                RS.StreamType st = RS.Capture.StreamTypeFromIndex(s);
                //profiles[st] = GetStreamConfiguration(st);
            }
            return profiles;
        }
        /*
        private RS.StreamProfile GetStreamConfiguration(RS.StreamType st)
        {
            ToolStripMenuItem menu = streamMenus[RS.Capture.StreamTypeToIndex(st)];
            if (menu != null)
                return GetConfiguration(menu);
            else
                return new RS.StreamProfile();
        }

        private RS.StreamProfile GetConfiguration(ComboBox m)
        {
            if (m.SelectedItem != null)
            {
                return profiles[m.SelectedItem];
            }
            
            return new RS.StreamProfile();
        }*/

        private string ProfileToString(RS.StreamProfile pinfo)
        {
            string line = "Unknown ";
            if (Enum.IsDefined(typeof(RS.PixelFormat), pinfo.imageInfo.format))
                line = pinfo.imageInfo.format.ToString().Substring(13) + " " + pinfo.imageInfo.width + "x" + pinfo.imageInfo.height + "x";
            else
                line += pinfo.imageInfo.width + "x" + pinfo.imageInfo.height + "x";
            if (pinfo.frameRate.min != pinfo.frameRate.max)
            {
                line += (float)pinfo.frameRate.min + "-" +
                      (float)pinfo.frameRate.max;
            }
            else
            {
                float fps = (pinfo.frameRate.min != 0) ? pinfo.frameRate.min : pinfo.frameRate.max;
                line += fps;
            }
            line += StreamOptionToString(pinfo.options);
            return line;
        }

        private string StreamOptionToString(RS.StreamOption streamOption)
        {
            switch (streamOption)
            {
                case RS.StreamOption.STREAM_OPTION_UNRECTIFIED:
                    return " RAW";
                case (RS.StreamOption)0x20000: // Depth Confidence
                    return " + Confidence";
                case RS.StreamOption.STREAM_OPTION_DEPTH_PRECALCULATE_UVMAP:
                case RS.StreamOption.STREAM_OPTION_STRONG_STREAM_SYNC:
                case RS.StreamOption.STREAM_OPTION_ANY:
                    return "";
                default:
                    return " (" + streamOption.ToString() + ")";
            }
        }

        private void cb_Devices_SelectedIndexChanged(object sender, EventArgs e)
        {
            //PopulateProfiles(devices[cb_Devices.SelectedItem]);
        }
    }
}
