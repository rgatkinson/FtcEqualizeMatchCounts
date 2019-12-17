using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Options;

namespace FEMC
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

        public TextWriter   StdOut => Console.Out;
        public TextWriter   StdErr => Console.Error;

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
            TextWriter writer = Console.Error;

            if (e != null)
                {
                writer.WriteLine(e.Message);
                }

            writer.WriteLine($"Usage: { ProgramName } [OPTIONS]* scoringDatabase.db:");
            writer.WriteLine("Equalize the number of matches played by all teams in the FTC scoring database");
            writer.WriteLine("by adding un-played surrogate matches as necessary. These added matches should");
            writer.WriteLine("then be manually scored as 0-0 ties in the FTC ScoreKeeper");
            writer.WriteLine();
            writer.WriteLine("Options:");
            options.WriteOptionDescriptions(writer);
            Environment.Exit(e == null ? 0 : -1);
            }

        private String ProgramName => Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName);
        }



    class Program
        {
        public ProgramOptions programOptions = new ProgramOptions();
        public Database Database = null;

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
            }

        void DoMain(string[] args)
            {
            programOptions.Parse(args);
            OutputBannerAndCopyright(programOptions.StdOut);

            Database = new Database(programOptions.Filename);
            Database.Load();

            Database.Report(programOptions.StdOut);

            Database.Close();
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }
        }
    }
