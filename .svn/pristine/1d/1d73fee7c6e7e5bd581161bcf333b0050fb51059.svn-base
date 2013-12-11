using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LexicalAnalysis2
{
    class FileOperation
    {

        public string[] readFile(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            List<string> list = new List<string> { };
            try
            {
                string aLine = "";
                while (aLine != null)
                {
                    aLine = sr.ReadLine();
                    list.Add(aLine);
                }
                list.RemoveAt(list.Count - 1);
                return list.ToArray();
            }
            catch
            {
                list.Add("nothing");
                return list.ToArray();
            }
            finally
            {
                sr.Close();
            }
        }
        public bool writeStringToFile(string filePath, string str)
        {
            FileStream fs = new FileStream(filePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            try
            {
                if (filePath == "SystemSymbols.txt" && fs.Length > 0)
                {
                    return true;
                }
                sw.Write(str);
                sw.Flush();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                sw.Close();
                fs.Close();
            }
        }
        public string readAllText(string filePath)
        {
            StreamReader sr = new StreamReader(filePath);
            try
            {
                string list = sr.ReadToEnd();
                return list;
            }
            catch
            {
                return null;
            }
            finally
            {
                sr.Close();
            }
        }
    }
}
