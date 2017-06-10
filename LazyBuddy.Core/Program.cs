using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Fiddler;
using Newtonsoft.Json;

namespace LazyBuddy.Core
{
	// Token: 0x02000002 RID: 2
	internal class Program
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		private static void Main(string[] args)
		{
			Console.Title = Program.RandomASCIIString(10) + Program.RandomGUIDString(10) + Program.RandomASCIIString(10);
			Program.InstallCertificate();
			string[] array = args[0].Split(new char[]
			{
				'|'
			});
			string user = array[0];
			string pass = array[1];
			string path = array[2];
			Program.Setup(user, pass, path);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x000020B0 File Offset: 0x000002B0
		private static void CallbackLoader(Process p)
		{
			while (true)
			{
				Thread.Sleep(1500);
				bool hasExited = p.HasExited;
				if (hasExited)
				{
					bool flag = FiddlerApplication.IsStarted() && Program.oSecureEndpoint != null;
					if (flag)
					{
						FiddlerApplication.Shutdown();
						Program.oSecureEndpoint.Dispose();
					}
					Thread.Sleep(1000);
					Environment.Exit(0);
				}
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x00002118 File Offset: 0x00000318
		private static byte[] RandomizeByte(byte[] array)
		{
			List<byte> list = array.ToList<byte>();
			int num = Program.rand2.Next(5000000, 15000000);
			for (int i = 0; i < num; i++)
			{
				list.Add(0);
			}
			return list.ToArray();
		}

		// Token: 0x06000004 RID: 4 RVA: 0x0000216C File Offset: 0x0000036C
		public static bool InstallCertificate()
		{
			bool flag = !CertMaker.rootCertExists();
			bool result;
			if (flag)
			{
				bool flag2 = !CertMaker.createRootCert();
				if (flag2)
				{
					result = false;
					return result;
				}
				bool flag3 = !CertMaker.trustRootCert();
				if (flag3)
				{
					result = false;
					return result;
				}
			}
			result = true;
			return result;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000021B0 File Offset: 0x000003B0
		private static void CreateLocalServer(Session oSession, string serverUrl, byte[] data)
		{
			bool flag = oSession.get_fullUrl().Contains(serverUrl);
			if (flag)
			{
				oSession.bBufferResponse = true;
				oSession.utilCreateResponseAndBypassServer();
				oSession.oResponse.get_headers().HTTPResponseStatus = "200 Ok";
				oSession.oResponse.get_headers().set_Item("Content-Length", ((long)data.Length).ToString());
				oSession.oResponse.set_Item("Content-Type", "application/x-msdownload");
				oSession.set_ResponseBody(data);
			}
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002234 File Offset: 0x00000434
		private static void Uyarı(string text)
		{
			bool flag = File.Exists("warn.txt");
			if (flag)
			{
				File.Delete("warn.txt");
			}
			File.WriteAllText("warn.txt", text);
			Process.Start("warn.txt");
			Application.Exit();
		}

		// Token: 0x06000007 RID: 7 RVA: 0x00002278 File Offset: 0x00000478
		private static bool GetData(string key)
		{
			string address = string.Format("https://leakod.com/auth/lazy/getData.php?key={0}", key);
			string data = new WebClient().DownloadString(address);
			string @string = Program.GetString("쮣향\udca7囹즫춭얯삱톳ﶵ\uddb7쎹", 0);
			string text = Program.DecryptData(data, Program.MD5(key + @string));
			bool flag = !text.Contains("Depen");
			bool result;
			if (flag)
			{
				MessageBox.Show("Invalid data");
				MessageBox.Show(text);
				result = false;
			}
			else
			{
				Data data2 = JsonConvert.DeserializeObject<Data>(text);
				Program.dataKek = data2;
				result = true;
			}
			return result;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002300 File Offset: 0x00000500
		private static string GetKey(string username, string password)
		{
			string arg = Program.HWID();
			string address = string.Format("https://leakod.com/auth/lazy/getKey.php?user={0}&pass={1}&hwid={2}", username, password, arg);
			string text = new WebClient().DownloadString(address);
			bool flag = !text.Contains('|');
			string result;
			if (flag)
			{
				MessageBox.Show(text);
				result = string.Empty;
			}
			else
			{
				string[] array = text.Split(new char[]
				{
					'|'
				});
				string str = array[1];
				string text2 = array[0];
				bool flag2 = !Program.IsGuid(text2);
				if (flag2)
				{
					MessageBox.Show("Key type is incorrect");
					result = string.Empty;
				}
				else
				{
					MessageBox.Show("Your remaining days => " + str + " days");
					result = text2;
				}
			}
			return result;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000023B0 File Offset: 0x000005B0
		private static string RandomASCIIString(int len)
		{
			char[] array = new char[len];
			string text = "¢¤¥§¶¼¾æð™š•ÆÁØêëùñð¢¤¥§¶¼¾æð™š•ÆÁØêëùñð¢¤¥§¶¼¾æð™š•ÆÁØêëùñð¢¤¥§¶¼¾æð™š•ÆÁØêëùñð";
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = text[Program.rand.Next(text.Length)];
			}
			return new string(array);
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002404 File Offset: 0x00000604
		private static bool IsGuid(string value)
		{
			Guid guid;
			return Guid.TryParse(value, out guid);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x00002420 File Offset: 0x00000620
		private static string RandomGUIDString(int len)
		{
			byte[] inArray = Guid.NewGuid().ToByteArray();
			string text = Convert.ToBase64String(inArray).Replace("=", "").Replace("+", "").Replace("/", "");
			bool flag = len == 0;
			string result;
			if (flag)
			{
				result = text;
			}
			else
			{
				bool flag2 = len > text.Length;
				if (flag2)
				{
					result = text;
				}
				else
				{
					result = text.Substring(0, len);
				}
			}
			return result;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x000024A0 File Offset: 0x000006A0
		private static string HWID()
		{
			ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select ProcessorID From Win32_processor");
			ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			string result;
			using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectCollection.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ManagementObject managementObject = (ManagementObject)enumerator.Current;
					result = managementObject["ProcessorID"].ToString();
					return result;
				}
			}
			result = null;
			return result;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x00002518 File Offset: 0x00000718
		private static string DecryptData(string data, string key)
		{
			RijndaelManaged rijndaelManaged = new RijndaelManaged();
			rijndaelManaged.Padding = PaddingMode.None;
			rijndaelManaged.Mode = CipherMode.ECB;
			rijndaelManaged.KeySize = 256;
			rijndaelManaged.BlockSize = 256;
			rijndaelManaged.Key = Encoding.ASCII.GetBytes(key);
			rijndaelManaged.IV = Encoding.ASCII.GetBytes(Program.MD5(Program.HWID()));
			ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
			byte[] bytes = cryptoTransform.TransformFinalBlock(Convert.FromBase64String(data), 0, Convert.FromBase64String(data).Length);
			return Encoding.ASCII.GetString(bytes);
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000025BC File Offset: 0x000007BC
		private static string MD5(string metin)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] array = Encoding.UTF8.GetBytes(metin);
			array = mD5CryptoServiceProvider.ComputeHash(array);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				byte b = array2[i];
				stringBuilder.Append(b.ToString("x2").ToLower());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000262C File Offset: 0x0000082C
		private static string SolveData(string base64)
		{
			return Encoding.ASCII.GetString(Convert.FromBase64String(base64));
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002650 File Offset: 0x00000850
		[MethodImpl(MethodImplOptions.NoInlining)]
		internal static string GetString(string text, int num)
		{
			int num2 = 806958241 + num;
			char[] array = text.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				char[] array2 = array;
				int num3 = i;
				char c = array[i];
				array2[num3] = (char)(((int)(c & 'ÿ') ^ num2++) << 8 | (int)((byte)((int)(c >> 8) ^ num2++)));
			}
			return string.Intern(new string(array));
		}

		// Token: 0x06000011 RID: 17 RVA: 0x000026C0 File Offset: 0x000008C0
		private static void Setup(string user, string pass, string path)
		{
			try
			{
				string key = Program.GetKey(user, pass);
				bool flag = !Program.GetData(key);
				if (flag)
				{
					string path2 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LazyBuddy.ini");
					bool flag2 = File.Exists(path2);
					if (flag2)
					{
						File.Delete(path2);
					}
					Program.Uyarı("There has been error while track data from net");
				}
				byte[] Eb = Program.RandomizeByte(Convert.FromBase64String(Program.dataKek.Eb));
				byte[] Core = Program.RandomizeByte(Convert.FromBase64String(Program.dataKek.Core));
				string Dependencies = Program.SolveData(Program.dataKek.Dependencies);
				string News = Program.SolveData(Program.dataKek.News);
				string Aka = Program.SolveData(Program.dataKek.Aka);
				string OKTWAuthSite = "oktw.me/test1.php";
				string OKTWAuthSite2 = "oktw.hekko24.pl/hekko.php";
				string EvadeAuthSite = "oktw.me/test5.php";
				string EvadeAuthSite2 = "oktw.hekko24.pl/authEvade.php";
				FiddlerApplication.add_BeforeRequest(delegate(Session oS)
				{
					oS.bBufferResponse = (oS.get_fullUrl().Contains("EloBuddy/EloBuddy.Dependencies") || oS.get_fullUrl().Contains("oktw.me/") || oS.get_fullUrl().Contains("oktw.hekko24.pl/") || oS.get_fullUrl().Contains("akaeb.com/connect.php"));
					Program.CreateLocalServer(oS, "lazybuddy.ca/eb.dll", Eb);
					Program.CreateLocalServer(oS, "lazybuddy.ca/core.dll", Core);
					bool flag4 = oS.get_fullUrl().Contains(OKTWAuthSite) || oS.get_fullUrl().Contains(OKTWAuthSite2);
					if (flag4)
					{
						string[] array = oS.get_fullUrl().Split(new char[]
						{
							'?'
						});
						string arg = array[1];
						string address = string.Format("https://leakod.com/auth/lazy/oktw.php?{0}&key={1}", arg, key);
						string kek = new WebClient().DownloadString(address);
						Program.Kek = kek;
					}
					bool flag5 = oS.get_fullUrl().Contains(EvadeAuthSite) || oS.get_fullUrl().Contains(EvadeAuthSite2);
					if (flag5)
					{
						string[] array2 = oS.get_fullUrl().Split(new char[]
						{
							'?'
						});
						string arg2 = array2[1];
						string address2 = string.Format("https://leakod.com/auth/lazy/evade.php?{0}&key={1}", arg2, key);
						string kek2 = new WebClient().DownloadString(address2);
						Program.Kek2 = kek2;
					}
				});
				FiddlerApplication.add_BeforeResponse(delegate(Session oS)
				{
					bool flag4 = oS.get_fullUrl().Contains("EloBuddy/EloBuddy.Dependencies");
					if (flag4)
					{
						oS.utilDecodeResponse();
						bool flag5 = oS.get_fullUrl().Contains("dependencies.json");
						if (flag5)
						{
							oS.utilSetResponseBody(Dependencies);
						}
						bool flag6 = oS.get_fullUrl().Contains("news.json");
						if (flag6)
						{
							oS.utilSetResponseBody(News);
						}
					}
					bool flag7 = oS.get_fullUrl().Contains("oktw.me/") || oS.get_fullUrl().Contains("oktw.hekko24.pl/");
					if (flag7)
					{
						oS.utilDecodeResponse();
						bool flag8 = oS.get_fullUrl().Contains(OKTWAuthSite) || oS.get_fullUrl().Contains(OKTWAuthSite2);
						if (flag8)
						{
							oS.utilSetResponseBody(Program.Kek);
						}
						bool flag9 = oS.get_fullUrl().Contains(EvadeAuthSite) || oS.get_fullUrl().Contains(EvadeAuthSite2);
						if (flag9)
						{
							oS.utilSetResponseBody(Program.Kek2);
						}
					}
					bool flag10 = oS.get_fullUrl().Contains("akaeb.com/connect.php");
					if (flag10)
					{
						oS.utilDecodeResponse();
						oS.utilSetResponseBody(Aka);
					}
				});
				CONFIG.set_IgnoreServerCertErrors(true);
				FiddlerApplication.Startup(5216, true, true);
				Program.oSecureEndpoint = FiddlerApplication.CreateProxyEndpoint(Program.port, true, Program.host);
				Console.WriteLine("Starting EloBuddy.Loader.exe..");
				Thread.Sleep(2000);
				IntPtr consoleWindow = Program.GetConsoleWindow();
				Process p = Process.Start(path);
				Program.ShowWindow(consoleWindow, 0);
				Program.CallbackLoader(p);
				object obj = new object();
				object obj2 = obj;
				lock (obj2)
				{
					Monitor.Wait(obj);
				}
			}
			catch (Exception ex)
			{
				Program.Uyarı(ex.ToString());
			}
		}

		// Token: 0x06000012 RID: 18
		[DllImport("kernel32.dll")]
		private static extern IntPtr GetConsoleWindow();

		// Token: 0x06000013 RID: 19
		[DllImport("user32.dll")]
		private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		// Token: 0x04000001 RID: 1
		private static Random rand2 = new Random();

		// Token: 0x04000002 RID: 2
		private static Proxy oSecureEndpoint = null;

		// Token: 0x04000003 RID: 3
		private static string host = "localhost";

		// Token: 0x04000004 RID: 4
		private static int port = 5216;

		// Token: 0x04000005 RID: 5
		private static Data dataKek = null;

		// Token: 0x04000006 RID: 6
		private static Random rand = new Random();

		// Token: 0x04000007 RID: 7
		private const int SW_HIDE = 0;

		// Token: 0x04000008 RID: 8
		private static string Kek = string.Empty;

		// Token: 0x04000009 RID: 9
		private static string Kek2 = string.Empty;
	}
}
