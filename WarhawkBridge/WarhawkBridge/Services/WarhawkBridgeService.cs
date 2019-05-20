using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Globalization;
using WarhawkBridge.Models;

namespace WarhawkBridge.Services
{
    class WarhawkBridgeService
    {
		private const int PORT_NAME = 10029;
		private Socket _socket;
		private const int bufSize = 16 * 1024;
		private List<byte[]> _outFrames;

		private class State
		{
			public byte[] Buffer = new byte[bufSize];
			public EndPoint From;

			public State()
			{
			//	Buffer = new byte[bufSize];
				From = new IPEndPoint(IPAddress.Any, 0);
			}
		}

		public WarhawkBridgeService()
		{

		}

		public void ToggleServiceActive(bool setActive)
		{
			if (setActive)
			{
				StartService();
			} else
			{
				StopService();
			}
		}



		private void Receive()
		{ 
			State state = new State();
			_socket.BeginReceiveFrom(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, ref state.From, (ar) =>
			{
				State so = (State)ar.AsyncState;
				try
				{
					int bytes = _socket.EndReceiveFrom(ar, ref so.From);
					this.Receive();
					Console.WriteLine("RECV: {0}: {1} bytes", so.From.ToString(), bytes);
					this.HandleFrame(so.Buffer, so.From);
				} catch (Exception ex)
				{
					Console.WriteLine("Tried to receive data but the socket is closed!");
				}
			}, state);
		}

		private void StartService()
		{
			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
			_socket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), PORT_NAME));

			Receive();
		}

		private void StopService()
		{
			if (null != _socket)
			{
				_socket.Close();
			}
		}

		private void HandleFrame(byte[] buffer, EndPoint from)
		{
			if (buffer.Length < 4) return;
			lock (this)
			{
				if (_outFrames == null) return;
				Console.WriteLine("Sending {0} in response..", _outFrames.Count);
				foreach (var f in _outFrames)
				{
					_socket.SendTo(f, from);
				}
			}
		}

		public void SetServerList(Collection<ServerEntry> entries)
		{
			var m_outFrames = new List<byte[]>();
			foreach (var e in entries)
			{
				if (!e.IsOnline) continue;
				byte[] frame = FromHex(e.Response);
				if (frame == null) continue;
				byte[] bip = null;
				try
				{
					var ip = Dns.GetHostEntry(e.Hostname);
					foreach (var i in ip.AddressList)
					{
						if (i.AddressFamily != AddressFamily.InterNetwork) continue;
						bip = i.GetAddressBytes();
						break;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Failed to resolve host " + e.Hostname);
				}
				if (bip == null) continue;
				bip.CopyTo(frame, 112);
				bip.CopyTo(frame, 176);
				m_outFrames.Add(frame);
			}
			lock (this)
			{
				_outFrames = m_outFrames;
			}
		}

		private byte[] FromHex(string response)
		{
			if (response.Length % 2 != 0) return null;
			byte[] res = new byte[response.Length / 2];
			for (int i = 0; i < response.Length / 2; i++)
			{
				res[i] = byte.Parse(response.Substring(i * 2, 2), NumberStyles.HexNumber);
			}
			return res;
		}
	}
}
