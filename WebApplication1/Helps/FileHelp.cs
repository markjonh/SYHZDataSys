using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace System
{
    public static  class FileHelp
    {
       
        public static byte[] ReadImageFile(string path)
        {
            
            FileStream fs = File.OpenRead(path); //OpenRead
            byte[] byData = new byte[fs.Length];
            fs.Read(byData, 0, byData.Length);
            fs.Close();

            return byData;
        }
    }
}