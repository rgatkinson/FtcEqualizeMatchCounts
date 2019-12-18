using Mono.Options;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FEMC
    {
    class ProgramOptions
        {
        public string       Filename = null;
        public int          AveragingMatchCountGoal = 10;
        public bool         AverageToExistingMax = true;
        public bool         ShowUsage = false;
        public bool         Verbose = false;
        public List<String> ExtraOptions = new List<string>();
        public string       IndentString = "   ";

        public string       EventCode = null;

        public IndentedTextWriter StdOut;
        public IndentedTextWriter StdErr;

        private OptionSet options;
        private Program program;

        public ProgramOptions(Program program)
            {
            this.program = program;
            options = new OptionSet
                {
                    { "m|max",     $"equalize to the maximum number of matches already played by any team (default)", (string m) => AverageToExistingMax = m != null },
                    { "c=|count=", $"equalize to the indicated number of averaging matches", (int count) => { AveragingMatchCountGoal = count; AverageToExistingMax = false; } },
                    { "f=|file=",  $"the name of the scoring database", (string f) => Filename = f },
                    { "v|verbose", $"use verbose reporting", (string v) => Verbose = v != null },
                    { "h|help|?",  $"show this message and exit", (string h) => ShowUsage = h != null },
                };
            }

        private IndentedTextWriter MakeIndentedTextWriter(TextWriter writer)
            {
            var result = new IndentedTextWriter(writer, IndentString);
            result.Indent = 0;
            return result;
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
                
                StdOut = MakeIndentedTextWriter(Console.Out);
                StdErr = MakeIndentedTextWriter(Console.Error);
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
            if (AveragingMatchCountGoal < 0)
                {
                Throw($"invalid match count: { AveragingMatchCountGoal }", "-c");
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
            IndentedTextWriter writer = StdErr;

            program.OutputBannerAndCopyright(writer);

            if (e != null)
                {
                writer.WriteLine(e.Message);
                }

            writer.WriteLine($"Usage: { ProgramName } [OPTIONS]* scoringDatabase.db:");
            writer.Indent++;
            writer.WriteLine("Equalize the number of matches played by all teams in the FTC scoring database");
            writer.WriteLine("by adding un-played Equalization Matches as necessary. These added matches need");
            writer.WriteLine("to then be manually scored as 0-0 ties using the FTC ScoreKeeper.");
            writer.WriteLine();
            writer.Indent--;
            writer.WriteLine("Options:");
            options.WriteOptionDescriptions(writer);
            Environment.Exit(e == null ? 0 : -1);
            }

        private String ProgramName => Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
        }



    class Program
        {
        public ProgramOptions ProgramOptions;
        public Database Database;

        public Program()
            {
            ProgramOptions = new ProgramOptions(this);
            Database = null;
            }

        string ProgramVersionString()
            {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            // return $"v{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            return version.ToString();
            }
        string ProgramDescription()
            {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault().Description;
            }
        string ProgramVersionDate()
            {
            DateTime versionDate = Constants.BuildTimestamp;
            return versionDate.ToString("dd MMM yyyy ") + versionDate.ToShortTimeString();
            }
        string ProgramCopyrightNotice()
            {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyCopyrightAttribute copyrightAttr = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false)[0] as System.Reflection.AssemblyCopyrightAttribute;
            return copyrightAttr?.Copyright;
            }

        public void OutputBannerAndCopyright(TextWriter writer)
            {
            string banner = $"{ProgramDescription()} {ProgramVersionString()} of {ProgramVersionDate()}";
            string under = new string('=', Math.Max(banner.Length, ProgramCopyrightNotice().Length));
            //
            writer.WriteLine(banner);
            writer.WriteLine(ProgramCopyrightNotice());
            writer.WriteLine(under);
            writer.WriteLine();
            }

        void DoMain(string[] args)
            {
            ProgramOptions.Parse(args);

            OutputBannerAndCopyright(ProgramOptions.StdOut);

            Database = new Database(ProgramOptions);
            Database.Open();
            Database.BeginTransaction();
            Database.Load();

            Database.ReportEvents(ProgramOptions.StdOut);

            ProgramOptions.StdOut.WriteLine();
            int equalizationMatchesNeeded = Database.ReportTeams(ProgramOptions.StdOut, ProgramOptions.Verbose);
            if (equalizationMatchesNeeded > 0)
                { 
                ProgramOptions.StdOut.WriteLine();
                ConsoleKey response;
                do  { 
                    ProgramOptions.StdOut.Write($"Update database with {equalizationMatchesNeeded} new equalization matches? [y|n] ");
                    response = Console.ReadKey(false).Key;   // true is intercept key (don't show), false is show
                    if (response != ConsoleKey.Enter)
                        {
                        Console.WriteLine();
                        }
                    }
                while (response != ConsoleKey.Y && response != ConsoleKey.N);

                if (response == ConsoleKey.Y)
                    {
                    int equalizationMatchesCreated = Database.CreateEqualizationMatches(ProgramOptions.StdOut, ProgramOptions.Verbose);
                    Database.CommitTransaction();

                    ProgramOptions.StdOut.WriteLine();
                    ProgramOptions.StdOut.WriteLine($"Database updated with {equalizationMatchesCreated} new equalization matches.");
                    ProgramOptions.StdOut.WriteLine();

                    Database.Load();
                    Database.ReportTeams(ProgramOptions.StdOut, false);
                    }
                else
                    {
                    ProgramOptions.StdOut.WriteLine($"No changes made to database");
                    }
                }

            Database.AbortTransaction();
            Database.Close();
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }
        }
    }
