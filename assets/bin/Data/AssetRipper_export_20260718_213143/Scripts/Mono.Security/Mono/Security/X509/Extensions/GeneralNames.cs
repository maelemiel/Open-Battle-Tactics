using System;
using System.Collections;
using System.Text;

namespace Mono.Security.X509.Extensions
{
	internal class GeneralNames
	{
		private ArrayList rfc822Name;

		private ArrayList dnsName;

		private ArrayList directoryNames;

		private ArrayList uris;

		private ArrayList ipAddr;

		private ASN1 asn;

		public string[] RFC822
		{
			get
			{
				if (rfc822Name == null)
				{
					return new string[0];
				}
				return (string[])rfc822Name.ToArray(typeof(string));
			}
		}

		public string[] DirectoryNames
		{
			get
			{
				if (directoryNames == null)
				{
					return new string[0];
				}
				return (string[])directoryNames.ToArray(typeof(string));
			}
		}

		public string[] DNSNames
		{
			get
			{
				if (dnsName == null)
				{
					return new string[0];
				}
				return (string[])dnsName.ToArray(typeof(string));
			}
		}

		public string[] UniformResourceIdentifiers
		{
			get
			{
				if (uris == null)
				{
					return new string[0];
				}
				return (string[])uris.ToArray(typeof(string));
			}
		}

		public string[] IPAddresses
		{
			get
			{
				if (ipAddr == null)
				{
					return new string[0];
				}
				return (string[])ipAddr.ToArray(typeof(string));
			}
		}

		public GeneralNames()
		{
		}

		public GeneralNames(string[] rfc822s, string[] dnsNames, string[] ipAddresses, string[] uris)
		{
			asn = new ASN1(48);
			if (rfc822s != null)
			{
				rfc822Name = new ArrayList();
				foreach (string s in rfc822s)
				{
					asn.Add(new ASN1(129, Encoding.ASCII.GetBytes(s)));
					rfc822Name.Add(rfc822s);
				}
			}
			if (dnsNames != null)
			{
				dnsName = new ArrayList();
				foreach (string text in dnsNames)
				{
					asn.Add(new ASN1(130, Encoding.ASCII.GetBytes(text)));
					dnsName.Add(text);
				}
			}
			if (ipAddresses != null)
			{
				ipAddr = new ArrayList();
				foreach (string text2 in ipAddresses)
				{
					string[] array = text2.Split('.', ':');
					byte[] array2 = new byte[array.Length];
					for (int l = 0; l < array.Length; l++)
					{
						array2[l] = byte.Parse(array[l]);
					}
					asn.Add(new ASN1(135, array2));
					ipAddr.Add(text2);
				}
			}
			if (uris != null)
			{
				this.uris = new ArrayList();
				foreach (string text3 in uris)
				{
					asn.Add(new ASN1(134, Encoding.ASCII.GetBytes(text3)));
					this.uris.Add(text3);
				}
			}
		}

		public GeneralNames(ASN1 sequence)
		{
			for (int i = 0; i < sequence.Count; i++)
			{
				switch (sequence[i].Tag)
				{
				case 129:
					if (rfc822Name == null)
					{
						rfc822Name = new ArrayList();
					}
					rfc822Name.Add(Encoding.ASCII.GetString(sequence[i].Value));
					break;
				case 130:
					if (dnsName == null)
					{
						dnsName = new ArrayList();
					}
					dnsName.Add(Encoding.ASCII.GetString(sequence[i].Value));
					break;
				case 132:
				case 164:
					if (directoryNames == null)
					{
						directoryNames = new ArrayList();
					}
					directoryNames.Add(X501.ToString(sequence[i][0]));
					break;
				case 134:
					if (uris == null)
					{
						uris = new ArrayList();
					}
					uris.Add(Encoding.ASCII.GetString(sequence[i].Value));
					break;
				case 135:
				{
					if (ipAddr == null)
					{
						ipAddr = new ArrayList();
					}
					byte[] value = sequence[i].Value;
					string value2 = ((value.Length != 4) ? ":" : ".");
					StringBuilder stringBuilder = new StringBuilder();
					for (int j = 0; j < value.Length; j++)
					{
						stringBuilder.Append(value[j].ToString());
						if (j < value.Length - 1)
						{
							stringBuilder.Append(value2);
						}
					}
					ipAddr.Add(stringBuilder.ToString());
					if (ipAddr == null)
					{
						ipAddr = new ArrayList();
					}
					break;
				}
				}
			}
		}

		public byte[] GetBytes()
		{
			return asn.GetBytes();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (rfc822Name != null)
			{
				foreach (string item in rfc822Name)
				{
					stringBuilder.Append("RFC822 Name=");
					stringBuilder.Append(item);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (dnsName != null)
			{
				foreach (string item2 in dnsName)
				{
					stringBuilder.Append("DNS Name=");
					stringBuilder.Append(item2);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (directoryNames != null)
			{
				foreach (string directoryName in directoryNames)
				{
					stringBuilder.Append("Directory Address: ");
					stringBuilder.Append(directoryName);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (uris != null)
			{
				foreach (string uri in uris)
				{
					stringBuilder.Append("URL=");
					stringBuilder.Append(uri);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			if (ipAddr != null)
			{
				foreach (string item3 in ipAddr)
				{
					stringBuilder.Append("IP Address=");
					stringBuilder.Append(item3);
					stringBuilder.Append(Environment.NewLine);
				}
			}
			return stringBuilder.ToString();
		}
	}
}
