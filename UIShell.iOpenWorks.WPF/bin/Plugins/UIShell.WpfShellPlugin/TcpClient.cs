using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UIShell.WpfShellPlugin
{
    class TcpClient
    {
        public class IsaConstHeader
        {
            public const UInt16 EKEDBG_PORT = 4101;
            public const String ISAIPadress = "127.0.0.1";
            public const UInt16 PSTM_ETH_HDR_TAG = 0xAAAA;     /* Ethernet header tag */
            public const UInt16 PSTM_CMD_REPLY_MASK = 0x80;
            public const UInt32 VAR_DEBUG_MAX_RESP_DATA = 1024 * 64;
            public const UInt16 VAR_DEBUG_MAX_REQ_DATA = 1024 * 4;

            public const UInt16 REQ_READ_VAR = 1;
            public const UInt16 REQ_WRITE_VAR = 2;
            public const UInt16 REQ_LOCK_IO = 3;
            public const UInt16 REQ_UNLOCK_IO = 4;
            public const UInt16 REQ_OUTPUTPHY_WRITE_VAR = 5;
            public const UInt16 REQ_START_COUNTER = 6;
            public const UInt16 REQ_RETAIN_VARS = 7;
            public const UInt16 REQ_ADD_TO_DBG_LIST = 8;
            public const UInt16 REQ_REMOVE_FROM_DBG_LIST = 9;
            public const UInt16 REQ_CLEAR_DBG_LIST = 10;

            public const UInt16 MAX_VARNAME = 80;
            public const UInt16 MAX_VARNUM = 50;

            public const UInt16 ISA_TYPBOOL = 1;     /* Boolean */
            public const UInt16 ISA_TYPSINT = 2;     /* Sint */
            public const UInt16 ISA_TYPDINT = 3;     /* Double Integer */
            public const UInt16 ISA_TYPTIME = 4;     /* Time */
            public const UInt16 ISA_TYPREAL = 5;     /* Real */
            public const UInt16 ISA_TYPSTRING = 6;   /* String Message */
            public const UInt16 ISA_TYPBLOCK = 7;    /* Memory Block of data (structs) */
            public const UInt16 ISA_TYPUSINT = 8;    /* USINT */
            public const UInt16 ISA_TYPINT = 9;      /* INT */
            public const UInt16 ISA_TYPUINT = 10;    /* UINT */
            public const UInt16 ISA_TYPUDINT = 11;    /* UDINT */
            public const UInt16 ISA_TYPLINT = 12;    /* LINT */
            public const UInt16 ISA_TYPULINT = 13;   /* ULINT */
            public const UInt16 ISA_TYPDATE = 14;    /* DATE */
            public const UInt16 ISA_TYPLREAL = 15;   /* LREAL */

            public const UInt16 ISA_IXL_SYM_SIMPLE = 0;     /* Type simple */
            public const UInt16 ISA_IXL_SYM_ARRAY = 1;      /* Type array */
            public const UInt16 ISA_IXL_SYM_STRUCT = 2;     /* Type structure */
        }

        public class CBServerConstType
        {
            public const byte headerMsg = (byte)0;
            public const byte startingInformationsMsg = (byte)98;
            public const byte setSignatureMsg = (byte)78;
            public const byte getSignatureMsg = (byte)79;

        }

        public class CBInitData
        {
            public static Int32 MinBoolIndex = 0x7FFFFFFF;
            public static Int32 MaxBoolIndex = 0;
            public static Int32 MinIntIndex = 0x7FFFFFFF;
            public static Int32 MaxIntIndex = 0;
            public static Int32 MinFloatIndex = 0x7FFFFFFF;
            public static Int32 MaxFloatIndex = 0;
            public static Int32 MinCharIndex = 0x7FFFFFFF;
            public static Int32 MaxCharIndex = 0;
            public static Int32 pospidIndex = 0;
            public static Int32 BoolMemSize = 0;
            public static Int32 IntMemSize = 0;
            public static Int32 FloatMemSize = 0;
            public static Int32 CharMemSize = 0;
            public static Int32 signature = 0;
            public static Int32 SignatureNum = 0;

        }

        public class IsaEthHeader
        {
            public static UInt16 tag;
            public static UInt32 leagth;
            public static UInt16 crc;
        }

        public class IsaMsgDataHdr
        {
            public static UInt16 cmd_code;
            public static UInt32 packet_id;
        }

        public class IsaReadVarReq
        {
            public static Int32 var_amount;
            public static ArrayList VarNameList = new ArrayList();
        }

        public class IsaVarDef
        {
            public Int32 var_size; /*Variable size, if zero variable was not recognized*/
            public Int32 var_type; /*Type of varialble. */
            public Int32 var_def = 0; /*Variable definition, array,struct etc*/
            public Int32 var_offset; /*Offset to variable inside response frame*/
        }

        public class IsaReadVarRep
        {
            public UInt32 var_amount;
            public static Byte[] var_definitions;
            public static Byte[] data_area;

        }

        public class IsaWriteVarInfo
        {
            public String var_name;
            public char var_type;
            public UInt32 var_size;
            public Int64 var_offset;
        }

        public class IsaWriteVarReq
        {
            public static Int32 var_amount;
            public static ArrayList VarNameList = new ArrayList();
            public static Byte[] data_area;
        }

        public class IsaWriteVarRep
        {
            public UInt32 var_amount;
            public Byte[] Result;
        }

        public class ISASeverState
        {
            // Client  socket.

            public Socket workSocket = null;
            // Size of receive buffer.

            public const int BufferSize = 2048;
            // Receive buffer.

            public byte[] buffer = new byte[BufferSize];



            public byte[] send_buffer = new byte[BufferSize];


            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        public static class ISASeverSock
        {
            // Client  socket.

            public static Socket handler = null;

        }

        public class VarListVar
        {
            public string VarType;
            public Int32 VarIndex;
        }

        public class CBVarList
        {
            public string CoiPilePath;
            static Hashtable VarList = new Hashtable();
            public string VarType = "a";
            public Int32 VarIndex = 0;
            public static String TaskCoiPilePathName;

            public CBVarList()
            {
                CoiPilePath = TaskCoiPilePathName;
                StreamReader sr = new StreamReader(CoiPilePath);

                if (File.Exists(CoiPilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        string srString = sr.ReadLine();
                        if (srString.StartsWith("#") && srString.EndsWith("/"))
                        {
                            Console.WriteLine("{0}", srString);
                            int i;
                            for (i = 0; (srString = sr.ReadLine()) != "#"; i++)
                            {
                                Console.WriteLine("{0}", srString);
                                string[] split = srString.Split(new Char[] { ' ' });
                                Console.WriteLine("{0}", split[1]);
                                Console.WriteLine("{0}", split[2]);
                                VarListVar VarTypeIndex = new VarListVar();
                                VarTypeIndex.VarType = split[2].Substring(1, 1);
                                VarTypeIndex.VarIndex = Int32.Parse(split[2].Substring(2));
                                VarList.Add(split[1], VarTypeIndex);
                                Console.WriteLine("{0}", VarList.Count);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("未找到变量清单");
                    Console.ReadKey();
                }
            }


            public string ReturnCBVarType(string VarName)
            {

                VarType = ((VarListVar)VarList[VarName]).VarType;
                return VarType;
            }
            public Int32 ReturnCBVarIndex(string VarName)
            {
                VarIndex = ((VarListVar)VarList[VarName]).VarIndex;
                return VarIndex;
            }

        }

        public class CBMsg
        {
            public static Int32 address;
            public static byte type;
            public static Int32 value;
        }

        public class CRC
        {
            public static ushort[] CRC16Table = 
        {  
	        0x0000, 0xc0c1, 0xc181, 0x0140, 0xc301, 0x03c0, 0x0280, 0xc241, 
	        0xc601, 0x06c0, 0x0780, 0xc741, 0x0500, 0xc5c1, 0xc481, 0x0440, 
	        0xcc01, 0x0cc0, 0x0d80, 0xcd41, 0x0f00, 0xcfc1, 0xce81, 0x0e40,
	        0x0a00, 0xcac1, 0xcb81, 0x0b40, 0xc901, 0x09c0, 0x0880, 0xc841, 
	        0xd801, 0x18c0, 0x1980, 0xd941, 0x1b00, 0xdbc1, 0xda81, 0x1a40, 
	        0x1e00, 0xdec1, 0xdf81, 0x1f40, 0xdd01, 0x1dc0, 0x1c80, 0xdc41, 
	        0x1400, 0xd4c1, 0xd581, 0x1540, 0xd701, 0x17c0, 0x1680, 0xd641, 
	        0xd201, 0x12c0, 0x1380, 0xd341, 0x1100, 0xd1c1, 0xd081, 0x1040, 
	        0xf001, 0x30c0, 0x3180, 0xf141, 0x3300, 0xf3c1, 0xf281, 0x3240, 
	        0x3600, 0xf6c1, 0xf781, 0x3740, 0xf501, 0x35c0, 0x3480, 0xf441, 
	        0x3c00, 0xfcc1, 0xfd81, 0x3d40, 0xff01, 0x3fc0, 0x3e80, 0xfe41, 
	        0xfa01, 0x3ac0, 0x3b80, 0xfb41, 0x3900, 0xf9c1, 0xf881, 0x3840, 
	        0x2800, 0xe8c1, 0xe981, 0x2940, 0xeb01, 0x2bc0, 0x2a80, 0xea41, 
	        0xee01, 0x2ec0, 0x2f80, 0xef41, 0x2d00, 0xedc1, 0xec81, 0x2c40, 
	        0xe401, 0x24c0, 0x2580, 0xe541, 0x2700, 0xe7c1, 0xe681, 0x2640, 
	        0x2200, 0xe2c1, 0xe381, 0x2340, 0xe101, 0x21c0, 0x2080, 0xe041, 
	        0xa001, 0x60c0, 0x6180, 0xa141, 0x6300, 0xa3c1, 0xa281, 0x6240, 
	        0x6600, 0xa6c1, 0xa781, 0x6740, 0xa501, 0x65c0, 0x6480, 0xa441, 
	        0x6c00, 0xacc1, 0xad81, 0x6d40, 0xaf01, 0x6fc0, 0x6e80, 0xae41, 
	        0xaa01, 0x6ac0, 0x6b80, 0xab41, 0x6900, 0xa9c1, 0xa881, 0x6840, 
	        0x7800, 0xb8c1, 0xb981, 0x7940, 0xbb01, 0x7bc0, 0x7a80, 0xba41, 
	        0xbe01, 0x7ec0, 0x7f80, 0xbf41, 0x7d00, 0xbdc1, 0xbc81, 0x7c40, 
	        0xb401, 0x74c0, 0x7580, 0xb541, 0x7700, 0xb7c1, 0xb681, 0x7640, 
	        0x7200, 0xb2c1, 0xb381, 0x7340, 0xb101, 0x71c0, 0x7080, 0xb041, 
	        0x5000, 0x90c1, 0x9181, 0x5140, 0x9301, 0x53c0, 0x5280, 0x9241, 
	        0x9601, 0x56c0, 0x5780, 0x9741, 0x5500, 0x95c1, 0x9481, 0x5440, 
	        0x9c01, 0x5cc0, 0x5d80, 0x9d41, 0x5f00, 0x9fc1, 0x9e81, 0x5e40, 
	        0x5a00, 0x9ac1, 0x9b81, 0x5b40, 0x9901, 0x59c0, 0x5880, 0x9841, 
	        0x8801, 0x48c0, 0x4980, 0x8941, 0x4b00, 0x8bc1, 0x8a81, 0x4a40, 
	        0x4e00, 0x8ec1, 0x8f81, 0x4f40, 0x8d01, 0x4dc0, 0x4c80, 0x8c41, 
	        0x4400, 0x84c1, 0x8581, 0x4540, 0x8701, 0x47c0, 0x4680, 0x8641, 
	        0x8201, 0x42c0, 0x4380, 0x8341, 0x4100, 0x81c1, 0x8081, 0x4040
        };

            public static ushort CRC16(Byte[] dat, int count)
            {
                ushort crc = 0xFFFF;

                for (int i = 0; i < count; i++)
                    crc = (ushort)(CRC16Table[((crc) ^ (dat[i])) & 0xff] ^ (((crc) >> 8) & 0xff));
                //crc = (ushort)(crc % 0x100 * 0x100 + crc / 0x100);

                return crc;
            }
        }

        public class CBClientState
        {
            public Socket workSocket = null;
            // Size of receive buffer.

            public const int BufferSize = 1460;
            // Receive buffer.

            public byte[] buffer = new byte[BufferSize];
            // Received data string.

            public StringBuilder sb = new StringBuilder();
        }

        public static class CBClientSock
        {
            public static Socket workSocket = null;

        }

        public class MyEventArgs : EventArgs
        {
            public byte[] buffer;

            public MyEventArgs(byte[] b)
            {
                buffer = b;
            }
        }

        public class ISAServer
        {
            public static IPEndPoint ISAEndPoint = new IPEndPoint(IPAddress.Parse(IsaConstHeader.ISAIPadress), IsaConstHeader.EKEDBG_PORT);

            public byte[] receive_bytes;

            public int received_bytes = 0;

            public event EventHandler<MyEventArgs> CmdMessage;

            public static ManualResetEvent allDone = new ManualResetEvent(false);

            public void StartListening()
            {
                // Data buffer for incoming data.

                byte[] bytes = new Byte[1024];

                // Create a TCP/IP socket.

                Socket listener = new Socket(AddressFamily.InterNetwork,
                    SocketType.Stream, ProtocolType.Tcp);

                // Bind the socket to the local endpoint and listen for incoming connections.

                try
                {
                    listener.Bind(ISAEndPoint);
                    listener.Listen(10);

                    while (true)
                    {
                        // Set the event to nonsignaled state.

                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.

                        Console.WriteLine("Waiting for a connection...");
                        listener.BeginAccept(
                            new AsyncCallback(this.AcceptCallback),
                            listener);

                        // Wait until a connection is made before continuing.

                        allDone.WaitOne();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                Console.WriteLine("\nPress ENTER to continue...");
                Console.Read();

            }

            public void AcceptCallback(IAsyncResult ar)
            {
                // Signal the main thread to continue.

                allDone.Set();

                // Get the socket that handles the client request.

                Socket listener = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                // Create the state object.

                ISASeverState state = new ISASeverState();
                state.workSocket = handler;
                receive_bytes = new byte[2048 * 4];
                handler.BeginReceive(state.buffer, 0, ISASeverState.BufferSize, 0,
                        new AsyncCallback(this.ReadCallback), state);

            }

            public void ReadCallback(IAsyncResult ar)
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket

                // from the asynchronous state object.

                ISASeverState state = (ISASeverState)ar.AsyncState;

                ISASeverSock.handler = state.workSocket;

                // Read data from the client socket. 

                int bytesRead = ISASeverSock.handler.EndReceive(ar);
                Buffer.BlockCopy(state.buffer, 0, receive_bytes, received_bytes, bytesRead);
                received_bytes = received_bytes + bytesRead;
                int MsgLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receive_bytes, 2));

                if (bytesRead > 0 && MsgLength + 8 == bytesRead)
                {

                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = state.sb.ToString();

                    Console.WriteLine("Read {0} {1}bytes from socket. \n Data : ",
                                        MsgLength, bytesRead);

                    for (int i = 0; i < bytesRead; i++)
                    {
                        Console.Write("{0} ", receive_bytes[i].ToString("x2"));
                    }
                    if (CmdMessage != null)
                    {
                        MyEventArgs mtcea = new MyEventArgs(receive_bytes);
                        CmdMessage(this, mtcea);
                    }
                    received_bytes = 0;
                    ISASeverSock.handler.BeginReceive(state.buffer, 0, ISASeverState.BufferSize, 0,
                        new AsyncCallback(ReadCallback), state);
                }
                else
                {
                    // Not all data received. Get more.

                    ISASeverSock.handler.BeginReceive(state.buffer, 0, ISASeverState.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);


                }
            }

            public static void Send(Socket handler, byte[] byteData)
            {


                byte[] IsaEthernetHeader = new byte[8];
                Array.Copy(byteData, 0, IsaEthernetHeader, 0, 8);
                // Begin sending the EthernetHeaderMsg to the remote device.
                handler.BeginSend(IsaEthernetHeader, 0, IsaEthernetHeader.Length, 0,
                    new AsyncCallback(SendCallback), handler);

                byte[] IsaMessage = new byte[byteData.Length - 8];
                Array.Copy(byteData, 8, IsaMessage, 0, byteData.Length - 8);
                // Begin sending the other data to the remote device.
                handler.BeginSend(IsaMessage, 0, IsaMessage.Length, 0,
                    new AsyncCallback(SendCallback), handler);
                for (int i = 0; i < byteData.Length; i++)
                {
                    Console.Write("{0} ", byteData[i].ToString("x2"));
                }
            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.

                    Socket handler = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.

                    int bytesSent = handler.EndSend(ar);
                    //Console.WriteLine("Sent {0} bytes to client.", bytesSent);

                    //if (bytesSent != 8)
                    //{
                    //   handler.Shutdown(SocketShutdown.Both);
                    //   handler.Close();
                    //}
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public class CBClient
        {
            public String CBSeverIPAdress;
            public UInt16 CBSeverPORT;
            public static Socket CBClientSocket;

            public static byte[] receive_bytes;
            public static int received_bytes = 0;

            // ManualResetEvent instances signal completion.

            private static ManualResetEvent connectDone =
                new ManualResetEvent(false);
            private static ManualResetEvent sendDone =
                new ManualResetEvent(false);
            private static ManualResetEvent receiveDone =
                new ManualResetEvent(false);

            // The response from the remote device.


            public CBClient(String IPAdress, UInt16 PORT)
            {
                CBSeverIPAdress = IPAdress;
                CBSeverPORT = PORT;
            }

            public void StartCBClient()
            {
                // Connect to a remote device.

                try
                {
                    // Establish the remote endpoint for the socket.

                    // The name of the 

                    // remote device is "host.contoso.com".
                    IPEndPoint CBEndPoint = new IPEndPoint(IPAddress.Parse(CBSeverIPAdress), CBSeverPORT);

                    // Create a TCP/IP socket.
                    CBClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    // Connect to the remote endpoint.

                    CBClientSocket.BeginConnect(CBEndPoint, new AsyncCallback(ConnectCallback), CBClientSocket);
                    connectDone.WaitOne();

                    CBClientSock.workSocket = CBClientSocket;

                    // Send test data to the remote device.

                    //Send(CBClent, CBCmd);
                    //sendDone.WaitOne();

                    // Receive the response from the remote device.

                    //Receive(CBClent);
                    //receiveDone.WaitOne();

                    // Write the response to the console.

                    //CBRespond = response;

                    // Release the socket.

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            public void EndCbClient()
            {
                CBClientSocket.Shutdown(SocketShutdown.Both);
                CBClientSocket.Close();
            }

            private static void ConnectCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.

                    Socket client = (Socket)ar.AsyncState;

                    // Complete the connection.

                    client.EndConnect(ar);

                    Console.WriteLine("Socket connected to {0}",
                        client.RemoteEndPoint.ToString());

                    // Signal that the connection has been made.

                    connectDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            public static void Receive(byte[] byteData)
            {
                try
                {

                    received_bytes = 0;
                    receive_bytes = new byte[2048 * 4];
                    // Create the state object.
                    CBClientState state = new CBClientState();
                    state.workSocket = CBClientSocket;

                    // Begin receiving the data from the remote device.

                    //CBClientSock.workSocket.BeginReceive(state.buffer, 0, CBClientState.BufferSize, 0,
                    //                       new AsyncCallback(ReceiveCallback), state);
                    //receiveDone.WaitOne(500);
                    int ReceiveMsgSize = 12;
                    CBClientSock.workSocket.Receive(state.buffer, 0, ReceiveMsgSize, SocketFlags.None);
                    Buffer.BlockCopy(state.buffer, 0, receive_bytes, 0, ReceiveMsgSize);
                    received_bytes = received_bytes + ReceiveMsgSize;
                    int CbMsgSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receive_bytes, 8)) * 12 + 12;
                    for (int i = 0; CbMsgSize - received_bytes > 0; i++)
                    {
                        if (CbMsgSize - received_bytes > CBClientState.BufferSize)
                        {
                            CBClientSock.workSocket.Receive(state.buffer, 0, CBClientState.BufferSize, SocketFlags.None);
                            Buffer.BlockCopy(state.buffer, 0, receive_bytes, received_bytes, CBClientState.BufferSize);
                            received_bytes = received_bytes + CBClientState.BufferSize;
                        }
                        else
                        {
                            CBClientSock.workSocket.Receive(state.buffer, 0, CbMsgSize - received_bytes, SocketFlags.None);
                            Buffer.BlockCopy(state.buffer, 0, receive_bytes, received_bytes, CbMsgSize - received_bytes);
                            received_bytes = received_bytes + CbMsgSize - received_bytes;
                        }
                    }
                    Buffer.BlockCopy(receive_bytes, 0, byteData, 0, byteData.Length);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            private static void ReceiveCallback(IAsyncResult ar)
            {

                String content = String.Empty;

                // Retrieve the state object and the handler socket

                // from the asynchronous state object.

                CBClientState state = (CBClientState)ar.AsyncState;

                Socket client = state.workSocket;

                // Read data from the client socket. 

                int bytesRead = client.EndReceive(ar);
                Buffer.BlockCopy(state.buffer, 0, receive_bytes, received_bytes, bytesRead);
                received_bytes = received_bytes + bytesRead;
                int HeaderMsgDefinedLength = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receive_bytes, 8));

                if (bytesRead > 0 && HeaderMsgDefinedLength * 12 + 12 == received_bytes)
                {

                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    content = state.sb.ToString();

                    Console.WriteLine("Read {0} {1}bytes from socket. \n Data : ",
                                        received_bytes, bytesRead);

                    for (int i = 0; i < received_bytes; i++)
                    {
                        Console.Write("{0} ", receive_bytes[i].ToString("x2"));
                    }

                }
                else
                {
                    // Not all data received. Get more.
                    client.BeginReceive(state.buffer, 0, CBClientState.BufferSize, 0,
                            new AsyncCallback(ReceiveCallback), state);
                }
            }

            public static void Send(byte[] byteData)
            {
                // Convert the string data to byte data using ASCII encoding.

                // byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.


                for (int i = 0; i < 12; i++)
                {
                    Console.Write("{0} ", byteData[i].ToString("x2"));
                }
                Console.Write("\n ");

                CBClientSock.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                  new AsyncCallback(SendCallback), CBClientSock.workSocket);

            }

            public static void SendWithHeaderMsg(byte[] byteData)
            {
                // Convert the string data to byte data using ASCII encoding.

                // byte[] byteData = Encoding.ASCII.GetBytes(data);

                // Begin sending the data to the remote device.

                Byte[] HeaderMsg = new Byte[12];

                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0)), 0, HeaderMsg, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(byteData.Length / 12)), 0, HeaderMsg, 8, 4);
                HeaderMsg[4] = CBServerConstType.headerMsg;

                Console.Write("HeaderMsg\n");
                for (int i = 0; i < 12; i++)
                {
                    Console.Write("{0} ", HeaderMsg[i].ToString("x2"));
                }
                Console.Write("\n ");

                CBClientSock.workSocket.BeginSend(HeaderMsg, 0, HeaderMsg.Length, 0,
                  new AsyncCallback(SendCallback), CBClientSock.workSocket);

                for (int i = 0; i < byteData.Length; i++)
                {
                    Console.Write("{0} ", byteData[i].ToString("x2"));
                }
                Console.Write("\n ");

                CBClientSock.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                  new AsyncCallback(SendCallback), CBClientSock.workSocket);

            }

            private static void SendCallback(IAsyncResult ar)
            {
                try
                {
                    // Retrieve the socket from the state object.

                    Socket client = (Socket)ar.AsyncState;

                    // Complete sending the data to the remote device.

                    int bytesSent = client.EndSend(ar);
                    Console.WriteLine("Sent {0} bytes to server. \n", bytesSent);

                    // Signal that all bytes have been sent.

                    sendDone.Set();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

        }

        public class ISADebugCmdCnv
        {
            public CBVarList CBVarHandle = new CBVarList();
            public Byte[] respond1 = new Byte[2048 * 4];
            public String[] var_name_array = new String[IsaConstHeader.MAX_VARNUM];

            public void ReadVariables(byte[] buffer)
            {
                Int32 var_offset_ret = 0;
                int BoolNum = 0;
                int IntNum = 0;
                int FloatNum = 0;
                int CharNum = 0;
                byte[] CBSendbuffer = new byte[IsaReadVarReq.var_amount * 12];

                IsaVarDef def = new IsaVarDef();
                def.var_size = 4;

                for (int i = 0; i < IsaReadVarReq.var_amount; i++)
                {
                    string type = CBVarHandle.ReturnCBVarType(var_name_array[i].Trim("\0".ToCharArray()));
                    CBMsg.address = CBVarHandle.ReturnCBVarIndex(var_name_array[i].Trim("\0".ToCharArray()));
                    Console.Write(" index is {0} ", CBMsg.address);
                    if (type == "B")
                    {
                        CBMsg.type = 2;
                        BoolNum++;
                    }
                    if (type == "I")
                    {
                        CBMsg.type = 4;
                        IntNum++;
                    }
                    if (type == "F")
                    {
                        CBMsg.type = 6;
                        FloatNum++;
                    }
                    if (type == "C")
                    {
                        CBMsg.type = 8;
                        CharNum++;
                    }
                    CBSendbuffer[4 + i * 12] = CBMsg.type;
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBMsg.address)), 0, CBSendbuffer, i * 12, 4);

                }

                for (int i = 0; i < 12; i++)
                {
                    Console.Write(" {0} ", CBSendbuffer[i].ToString("x2"));
                }

                CBClient.SendWithHeaderMsg(CBSendbuffer);

                CBMsg.type = 99;
                Byte[] buffer1 = new Byte[12];
                buffer1[4] = CBMsg.type;
                for (int i = 0; i < 12; i++)
                {
                    Console.Write(" {0} ", buffer1[i].ToString("x2"));
                }

                CBClient.SendWithHeaderMsg(buffer1);

                CBClient.Receive(respond1);

                for (int i = 0; i < IsaReadVarReq.var_amount; i++)
                {
                    Int32 a = Convert.ToInt32(CBSendbuffer[i * 12 + 5]) + 1;
                    CBSendbuffer[i * 12 + 4] = (byte)(CBSendbuffer[i * 12 + 4] + 1);
                    for (int j = 0; j < 12; j++)
                    {
                        Console.Write(" {0} ", CBSendbuffer[j].ToString("x2"));
                    }

                }
                CBClient.SendWithHeaderMsg(CBSendbuffer);

                IsaReadVarRep.var_definitions = new Byte[IsaReadVarReq.var_amount * 16];
                IsaReadVarRep.data_area = new Byte[BoolNum + IntNum * 4 + FloatNum * 4 + CharNum];
                def.var_offset = 0;//初始化偏移量
                for (int i = 0; i < IsaReadVarReq.var_amount; i++)
                {
                    string type = CBVarHandle.ReturnCBVarType(var_name_array[i].Trim("\0".ToCharArray()));
                    if (type == "B")
                    {
                        def.var_type = 1;
                        def.var_size = 1;
                        def.var_offset = var_offset_ret;
                        var_offset_ret = var_offset_ret + 1;

                    }
                    if (type == "I")
                    {
                        def.var_type = 9;
                        def.var_size = 4;
                        def.var_offset = var_offset_ret;
                        var_offset_ret = var_offset_ret + 4;

                    }
                    if (type == "F")
                    {
                        def.var_type = 5;//EKE接口中5为real
                        def.var_size = 4;
                        def.var_offset = var_offset_ret;
                        var_offset_ret = var_offset_ret + 4;

                    }
                    if (type == "C")
                    {
                        def.var_type = 6;//EKE接口中6为string
                        def.var_size = 1;
                        def.var_offset = var_offset_ret;
                        var_offset_ret = var_offset_ret + 1;

                    }

                    byte[] IsaDef = new byte[16];
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(def.var_size)), 0, IsaDef, 0, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(def.var_type)), 0, IsaDef, 4, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(def.var_def)), 0, IsaDef, 8, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(def.var_offset)), 0, IsaDef, 12, 4);
                    Buffer.BlockCopy(IsaDef, 0, IsaReadVarRep.var_definitions, i * 16, 16);

                    Buffer.BlockCopy(respond1, (i + 1) * 12 + 8, IsaReadVarRep.data_area, def.var_offset, def.var_size);//respond的value需修改 

                }


                Console.Write("\n ");

                byte[] IsaReadVarRepMsg = new byte[13 + 4 + 16 * IsaReadVarReq.var_amount + BoolNum + IntNum * 4 + FloatNum * 4 + CharNum];
                Buffer.BlockCopy(buffer, 0, IsaReadVarRepMsg, 0, 17);
                //Buffer.BlockCopy(BitConverter.GetBytes(IsaReadVarReq.var_amount), 0, IsaReadVarRepMsg, 13, 4);
                Buffer.BlockCopy(IsaReadVarRep.var_definitions, 0, IsaReadVarRepMsg, 17, (int)(16 * IsaReadVarReq.var_amount));
                Buffer.BlockCopy(IsaReadVarRep.data_area, 0, IsaReadVarRepMsg, 17 + (int)(16 * IsaReadVarReq.var_amount), BoolNum + IntNum * 4 + FloatNum * 4 + CharNum);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(IsaReadVarRepMsg.Length - 8)), 0, IsaReadVarRepMsg, 2, 4);
                IsaReadVarRepMsg[8] = 0x81;
                Buffer.BlockCopy(BitConverter.GetBytes((Int16)CRC.CRC16(IsaReadVarRepMsg, 6)), 0, IsaReadVarRepMsg, 6, 2);
                ISAServer.Send(ISASeverSock.handler, IsaReadVarRepMsg);

            }
            public void ReadVariablesValue(byte[] buffer)
            {
                Int32 var_offset_ret = 0;
                int BoolNum = 0;
                int IntNum = 0;
                int FloatNum = 0;
                int CharNum = 0;
                byte[] CBSendbuffer = new byte[IsaReadVarReq.var_amount * 12];

                IsaVarDef def = new IsaVarDef();
                def.var_size = 4;

                for (int i = 0; i < IsaReadVarReq.var_amount; i++)
                {
                    string type = CBVarHandle.ReturnCBVarType(var_name_array[i].Trim("\0".ToCharArray()));
                    CBMsg.address = CBVarHandle.ReturnCBVarIndex(var_name_array[i].Trim("\0".ToCharArray()));
                    Console.Write(" index is {0} ", CBMsg.address);
                    if (type == "B")
                    {
                        CBMsg.type = 2;
                        BoolNum++;
                    }
                    if (type == "I")
                    {
                        CBMsg.type = 4;
                        IntNum++;
                    }
                    if (type == "F")
                    {
                        CBMsg.type = 6;
                        FloatNum++;
                    }
                    if (type == "C")
                    {
                        CBMsg.type = 8;
                        CharNum++;
                    }
                    CBSendbuffer[4 + i * 12] = CBMsg.type;
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBMsg.address)), 0, CBSendbuffer, i * 12, 4);

                }

                for (int i = 0; i < 12; i++)
                {
                    Console.Write(" {0} ", CBSendbuffer[i].ToString("x2"));
                }

                CBClient.SendWithHeaderMsg(CBSendbuffer);

                CBMsg.type = 99;
                Byte[] buffer1 = new Byte[12];
                buffer1[4] = CBMsg.type;
                for (int i = 0; i < 12; i++)
                {
                    Console.Write(" {0} ", buffer1[i].ToString("x2"));
                }

                CBClient.SendWithHeaderMsg(buffer1);

                CBClient.Receive(respond1);

                for (int i = 0; i < IsaReadVarReq.var_amount; i++)
                {
                    Int32 a = Convert.ToInt32(CBSendbuffer[i * 12 + 5]) + 1;
                    CBSendbuffer[i * 12 + 4] = (byte)(CBSendbuffer[i * 12 + 4] + 1);
                    for (int j = 0; j < 12; j++)
                    {
                        Console.Write(" {0} ", CBSendbuffer[j].ToString("x2"));
                    }

                }
                CBClient.SendWithHeaderMsg(CBSendbuffer);
            }
            public void WriteVariables(byte[] buffer)
            {


                byte[] data_area = new byte[IsaWriteVarReq.var_amount * 8];
                Buffer.BlockCopy(buffer, 17 + 89 * IsaWriteVarReq.var_amount, data_area, 0, IsaWriteVarReq.var_amount * 8);
                IsaWriteVarRep WriteVarReply = new IsaWriteVarRep();
                Byte[] CBSendbuffer = new Byte[IsaWriteVarReq.var_amount * 12];
                int[] VariablesValueArray = new int[IsaWriteVarReq.var_amount];
                string[] VariablesTypeArray = new string[IsaWriteVarReq.var_amount];

                for (int i = 0; i < IsaWriteVarReq.var_amount; i++)
                {


                    IsaWriteVarInfo write_var_info = new IsaWriteVarInfo();
                    write_var_info.var_type = (char)buffer[97 + i * 89];
                    write_var_info.var_size = BitConverter.ToUInt32(buffer, 98 + i * 89);
                    write_var_info.var_offset = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(buffer, 13 + (i + 1) * 89));
                    string var_type = CBVarHandle.ReturnCBVarType(var_name_array[i].Trim("\0".ToCharArray()));
                    CBMsg.value = (Int32)IPAddress.NetworkToHostOrder(BitConverter.ToInt64(data_area, (Int32)write_var_info.var_offset));
                    VariablesValueArray[i] = CBMsg.value;
                    VariablesTypeArray[i] = var_type;

                    if (var_type == "B")
                    {
                        CBMsg.type = 10;
                        CBSendbuffer[i * 12 + 8] = (byte)CBMsg.value;
                    }
                    else if (var_type == "I")
                    {
                        CBMsg.type = 11;
                        Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(CBMsg.value)), 0, CBSendbuffer, i * 12 + 8, 4);
                    }
                    else if (var_type == "F")
                    {
                        CBMsg.type = 12;
                        Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.NetworkToHostOrder(CBMsg.value)), 0, CBSendbuffer, i * 12 + 8, 4);
                    }
                    else if (var_type == "C")
                    {
                        CBMsg.type = 13;
                        CBSendbuffer[i * 12 + 8] = (byte)CBMsg.value;
                    }
                    else
                        Console.WriteLine("error: unkown variable type ");

                    CBMsg.address = CBVarHandle.ReturnCBVarIndex(var_name_array[i].Trim("\0".ToCharArray()));
                    CBSendbuffer[i * 12 + 4] = CBMsg.type;
                    Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBMsg.address)), 0, CBSendbuffer, i * 12, 4);

                }

                /* for (int i = 0; i < 12; i++)
                 {
                     Console.Write(" {0} ", CBSendbuffer[i].ToString("x2"));
                 }*/

                CBClient.SendWithHeaderMsg(CBSendbuffer);

                ReadVariablesValue(buffer);

                byte[] Result = new byte[IsaWriteVarReq.var_amount];

                for (int i = 0; i < IsaWriteVarReq.var_amount; i++)
                {
                    if (VariablesTypeArray[i] == "B" || VariablesTypeArray[i] == "C")
                    {
                        if (respond1[(i + 1) * 12 + 8] == VariablesValueArray[i])
                        {
                            Result[i] = 0;

                        }
                        else
                        {
                            Result[i] = 1;//EKE接口中写入变量失败有5种情况
                            Console.WriteLine("error: write variablec {0} failure ", i);
                        }
                    }
                    else if (VariablesTypeArray[i] == "I" || VariablesTypeArray[i] == "F")
                    {
                        if (IPAddress.NetworkToHostOrder(BitConverter.ToInt32(respond1, (i + 1) * 12 + 8)) == VariablesValueArray[i])
                        {
                            Result[i] = 0;

                        }
                        else
                        {
                            Result[i] = 1;//EKE接口中写入变量失败有5种情况
                            Console.WriteLine("error: write variablec {0} failure ", i);
                        }
                    }

                }

                byte[] IsaWriteVarRepMsg = new byte[17 + IsaWriteVarReq.var_amount];
                Buffer.BlockCopy(buffer, 0, IsaWriteVarRepMsg, 0, 17);
                Buffer.BlockCopy(Result, 0, IsaWriteVarRepMsg, 17, IsaWriteVarReq.var_amount);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(9 + IsaWriteVarReq.var_amount)), 0, IsaWriteVarRepMsg, 2, 4);
                IsaWriteVarRepMsg[8] = 0x82;
                Buffer.BlockCopy(BitConverter.GetBytes((Int16)CRC.CRC16(IsaWriteVarRepMsg, 6)), 0, IsaWriteVarRepMsg, 6, 2);
                ISAServer.Send(ISASeverSock.handler, IsaWriteVarRepMsg);


            }
            public void CmdCnv(Object obj, MyEventArgs e)
            {
                Console.WriteLine("事件处理程序");
                IsaEthHeader.tag = BitConverter.ToUInt16(e.buffer, 0);
                if (IsaEthHeader.tag == IsaConstHeader.PSTM_ETH_HDR_TAG)
                {
                    IsaEthHeader.leagth = BitConverter.ToUInt32(e.buffer, 2);
                    IsaEthHeader.crc = BitConverter.ToUInt16(e.buffer, 6);
                    Console.WriteLine("IsaEthHeader.crc = {0}", IsaEthHeader.crc);
                    Console.WriteLine("CRC.CRC16(e.buffer, 6) = {0}", CRC.CRC16(e.buffer, 6));

                    IsaReadVarReq.var_amount = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(e.buffer, 13));

                    if (IsaEthHeader.crc == CRC.CRC16(e.buffer, 6))
                    {
                        IsaMsgDataHdr.cmd_code = (UInt16)e.buffer[8];
                        IsaMsgDataHdr.packet_id = BitConverter.ToUInt32(e.buffer, 9);

                        if (IsaMsgDataHdr.cmd_code == 0x01)
                        {
                            IsaReadVarReq.var_amount = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(e.buffer, 13));
                            Console.Write(" IsaReadVarReq.var_amount is {0} ", IsaReadVarReq.var_amount);
                            IsaReadVarReq.VarNameList = new ArrayList();
                            for (int i = 0; i < IsaReadVarReq.var_amount; i++)
                            {
                                String VarName;
                                Byte[] VarNameByte = new Byte[80];
                                Buffer.BlockCopy(e.buffer, 17 + i * 80, VarNameByte, 0, 80);
                                VarName = Encoding.ASCII.GetString(VarNameByte, 0, 80);
                                IsaReadVarReq.VarNameList.Add(VarName);
                            }
                            var_name_array = (string[])IsaReadVarReq.VarNameList.ToArray(typeof(string));

                            ReadVariables(e.buffer);

                        }

                        if (IsaMsgDataHdr.cmd_code == 0x02)
                        {

                            IsaWriteVarReq.var_amount = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(e.buffer, 13));
                            Console.Write(" IsaWriteVarReq.var_amount is {0} ", IsaWriteVarReq.var_amount);
                            IsaWriteVarReq.VarNameList = new ArrayList();
                            for (int i = 0; i < IsaWriteVarReq.var_amount; i++)
                            {
                                String VarName;
                                Byte[] VarNameByte = new Byte[80];
                                Buffer.BlockCopy(e.buffer, 17 + i * 89, VarNameByte, 0, 80);
                                VarName = Encoding.ASCII.GetString(VarNameByte, 0, 80);
                                IsaWriteVarReq.VarNameList.Add(VarName);
                            }
                            var_name_array = (string[])IsaWriteVarReq.VarNameList.ToArray(typeof(string));
                            WriteVariables(e.buffer);

                        }

                    }
                    else
                        Console.WriteLine("error: the EthHeader's CRC error ");
                }
                else
                    Console.WriteLine("error: the error TCP message!");

            }
        }

        public class CBDebugServer
        {
            private static ManualResetEvent sendDone =
                new ManualResetEvent(false);
            private static ManualResetEvent receiveDone =
                new ManualResetEvent(false);
            public CBClient NewCBClient;
            public void CBInitDataDeal(String CoiPilePath)
            {
                StreamReader sr = new StreamReader(CoiPilePath);

                if (File.Exists(CoiPilePath))
                {
                    while (sr.Peek() >= 0)
                    {

                        string srString = sr.ReadLine();


                        if (srString.StartsWith("#") && srString.EndsWith("/"))
                        {
                            Console.WriteLine("{0}", srString);
                            int i;
                            for (i = 0; (srString = sr.ReadLine()) != "#"; i++)
                            {
                                Console.WriteLine("{0}", srString);
                                string[] split = srString.Split(new Char[] { ' ' });
                                Console.WriteLine("{0}", split[1]);
                                Console.WriteLine("{0}", split[2]);
                                VarListVar VarTypeIndex = new VarListVar();
                                VarTypeIndex.VarType = split[2].Substring(1, 1);
                                VarTypeIndex.VarIndex = Int32.Parse(split[2].Substring(2));
                                if (VarTypeIndex.VarType == "B")
                                {
                                    CBInitData.BoolMemSize++;
                                }
                                if (VarTypeIndex.VarType == "I")
                                {
                                    CBInitData.IntMemSize++;
                                }
                                if (VarTypeIndex.VarType == "F")
                                {
                                    CBInitData.FloatMemSize++;
                                }
                                if (VarTypeIndex.VarType == "C")
                                {
                                    CBInitData.CharMemSize++;
                                }
                            }
                        }
                        else if (srString.StartsWith(" *FREE") || (srString != "#" && !(srString).EndsWith("/") && !srString.StartsWith(" *")))
                        {

                            Console.WriteLine("{0}", srString);
                            string[] split = srString.Split(new Char[] { ' ' });
                            Console.WriteLine("{0}", split[1]);
                            Console.WriteLine("{0}", split[2]);
                            VarListVar VarTypeIndex = new VarListVar();
                            VarTypeIndex.VarType = split[2].Substring(1, 1);
                            VarTypeIndex.VarIndex = Int32.Parse(split[2].Substring(2));
                            if (split[1] == "pid")
                                CBInitData.pospidIndex = VarTypeIndex.VarIndex;
                            if (split[1] == "signature")
                                CBInitData.signature = VarTypeIndex.VarIndex;
                            if (VarTypeIndex.VarType == "B")
                            {
                                if (VarTypeIndex.VarIndex < CBInitData.MinBoolIndex)
                                    CBInitData.MinBoolIndex = VarTypeIndex.VarIndex;
                                else if (VarTypeIndex.VarIndex > CBInitData.MaxBoolIndex)
                                    CBInitData.MaxBoolIndex = VarTypeIndex.VarIndex;
                            }
                            if (VarTypeIndex.VarType == "I")
                            {
                                if (VarTypeIndex.VarIndex < CBInitData.MinIntIndex)
                                    CBInitData.MinIntIndex = VarTypeIndex.VarIndex;
                                else if (VarTypeIndex.VarIndex > CBInitData.MaxIntIndex)
                                    CBInitData.MaxIntIndex = VarTypeIndex.VarIndex;
                            }
                            if (VarTypeIndex.VarType == "F")
                            {
                                if (VarTypeIndex.VarIndex < CBInitData.MinFloatIndex)
                                    CBInitData.MinFloatIndex = VarTypeIndex.VarIndex;
                                else if (VarTypeIndex.VarIndex > CBInitData.MaxFloatIndex)
                                    CBInitData.MaxFloatIndex = VarTypeIndex.VarIndex;
                            }
                            if (VarTypeIndex.VarType == "C")
                            {
                                if (VarTypeIndex.VarIndex < CBInitData.MinCharIndex)
                                    CBInitData.MinCharIndex = VarTypeIndex.VarIndex;
                                else if (VarTypeIndex.VarIndex > CBInitData.MaxCharIndex)
                                    CBInitData.MaxCharIndex = VarTypeIndex.VarIndex;

                            }

                        }

                    }
                }
                else
                {
                    Console.WriteLine("未找到变量清单");
                    Console.ReadKey();
                }
                //NewCBClient.EndCbClient();
                // Write the response to the console.
                string[] TaskPath = CoiPilePath.Split(new Char[] { '.' });
                string TaskParamsPath = TaskPath[0] + "_params.txt";

                if (File.Exists(TaskParamsPath))
                {
                    StreamReader sr1 = new StreamReader(TaskParamsPath);
                    string Signature = sr1.ReadLine();
                    CBInitData.SignatureNum = Int32.Parse(Signature);
                }
                else
                {
                    Console.WriteLine("未找到变量清单");
                    Console.ReadKey();
                }
                CBInitData.BoolMemSize = CBInitData.BoolMemSize + CBInitData.MaxBoolIndex;
                CBInitData.IntMemSize = CBInitData.IntMemSize + CBInitData.MaxIntIndex;
                CBInitData.FloatMemSize = CBInitData.FloatMemSize + CBInitData.MaxFloatIndex;
                CBInitData.CharMemSize = CBInitData.CharMemSize + CBInitData.MaxCharIndex;
                Console.WriteLine(CBInitData.MinBoolIndex);
                Console.WriteLine(CBInitData.MinIntIndex);
                Console.WriteLine(CBInitData.MinFloatIndex);
                Console.WriteLine(CBInitData.MinCharIndex);
                Console.WriteLine(CBInitData.MaxBoolIndex);
                Console.WriteLine(CBInitData.MaxIntIndex);
                Console.WriteLine(CBInitData.MaxFloatIndex);
                Console.WriteLine(CBInitData.MaxCharIndex);
                Console.WriteLine(CBInitData.pospidIndex);
                Console.WriteLine(CBInitData.BoolMemSize);
                Console.WriteLine(CBInitData.IntMemSize);
                Console.WriteLine(CBInitData.FloatMemSize);
                Console.WriteLine(CBInitData.CharMemSize);
                Console.WriteLine(CBInitData.pospidIndex);
                Console.WriteLine(CBInitData.signature);
                Console.WriteLine(CBInitData.SignatureNum);
                Console.ReadKey();
            }

            public int InitCBDebugServer(String IPAdress, UInt16 PORT, String CoiPilePath)
            {
                CBInitDataDeal(CoiPilePath);
                NewCBClient = new CBClient(IPAdress, PORT);
                NewCBClient.StartCBClient();

                Byte[] buffer = new Byte[12];
                byte[] respond = new byte[CBClientState.BufferSize];

                //startingInformationsMsg 01 MinBool

                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(1)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.MinBoolIndex)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 02 MaxBool
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(2)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.BoolMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 03 MinInt
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(3)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.MinIntIndex)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 04 MaxInt
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(4)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.IntMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 05 MinFloat
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(5)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.MinFloatIndex)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 06 MaxFloat
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(6)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.FloatMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 07 MinChar
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(7)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.MinCharIndex)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 08 MaxChar
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(8)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.CharMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 09 pospid
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(9)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.pospidIndex)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 10 bool mem size
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(10)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.BoolMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 11 int mem size
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(11)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.IntMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 12 float mem size
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(12)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.FloatMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //startingInformationsMsg 13 char mem size
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(13)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.CharMemSize)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.startingInformationsMsg;
                CBClient.SendWithHeaderMsg(buffer);

                //setSignatureMsg
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(CBInitData.signature)), 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(0)), 0, buffer, 8, 4);
                buffer[4] = CBServerConstType.setSignatureMsg;
                CBClient.SendWithHeaderMsg(buffer);

                // Receive the response from the remote device.               

                CBClient.Receive(respond);
                Int32 GetSignatureMsg = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(respond, 12 + 8));
                Console.WriteLine(" GetSignatureMsg = {0}\n", GetSignatureMsg);
                if (GetSignatureMsg == CBInitData.SignatureNum)
                    return 1;
                else
                    return 0;
            }

            ISAServer NewISAServer = new ISAServer();
            public void StartCBDebugServer(String TargetIPAdress, UInt16 TargetPort, String CoiPilePath)
            {
                //Initialize varlist
                CBVarList.TaskCoiPilePathName = CoiPilePath;
                InitCBDebugServer(TargetIPAdress, TargetPort, CoiPilePath);
                ISAServer NewISAServer = new ISAServer();
                ISADebugCmdCnv cnv = new ISADebugCmdCnv();
                NewISAServer.CmdMessage += new EventHandler<MyEventArgs>(cnv.CmdCnv);
                NewISAServer.StartListening();
                Console.ReadKey();
            }
        }

        public class CabinetTCPServer
        {
            private static ManualResetEvent sendDone = new ManualResetEvent(false);
            private static ManualResetEvent receiveDone = new ManualResetEvent(false);
            public CBClient NewCBClient;
            

            public void InitCBDebugServer(String IPAdress, UInt16 PORT)
            {


                NewCBClient = new CBClient(IPAdress, PORT);
                NewCBClient.StartCBClient();

            }

            public static void CabinetOpen(int ID)
            {
                Byte[] buffer = new Byte[20];
                buffer[0] = 0xaa;
                buffer[1] = 0x0f;
                buffer[2] = (Byte)(ID-1);
                buffer[3] = 0x1;
                buffer[19] = 0xbb;
                CBClient.Send(buffer);
                System.Threading.Thread.Sleep(1000);
                buffer[3] = 0x2;
                CBClient.Send(buffer);


            }            

        }

    }
}
