using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Hangman
{
    class HangmanConfig
    {
        [JsonInclude]
        public int MaxWordLength { get; set; }

        [JsonInclude]
        public int MinWordLength { get; set; }
    }
}
