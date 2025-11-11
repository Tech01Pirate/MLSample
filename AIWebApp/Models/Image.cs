using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIWebApp.Models
{
    /// <summary>
    /// Class representing an image.
    /// </summary>
    public class Image
    {
        public string Content { get; set; }

        /// <summary>
        /// Constructor for the Image class.
        /// </summary>
        /// <param name="content">The base64-encoded content of the image.</param>
        public Image(string content)
        {
            Content = content;
        }
    }
}
