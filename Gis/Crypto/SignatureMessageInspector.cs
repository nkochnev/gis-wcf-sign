using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Xml;
using Xades.Implementations;

namespace Gis.Crypto
{
	public class SignatureMessageInspector : IClientMessageInspector
	{
		public object BeforeSendRequest(ref Message request, IClientChannel channel)
		{
			//remove extra tags VsDebuggerCausalityData
			int limit = request.Headers.Count;
			for (int i = 0; i < limit; ++i)
			{
				if (request.Headers[i].Name.Equals("VsDebuggerCausalityData"))
				{
					request.Headers.RemoveAt(i);
					break;
				}
			}

			string st = GetSignElement(MessageString(ref request));

			//place for log request

			request = CreateMessageFromString(st, request.Version);

			return null;
		}

		public void AfterReceiveReply(ref Message reply, object correlationState)
		{
			string st = MessageString(ref reply);

			//place for log response

			reply = CreateMessageFromString(st, reply.Version);
		}

		public static string GetSignElement(string messageString)
		{
			var originalDoc = new XmlDocument { PreserveWhitespace = true };
			originalDoc.LoadXml(messageString);

			var nodes = originalDoc.SelectNodes($"//node()[@Id='{CryptoConsts.CONTAINER_ID}']");
			if (nodes == null || nodes.Count == 0)
			{
				return originalDoc.OuterXml;
			}

			var gostXadesBesService = new GostXadesBesService();

			string st = gostXadesBesService.Sign(messageString, CryptoConsts.CONTAINER_ID, CryptoConsts.CERTIFICATE_THUMBPRINT, string.Empty);

			return st;
		}

		Message CreateMessageFromString(String xml, MessageVersion ver)
		{
			return Message.CreateMessage(XmlReaderFromString(xml), int.MaxValue, ver);
		}

		XmlReader XmlReaderFromString(String xml)
		{
			var stream = new MemoryStream();
			// NOTE: don't use using(var writer ...){...}
			//  because the end of the StreamWriter's using closes the Stream itself.
			//
			var writer = new StreamWriter(stream);
			writer.Write(xml);
			writer.Flush();
			stream.Position = 0;
			return XmlReader.Create(stream);
		}

		String MessageString(ref Message m)
		{
			// copy the message into a working buffer.
			MessageBuffer mb = m.CreateBufferedCopy(int.MaxValue);

			// re-create the original message, because "copy" changes its state.
			m = mb.CreateMessage();

			Stream s = new MemoryStream();
			XmlWriter xw = XmlWriter.Create(s);
			mb.CreateMessage().WriteMessage(xw);
			xw.Flush();
			s.Position = 0;

			byte[] bXml = new byte[s.Length];
			s.Read(bXml, 0, (int) s.Length);

			// sometimes bXML[] starts with a BOM
			if (bXml[0] != (byte) '<')
			{
				return Encoding.UTF8.GetString(bXml, 3, bXml.Length - 3);
			}
			return Encoding.UTF8.GetString(bXml, 0, bXml.Length);
		}
	}
}