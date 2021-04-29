using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BMS
{
	public partial class frmLine : Form
	{
		string _socketIPAddress = "";
		Socket _socket;
		int _socketPort = 3000;
		ASCIIEncoding _encoding = new ASCIIEncoding();
		public frmLine()
		{
			InitializeComponent();
		}

		private void frmLine_Load(object sender, EventArgs e)
		{
			//kết nối với server qua tcp ip
			ConnectAnDon();
		}
		void ConnectAnDon()
		{
			try
			{
				if (_socket != null && _socket.Connected)
				{

				}
				else
				{
					//Địa chỉ IP của máy server
					_socketIPAddress = "192.168.1.120";
					//Địa chỉ Port cài trên server
					_socketPort = 3000;
					IPAddress ipAddOut = IPAddress.Parse(_socketIPAddress);
					IPEndPoint endPoint = new IPEndPoint(ipAddOut, _socketPort);
					_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					_socket.Connect(endPoint);
				}
			}
			catch (Exception ex)
			{
				//MessageBox.Show("Can't connect to Andon!");
				_socket = null;
			}
		}

		/// <summary>
		/// Gửi data lên server
		/// </summary>
		/// <param name="value">Giá trị, trạng thái</param>
		/// <param name="type">1:sự cố, 2: đã hoàn thành, 3: cập nhật SL thực tế, 4: khởi động ca</param>
		void sendDataTCP(string value, string type)
		{
			try
			{
				//Gửi tín hiệu delay xuống server qua TCP/IP
				if (_socket != null && _socket.Connected)
				{
					this.Invoke((MethodInvoker)delegate
					{
						string sendData;
						sendData = string.Format("{0};{1};{2}", "Data", value, type);
						byte[] data = _encoding.GetBytes(sendData);
						_socket.Send(data);

					});
				}
			}
			catch (Exception ex)
			{
				//Ghi log vào 
				_socket = null;
				MessageBox.Show(ex.ToString() + Environment.NewLine);
			}
		}
	}
}
