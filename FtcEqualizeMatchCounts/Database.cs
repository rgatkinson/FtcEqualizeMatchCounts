﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using FEMC.DAL;
using Microsoft.Data.Sqlite;

namespace FEMC
    {
    class Database : IDisposable
        {
        //---------------------------------------------------------------------------------------------------
        // State
        //---------------------------------------------------------------------------------------------------

        public SqliteConnection Connection = null;
        public Tables Tables;

        public IDictionary<long, Team> TeamsByNumber = new Dictionary<long, Team>();
        public IDictionary<FMSTeamId, Team> TeamsById = new Dictionary<FMSTeamId, Team>();
        public List<Team> Teams = new List<Team>();

        public IDictionary<long, ScheduledMatch> ScheduledMatchesByNumber = new Dictionary<long, ScheduledMatch>();
        public IDictionary<FMSScheduleDetailId, ScheduledMatch> ScheduledMatchesById = new Dictionary<FMSScheduleDetailId, ScheduledMatch>();

        public IDictionary<string, Event> EventsByCode = new Dictionary<string, Event>();

        public string ThisEventCode => Tables.Config.Map["code"].Value.Value;
        public Event ThisEvent => EventsByCode[ThisEventCode];
        public FMSEventId FMSEventId => new FMSEventId(Tables.Config.Map["FMSEventId"].Value.Value);

        public List<Event> OtherEvents
            {
            get {
                List<Event> result = new List<Event>();
                foreach (var anEvent in EventsByCode.Values)
                    {
                    if (anEvent != ThisEvent)
                        {
                        result.Add(anEvent);
                        }
                    }
                result.Sort((a,b) => (int)(a.StartNonNull.ToUniversalTime().Ticks - b.StartNonNull.ToUniversalTime().Ticks));
                return result;
                }
            }


        string fileName = null;
        bool disposed = false;

        //---------------------------------------------------------------------------------------------------
        // Construction
        //---------------------------------------------------------------------------------------------------

        public Database(string fileName)
            {
            this.fileName = Path.GetFullPath(fileName);
            this.Tables = new Tables(this);
            
            Open();
            }

        ~Database()
            {
            Dispose(false);
            }

        public void Dispose() // called by consumers
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }

        protected virtual void Dispose(bool explicitlyDisposed)
            {
            if (!disposed)
                { 
                if (explicitlyDisposed)
                    {
                    // Free managed objects
                    Close();
                    }
                // Free native objects
                disposed = true;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Opening and closing
        //---------------------------------------------------------------------------------------------------

        public void Open()
            {
            Close();

            string uri = new System.Uri(fileName).AbsoluteUri;
            string cs = "Filename=" + fileName;

            Connection = new SqliteConnection(cs);
            Connection.Open();
            }

        public void Close()
            {
            if (Connection != null)
                {
                Connection.Close();
                Connection = null;
                }
            }

        //---------------------------------------------------------------------------------------------------
        // Querying
        //---------------------------------------------------------------------------------------------------

        public void Load()
            {
            Clear();
            Tables.Load();
            LoadDataAccessLayer();
            }

        void Clear()
            {
            Tables.Clear();

            TeamsById.Clear();
            TeamsByNumber.Clear();
            ScheduledMatchesById.Clear();
            ScheduledMatchesByNumber.Clear();
            EventsByCode.Clear();
            }

        public void LoadDataAccessLayer()
            {
            foreach (var row in Tables.LeagueMeets.Rows)
                {
                Event anEvent = new Event(this, row);
                EventsByCode[anEvent.EventCode] = anEvent;
                }

            foreach (var row in Tables.Team.Rows)
                {
                Team team = new Team(this, row);
                TeamsByNumber[team.TeamNumber] = team;
                TeamsById[team.TeamId] = team;
                Teams.Add(team);
                }
            Teams.Sort((a, b) => a.TeamNumber - b.TeamNumber);

            foreach (var row in Tables.ScheduledMatch.Rows)
                {
                ScheduledMatch scheduledMatch = new ScheduledMatch(this, row);
                ScheduledMatchesByNumber[scheduledMatch.MatchNumber] = scheduledMatch;
                ScheduledMatchesById[scheduledMatch.FMSScheduleDetailId] = scheduledMatch;
                }

            foreach (var row in Tables.PlayedMatch.Rows)
                {
                PlayedMatch playedMatch = new PlayedMatch(this, row);
                }

            foreach (var row in Tables.LeagueHistory.Rows)
                {
                LeagueHistoryMatch.Process(this, row);
                }
            }

        public void ReportEvents(IndentedTextWriter writer)
            {
            writer.WriteLine("This event:");
            writer.Indent++;
            ThisEvent.Report(writer);
            writer.Indent--;

            List<Event> otherEvents = OtherEvents;
            if (otherEvents.Count > 0)
                {
                writer.WriteLine();
                writer.WriteLine("Previous events:");
                bool first = true;
                writer.Indent++;
                foreach (var otherEvent in otherEvents)
                    {
                    if (!first)
                        {
                        writer.WriteLine();
                        }
                    otherEvent.Report(writer);
                    first = false;
                    }
                writer.Indent--;
                }
            }

        public void ReportTeams(IndentedTextWriter writer)
            {
            bool firstTeam = true;
            foreach (Team team in Teams)
                {
                if (!firstTeam)
                    {
                    writer.WriteLine("");
                    }
                team.Report(writer);
                firstTeam = false;
                }
            }
        }
    }
