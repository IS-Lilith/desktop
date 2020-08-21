using System;
using System.IO;

namespace AlipayBank.Web.utils
{
	public class WritLog
	{
		public static void Info(string log)
		{
			string path = "C:\\NLog\\" + DateTime.Now.ToString("yyyy-MM-dd") + "日志.txt";
			StreamWriter streamWriter = null;
			if (!File.Exists(path))
			{
				FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
				streamWriter = new StreamWriter(fileStream);
				streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + log + "\r\n");
				streamWriter.Close();
				fileStream.Close();
			}
			else
			{
				streamWriter = File.AppendText(path);
				streamWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + log);
				streamWriter.Close();
			}
		}
	}
}
