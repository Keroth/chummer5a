﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Windows.Forms;

namespace Chummer
{
	static class Log
	{
		private static StreamWriter logWriter;
		private static StringBuilder stringBuilder;  //This will break in case of multithreading
		private static bool logEnabled = false;
		static Log()
		{
			if (GlobalOptions.Instance.UseLogging)
			{
				//TODO: Add listner to UseLogging to be able to start it mid run
				string strFile = Path.Combine(Environment.CurrentDirectory, "chummerlog.txt");
				logWriter = new StreamWriter(strFile);
				stringBuilder = new StringBuilder();
				logEnabled = true;
			}
		}

		/// <summary>
		/// This will disabled logging and free any resources used by it
		/// </summary>
		public static void Kill()
		{
			logWriter.Flush();
			logWriter.Close();
			logEnabled = false;
		}

		/// <summary>
		/// Log that the execution path is entering a method
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Enter
		(
			object[] info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
		)
		{
			writeLog(info, file, method, line, "Entering ");
		}

		/// <summary>
		/// Log that the execution path is entering a method
		/// </summary>
		/// <param name="info">An optional object providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Enter
		(
			string info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
		)
		{
			writeLog(new object[] {info}, file, method, line, "Entering ");
		}

		/// <summary>
		/// Log that the execution path is entering a method
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Exit
		(
			string info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
		)
		{
			writeLog(new object[]{info},file, method, line, "Exiting   ");
		}

		/// <summary>
		/// Log that an error occoured
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Error
			(
			object[] info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
			)
		{
			writeLog(info,file, method, line, "Error     ");
		}

		/// <summary>
		/// Log that an error occoured
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Error
			(
			object info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
			)
		{
			writeLog(new object[]{info},file, method, line, "Error     ");
		}

		/// <summary>
		/// Log an exception has occoured
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Exception
		(
			Exception exception
		)
		{
			if(!logEnabled)
				return;

			writeLog(
				new object[]{exception, exception.StackTrace},
				exception.Source, 
				exception.TargetSite.Name, 
				(new StackTrace(exception, true)).GetFrame(0).GetFileLineNumber(), 
				"Exception ");
		}

		/// <summary>
		/// Log a warning
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Warning
			(
			object[] info= null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
			)
		{
			writeLog(info, file, method, line, "Warning   ");
		}

		/// <summary>
		/// Log a warning
		/// </summary>
		/// <param name="info">An optional object providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Warning
			(
			object info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
			)
		{
			writeLog(new object[]{info},file, method, line, "Warning   ");
		}

		/// <summary>
		/// Log some info
		/// </summary>
		/// <param name="info">An optional object providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Info
			(
			object[] info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
			)
		{
			writeLog(info,file, method, line, "Info      ");
		}

		/// <summary>
		/// Log some info
		/// </summary>
		/// <param name="info">An optional array of objects providing additional data</param>
		/// <param name="file">Do not use this</param>
		/// <param name="method">Do not use this</param>
		/// <param name="line">Do not use this</param>
		public static void Info
			(
			String info = null,
#if LEGACY
			string file = "LEGACY",
			string method = "LEGACY",
			int line = 0
#else
			[CallerFilePath] string file = "",
			[CallerMemberName] string method = "",
			[CallerLineNumber] int line = 0
#endif
			)
		{
			writeLog(new object[]{info},file, method, line, "Info      ");
		}

		private static void writeLog(object[] info, string file, string method, int line, string pre)
		{
			if (!logEnabled)
				return;

			//TODO: Add timestamp to logs

			stringBuilder.Clear();
			stringBuilder.Append(pre);
			string[] classPath = file.Split(new char[] {'\\', '/'});
			stringBuilder.Append(classPath[classPath.Length - 1]);
			stringBuilder.Append(".");
			stringBuilder.Append(method);
			stringBuilder.Append(":");
			stringBuilder.Append(line);

			if (info != null)
			{
				stringBuilder.Append(" ");
				foreach (object o in info)
				{
					stringBuilder.Append(o);
					stringBuilder.Append(", ");
				}

				stringBuilder.Length -= 2;
			}

			logWriter.WriteLine(stringBuilder.ToString());
		}

		public static void FirstChanceException(object sender, FirstChanceExceptionEventArgs e)
		{
			if (!logEnabled)
				return;

			logWriter.WriteLine("First chance exception: " +e.Exception);
		}
	}
}
