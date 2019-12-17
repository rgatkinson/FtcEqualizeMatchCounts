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
            LoadTeams();
            }

        public void LoadTeams()
            {
            foreach (DBTables.Team.Row row in Tables.Team.Rows)
                {
                Team team = new Team(Tables, row);
                Console.Out.WriteLine($"name={team.Name} number={team.TeamNumber} id={team.TeamId}");
                TeamsByNumber[team.TeamNumber] = team;
                TeamsById[team.TeamId] = team;
                }
            }
        }
    }
