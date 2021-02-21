using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Hangman
{
    class Hangman
    {
        static string projPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\"; //@"C:\Users\Hayden\Documents\App_Development\Hangman\";
        static string assetPath = projPath + @"assets\";
        static string artPath = assetPath + @"art\";

        private List<char> incorrectGuesses;
        private List<char> correctGuesses;
        private int maxWordLength = 4;
        private int minWordLength = 7;

        public string SecretWord { get; }
        public bool GameOver { get; private set; }
        public bool Victory 
        { 
            get
            {
                return GameOver && incorrectGuesses.Count < 8;
            } 
        }

        public Hangman()
        {
            incorrectGuesses = new List<char>();
            correctGuesses = new List<char>();
            if (File.Exists(projPath + "config.json"))
            {
                var cfgString = File.ReadAllText(projPath + "config.json");
                var config = JsonSerializer.Deserialize<HangmanConfig>(cfgString);
                maxWordLength = config.MaxWordLength;
                minWordLength = config.MinWordLength;
                ValidateWordLengthParams();
            }
            SecretWord = GetRandomWord(assetPath + "dictionary.txt", minWordLength, maxWordLength);
            
        }

        public void Update(string input)
        {
            if (String.IsNullOrWhiteSpace(input))
                throw new ArgumentException("input string must contain at least one non-whitespace character");
            Update(input[0]);
        }

        public void Update(char input)
        {
            if (char.IsWhiteSpace(input))
                throw new ArgumentException("input char must not be whitespace or empty");
            input = char.ToUpper(input);
            if (incorrectGuesses.Contains(input) || correctGuesses.Contains(input))
                return;
            bool correct = false;
            foreach (var c in SecretWord)
            {
                if (c == input)
                {
                    correct = true;
                    break;
                }
            }
            if (correct)
            {
                correctGuesses.Add(input);
            }
            else
            {
                incorrectGuesses.Add(input);
            }
            UpdateGameOver();
        }

        public string GetState()
        {
            StringBuilder state = new StringBuilder("----HANGMAN----\n")
                .AppendLine(GetArt());
            if (!GameOver)
            {
                state.AppendLine(GetWordMask())
                    .Append(GetGuesses());
            }
            else
            {
                if (SecretWord == null)
                {
                    state.Append($"No words between {minWordLength} and {maxWordLength} letters long were found.");
                }
                else
                {
                    if (Victory)
                    {
                        state.AppendLine("YOU WIN!");
                    }
                    else
                    {
                        state.AppendLine("You Lose.");
                    }
                    state.Append($"The secret word was: {SecretWord}");
                }
            }
            return state.ToString();
        }

        public static string GetRandomWord(string dictionaryFileName, int minWordLength, int maxWordLength)
        {
            if (!File.Exists(dictionaryFileName))
                throw new ArgumentException("dictionaryFileName must reference an existing file");
            var wordList = File.ReadAllText(dictionaryFileName).Split('\n');
            var validDict = (from word in wordList
                            where word.Length >= minWordLength && word.Length <= maxWordLength
                            select word).ToArray();
            if (validDict.Length == 0)
            {
                return null;
            }
            return validDict[new Random().Next(0, validDict.Length)].ToUpper();
        }

        private string GetArt()
        {
            var artFilePath = artPath + $"hangman{incorrectGuesses.Count}.txt";
            if (!File.Exists(artFilePath))
                throw new FileNotFoundException();
            return File.ReadAllText(artFilePath);
        }

        private string GetWordMask()
        {
            var mask = new StringBuilder("Secret Word: ");
            foreach(var c in SecretWord)
            {
                if (correctGuesses.Contains(c))
                    mask.Append(c);
                else
                    mask.Append("_");
                mask.Append(" ");
            }
            return mask.ToString();
        }

        private string GetGuesses()
        {
            var guessList = new StringBuilder();
            foreach (var c in incorrectGuesses)
                guessList.Append(c)
                    .Append(", ");
            if(incorrectGuesses.Count > 0)
                guessList.Remove(guessList.Length - 2, 2);
            return guessList.ToString();
        }

        private void UpdateGameOver()
        {
            if (incorrectGuesses.Count >= 8)
            {
                GameOver = true;
                return;
            }
            int lettersRevealed = 0;
            foreach (var guess in correctGuesses)
            {
                foreach (var c in SecretWord)
                {
                    if (guess == c)
                        lettersRevealed++;
                }
            }
            GameOver = lettersRevealed >= SecretWord.Length;
        }

        private void ValidateWordLengthParams()
        {
            if(maxWordLength <= 0 || minWordLength <= 0 || maxWordLength < minWordLength)
            {
                Console.WriteLine("Invalid word length params from config file. Reverting to defaults...");
                maxWordLength = 7;
                minWordLength = 4;
            }
        }
    }
}
