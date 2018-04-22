using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server.Networking.Stun
{
    public class StunMessage
    {
        private enum AttributeType
        {
            MappedAddress = 0x0001,
            ResponseAddress = 0x0002,
            ChangeRequest = 0x0003,
            SourceAddress = 0x0004,
            ChangedAddress = 0x0005,
            Username = 0x0006,
            Password = 0x0007,
            MessageIntegrity = 0x0008,
            ErrorCode = 0x0009,
            UnknownAttribute = 0x000A,
            ReflectedFrom = 0x000B,
            XorMappedAddress = 0x8020,
            XorOnly = 0x0021,
            ServerName = 0x8022,
        }

        private enum IpFamily
        {
            Pv4 = 0x01,
            Pv6 = 0x02,
        }

        private Guid _transactionId;

        public StunMessage() {
            _transactionId = Guid.NewGuid();
        }

        public void Parse(byte[] data) {
            /* RFC 3489 11.1.             
                All STUN messages consist of a 20 byte header:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |      STUN Message Type        |         Message Length        |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                        Transaction ID
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                                                               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
              
               The message length is the count, in bytes, of the size of the
               message, not including the 20 byte header.
            */

            if (data.Length < 20) {
                throw new ArgumentException("Invalid STUN message value !");
            }

            var offset = 0;

            // Header
            // STUN Message Type
            var messageType = (data[offset++] << 8 | data[offset++]);
            if (messageType == (int)StunMessageType.BindingErrorResponse) {
                Type = StunMessageType.BindingErrorResponse;
            } else if (messageType == (int)StunMessageType.BindingRequest) {
                Type = StunMessageType.BindingRequest;
            } else if (messageType == (int)StunMessageType.BindingResponse) {
                Type = StunMessageType.BindingResponse;
            } else if (messageType == (int)StunMessageType.SharedSecretErrorResponse) {
                Type = StunMessageType.SharedSecretErrorResponse;
            } else if (messageType == (int)StunMessageType.SharedSecretRequest) {
                Type = StunMessageType.SharedSecretRequest;
            } else if (messageType == (int)StunMessageType.SharedSecretResponse) {
                Type = StunMessageType.SharedSecretResponse;
            } else {
                throw new ArgumentException("Invalid STUN message type value !");
            }

            // Message length
            var messageLength = (data[offset++] << 8 | data[offset++]);

            // Transaction id
            var guid = new byte[16];
            Array.Copy(data, offset, guid, 0, 16);
            _transactionId = new Guid(guid);
            offset += 16;

            // Attributes
            while ((offset - 20) < messageLength) {
                ParseAttribute(data, ref offset);
            }
        }

        public byte[] ToByteData() {
            /* RFC 3489 11.1.             
                All STUN messages consist of a 20 byte header:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |      STUN Message Type        |         Message Length        |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                        Transaction ID
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                                                                               |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
             
               The message length is the count, in bytes, of the size of the
               message, not including the 20 byte header.

            */

            var msg = new byte[512];

            var offset = 0;

            // Header
            // STUN Message Type (2 bytes)
            msg[offset++] = (byte)((int)Type >> 8);
            msg[offset++] = (byte)((int)Type & 0xFF);

            // Message length (2 bytes)
            msg[offset++] = 0;
            msg[offset++] = 0;

            // Transaction id (16 bytes)
            Array.Copy(_transactionId.ToByteArray(), 0, msg, offset, 16);
            offset += 16;

            // Attributes below

            /* RFC 3489 11.2.
                After the header are 0 or more attributes.  Each attribute is TLV
                encoded, with a 16 bit type, 16 bit length, and variable value:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |         Type                  |            Length             |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |                             Value                             ....
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            if (MappedAddress != null) {
                StoreEndPoint(AttributeType.MappedAddress, MappedAddress, msg, ref offset);
            } else if (ResponseAddress != null) {
                StoreEndPoint(AttributeType.ResponseAddress, ResponseAddress, msg, ref offset);
            } else if (ChangeRequest != null) {
                /*
                    The CHANGE-REQUEST attribute is used by the client to request that
                    the server use a different address and/or port when sending the
                    response.  The attribute is 32 bits long, although only two bits (A
                    and B) are used:

                     0                   1                   2                   3
                     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 A B 0|
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                    The meaning of the flags is:

                    A: This is the "change IP" flag.  If true, it requests the server
                       to send the Binding Response with a different IP address than the
                       one the Binding Request was received on.

                    B: This is the "change port" flag.  If true, it requests the
                       server to send the Binding Response with a different port than the
                       one the Binding Request was received on.
                */

                // Attribute header
                msg[offset++] = (int)AttributeType.ChangeRequest >> 8;
                msg[offset++] = (int)AttributeType.ChangeRequest & 0xFF;
                msg[offset++] = 0;
                msg[offset++] = 4;

                msg[offset++] = 0;
                msg[offset++] = 0;
                msg[offset++] = 0;
                msg[offset++] = (byte)(Convert.ToInt32(ChangeRequest.ChangeIp) << 2 | Convert.ToInt32(ChangeRequest.ChangePort) << 1);
            } else if (SourceAddress != null) {
                StoreEndPoint(AttributeType.SourceAddress, SourceAddress, msg, ref offset);
            } else if (ChangedAddress != null) {
                StoreEndPoint(AttributeType.ChangedAddress, ChangedAddress, msg, ref offset);
            } else if (UserName != null) {
                var userBytes = Encoding.ASCII.GetBytes(UserName);

                // Attribute header
                msg[offset++] = (int)AttributeType.Username >> 8;
                msg[offset++] = (int)AttributeType.Username & 0xFF;
                msg[offset++] = (byte)(userBytes.Length >> 8);
                msg[offset++] = (byte)(userBytes.Length & 0xFF);

                Array.Copy(userBytes, 0, msg, offset, userBytes.Length);
                offset += userBytes.Length;
            } else if (Password != null) {
                var userBytes = Encoding.ASCII.GetBytes(UserName);

                // Attribute header
                msg[offset++] = (int)AttributeType.Password >> 8;
                msg[offset++] = (int)AttributeType.Password & 0xFF;
                msg[offset++] = (byte)(userBytes.Length >> 8);
                msg[offset++] = (byte)(userBytes.Length & 0xFF);

                Array.Copy(userBytes, 0, msg, offset, userBytes.Length);
                offset += userBytes.Length;
            } else if (ErrorCode != null) {
                /* 3489 11.2.9.
                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |                   0                     |Class|     Number    |
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |      Reason Phrase (variable)                                ..
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                */

                var reasonBytes = Encoding.ASCII.GetBytes(ErrorCode.ReasonText);

                // Header
                msg[offset++] = 0;
                msg[offset++] = (int)AttributeType.ErrorCode;
                msg[offset++] = 0;
                msg[offset++] = (byte)(4 + reasonBytes.Length);

                // Empty
                msg[offset++] = 0;
                msg[offset++] = 0;
                // Class
                msg[offset++] = (byte)Math.Floor(ErrorCode.Code / 100f);
                // Number
                msg[offset++] = (byte)(ErrorCode.Code & 0xFF);
                // ReasonPhrase
                Array.Copy(reasonBytes, msg, reasonBytes.Length);
                offset += reasonBytes.Length;
            } else if (ReflectedFrom != null) {
                StoreEndPoint(AttributeType.ReflectedFrom, ReflectedFrom, msg, ref offset);
            }

            // Update message length, not including 20 bytes of header
            msg[2] = (byte)((offset - 20) >> 8);
            msg[3] = (byte)((offset - 20) & 0xFF);

            // Make reatval with actual size
            var retVal = new byte[offset];
            Array.Copy(msg, retVal, retVal.Length);

            return retVal;
        }

        private void ParseAttribute(byte[] data, ref int offset) {
            /* RFC 3489 11.2.
                Each attribute is TLV encoded, with a 16 bit type, 16 bit length, and variable value:

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |         Type                  |            Length             |
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
               |                             Value                             ....
               +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+                            
            */

            // Type
            var type = (AttributeType)(data[offset++] << 8 | data[offset++]);

            // Length
            var length = (data[offset++] << 8 | data[offset++]);

            if (type == AttributeType.MappedAddress) {
                MappedAddress = ParseEndPoint(data, ref offset);
            } else if (type == AttributeType.ResponseAddress) {
                ResponseAddress = ParseEndPoint(data, ref offset);
            } else if (type == AttributeType.ChangeRequest) {
                /*
                    The CHANGE-REQUEST attribute is used by the client to request that
                    the server use a different address and/or port when sending the
                    response.  The attribute is 32 bits long, although only two bits (A
                    and B) are used:

                     0                   1                   2                   3
                     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 A B 0|
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

                    The meaning of the flags is:

                    A: This is the "change IP" flag.  If true, it requests the server
                       to send the Binding Response with a different IP address than the
                       one the Binding Request was received on.

                    B: This is the "change port" flag.  If true, it requests the
                       server to send the Binding Response with a different port than the
                       one the Binding Request was received on.
                */

                // Skip 3 bytes
                offset += 3;

                ChangeRequest = new StunChangeRequest((data[offset] & 4) != 0, (data[offset] & 2) != 0);
                offset++;
            } else if (type == AttributeType.SourceAddress) {
                SourceAddress = ParseEndPoint(data, ref offset);
            } else if (type == AttributeType.ChangedAddress) {
                ChangedAddress = ParseEndPoint(data, ref offset);
            } else if (type == AttributeType.Username) {
                UserName = Encoding.Default.GetString(data, offset, length);
                offset += length;
            } else if (type == AttributeType.Password) {
                Password = Encoding.Default.GetString(data, offset, length);
                offset += length;
            } else if (type == AttributeType.MessageIntegrity) {
                offset += length;
            } else if (type == AttributeType.ErrorCode) {
                /* 3489 11.2.9.
                    0                   1                   2                   3
                    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |                   0                     |Class|     Number    |
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                    |      Reason Phrase (variable)                                ..
                    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                */

                var errorCode = (data[offset + 2] & 0x7) * 100 + (data[offset + 3] & 0xFF);

                ErrorCode = new StunErrorCode(errorCode, Encoding.Default.GetString(data, offset + 4, length - 4));
                offset += length;
            } else if (type == AttributeType.UnknownAttribute) {
                offset += length;
            } else if (type == AttributeType.ReflectedFrom) {
                ReflectedFrom = ParseEndPoint(data, ref offset);
            }  else if (type == AttributeType.ServerName) {
                // XorMappedAddress
                // XorOnly
                // ServerName
                ServerName = Encoding.Default.GetString(data, offset, length);
                offset += length;
            } else {
                // Unknown shit
                offset += length;
            }
        }

        private static IPEndPoint ParseEndPoint(byte[] data, ref int offset) {
            /*
                It consists of an eight bit address family, and a sixteen bit
                port, followed by a fixed length value representing the IP address.

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |x x x x x x x x|    Family     |           Port                |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |                             Address                           |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
            */

            // Skip family
            offset++;
            offset++;

            // Port
            var port = (data[offset++] << 8 | data[offset++]);

            // Address
            var ip = new byte[4];
            ip[0] = data[offset++];
            ip[1] = data[offset++];
            ip[2] = data[offset++];
            ip[3] = data[offset++];

            return new IPEndPoint(new IPAddress(ip), port);
        }

        private static void StoreEndPoint(AttributeType type, IPEndPoint endPoint, byte[] message, ref int offset) {
            /*
                It consists of an eight bit address family, and a sixteen bit
                port, followed by a fixed length value representing the IP address.

                0                   1                   2                   3
                0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |x x x x x x x x|    Family     |           Port                |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
                |                             Address                           |
                +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+             
            */

            // Header
            message[offset++] = (byte)((int)type >> 8);
            message[offset++] = (byte)((int)type & 0xFF);
            message[offset++] = 0;
            message[offset++] = 8;

            // Unused
            message[offset++] = 0;
            // Family
            message[offset++] = (byte)IpFamily.Pv4;
            // Port
            message[offset++] = (byte)(endPoint.Port >> 8);
            message[offset++] = (byte)(endPoint.Port & 0xFF);
            // Address
            var ipBytes = endPoint.Address.GetAddressBytes();
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[0];
            message[offset++] = ipBytes[0];
        }

        /// <summary>
        /// Gets STUN message type.
        /// </summary>
        public StunMessageType Type {
            get;
            set;
        } = StunMessageType.BindingRequest;

        /// <summary>
        /// Gets transaction ID.
        /// </summary>
        public Guid TransactionId => _transactionId;

        /// <summary>
        /// Gets or sets IP end point what was actually connected to STUN server. Returns null if not specified.
        /// </summary>
        public IPEndPoint MappedAddress {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets IP end point where to STUN client likes to receive response.
        /// Value null means not specified.
        /// </summary>
        public IPEndPoint ResponseAddress {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets how and where STUN server must send response back to STUN client.
        /// Value null means not specified.
        /// </summary>
        public StunChangeRequest ChangeRequest {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets STUN server IP end point what sent response to STUN client. Value null
        /// means not specified.
        /// </summary>
        public IPEndPoint SourceAddress {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets IP end point where STUN server will send response back to STUN client 
        /// if the "change IP" and "change port" flags had been set in the ChangeRequest.
        /// </summary>
        public IPEndPoint ChangedAddress {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets user name. Value null means not specified.
        /// </summary>          
        public string UserName {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets password. Value null means not specified.
        /// </summary>
        public string Password {
            get;
            set;
        }

        //public MessageIntegrity

        /// <summary>
        /// Gets or sets error info. Returns null if not specified.
        /// </summary>
        public StunErrorCode ErrorCode {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets IP endpoint from which IP end point STUN server got STUN client request.
        /// Value null means not specified.
        /// </summary>
        public IPEndPoint ReflectedFrom {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets server name.
        /// </summary>
        public string ServerName {
            get;
            set;
        }
    }
}
