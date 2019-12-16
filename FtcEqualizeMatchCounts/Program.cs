using System;
using System.Collections.Generic;
using System.IO;
using Mono.Options;

namespace FtcEqualizeMatchCounts
    {
    class ProgramOptions
        {
        public static int MatchCountDefault = 10;

        public string       Filename = null;
        public int?         MatchCount = MatchCountDefault;
        public bool         ShowUsage = false;
        public List<String> ExtraOptions = new List<string>();
        public bool         ReportOnly = false;

        public string       EventCode = null;

        private OptionSet options;

        public ProgramOptions()
            {
            options = new OptionSet
                {
                    { "c=|count=", $"equalize to the indicated number of matches; default: {MatchCountDefault}", (int count) => MatchCount = count },
                    { "m|max",     $"equalize to the maximum number of matches already played by any team", (string m) => MatchCount = null },
                    { "f=|file=",  $"the name of the scoring database", (string f) => Filename = f },
                    { "r|report",  $"make no changes; just report on the current contents of the database", (string r) => ReportOnly = r != null },
                    { "h|help|?",  $"show this message and exit", (string h) => ShowUsage = h != null },
                };
            }

        public void Parse(string[] args)
            {
            try
                {
                ExtraOptions = options.Parse(args);
                if (Filename == null && ExtraOptions.Count > 0)
                    {
                    Filename = ExtraOptions[0];
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
                Throw($"invalid match count: { MatchCount }", "-c");
                }
            if (Filename == null)
                {
                Throw($"name of database not given", "-f");
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

            using (Database db = new Database(programOptions.Filename))
                {
                var table = new Table_matchSchedule(db);
                var values = table.SelectAll();
                }
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }
        }
    }
