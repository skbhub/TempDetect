using Base;
using SDK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TempDetect
{
    public partial class ParamForm : Form
    {
        public ParamForm(OWBCamera camera)
        {
            InitializeComponent();
            OWBGlobal.Camera = camera;
            cbLens.Items.Clear();
            foreach (string lens in OWBGlobal.Camera.Lens)
            {
                cbLens.Items.Add(lens);
            }
            cbLens.SelectedIndex = OWBGlobal.Camera.LensIndex;

            cbTemperatureRange.Items.Clear();
            foreach (TemperatureRange tRange in OWBGlobal.Camera.TRanges)
            {
                cbTemperatureRange.Items.Add(tRange);
            }
            cbTemperatureRange.SelectedIndex = OWBGlobal.Camera.TRangeIndex;
        }

        private bool SetTempParameter()
        {
            OWBTypes.InstrumentJconfig instrumentJconfig = new OWBTypes.InstrumentJconfig();
            instrumentJconfig.Atmosphere_t = (float)nudAtmosphericTemperatureValue.Value;
            instrumentJconfig.Emission = (float)nudEmissivityValue.Value;
            instrumentJconfig.Distance = (float)nudDistanceValue.Value;
            instrumentJconfig.RH = (float)nudRelativeHumidityValue.Value;
            instrumentJconfig.Reflection_t = (float)nudReflectedTemperatureValue.Value;
            instrumentJconfig.Lens_t = (float)nudLensTValue.Value;
            instrumentJconfig.Lens_transmission = (float)nudLensTransValue.Value;
            instrumentJconfig.Offset = (float)nudOffsetValue.Value;
            return OWBGlobal.Camera.PutInstrumentJconfig(instrumentJconfig);

        }
        //反射率
        private void nudEmissivityValue_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudEmissivityValue.Value = (decimal)instrumentJconfig.Emission;
            }
        }
        //湿度
        private void nudRelativeHumidityValue_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudRelativeHumidityValue.Value = (decimal)instrumentJconfig.RH;
            }
        }
        //反射温度
        private void nudReflectedTemperatureValue_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudReflectedTemperatureValue.Value = (decimal)instrumentJconfig.Reflection_t;
            }
        }
        //环境温度
        private void nudAtmosphericTemperatureValue_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudAtmosphericTemperatureValue.Value = (decimal)instrumentJconfig.Atmosphere_t;
            }
        }
        //距离
        private void nudDistanceValue_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudDistanceValue.Value = (decimal)instrumentJconfig.Distance;
            }
        }
        //外部光学温度
        private void nudLensTValue_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudLensTValue.Value = (decimal)instrumentJconfig.Lens_t;
            }
        }
        //外部光学透射率
        private void nudLensTrans_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudLensTransValue.Value = (decimal)instrumentJconfig.Lens_transmission;
            }
        }
        //偏移
        private void nudOffset_ValueChanged(object sender, EventArgs e)
        {
            if (SetTempParameter())
            {
                OWBTypes.InstrumentJconfig instrumentJconfig = OWBGlobal.Camera.GetInstrumentJconfig();
                nudOffsetValue.Value = (decimal)instrumentJconfig.Offset;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            SetTempParameter();
            this.Close();
        }

        private void btnAutoFocus_Click(object sender, EventArgs e)
        {
            OWBGlobal.Camera.PostIspAF();
        }

        private void btnFarFocus_Click(object sender, EventArgs e)
        {
            OWBGlobal.Camera.PostFarFocus();
        }

        private void btnNearFocus_Click(object sender, EventArgs e)
        {
            OWBGlobal.Camera.PostNearFocus();
        }

        private void cbLens_SelectedIndexChanged(object sender, EventArgs e)
        {
            OWBGlobal.Camera.LensIndex = cbLens.SelectedIndex;
        }

        private void cbTemperatureRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            OWBGlobal.Camera.TRangeIndex = cbTemperatureRange.SelectedIndex;
        }
    }
}
