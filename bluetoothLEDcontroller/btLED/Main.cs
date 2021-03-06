﻿using System.Windows.Forms;
using System.IO.Ports;
using System;

namespace btLED
{
    public partial class frmMain : Form
    {
        enum ledColor : int { Red, Blue, Green };
        private int lastTick;

        public frmMain()
        {
            InitializeComponent();
            UpdateCOMportList();

            /* Set labels to zero */
            lblPwmBlue.Text = "0";
            lblPwmRed.Text = "0";
            lblPwmGreen.Text = "0";

            lastTick = Environment.TickCount;

            OpenCOM();

        }

        void OpenCOM()
        {
            if (!btSerialPort.IsOpen)
            {
                try
                {
                    btSerialPort.Open();
                    textBoxSerialData.AppendText(
                        "COM-port öppnad!\r\n");
                }

                catch (Exception)
                {
                    textBoxSerialData.AppendText(
                        "Kan inte öppna COM-port!\r\n");
                }

            }
        }

        /* Update drop-down list with available COM-ports */
        void UpdateCOMportList()
        {
            string[] ports = SerialPort.GetPortNames();

            comboBoxPortList.Items.Clear();
            comboBoxPortList.Items.AddRange(ports);

        }

        /* Data recieved from COM-port */
        private string data;
        private void btSerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            data = btSerialPort.ReadExisting();
            this.Invoke(new EventHandler(DisplayText));
        }

        /* Display serial data and misc */
        private void DisplayText(object o, EventArgs e)
        {
            /* Display data in textbox */
            textBoxSerialData.AppendText(data);
        }

        /* Event for port select in drop-down */
        private void comboBoxPortList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string port = comboBoxPortList.Text;

            btSerialPort.Close();

            btSerialPort.PortName = port == "" ? btSerialPort.PortName : port;

            textBoxSerialData.AppendText("Port satt till " + port + "\r\n");
            OpenCOM();
            
        }

        /* Event for refrsh-button */
        private void btnRefreshPortList_Click(object sender, EventArgs e)
        {
            UpdateCOMportList();
        }

        private void scrollPwmRed_Scroll(object sender, ScrollEventArgs e)
        {
            lblPwmRed.Text = scrollPwmRed.Value.ToString();
            setLedPWM(ledColor.Red, scrollPwmRed.Value);
        }

        private void scrollPwmGreen_Scroll(object sender, ScrollEventArgs e)
        {
            lblPwmGreen.Text = scrollPwmGreen.Value.ToString();
            setLedPWM(ledColor.Green, scrollPwmGreen.Value);
        }

        private void scrollPwmBlue_Scroll(object sender, ScrollEventArgs e)
        {
            lblPwmBlue.Text = scrollPwmBlue.Value.ToString();
            setLedPWM(ledColor.Blue, scrollPwmBlue.Value);
        }

        /* Set GRB LED PWM with Serial communication */
        private void setLedPWM(ledColor color, int pwm)
        {
            string commandColor = "";

            switch (color)
            {
                case ledColor.Red:
                    commandColor = "R";
                    break;

                case ledColor.Blue:
                    commandColor = "B";
                    break;

                case ledColor.Green:
                    commandColor = "G";
                    break;

                default:
                    break;
            }

            /* Limit PWM to 255 */
            pwm = pwm > 255 ? 255 : pwm;

            if (Environment.TickCount -lastTick > 30)
            {
                btSerialPort.Write(commandColor + pwm + "X");
                lastTick = Environment.TickCount;
            }
            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            btSerialPort.Write("NX");

            /* Set scroll sliders to zero */
            scrollPwmBlue.Value = 0;
            scrollPwmRed.Value = 0;
            scrollPwmGreen.Value = 0;

            /* Set labels to zero */
            lblPwmBlue.Text = "0";
            lblPwmRed.Text = "0";
            lblPwmGreen.Text = "0";
        }

        /* Status button */
        private void btnStatus_Click(object sender, EventArgs e)
        {
            btSerialPort.Write("SX");
        }

    }

}
