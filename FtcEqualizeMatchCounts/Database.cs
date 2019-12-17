using System;
using System.Collections.Generic;
using System.IO;

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

        public string EventCode => Tables.Config.Map["code"].Value.Value;
        public string EventName => Tables.Config.Map["name"].Value.Value;
        public FMSEventId FMSEventId => new FMSEventId(Tables.Config.Map["FMSEventId"].Value.Value);

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
            Tables.Load();
            LoadData();
            }

        public void LoadData()
            {
            foreach (DBTables.Team.Row row in Tables.Team.Rows)
                {
                Team team = new Team(this, row);
                TeamsByNumber[team.TeamNumber] = team;
                TeamsById[team.TeamId] = team;
                Teams.Add(team);
                }
            Teams.Sort((a, b) => a.TeamNumber - b.TeamNumber);

            foreach (DBTables.ScheduledMatch.Row row in Tables.ScheduledMatch.Rows)
                {
                ScheduledMatch scheduledMatch = new ScheduledMatch(this, row);
                ScheduledMatchesByNumber[scheduledMatch.MatchNumber] = scheduledMatch;
                ScheduledMatchesById[scheduledMatch.FMSScheduleDetailId] = scheduledMatch;
                }

            foreach (DBTables.PlayedMatch.Row row in Tables.PlayedMatch.Rows)
                {
                PlayedMatch playedMatch = new PlayedMatch(this, row);
                }
            }

        public void Report(TextWriter writer)
            {
            bool first = true;
            foreach (Team team in Teams)
                {
                if (!first)
                    {
                    writer.WriteLine("");
                    }
                team.Report(writer);
                first = false;
                }
            }
        }
    }
