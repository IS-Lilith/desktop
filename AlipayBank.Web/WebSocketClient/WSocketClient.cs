using SuperSocket.ClientEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebSocket4Net;

namespace WebSocketClient
{
	public class WSocketClient : IDisposable
	{
		private WebSocket _webSocket;

		private Thread _thread;

		private bool _isRunning = true;

		public string ServerPath
		{
			get;
			set;
		}

		public event Action<string> MessageReceived;

		public event Action<string> setLog;

		public WSocketClient(string url)
		{
			ServerPath = url;
			_webSocket = new WebSocket(url);
			_webSocket.Opened += WebSocket_Opened;
			_webSocket.Error += WebSocket_Error;
			_webSocket.Closed += WebSocket_Closed;
			_webSocket.MessageReceived += WebSocket_MessageReceived;
		}

		public bool Start()
		{
			bool result = true;
			try
			{
				_webSocket.Open();
				_isRunning = true;
				_thread = new Thread(CheckConnection);
				_thread.Start();
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		private void WebSocket_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			this.MessageReceived?.Invoke(e.Message);
		}

		private void WebSocket_Closed(object sender, EventArgs e)
		{
			this.MessageReceived?.Invoke("websocket_Closed");
		}

		private void WebSocket_Error(object sender, ErrorEventArgs e)
		{
			this.MessageReceived?.Invoke("websocket_Error:" + e.Exception.ToString());
		}

		private void WebSocket_Opened(object sender, EventArgs e)
		{
			this.MessageReceived?.Invoke("websocket_Opened");
		}

		private void CheckConnection()
		{
			do
			{
				try
				{
					if (_webSocket.State != WebSocketState.Open && _webSocket.State != 0)
					{
						_webSocket.Close();
						_webSocket.Open();
					}
				}
				catch (Exception)
				{
				}
				Thread.Sleep(1000);
			}
			while (_isRunning);
		}

		public void SendMessage(string Message)
		{
			Task.Factory.StartNew(delegate
			{
				if (_webSocket != null && _webSocket.State == WebSocketState.Open)
				{
					_webSocket.Send(Message);
				}
			});
		}

		public void Dispose()
		{
			_isRunning = false;
			try
			{
				_thread.Abort();
			}
			catch
			{
			}
			_webSocket.Close();
			_webSocket.Dispose();
			_webSocket = null;
		}
	}
}
