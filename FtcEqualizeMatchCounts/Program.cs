using Mono.Options;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FEMC.DAL;

namespace FEMC
    {
    class ProgramOptions
        {
        public string       Filename = null;
        public int          AveragingMatchCountCap = 10;
        public bool         OptionMGiven = false;
        public bool         OptionCGiven = false;
        public bool         ScoreEqualizationMatches = false;
        public bool         ShowUsage = false;
        public bool         Verbose = false;
        public List<String> ExtraOptions = new List<string>();
        public string       IndentString = "   ";
        public bool         Quiet = false;

        public int          LeagueMatchesToConsider = 10; // per LeagueSubystem.LEAGUE_MATCHES_TO_CONSIDER
        public int?         AveragingMatchCountGoal = null; // null means use max
        public string       EventCode = null;

        public IndentedTextWriter StdOut;
        public IndentedTextWriter StdErr;

        public string ProgramVersionString()
            {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString(3);
            }
        public string ProgramDescription()
            {
            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            return assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault().Description;
            }
        public string ProgramVersionDate()
            {
            DateTime versionDate = Constants.BuildTimestamp;
            return versionDate.ToString("dd MMM yyyy ") + versionDate.ToShortTimeString();
            }
        public string ProgramCopyrightNotice()
            {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyCopyrightAttribute copyrightAttr = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCopyrightAttribute), false)[0] as System.Reflection.AssemblyCopyrightAttribute;
            return copyrightAttr?.Copyright;
            }

        private OptionSet Options;
        private Program Program;

        public ProgramOptions(Program program)
            {
            this.Program = program;
            Options = new OptionSet
                {
                    { "m|max",     $"equalize to the maximum number of averaging matches of any team (default)", (string m) => OptionMGiven = m != null },
                    { "c=|count=", $"equalize to the (at most) this number of averaging matches", (int count) => { AveragingMatchCountCap = count; OptionCGiven = true; } },
                    { "f=|file=",  $"the name of the scoring database", (string f) => Filename = f },
                    { "s|score",   $"automatically score equalization matches", (string s) => ScoreEqualizationMatches = s != null },
                    { "v|verbose", $"use verbose reporting", (string v) => Verbose = v != null },
                    { "h|help|?",  $"show this message and exit", (string h) => ShowUsage = h != null },
                #if DEBUG
                    { "q|quiet",  $"accept all prompts silently", (string q) => Quiet = q != null },
                #endif
                };

            // In case we don't validate
            StdOut = MakeIndentedTextWriter(Console.Out);
            StdErr = MakeIndentedTextWriter(Console.Error);
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
                ExtraOptions = Options.Parse(args);
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

                if (OptionMGiven)
                    {
                    // Explicit request for 'max', either w/ or w/o custom cap value
                    AveragingMatchCountGoal = null;
                    }
                else if (OptionCGiven)
                    {
                    // Explicit count given, no 'max': use that value
                    AveragingMatchCountGoal = AveragingMatchCountCap;
                    }
                else
                    {
                    // Default options 
                    AveragingMatchCountGoal = null;
                    }
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
            if (AveragingMatchCountCap <= 0)
                {
                Throw($"invalid match count: { AveragingMatchCountCap }", "-c");
                }
            if (Filename == null)
                {
                Throw($"name of database not given", "-f");
                }
            if (!File.Exists(Filename))
                {
                Throw($"database '{Filename}' does not exist");
                }
            }

        private void Throw(string message, string optionName = null)
            {
            throw new OptionException(Options.MessageLocalizer($"Error: { message }"), optionName);
            }


        private void Usage(OptionException e)
            {
            IndentedTextWriter writer = StdErr;

            Program.OutputBannerAndCopyright(writer);

            if (e != null)
                {
                writer.WriteLine(e.Message);
                }

            writer.WriteLine($"Usage: { ProgramName } [OPTIONS]* scoringDatabase.db:");
            writer.Indent++;
            writer.WriteLine("Equalize the number of Averaging Matches of all teams in the FTC scoring database by");
            writer.WriteLine("adding never-to-be-played Equalization Matches as necessary to achieve for each the team");
            writer.WriteLine("an averaging match count that is at least the indicated averaging match count goal.");
            writer.WriteLine();
            writer.WriteLine("If the -s option is used, then these added Equalization Matches are scored automatically.");
            writer.WriteLine("Otherwise, after importing the updated database into the FTC Score Keeper, these added ");
            writer.WriteLine("matches need to be manually scored as Wins for Blue.");
            writer.WriteLine();
            writer.WriteLine("In no case are any matches ever *removed*.");
            writer.WriteLine();
            writer.Indent--;
            writer.WriteLine("Options:");
            Options.WriteOptionDescriptions(writer);
            writer.WriteLine();
            writer.Indent++;
            writer.WriteLine("If both -m and -c are used, the averaging match count goal will be the maximum current ");
            writer.WriteLine("averaging match count of any existing team, but capped by the value provided with -c.");
            writer.WriteLine();
            writer.WriteLine("If -m is used without -c, the behavior is as if both -m and -c 10 were used.");
            writer.WriteLine();
            writer.WriteLine("If -c is used without -m, the averaging match count goal is the value provided.");
            writer.Indent--;
            if (e==null)
                Program.ExitApp();
            else
                Program.FailFast();
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


        public void OutputBannerAndCopyright(TextWriter writer)
            {
            string banner = $"{ProgramOptions.ProgramDescription()} {ProgramOptions.ProgramVersionString()} of {ProgramOptions.ProgramVersionDate()}";
            string under = new string('=', Math.Max(banner.Length, ProgramOptions.ProgramCopyrightNotice().Length));
            //
            writer.WriteLine(banner);
            writer.WriteLine(ProgramOptions.ProgramCopyrightNotice());
            writer.WriteLine(under);
            writer.WriteLine();
            }

        void DoMain(string[] args)
            {
            bool changesMade = false;
            ProgramOptions.Parse(args);

            OutputBannerAndCopyright(ProgramOptions.StdOut);

            try { 
                Database = new Database(ProgramOptions);
                Database.Open();
                Database.BeginTransaction();
                Database.ClearAndLoad();

                Database.ReportEvents(ProgramOptions.StdOut);
                Database.ValidateReadyForEqualization();

                int equalizationMatchesNeeded = Database.ReportTeamsAndPlanMatches(ProgramOptions.StdOut, ProgramOptions.Verbose, false);
                bool equalizationNeeded = equalizationMatchesNeeded > 0;
                if (equalizationNeeded)
                    {
                    if (Database.LoadedEqualizationMatches.Count > 0)
                        {
                        throw new EqualizationMatchesPresentException();
                        }
                    }

                bool scoringNeeded = false;
                if (ProgramOptions.ScoreEqualizationMatches)
                    {
                    if (equalizationNeeded || Database.LoadedEqualizationMatches.Exists(match => !match.IsScored))
                        {
                        scoringNeeded = true;
                        }
                    }

                if (equalizationNeeded || scoringNeeded)
                    {
                    ProgramOptions.StdOut.WriteLine();
                    ConsoleKey response = ConsoleKey.Y;

                    if (!ProgramOptions.Quiet)
                        { 
                        do  { 
                            string msg;
                            if (equalizationNeeded && scoringNeeded)
                                {
                                msg = $"Update this event with {equalizationMatchesNeeded} new equalization matches and score them?";
                                }
                            else if (equalizationNeeded)
                                {
                                msg = $"Update this event with {equalizationMatchesNeeded} new equalization matches?";
                                }
                            else
                                {
                                int scoringMatchesNeeded = Database.LoadedEqualizationMatches.FindAll(match => !match.IsScored).Count;
                                msg = $"Score the {scoringMatchesNeeded} equalization matches in this event?";
                                }

                            ProgramOptions.StdOut.Write($"{msg} [y|n]");
                            response = Console.ReadKey(false).Key;   // true is intercept key (don't show), false is show
                            if (response != ConsoleKey.Enter)
                                {
                                Console.WriteLine();
                                }
                            }
                        while (response != ConsoleKey.Y && response != ConsoleKey.N);
                        }

                    if (response == ConsoleKey.Y)
                        {
                        if (Database.BackupFile())
                            { 
                            if (equalizationNeeded)
                                { 
                                Database.SaveNewEqualizationMatches(ProgramOptions.StdOut, ProgramOptions.Verbose);
                                changesMade = true;
                                }

                            List<EqualizationMatch> matchesToScore = Database.UnscoredEqualizationMatches;

                            if (scoringNeeded)
                                {
                                Trace.Assert(matchesToScore.Count > 0);
                                foreach (var match in matchesToScore)
                                    {
                                    match.ScoreMatch();
                                    changesMade = true;
                                    }
                                }

                            if (equalizationNeeded || scoringNeeded)
                                {
                                Trace.Assert(changesMade);
                                ProgramOptions.StdOut.WriteLine();
                                if (equalizationNeeded)
                                    {
                                    ProgramOptions.StdOut.WriteLine($"Database updated with {Database.NewEqualizationMatches.Count} new equalization matches.");
                                    }
                                if (scoringNeeded)
                                    {
                                    ProgramOptions.StdOut.WriteLine($"Scored {matchesToScore.Count} equalization event matches in database.");
                                    }
                                ProgramOptions.StdOut.WriteLine();
                                }

                            }
                        }
                    }

                if (changesMade)
                    {
                    Database.CommitTransaction();

                    // Report new state
                    Database.BeginTransaction(); // mostly (?) for isolation; harmless in any case
                    Database.ClearAndLoad();

                    ProgramOptions.StdOut.WriteLine("Post-Update Report:");
                    Database.ReportTeamsAndPlanMatches(ProgramOptions.StdOut, false, true);
                    }
                else
                    {
                    ProgramOptions.StdOut.WriteLine($"No changes made to database");
                    }
                }
            catch (DatabaseNotReadyException e)
                {
                ProgramOptions.StdErr.WriteLine($"This event database not ready: {e.Message}");
                }
            catch (Exception e)
                {
                MiscUtil.DumpStackTrance(ProgramOptions.ProgramDescription(), ProgramOptions.StdErr, e);
                }
            finally
                {
                Database.AbortTransaction();
                Database.Close();
                }
            }

        static void Main(string[] args)
            {
            var program = new Program();
            program.DoMain(args);
            }

        public static void FailFast()
            {
            Environment.Exit(-1);
            }

        public static void ExitApp()
            {
            Environment.Exit(0);
            }
        }
    }
