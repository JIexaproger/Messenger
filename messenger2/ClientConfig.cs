using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace messenger2
{
    internal class ClientConfig
    {
        private TcpClient _tcpClient;
        private StreamReader _reader;
        private StreamWriter _writer;
        private int _roomId;


        public void SetTcpClient(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }
        public TcpClient GetTcpClient()
        {
            return _tcpClient;
        }


        public void SetWriter(StreamWriter writer)
        {
            _writer = writer;
        }
        public StreamWriter GetStreamWriter()
        {
            return _writer;
        }


        public void SetReader(StreamReader reader)
        {
            _reader = reader;
        }
        public StreamReader GetStreamReader()
        {
            return _reader;
        }


        public void SetRoomId(int roomId)
        {
            _roomId = roomId;
        }
        public int GetRoomId()
        {
            return _roomId;
        }


        public ClientConfig(TcpClient tcpClient, StreamReader reader, StreamWriter writer)
        {
            _tcpClient = tcpClient;
            _reader = reader;
            _writer = writer;
        }
    }
}
