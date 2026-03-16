using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptRecognition.Core;

public interface IImagePreprocessor
{
    Task<Stream> PreprocessImageAsync(Stream stream);
}
