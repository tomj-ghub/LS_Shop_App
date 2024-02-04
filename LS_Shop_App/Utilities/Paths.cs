using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LS_Shop_App.Utilities
{
    public class Paths
    {
        private static readonly string AppName = Assembly.GetCallingAssembly().GetName().Name.ToString();
        private static readonly string PublicDocs = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

        //directories
        public static readonly string DataDir = $@"{PublicDocs}\{AppName}\Data\";
        public static readonly string PickListDir = $@"{PublicDocs}\{AppName}\PickList2Import\";
        public static readonly string Boards2Print = $@"{PublicDocs}\{AppName}\Boards2Print\";
        public static readonly string ImagesDir = $@"{PublicDocs}\{AppName}\Sign_Images\";
        public static readonly string ErrorDir = $@"{PublicDocs}\{AppName}\Errors\";

        //files
        public static readonly string DatabaseFile = $@"{PublicDocs}\{AppName}\Data\SQLite_Database.sqlite";
    }
}
