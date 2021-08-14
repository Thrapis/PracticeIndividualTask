using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace MaterailReport.Models.ImageTools
{
    public static class ImageHelper
    {
        public static byte[] ImageToByteArray(this Image imageIn)
        {
            using (var ms = new MemoryStream())
            {
                imageIn.Save(ms, imageIn.RawFormat);
                return ms.ToArray();
            }
        }

        public static string GetImageForPage(this Image photo)
        {
            byte[] binary = photo.ImageToByteArray();
            string base64String = Convert.ToBase64String(binary, 0, binary.Length);
            return "data:image/jpg;base64," + base64String;
        }
    }
}