using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace AseitaRobotGUI
{
    public partial class Form1 : Form
    {
        SerialPort SerialCom;
        string recep = string.Empty; //string para recepção serial
        string trata_recep = string.Empty; //string para tratamento de recepção serial

        string frente = "frnt", atras = "tras", rotacaoDireita = "rotd", rotacaoEsquerda = "rote";
        int dutyMotor1, dutyMotor2;


        public delegate void delegateF(string a);

        public Form1()
        {
            InitializeComponent();
            this.SerialCom = new SerialPort();
            this.dutyMotor1 = 50; 
            this.dutyMotor1 = 50;
            this.SerialCom.DataReceived += new SerialDataReceivedEventHandler(SerialCom_DataReceived);
        }

        void SerialCom_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            recep = SerialCom.ReadExisting();

            this.BeginInvoke(new delegateF(recebe_serial), new object[] { recep });
        }

        public void recebe_serial(string a) 
        {
            trata_recep += a;
            label1.Text = trata_recep;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string str in SerialPort.GetPortNames())
                this.comboBox1.Items.Add(str);

            try
            {
                this.comboBox1.SelectedIndex = 0;
            }
            catch { }
            this.SerialCom.BaudRate = 9600;
            this. SerialCom.StopBits = StopBits.One;
            this.SerialCom.DataBits = 8;
            this.SerialCom.Parity = Parity.None;
        }

        
        private void button8_Click(object sender, EventArgs e)
        {
            this.comboBox1.Items.Clear();
            foreach (string str in SerialPort.GetPortNames())
                this.comboBox1.Items.Add(str);

            try
            {
                this.comboBox1.SelectedIndex = 0;
            }
            catch { }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //abrir porta
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Close();
            }
            else 
            {
                this.SerialCom.PortName = comboBox1.Text;
                this.SerialCom.Open();
                this.button6.Enabled = false;
                this.button7.Enabled = true;
            }
            

        }

        private void button7_Click(object sender, EventArgs e)
        {
            //fechar porta
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Close();
                this.button6.Enabled = true;
                this.button7.Enabled = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //frente
            if (this.SerialCom.IsOpen) {
                this.SerialCom.Write(frente + Convert.ToChar((int)numericUpDown1.Value) + Convert.ToChar((int)numericUpDown2.Value));
            }
        }

        
        private void button2_Click(object sender, EventArgs e)
        {
            //atrás
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(atras + Convert.ToChar((int)numericUpDown1.Value) + Convert.ToChar((int)numericUpDown2.Value));
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //rotação 1
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(rotacaoDireita + Convert.ToChar((int)numericUpDown1.Value) + Convert.ToChar((int)numericUpDown2.Value));
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //rotação 2
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write(rotacaoEsquerda + Convert.ToChar((int)numericUpDown1.Value) + Convert.ToChar((int)numericUpDown2.Value));
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //parar
            if (this.SerialCom.IsOpen)
            {
                this.SerialCom.Write("stop" + Convert.ToChar((int)numericUpDown1.Value) + Convert.ToChar((int)numericUpDown2.Value));
            }
        }


    }
}
