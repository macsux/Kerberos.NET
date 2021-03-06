﻿using Kerberos.NET.Asn1;
using Kerberos.NET.Crypto;
using System;

namespace Kerberos.NET.Entities
{
    public partial class KrbApReq : IAsn1ApplicationEncoder<KrbApReq>
    {
        public KrbApReq()
        {
            ProtocolVersionNumber = 5;
            MessageType = MessageType.KRB_AP_REQ;
        }

        internal const int ApplicationTagValue = 14;

        public KrbApReq DecodeAsApplication(ReadOnlyMemory<byte> data)
        {
            return DecodeApplication(data);
        }

        public static KrbApReq CreateApReq(KrbKdcRep tgsRep, KerberosKey authenticatorKey, ApOptions options)
        {
            var ticket = tgsRep.Ticket;

            KerberosConstants.Now(out DateTimeOffset time, out int usec);

            var authenticator = new KrbAuthenticator
            {
                CName = tgsRep.CName,
                CTime = time,
                Cusec = usec,
                Realm = ticket.Realm,
                SequenceNumber = KerberosConstants.GetNonce(),
                Subkey = null,
                AuthenticatorVersionNumber = 5
            };

            var apReq = new KrbApReq
            {
                Ticket = ticket,
                ApOptions = options,
                Authenticator = KrbEncryptedData.Encrypt(
                    authenticator.EncodeApplication(),
                    authenticatorKey,
                    KeyUsage.ApReqAuthenticator
                )
            };

            return apReq;
        }
    }
}
