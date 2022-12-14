using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
//using Basler.Pylon;

namespace RS485Temperature
{
    public abstract class LongLifeWorker : IDisposable
    {
        protected Task worker;

        /// <summary>
        /// Sampling interval
        /// </summary>
        public int Interval { get; set; } = 1000;

        /// <summary>
        /// If worker paused flag
        /// </summary>
        public bool Paused { get; set; } = false;

        /// <summary>
        /// If worker completed flag
        /// </summary>
        public bool Completed => worker != null && worker.IsCompleted;

        /// <summary>
        /// Cancel worker token
        /// </summary>
        protected CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();

        public virtual void Initialize()
        {
            CancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Worker Start
        /// </summary>
        public virtual void Start()
        {
            if (worker == null)
            {
                worker = Task.Factory.StartNew(() =>
                {
                    while (!CancellationTokenSource.IsCancellationRequested)
                    {
                        SpinWait.SpinUntil(() => !Paused || CancellationTokenSource.IsCancellationRequested);
                        if (CancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }

                        DoWork();
                        _ = SpinWait.SpinUntil(() => CancellationTokenSource.IsCancellationRequested, Interval);
                    }
                }, TaskCreationOptions.LongRunning);
            }
            else
            {
                throw new Exception("Worker is running");
            }
        }

        /// <summary>
        /// Worker end
        /// </summary>
        public virtual void End()
        {
            if (!CancellationTokenSource.IsCancellationRequested)
            {
                CancellationTokenSource.Cancel();
            }

            if (worker != null)
            {
                worker.Wait();
                worker = null;
            }
            Console.WriteLine("Worker end");
        }

        /// <summary>
        /// Worker resume
        /// </summary>
        public void Resume()
        {
            Paused = false;
        }

        /// <summary>
        /// Worker pause
        /// </summary>
        public void Pause()
        {
            Paused = true;
        }

        public virtual void DoWork()
        {
            try
            {
                throw new Exception("This method must be reimplemented");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public virtual void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class Thermometer : LongLifeWorker, INotifyPropertyChanged, IDisposable
    {
        private double _temperature;

        private SerialPort serialPort;

        /// <summary>
        /// Serial Port Opened
        /// </summary>
        public bool IsSerialPortOpen => serialPort != null && serialPort.IsOpen;

        /// <summary>
        /// 測得溫度
        /// </summary>
        public double Temperature
        {
            get => _temperature;
            set
            {
                if (value != _temperature)
                {
                    _temperature = value;
                    OnPropertyChanged(nameof(Temperature));
                }
            }
        }

        public void OpenSerialPort(string com = null)
        {
            string[] portNames = com == null ? SerialPort.GetPortNames() : new string[] { com };
            //Console.WriteLine(string.Join(", ", portNames));
            if (portNames.Length > 0)
            {
                serialPort = new SerialPort(portNames[0], 9600, Parity.None, 8, StopBits.One);
                //serialPort.DataReceived += SerialPort_DataReceived;

                try
                {
                    serialPort.Open();

                    if (serialPort.IsOpen)
                    {
                        Start();
                        OnPropertyChanged(nameof(IsSerialPortOpen));
                    }
                }
                catch (Exception ex)
                {
                    // Display in message list
                    Console.WriteLine($"Exception occurred : {ex.Message}");
                }
            }
            else
            {
                //Display in message list
                Console.WriteLine("No serial port found");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Console.WriteLine(serialPort.ReadBufferSize);
        }

        public void CloseSerialPort()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
            OnPropertyChanged(nameof(IsSerialPortOpen));
        }

        public override void End()
        {
            CloseSerialPort();
            base.End();
        }

        /// <summary>
        /// 工作內容
        /// </summary>
        public override void DoWork()
        {
            try
            {
                // Stop number: 0x01, Function code: 0x03 (Read Holding Registers)
                // Temperature address: 0x2000, Length: 0x0001, CRC: 0x8FCA
                byte[] data_write = new byte[] { 0x01, 0x06, 0x10, 0x00, 0x00, 0x88, 0x8D, 0x6C };
                //byte[] data_write = new byte[] { 0x01, 0x03, 0x10, 0x00, 0x00, 0x01, 0x80, 0xCA };

                serialPort.Write(data_write, 0, data_write.Length);
                Debug.WriteLine($"write {string.Join(',', data_write)}");

#if true
                byte[] data_read = new byte[7];
                _ = serialPort.Read(data_read, 0, data_read.Length);

                Debug.WriteLine($"read {string.Join(',', data_read)}");

                // (-32768 ~ 32767) / 10
                if ((data_read[3] & 0b10000000) == 0b10000000) // if bit7 is 1 
                {
                    Temperature = ((data_read[3] << 8) + data_read[4] - 65536) / 10.0;
                }
                else
                {
                    Temperature = ((data_read[3] << 8) + data_read[4]) / 10.0;
                }
#endif

                //string[] array = Array.ConvertAll(data_read, b => b.ToString("X").PadLeft(2, '0'));
                //Console.WriteLine(string.Join(",", array));
            }
            catch (Exception ex)
            {
                // Display in message list
                Console.WriteLine($"Thermometer exception occurred: {ex.Message}");

                // 寫入失敗，關閉通訊埠，終止worker
                // 新建Task，否則死鎖
                Task.Run(End);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void Dispose()
        {
            serialPort.Dispose();
            //throw new NotImplementedException();
        }
    }
}
