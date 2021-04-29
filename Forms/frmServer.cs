using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace BMS
{
	public partial class frmServer : Form
	{
		private FxSerialDeamon _FxSerial;
		string _cmd;
		//public SendData _SendData;
		List<Socket> clientSockets = new List<Socket>();


		Socket serverSocket;
		int BUFFER_SIZE = 1024;
		bool _isStart = false;
		byte[] buffer;
		public frmServer()
		{
			InitializeComponent();
		}
		private void frmServer_Load(object sender, EventArgs e)
		{
			btnSend.Enabled = false;
			buffer = new byte[BUFFER_SIZE];
			OpenPort(1);

			btnStart_Click(null, null);
		}
		public void OpenPort(int port)
		{
			if (_FxSerial == null)
			{
				_FxSerial = new FxSerialDeamon();
				_FxSerial.Start(port);
			}
		}

		/// <summary>
		/// nhận data từ máy khác truyền tới
		/// </summary>
		/// <param name="AR"></param>
		private void ReceiveCallback(IAsyncResult AR)
		{
			Socket current = (Socket)AR.AsyncState;
			int received = 0;

			try
			{
				received = current.EndReceive(AR);

				byte[] recBuf = new byte[received];
				Array.Copy(buffer, recBuf, received);
				string text = Encoding.ASCII.GetString(recBuf);

				if (string.IsNullOrEmpty(text)) return;
				if (!text.Contains(";")) return;

				string[] arr = text.Split(';');
				if (arr.Length != 3) return;

				string step = arr[0];
				string value = arr[1];
				string type = arr[2];

			}
			catch (Exception)
			{

			}
			try
			{
				current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
			}
			catch (Exception)
			{
				//return;
			}
		}
		private void AcceptCallback(IAsyncResult AR)
		{
			Socket socket;
			if (serverSocket == null)
			{
				return;
			}
			try
			{
				socket = serverSocket.EndAccept(AR);
				clientSockets.Add(socket);
			}
			catch (ObjectDisposedException)
			{
				return;
			}

			socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
			serverSocket.BeginAccept(AcceptCallback, null);
		}
		private int SetupServer()
		{
			try
			{
				serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				serverSocket.Bind(new IPEndPoint(IPAddress.Any, int.Parse(txtPort.Text.Trim())));
				serverSocket.Listen(0);
				serverSocket.BeginAccept(AcceptCallback, null);
				return 1;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return 0;
			}
		}

		/// <summary>
		/// Đóng server
		/// </summary>
		private void CloseSocketServer()
		{
			serverSocket.Close();
		}
		private void frmServer_FormClosed(object sender, FormClosedEventArgs e)
		{
			CloseSocketServer();
		}
		private void btnSend_Click(object sender, EventArgs e)
		{
			//byte[] data = Encoding.ASCII.GetBytes(txtSendValue.Text.Trim());
			////current.Send(data);
			//foreach (Socket socket in clientSockets)
			//{
			//    socket.Send(data);
			//}
			////serverSocket.Send(data);
		}

		void sendAll(byte[] data)
		{
			foreach (Socket socket in clientSockets)
			{
				try
				{
					socket.Send(data);
				}
				catch (Exception)
				{
				}
			}
		}
		/// <summary>
		/// Gửi lại tín hiệu cho các máy đã kết nối máy nào ngắt sẽ tự xóa 
		/// </summary>
		/// <param name="text">Cái gì muốn ghi</param>
		public void SendAll(string text)
		{
			try
			{
				byte[] data = Encoding.ASCII.GetBytes(text.Trim());
				foreach (Socket socket in clientSockets)
				{
					try
					{
						if (socket.Poll(1000, SelectMode.SelectRead))
						{
							clientSockets.Remove(socket);
							continue;
						}
						socket.Send(data);
					}
					catch
					{

					}
				}
				//serverSocket.Send(data);
			}
			catch (Exception ex)
			{

			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (!_isStart)
			{
				if (SetupServer() == 0) return;
				btnStart.Text = "Stop";
				btnStart.BackColor = Color.Red;
				btnSend.Enabled = true;
				_isStart = true;
			}
			else
			{
				CloseSocketServer();
				serverSocket = null;
				listBox1.Items.Clear();
				btnStart.Text = "Start";
				btnStart.BackColor = Color.Green;
				btnSend.Enabled = false;
				_isStart = false;
			}
		}
	}
}
