using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HDRControl
{
    public partial class frmCountDown : Form
    {
        private int mDelaySeconds;
        private int DelaySeconds
        {
            get
            {
                return mDelaySeconds;
            }
            set
            {
                //SET THE NEW VALUE AND DISPLAY IT TO THE USER
                mDelaySeconds = value;
                lblTimeRemaining.Text = TimeSpan.FromSeconds(mDelaySeconds).ToString();
            }
        }

        public frmCountDown(int Delay)
        {
            InitializeComponent();

            //INIT THE TIMER DISPLAY
            this.DelaySeconds = Delay;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //STORE THE CANCEL VALUE AND CLOSE THIS WINDOW
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void frmCountDown_Shown(object sender, EventArgs e)
        {
            //START THE COUNT DOWN TIMER
            tmrCount.Enabled = true;
        }

        private void tmrCount_Tick(object sender, EventArgs e)
        {
            //DECREASE THE DELAY BY 1 SECOND
            this.DelaySeconds--;

            //IF THERE IS STILL TIME TO COUNT DISPLAY IT OTHERWISE RETURN TO CALLING WINDOW
            if (this.DelaySeconds < 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

    }
}
