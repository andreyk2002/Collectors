using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Collectors.Classes
{
    public class ImageProcessor
    {
        public byte[] ConvertImageToBytes(IFormFile Image)
        {
            byte[] imageData = null;
            using (var binaryReader = new BinaryReader(Image.OpenReadStream()))
            {
                imageData = binaryReader.ReadBytes((int)Image.Length);
            }
            return imageData;
        }
    }
}
