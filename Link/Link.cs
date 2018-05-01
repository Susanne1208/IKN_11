using System;
using System.IO.Ports;
using System.Text;
using Library;


/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const byte DELIMITER = (byte)'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttySn0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			// TO DO Your own code
			string xx = Encoding.ASCII.GetBytes(buf);
			StringBuilder sb = new StringBuilder (xx);

			sb.Append (DELIMITER);

			for (int i = 0; i < size; i++) 
			{
				switch (buf [i]) 
				{
				case DELIMITER:
					sb.Append ("BC");
					break;
				case (byte)'B':
					sb.Append ("BD");
					break;
				default:
					sb.Append(buf[i]);
					break;
				}
			}

			sb.Append (DELIMITER);


			string SendData = sb.ToString ();
			serialPort.Write (SendData);
		}

		/// <summary>
		/// Receive the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public int receive (ref byte[] buf)
		{
	    	// TO DO Your own code
			var i = 0;

			//Does not read bytes unless it begins with delimiter - 'A'
			while (true) {
				if (serialPort.ReadByte() == DELIMITER)
					break;
			}

			var tempByte = (byte)serialPort.ReadByte();

			while (tempByte != DELIMITER)
			{
				if (tempByte == (byte)'B')
				{
					var newByte = serialPort.ReadByte();

					switch (newByte)
					{
					case (byte)'C':
						buf[i++] = (byte)'A';
						break;
					case (byte)'D':
						buf[i++] = (byte)'B';
						break;
					default:
						return 0;
					}
				}
				else
					buf[i++] = tempByte;

				tempByte = (byte)serialPort.ReadByte();
			}
			return i;
		}

	}
}
