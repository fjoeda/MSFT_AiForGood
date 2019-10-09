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

        private void ResetStreamTypes()
        {
            streams.MainPanel = RS.StreamType.STREAM_TYPE_ANY;
        }

        private void Pb_Main_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Streams_RenderFrame(object sender, RenderFrameEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Streams_UpdateStatus(object sender, UpdateStatusEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            btn_Start.Enabled = false;
            btn_Stop.Enabled = true;
            //streams.StreamProfileSet = ;
            //streams.DeviceInfo = ;
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

        /*
        private RS.StreamProfileSet GetStreamSetConfiguration()
        {
            RS.StreamProfileSet profiles = new RS.StreamProfileSet();
            for (int s = 0; s < RS.Capture.STREAM_LIMIT; s++)
            {
                RS.StreamType st = RS.Capture.StreamTypeFromIndex(s);
                profiles[st] = GetStreamConfiguration(st);
            }
            return profiles;
        }

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
                return (RS.StreamProfile)m.SelectedItem;
            }
            
            return new RS.StreamProfile();
        }
        */
    }
}
