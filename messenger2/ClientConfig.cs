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
        private int _id;


        private void SetTcpClient(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
        }
        public TcpClient GetTcpClient()
        {
            return _tcpClient;
        }


        private void SetWriter(StreamWriter writer)
        {
            _writer = writer;
        }
        public StreamWriter GetStreamWriter()
        {
            return _writer;
        }


        private void SetReader(StreamReader reader)
        {
            _reader = reader;
        }
        public StreamReader GetStreamReader()
        {
            return _reader;
        }


        private void SetId(int id)
        {
            _id = id;
        }
        public int GetId()
        {
            return _id;
        }


        public ClientConfig(TcpClient tcpClient, StreamReader reader, StreamWriter writer, int id)
        {
            _tcpClient = tcpClient;
            _reader = reader;
            _writer = writer;
            _id = id;
        }
    }
}
