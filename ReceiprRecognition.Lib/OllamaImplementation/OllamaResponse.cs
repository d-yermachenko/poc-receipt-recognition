using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ReceiptRecognition.Lib.OllamaImplementation;

public record OllamaResponse(string model,
        DateTime created_at,
        string response,
        bool done,
        int[] context,
        long total_duration,
        long load_duration,
        int prompt_eval_count,
        long prompt_eval_duration,
        int eval_count,
        long eval_duration);

