using SnmpSharpNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RMS_DAL
{
    public class Test
    {
        public void testSNMP()
        {
            //10.240.231.253 //b&w // HP 402dn
            //10.240.230.249 //colo //richo
            //10.240.231.237 // HP Color LaserJet M553  // color
            //10.240.230.215 //RICOH MP 5054 //B&W

            string host = "10.240.231.253";
            //string host = "10.240.230.249";
            //string host = "10.240.231.237";
            //string host = "10.240.230.215";
            string community = "public";
            SimpleSnmp snmp = new SimpleSnmp(host);


            if (!snmp.Valid)
            {
                Console.WriteLine("SNMP agent host name/ip address is invalid.");
                return;
            }

            IPAddress address = IPAddress.Parse(host);

            UdpTarget target = new UdpTarget(address, 161, 2000, 1);

            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.Get);
            pdu.VbList.Add("1.3.6.1.2.1.1.1.0"); //sysDescr
            pdu.VbList.Add("1.3.6.1.2.1.1.2.0"); //sysObjectID
            pdu.VbList.Add("1.3.6.1.2.1.1.3.0"); //sysUpTime
            pdu.VbList.Add("1.3.6.1.2.1.1.4.0"); //sysContact
            pdu.VbList.Add("1.3.6.1.2.1.1.5.0"); //sysName
            pdu.VbList.Add("1.3.6.1.2.1.43.10.2.1.4.1.1"); //sysName
            pdu.VbList.Add("1.3.6.1.2.43.11.1.1.6.0.1"); //sysName "black ink"
            pdu.VbList.Add("1.3.6.1.2.43.11.1.1.6.0.2");  //"yellow ink"
            pdu.VbList.Add("1.3.6.1.2.43.11.1.1.6.0.3"); //"cyan ink"
            pdu.VbList.Add("1.3.6.1.2.43.11.1.1.6.0.4");  //"magenta ink"


            //Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver2, new string[] { ".1.3.6.1.2.1.1.1.0" });
            Dictionary<Oid, AsnType> result = snmp.Get(SnmpVersion.Ver2, pdu);

            if (result == null)
            {
                Console.WriteLine("No results received.");
                return;
            }

            //foreach (KeyValuePair kvp in result)
            //{
            //    Console.WriteLine("{0}: {1} {2}", kvp.Key.ToString(),
            //                          SnmpConstants.GetTypeName(kvp.Value.Type),
            //                          kvp.Value.ToString());
            //}
        }

        public string testSNMP2()
        {
            StringBuilder sb = new StringBuilder();
            // SNMP community name
            OctetString community = new OctetString("public");

            // Define agent parameters class
            AgentParameters param = new AgentParameters(community);
            // Set SNMP version to 1
            param.Version = SnmpVersion.Ver1;
            // Construct the agent address object
            // IpAddress class is easy to use here because
            //  it will try to resolve constructor parameter if it doesn't
            //  parse to an IP address
            IpAddress agent = new IpAddress("10.240.231.253");

            // Construct target
            UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

            // Define Oid that is the root of the MIB
            //  tree you wish to retrieve
            Oid rootOid = new Oid("1.3.6.1.2.1.2.2.1.2"); // ifDescr

            // This Oid represents last Oid returned by
            //  the SNMP agent
            Oid lastOid = (Oid)rootOid.Clone();

           

            // Loop through results
            while (lastOid != null)
            {
                // Pdu class used for all requests
                Pdu pdu = new Pdu(PduType.GetNext);
                // When Pdu class is first constructed, RequestId is set to a random value
                // that needs to be incremented on subsequent requests made using the
                // same instance of the Pdu class.
                if (pdu.RequestId != 0)
                {
                    pdu.RequestId += 1;
                }
                // Clear Oids from the Pdu class.
                pdu.VbList.Clear();
                // Initialize request PDU with the last retrieved Oid
                pdu.VbList.Add(lastOid);
                try
                {
                    // Make SNMP request
                    SnmpV1Packet result = (SnmpV1Packet)target.Request(pdu, param);
                    // You should catch exceptions in the Request if using in real application.

                    // If result is null then agent didn't reply or we couldn't parse the reply.
                    if (result != null)
                    {
                        // ErrorStatus other then 0 is an error returned by 
                        // the Agent - see SnmpConstants for error definitions
                        if (result.Pdu.ErrorStatus != 0)
                        {
                            // agent reported an error with the request
                            Console.WriteLine("Error in SNMP reply. Error {0} index {1}",
                                result.Pdu.ErrorStatus,
                                result.Pdu.ErrorIndex);
                            lastOid = null;
                            break;
                        }
                        else
                        {
                            // Walk through returned variable bindings
                            foreach (Vb v in result.Pdu.VbList)
                            {
                                // Check that retrieved Oid is "child" of the root OID
                                if (rootOid.IsRootOf(v.Oid))
                                {
                                    sb.Append(Environment.NewLine);
                                    sb.AppendLine(Environment.NewLine);
                                    sb.Append("OID: " + v.Oid.ToString());
                                    sb.Append(Environment.NewLine);
                                    sb.AppendLine(Environment.NewLine);
                                    sb.Append("Name: " + SnmpConstants.GetTypeName(v.Value.Type));
                                    sb.Append(Environment.NewLine);
                                    sb.AppendLine(Environment.NewLine);
                                    sb.Append("Value: " + v.Value.ToString());
                                    sb.Append(Environment.NewLine);
                                    sb.AppendLine(Environment.NewLine);
                                    sb.Append("--------------------------------------------------");
                                    //Console.WriteLine("{0} ({1}): {2}",
                                    //    v.Oid.ToString(),
                                    //    SnmpConstants.GetTypeName(v.Value.Type),
                                    //    v.Value.ToString());
                                    lastOid = v.Oid;
                                }
                                else
                                {
                                    // we have reached the end of the requested
                                    // MIB tree. Set lastOid to null and exit loop
                                    lastOid = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No response received from SNMP agent.");
                    }
                }
                catch { }
            }
            target.Close();
            return sb.ToString();
        }

        public DataTable testSNMP3()
        {


            DataTable table = new DataTable();
            table.Columns.Add("OID");
            table.Columns.Add("Type");
            table.Columns.Add("Value");

           
            StringBuilder sb = new StringBuilder();

            // SNMP community name
            OctetString community = new OctetString("public");

            // Define agent parameters class
            AgentParameters param = new AgentParameters(community);
            // Set SNMP version to 1
            param.Version = SnmpVersion.Ver2;
            // Construct the agent address object
            // IpAddress class is easy to use here because
            //  it will try to resolve constructor parameter if it doesn't
            //  parse to an IP address
            IpAddress agent = new IpAddress("10.240.231.253");

            // Construct target
            UdpTarget target = new UdpTarget((IPAddress)agent, 161, 2000, 1);

            // Define Oid that is the root of the MIB
            //  tree you wish to retrieve
            Oid rootOid = new Oid(".1"); // ifDescr

            // This Oid represents last Oid returned by
            //  the SNMP agent
            Oid lastOid = (Oid)rootOid.Clone();

            // Pdu class used for all requests
            Pdu pdu = new Pdu(PduType.GetNext);

            // Loop through results
            while (lastOid != null)
            {
                // When Pdu class is first constructed, RequestId is set to a random value
                // that needs to be incremented on subsequent requests made using the
                // same instance of the Pdu class.
                if (pdu.RequestId != 0)
                {
                    pdu.RequestId += 1;
                }
                // Clear Oids from the Pdu class.
                pdu.VbList.Clear();
                // Initialize request PDU with the last retrieved Oid
                pdu.VbList.Add(lastOid);
                // Make SNMP request
                SnmpV2Packet result = (SnmpV2Packet)target.Request(pdu, param);
                // You should catch exceptions in the Request if using in real application.

                // If result is null then agent didn't reply or we couldn't parse the reply.
                if (result != null)
                {
                    // ErrorStatus other then 0 is an error returned by 
                    // the Agent - see SnmpConstants for error definitions
                    if (result.Pdu.ErrorStatus != 0)
                    {
                        // agent reported an error with the request
                        Console.WriteLine("Error in SNMP reply. Error {0} index {1}",
                            result.Pdu.ErrorStatus,
                            result.Pdu.ErrorIndex);
                        lastOid = null;
                        break;
                    }
                    else
                    {
                        // Walk through returned variable bindings
                        foreach (Vb v in result.Pdu.VbList)
                        {
                            // Check that retrieved Oid is "child" of the root OID
                            if (rootOid.IsRootOf(v.Oid))
                            {
                                //Console.WriteLine("{0} ({1}): {2}",
                                //    v.Oid.ToString(),
                                //    SnmpConstants.GetTypeName(v.Value.Type),
                                //    v.Value.ToString());
                                DataRow row = table.NewRow();
                                row["OID"] = v.Oid.ToString();
                                row["Type"] = SnmpConstants.GetTypeName(v.Value.Type);
                                row["Value"] = v.Value.ToString();
                                table.Rows.Add(row);

                                //DataRow dr = new DataRow();
                                //dr.Field("ASD");
                                //dt.Rows.Add()
                                //sb.Append(Environment.NewLine);
                                //sb.AppendLine(Environment.NewLine);
                                //sb.Append("OID: " + v.Oid.ToString());
                                //sb.Append(Environment.NewLine);
                                //sb.AppendLine(Environment.NewLine);
                                //sb.Append("Name: " + SnmpConstants.GetTypeName(v.Value.Type));
                                //sb.Append(Environment.NewLine);
                                //sb.AppendLine(Environment.NewLine);
                                //sb.Append("Value: " + v.Value.ToString());
                                //sb.Append(Environment.NewLine);
                                //sb.AppendLine(Environment.NewLine);
                                //sb.Append("--------------------------------------------------");
                                lastOid = v.Oid;
                            }
                            else
                            {
                                // we have reached the end of the requested
                                // MIB tree. Set lastOid to null and exit loop
                                lastOid = null;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No response received from SNMP agent.");
                }
            }
            target.Close();
            return table;
        }
    }
}
