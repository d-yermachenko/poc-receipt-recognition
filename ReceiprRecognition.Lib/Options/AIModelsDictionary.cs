namespace ReceiptRecognition.Ollama.Options;

internal class AIModelsDictionary
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    public const string LlamaVision = "llama3.2-vision:latest";

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public const string MiniCPM = "minicpm-v:latest";


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Best model for direct conversion from image to json. But lacks performance
    /// </remarks>
    public const string Qwen = "qwen2.5vl:7b";

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    public const string Llava7b = "llava:7b";

    /// <summary>
    /// A compact and efficient vision-language model, specifically designed for visual document understanding, enabling automated content extraction from tables, charts, infographics, plots, diagrams, and more.
    /// </summary>
    /// <remarks>
    /// Halutinating
    /// </remarks>
    /// <see cref="https://ollama.com/library/granite3.2-vision"/>
    public const string Granite = "granite3.2-vision";

    public class Llama
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string V3_1B8 = "llama3.1:8b";

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string V3_2Vision = "llama3.2-vision:latest";
    }

    public class Gemma
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string Latest = "gemma3:latest";

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string B4 = "gemma3:4b";

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// </remarks>
        public const string B12 = "gemma3:12b";

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Too slow
        /// </remarks>
        [Obsolete("Model to slow on Nvidia3070")]
        public const string B27 = "gemma3:27b";


    }
    




}
