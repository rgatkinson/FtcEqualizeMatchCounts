using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace FtcEqualizeMatchCounts
    {
    class ProgramOptions
        {
        public static int MatchCountDefault = 10;

        public string       DatabaseName = null;
        public int?         MatchCount = MatchCountDefault;
        public bool         ShowUsage = false;
        public List<String> ExtraOptions = new List<string>();

        private OptionSet options;

        public ProgramOptions()
            {
            options = new OptionSet
                {
                    { "c|count",  $"number of matches to equalize to (default: {MatchCountDefault})", (int count) => MatchCount = count },
                    { "m|max",    $"equalize to the maximum number of matches played by any team", (string m) => MatchCount = null },
                    { "f|file",   $"the name of the scoring database", (string f) => DatabaseName = f },
                    { "h|help|?", $"show this message and exit", (string h) => ShowUsage = h != null },
                };
            }

        public void Parse(string[] args)
            {
            try
                {
                ExtraOptions = options.Parse(args);
                if (DatabaseName == null && ExtraOptions.Count > 0)
                    {
                    DatabaseName = ExtraOptions[0];
                    ExtraOptions.RemoveAt(0);
                    }
                if (ShowUsage)
                    {
                    Usage(null);
                    }
                Validate();
                }
            catch (OptionException e)
                {
                Usage(e);
                }
            }

        private void Validate()
            {
            if (ExtraOptions.Count > 0)
                {
                Throw($"{ ExtraOptions.Count } extra options given");
                }
            if (MatchCount.HasValue && MatchCount < 0)
                {
                Throw($"invalid match count: { MatchCount }", "c");
                }
            }

        private void Throw(string message, string optionName = null)
            {
            throw new OptionException(options.MessageLocalizer($"Error: { message }"), optionName);
            }


        private void Usage(OptionException e)
            {
            TextWriter output = Console.Error;

            if (e != null)
                {
                output.WriteLine(e.Message);
                }

            output.WriteLine($"Usage: { ProgramName } [OPTIONS]* scoringDatabase.db:");
            output.WriteLine("Equalize the number of matches played by all teams in the FTC scoring database");
            output.WriteLine("by adding zero-scoring surrogate matches as necessary.");
            output.WriteLine();
            output.WriteLine("Options:");
            options.WriteOptionDescriptions(output);
            Environment.Exit(e == null ? 0 : -1);
            }

        private String ProgramName => Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
        }



    class Program
        {
        ProgramOptions programOptions = new ProgramOptions();

        void DoMain(string[] args)
            {
            programOptions.Parse(args);
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }
        }
    }
