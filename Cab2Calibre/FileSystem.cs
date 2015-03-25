namespace Cab2Calibre
{
    using System.Collections.Generic;
    using System.IO;

    public class FileSystem
    {
        public void CopyFile(string sourceFileName, string destFileName)
        {
            if (!File.Exists(destFileName))
            {
                File.Copy(sourceFileName, destFileName);
            }
        }

        public void MakeDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}