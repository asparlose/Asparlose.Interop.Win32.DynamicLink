using System;
using System.IO;

namespace Asparlose.Interop.Win32
{
    public abstract class DynamicLinkFromBinary : DynamicLink
    {
        static string CreateTemporaryFile()
        {
            var tempPath = Path.GetTempPath();

            while (true)
            {
                var filename = Path.Combine(tempPath, Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".dll");
                try
                {
                    using (var s = File.Open(filename, FileMode.CreateNew))
                    {
                        return filename;
                    }
                }
                catch (Exception) { }
            }
        }

        static string WriteToTemporaryFile(byte[] bytes)
        {
            var fileName = CreateTemporaryFile();
            using (var s = File.Create(fileName))
            {
                s.Write(bytes, 0, bytes.Length);
            }
            return fileName;
        }

        protected DynamicLinkFromBinary(byte[] dll) : base(WriteToTemporaryFile(dll))
        {
            Disposed += DynamicLinkFromBinary_Disposed;
        }

        private void DynamicLinkFromBinary_Disposed(object sender, EventArgs e)
        {
            File.Delete(FilePath);
        }
    }
}
