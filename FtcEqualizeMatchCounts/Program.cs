﻿using Mono.Options;
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
        public int          AveragingMatchCountCap = 10;
        public bool         OptionMGiven = false;
        public bool         OptionCGiven = false;
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

        private OptionSet options;
        private Program program;

        public ProgramOptions(Program program)
            {
            this.program = program;
            options = new OptionSet
                {
                    { "m|max",     $"equalize to the maximum number of averaging matches of any team (default)", (string m) => OptionMGiven = m != null },
                    { "c=|count=", $"equalize to the indicated number of averaging matches", (int count) => { AveragingMatchCountCap = count; OptionCGiven = true; } },
                    { "f=|file=",  $"the name of the scoring database", (string f) => Filename = f },
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
            writer.WriteLine("Equalize the number of Averaging Matches of all teams in the FTC scoring database");
            writer.WriteLine("by adding never-played Equalization Matches as necessary. After importing into ");
            writer.WriteLine("the FTC Score Keeper, these added matches need to be manually scored as Wins for Blue.");
            writer.WriteLine();
            writer.Indent--;
            writer.WriteLine("Options:");
            options.WriteOptionDescriptions(writer);
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
            ProgramOptions.Parse(args);

            OutputBannerAndCopyright(ProgramOptions.StdOut);

            try { 
                Database = new Database(ProgramOptions);
                Database.Open();
                Database.BeginTransaction();
                Database.Load();

                Database.ReportEvents(ProgramOptions.StdOut);
                Database.ValidateReadyForEqualization();

                int equalizationMatchesNeeded = Database.ReportTeamsAndPlanMatches(ProgramOptions.StdOut, ProgramOptions.Verbose);
                if (equalizationMatchesNeeded > 0)
                    { 
                    ProgramOptions.StdOut.WriteLine();
                    ConsoleKey response = ConsoleKey.Y;

                    if (!ProgramOptions.Quiet)
                        { 
                        do  { 
                            ProgramOptions.StdOut.Write($"Update this event with {equalizationMatchesNeeded} new equalization matches? [y|n] ");
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
                            int equalizationMatchesCreated = Database.SaveEqualizationMatches(ProgramOptions.StdOut, ProgramOptions.Verbose);
                            Database.CommitTransaction();
                            ProgramOptions.StdOut.WriteLine();
                            ProgramOptions.StdOut.WriteLine($"Database updated with {equalizationMatchesCreated} new equalization matches.");
                            ProgramOptions.StdOut.WriteLine();

                            Database.Load();
                            Database.ReportTeamsAndPlanMatches(ProgramOptions.StdOut, false);
                            }
                        }
                    else
                        {
                        ProgramOptions.StdOut.WriteLine($"No changes made to database");
                        }
                    }

                Database.AbortTransaction();
                Database.Close();
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
